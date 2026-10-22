
(function () {
    const root = document.querySelector(".decision-rules-page");

    if (!root || root.dataset.drInitialized === "true") {
        return;
    }

    root.dataset.drInitialized = "true";

    let nextId = 4;
    let nextConditionId = 1;
    let editingId = null;
    let deletingId = null;
    let toastTimer = null;
    let conditionLogic = "all";
    let conditions = [];

    let questionOptions = [];
    let recommendationOptions = [];
    let rules = [];
    let widgets = [];
    let selectedWidgetKey = localStorage.getItem("askpearls.activeWidgetPublicKey") || "";
    let currentVersionNumber = "";
    let saveInProgress = false;
    const apiBase = (root.dataset.apiBase || window.AskPearlsCS_API_URL || (window.location.origin + "/api")).replace(/\/+$/, "");
    const ownerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const $ = function (selector) {
        return root.querySelector(selector);
    };

    const apiHeaders = function () {
        const headers = { Accept: "application/json", "Content-Type": "application/json" };
        if (ownerToken) headers[ "X-Demo-Owner-User-Token" ] = ownerToken;
        return headers;
    };

    async function readApi (response, endpoint) {
        const body = await response.text();
        let data = {};
        try { data = body ? JSON.parse(body) : {}; }
        catch {
            const contentType = response.headers.get("content-type") || "unknown content type";
            const preview = body.replace(/\s+/g, " ").trim().slice(0, 180);
            throw new Error(`${ endpoint || "API request" } returned HTTP ${ response.status } as ${ contentType }${ preview ? `: ${ preview }` : "." }`);
        }
        if (!response.ok || data.success === false) throw new Error(data.message || `API request failed (HTTP ${ response.status }).`);
        return data;
    }

    function showApiMessage (text, error) {
        const element = $(`[data-dr-api-message]`);
        if (!element) return;
        element.textContent = text || "";
        element.hidden = !text;
        element.classList.toggle("is-error", !!error);
    }

    function versionLabel (widget) {
        if (!widget) return "—";
        const draft = widget.draftVersionNumber ?? widget.DraftVersionNumber;
        const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber;
        const current = widget.versionNumber ?? widget.VersionNumber;
        return draft ? `Draft v${ draft }${ live ? ` · Live v${ live }` : "" }` : (live || current ? `Live v${ live || current }` : "—");
    }

    function recommendationName (key) {
        const item = recommendationOptions.find(option => option.key === key);
        return item ? item.name : (key || "");
    }

    function populateRecommendationSelect (selected) {
        const select = $("#dr-recommendation");
        if (!select) return;
        select.innerHTML = '<option value="">Select a recommendation</option>' + recommendationOptions.map(item =>
            `<option value="${ escapeHtml(item.key) }">${ escapeHtml(item.name) }</option>`).join("");
        if (selected) select.value = selected;
        select.disabled = recommendationOptions.length === 0;
    }

    function renderTestQuestions () {
        const grid = $("[data-dr-test-grid]");
        if (!grid) return;
        grid.innerHTML = questionOptions.map(question =>
            `<div class="dr-field"><label for="dr-test-${ escapeHtml(question.id) }">${ escapeHtml(question.text) }</label>` +
            `<select id="dr-test-${ escapeHtml(question.id) }" class="dr-select" data-dr-test-question="${ escapeHtml(question.id) }">` +
            `<option value="">Not answered</option>${ question.answers.map(answer => `<option value="${ escapeHtml(answer.value) }">${ escapeHtml(answer.text) }</option>`).join("") }</select></div>`
        ).join("");
    }

    async function loadRulesPage () {
        const select = $("#drWidgetSelect");
        try {
            const widgetData = await readApi(await fetch(`${ apiBase }/widgets`, { headers: apiHeaders() }), "Widget list");
            widgets = widgetData.widgets || [];
            select.innerHTML = widgets.length ? widgets.map(widget => {
                const key = widget.publicKey ?? widget.PublicKey;
                const name = widget.name ?? widget.Name ?? "Untitled widget";
                return `<option value="${ escapeHtml(key) }">${ escapeHtml(name) }</option>`;
            }).join("") : '<option value="">No widgets available</option>';
            if (!widgets.some(widget => String(widget.publicKey ?? widget.PublicKey) === selectedWidgetKey))
                selectedWidgetKey = String(widgets[ 0 ]?.publicKey ?? widgets[ 0 ]?.PublicKey ?? "");
            select.value = selectedWidgetKey;
            select.disabled = !selectedWidgetKey;
            if (selectedWidgetKey) localStorage.setItem("askpearls.activeWidgetPublicKey", selectedWidgetKey);
        } catch (error) {
            select.innerHTML = '<option value="">Could not load widgets</option>';
            select.disabled = true;
            showApiMessage(error.message, true);
            return;
        }
        try { await loadWidgetData(); }
        catch (error) { rules = []; questionOptions = []; recommendationOptions = []; renderTestQuestions(); populateRecommendationSelect(); renderRows(); showApiMessage(error.message, true); }
    }

    async function loadWidgetData () {
        const widget = widgets.find(item => String(item.publicKey ?? item.PublicKey) === selectedWidgetKey);
        $("#drWidgetVersion").textContent = versionLabel(widget);
        if (!widget) { rules = []; questionOptions = []; recommendationOptions = []; renderTestQuestions(); populateRecommendationSelect(); renderRows(); return; }
        showApiMessage("Loading questions, recommendations, and rules…");
        const key = encodeURIComponent(selectedWidgetKey);
        const [ questionData, recommendationData, ruleData ] = await Promise.all([
            fetch(`${ apiBase }/widgets/${ key }/draft/questions`, { headers: apiHeaders() }).then(response => readApi(response, "Draft questions")),
            fetch(`${ apiBase }/widgets/${ key }/draft/recommendations`, { headers: apiHeaders() }).then(response => readApi(response, "Draft recommendations")),
            fetch(`${ apiBase }/widgets/${ key }/draft/rules`, { headers: apiHeaders() }).then(response => readApi(response, "Draft decision rules"))
        ]);
        questionOptions = (questionData.questions || []).map(question => ({
            id: question.key ?? question.Key,
            text: question.text ?? question.Text ?? question.key ?? "Question",
            answers: (question.options || question.Options || []).map((answer, index) => ({ value: `option_${ index + 1 }`, text: String(answer) }))
        }));
        renderTestQuestions();
        recommendationOptions = (recommendationData.items || []).filter(item => (item.active ?? item.Active) !== false)
            .map(item => ({ key: item.key ?? item.Key, name: item.name ?? item.Name }));
        populateRecommendationSelect();
        currentVersionNumber = ruleData.selectedVersionNumber || questionData.versionNumber || recommendationData.versionNumber || ruleData.versionNumber || "";
        if (currentVersionNumber) {
            const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber;
            $("#drWidgetVersion").textContent = widget.draftVersionNumber || widget.DraftVersionNumber
                ? `Draft v${ currentVersionNumber }${ live ? ` · Live v${ live }` : "" }` : versionLabel(widget);
        }
        rules = (ruleData.rules || []).map((rule, index) => ({
            id: rule.id || rule.Id || rule.key || rule.Key || String(index + 1), key: rule.key || rule.Key || "", name: rule.name || rule.Name || "",
            description: rule.description || rule.Description || "", recommendation: rule.recommendationKey || rule.RecommendationKey || "",
            priority: Number(rule.priority ?? rule.Priority) || index + 1, status: rule.status || rule.Status || "active",
            conditionLogic: (rule.conditionLogic || rule.ConditionLogic) === "any" ? "any" : "all",
            conditionItems: (rule.conditions || rule.Conditions || []).map((condition) => ({
                id: nextConditionId++, questionId: condition.questionKey || condition.QuestionKey, answerValue: condition.answerKey || condition.AnswerKey, questionText: condition.questionText || condition.QuestionText, answerText: condition.answerText || condition.AnswerText
            }))
        }));
        renderRows();
        showApiMessage(ruleData.usedLiveFallback
            ? `The draft has no saved rules yet. Showing ${ rules.length } rule${ rules.length === 1 ? "" : "s" } from the live version.`
            : rules.length
                ? `Loaded ${ rules.length } decision rule${ rules.length === 1 ? "" : "s" } from the ${ ruleData.status || "selected" } version.`
                : `No decision rules are saved for this widget yet. Create a rule and save it to the draft.`);
    }

    async function persistRules (nextRules) {
        if (saveInProgress) return;
        if (!selectedWidgetKey) throw new Error("Select a widget first.");
        saveInProgress = true;
        try {
            const payload = {
                rules: nextRules.map(rule => ({
                    key: rule.key, name: rule.name, description: rule.description || "",
                    recommendationKey: rule.recommendation,
                    recommendationName: recommendationName(rule.recommendation),
                    conditionLogic: rule.conditionLogic, priority: Number(rule.priority), status: rule.status,
                    conditions: (rule.conditionItems || []).map(condition => {
                        const question = getQuestion(condition.questionId);
                        const answer = getAnswer(condition.questionId, condition.answerValue);
                        return {
                            questionKey: condition.questionId, questionText: question?.text || condition.questionText || "",
                            answerKey: condition.answerValue, answerText: answer?.text || condition.answerText || condition.answerValue
                        };
                    })
                }))
            };
            const result = await readApi(await fetch(`${ apiBase }/widgets/${ encodeURIComponent(selectedWidgetKey) }/draft/rules`, {
                method: "PUT", headers: apiHeaders(), body: JSON.stringify(payload)
            }), "Save decision rules");
            rules = nextRules;
            currentVersionNumber = result.versionNumber || currentVersionNumber;
            const widget = widgets.find(item => String(item.publicKey ?? item.PublicKey) === selectedWidgetKey);
            if (widget) { widget.draftVersionNumber = currentVersionNumber; widget.hasDraft = true; }
            $("#drWidgetVersion").textContent = `Draft v${ currentVersionNumber }${ widget?.publishedVersionNumber ? ` · Live v${ widget.publishedVersionNumber }` : "" }`;
            renderRows();
            showApiMessage(`Draft v${ currentVersionNumber } saved. The live version is unchanged until you publish it.`);
        } finally { saveInProgress = false; }
    }

    function escapeHtml (value) {
        return String(value ?? "").replace(/[&<>"']/g, function (char) {
            return {
                "&": "&amp;",
                "<": "&lt;",
                ">": "&gt;",
                '"': "&quot;",
                "'": "&#39;"
            }[ char ];
        });
    }

    function showToast (message) {
        const toast = $("[data-dr-toast]");
        toast.textContent = message;
        toast.classList.add("is-visible");

        clearTimeout(toastTimer);

        toastTimer = setTimeout(function () {
            toast.classList.remove("is-visible");
        }, 2600);
    }

    function openModal (modal) {
        modal.classList.add("is-open");
        modal.setAttribute("aria-hidden", "false");
        document.body.style.overflow = "hidden";
    }

    function closeModal (modal) {

        resetTestRulePreview();


        modal.classList.remove("is-open");
        modal.setAttribute("aria-hidden", "true");

        if (!root.querySelector(".dr-modal-overlay.is-open")) {
            document.body.style.overflow = "";
        }
    }

    function getQuestion (questionId) {
        return questionOptions.find(function (question) {
            return question.id === questionId;
        });
    }

    function getAnswer (questionId, answerValue) {
        const question = getQuestion(questionId);

        if (!question) {
            return null;
        }

        return question.answers.find(function (answer) {
            return answer.value === answerValue;
        }) || null;
    }

    function getQuestionText (questionId) {
        const question = getQuestion(questionId);
        return question ? question.text : "Select a question";
    }

    function getAnswerText (questionId, answerValue) {
        const answer = getAnswer(questionId, answerValue);
        return answer ? answer.text : "Select an answer";
    }

    function createCondition (questionId, answerValue) {
        return {
            id: nextConditionId++,
            questionId: questionId || "",
            answerValue: answerValue || ""
        };
    }

    function getConditionSummary (condition) {
        if (!condition.questionId || !condition.answerValue) {
            return "Complete the question and answer selections.";
        }

        return getQuestionText(condition.questionId) +
            " is \"" +
            getAnswerText(condition.questionId, condition.answerValue) +
            "\"";
    }

    function updateConditionCount () {
        const count = conditions.length;

        $("[data-dr-condition-count]").textContent =
            count + " condition" + (count === 1 ? "" : "s");
    }

    function updateLogicButtons () {
        root.querySelectorAll("[data-dr-logic]").forEach(function (button) {
            const selected = button.dataset.drLogic === conditionLogic;

            button.classList.toggle("is-selected", selected);
            button.setAttribute("aria-pressed", selected ? "true" : "false");
        });

        $("[data-dr-logic-explanation]").textContent =
            conditionLogic === "all"
                ? "Every condition must match for this rule to apply."
                : "At least one condition must match for this rule to apply.";
    }

    function updateRuleSummary () {
        const summary = $("[data-dr-rule-summary]");

        if (conditions.length === 0) {
            summary.textContent =
                "Add a condition to define when this rule should apply.";
            return;
        }

        const completed = conditions.filter(function (condition) {
            return condition.questionId && condition.answerValue;
        });

        if (completed.length === 0) {
            summary.textContent =
                "Select a question and answer to define the rule.";
            return;
        }

        const connector = conditionLogic === "all" ? " AND " : " OR ";

        summary.textContent = completed.map(function (condition) {
            return getConditionSummary(condition);
        }).join(connector);

        if (completed.length < conditions.length) {
            summary.textContent +=
                " — Complete the remaining condition selections.";
        }
    }

    function renderConditions () {
        const list = $("[data-dr-condition-list]");

        list.innerHTML = conditions.map(function (condition, index) {
            const questionOptionsHtml = [
                '<option value="">Select a question</option>'
            ].concat(questionOptions.map(function (question) {
                return '<option value="' + escapeHtml(question.id) + '"' +
                    (condition.questionId === question.id ? " selected" : "") +
                    ">" + escapeHtml(question.text) + "</option>";
            })).join("");

            const selectedQuestion = getQuestion(condition.questionId);

            const answerOptionsHtml = [
                '<option value="">Select an answer</option>'
            ].concat(
                selectedQuestion
                    ? selectedQuestion.answers.map(function (answer) {
                        return '<option value="' + escapeHtml(answer.value) + '"' +
                            (condition.answerValue === answer.value ? " selected" : "") +
                            ">" + escapeHtml(answer.text) + "</option>";
                    })
                    : []
            ).join("");

            const connectorHtml = index > 0
                ? '<div class="dr-condition-connector" aria-hidden="true">' +
                (conditionLogic === "all" ? "AND" : "OR") +
                "</div>"
                : "";

            return connectorHtml +
                '<div class="dr-condition-card" data-dr-condition-card="' +
                condition.id + '">' +

                '<div class="dr-condition-card-header">' +
                '<div class="dr-condition-card-title">' +
                '<span class="dr-condition-number">' +
                (index + 1) +
                "</span>" +
                "<span>Condition " + (index + 1) + "</span>" +
                "</div>" +

                '<button type="button" class="dr-condition-remove" ' +
                'data-dr-action="remove-condition" ' +
                'data-condition-id="' + condition.id + '"' +
                ' aria-label="Remove condition ' + (index + 1) + '">' +
                "&#10005; Remove" +
                "</button>" +
                "</div>" +

                '<div class="dr-condition-fields">' +
                '<div class="dr-condition-field">' +
                '<label for="dr-question-' + condition.id + '">' +
                "Question" +
                "</label>" +
                '<select class="dr-select" ' +
                'id="dr-question-' + condition.id + '" ' +
                'data-dr-question="' + condition.id + '">' +
                questionOptionsHtml +
                "</select>" +
                "</div>" +

                '<div class="dr-condition-field">' +
                '<label for="dr-answer-' + condition.id + '">' +
                "Answer" +
                "</label>" +
                '<select class="dr-select" ' +
                'id="dr-answer-' + condition.id + '" ' +
                'data-dr-answer="' + condition.id + '"' +
                (selectedQuestion ? "" : " disabled") + ">" +
                answerOptionsHtml +
                "</select>" +
                "</div>" +
                "</div>" +

                '<div class="dr-condition-error" ' +
                'data-dr-condition-error="' + condition.id + '">' +
                "Select both a question and an answer." +
                "</div>" +
                "</div>";
        }).join("");

        updateConditionCount();
        updateLogicButtons();
        updateRuleSummary();
    }

    function addCondition () {
        conditions.push(createCondition("", ""));
        renderConditions();

        const lastCondition = conditions[ conditions.length - 1 ];

        const questionSelect = root.querySelector(
            '[data-dr-question="' + lastCondition.id + '"]'
        );

        if (questionSelect) {
            questionSelect.focus();
        }
    }

    function removeCondition (conditionId) {
        conditions = conditions.filter(function (condition) {
            return condition.id !== conditionId;
        });

        renderConditions();
    }

    function setConditionLogic (logic) {
        if (logic !== "all" && logic !== "any") {
            return;
        }

        conditionLogic = logic;
        updateLogicButtons();
        renderConditions();
    }

    function updateConditionQuestion (conditionId, questionId) {
        const condition = conditions.find(function (item) {
            return item.id === conditionId;
        });

        if (!condition) {
            return;
        }

        condition.questionId = questionId;
        condition.answerValue = "";

        renderConditions();
    }

    function updateConditionAnswer (conditionId, answerValue) {
        const condition = conditions.find(function (item) {
            return item.id === conditionId;
        });

        if (!condition) {
            return;
        }

        condition.answerValue = answerValue;

        updateRuleSummary();

        const error = root.querySelector(
            '[data-dr-condition-error="' + conditionId + '"]'
        );

        if (error) {
            error.classList.remove("is-visible");
        }

        const card = root.querySelector(
            '[data-dr-condition-card="' + conditionId + '"]'
        );

        if (card) {
            card.classList.remove("has-error");
        }
    }

    function validateConditions () {
        const validation = $("[data-dr-validation]");
        let firstInvalidCondition = null;

        root.querySelectorAll(".dr-condition-card").forEach(function (card) {
            card.classList.remove("has-error");
        });

        root.querySelectorAll(".dr-condition-error").forEach(function (error) {
            error.classList.remove("is-visible");
        });

        if (conditions.length === 0) {
            validation.textContent =
                "Add at least one condition before saving this rule.";
            validation.classList.add("is-visible");
            return false;
        }

        const invalidConditions = conditions.filter(function (condition) {
            const question = getQuestion(condition.questionId);
            const answer = getAnswer(condition.questionId, condition.answerValue);

            return !question || !answer;
        });

        if (invalidConditions.length > 0) {
            invalidConditions.forEach(function (condition) {
                const card = root.querySelector(
                    '[data-dr-condition-card="' + condition.id + '"]'
                );

                const error = root.querySelector(
                    '[data-dr-condition-error="' + condition.id + '"]'
                );

                if (card) {
                    card.classList.add("has-error");
                }

                if (error) {
                    error.classList.add("is-visible");
                }

                if (!firstInvalidCondition) {
                    firstInvalidCondition = condition.id;
                }
            });

            validation.textContent =
                "Complete the question and answer selections for every condition.";
            validation.classList.add("is-visible");

            const firstSelect = root.querySelector(
                '[data-dr-question="' + firstInvalidCondition + '"]'
            );

            if (firstSelect) {
                firstSelect.focus();
            }

            return false;
        }

        return true;
    }

    function updateStats () {
        $("[data-dr-stat='total']").textContent = rules.length;

        $("[data-dr-stat='active']").textContent =
            rules.filter(function (rule) {
                return rule.status === "active";
            }).length;

        $("[data-dr-stat='draft']").textContent =
            rules.filter(function (rule) {
                return rule.status === "draft";
            }).length;

        const covered = new Set(
            rules.filter(function (rule) {
                return rule.status === "active";
            }).map(function (rule) {
                return rule.recommendation;
            }).filter(Boolean)
        );

        $("[data-dr-stat='covered']").textContent = covered.size;
    }

    function statusMarkup (status) {
        const labels = {
            active: "Active",
            draft: "Draft",
            inactive: "Inactive"
        };

        return '<span class="dr-status dr-status-' + status + '">' +
            labels[ status ] + '</span>';
    }

    function renderRows () {
        const search = $("[data-dr-search]").value.trim().toLowerCase();
        const filter = $("[data-dr-filter]").value;

        const filtered = rules
            .filter(function (rule) {
                const matchesSearch =
                    rule.name.toLowerCase().includes(search) ||
                    rule.key.toLowerCase().includes(search) ||
                    recommendationName(rule.recommendation).toLowerCase().includes(search);

                return matchesSearch &&
                    (filter === "all" || rule.status === filter);
            })
            .sort(function (a, b) {
                return a.priority - b.priority;
            });

        const tbody = $("[data-dr-rows]");

        tbody.innerHTML = filtered.map(function (rule) {
            const toggleLabel =
                rule.status === "active" ? "Deactivate" : "Activate";

            const conditionCount =
                Array.isArray(rule.conditionItems)
                    ? rule.conditionItems.length
                    : Number(rule.conditions) || 0;

            const logicLabel =
                rule.conditionLogic === "any" ? "ANY" : "ALL";

            return `
                <tr>
                    <td>
                        <div class="dr-rule-name">${ escapeHtml(rule.name) }</div>
                        <div class="dr-rule-id">${ escapeHtml(rule.key) }</div>
                    </td>
                    <td>
                        ${ conditionCount } condition${ conditionCount === 1 ? "" : "s" }
                        ${ conditionCount > 1
                    ? '<div class="dr-rule-id">Match ' + logicLabel + '</div>'
                    : "" }
                    </td>
                    <td>${ escapeHtml(recommendationName(rule.recommendation)) }</td>
                    <td>${ rule.priority }</td>
                    <td>${ statusMarkup(rule.status) }</td>
                    <td>
                        <div class="dr-actions">
                            <button type="button" class="dr-btn"
                                data-dr-action="edit"
                                data-id="${ rule.id }">Edit</button>
                            <button type="button" class="dr-btn"
                                data-dr-action="duplicate"
                                data-id="${ rule.id }">Duplicate</button>
                            <button type="button" class="dr-btn"
                                data-dr-action="toggle"
                                data-id="${ rule.id }">${ toggleLabel }</button>
                            <button type="button" class="dr-btn dr-btn-danger"
                                data-dr-action="delete"
                                data-id="${ rule.id }">Delete</button>
                        </div>
                    </td>
                </tr>`;
        }).join("");

        $("[data-dr-empty]").hidden = filtered.length > 0;

        $("[data-dr-count]").textContent =
            `Showing ${ filtered.length } of ${ rules.length } rule${ rules.length === 1 ? "" : "s" }`;

        updateStats();
    }

    function resetForm () {
        $("[data-dr-form]").reset();

        $("[data-dr-validation]").classList.remove("is-visible");

        conditions = [];
        conditionLogic = "all";

        renderConditions();
    }

    function openCreate () {

        resetTestRulePreview();

        editingId = null;
        resetForm();

        $("[data-dr-modal-title]").textContent = "Create Decision Rule";
        $("[data-dr-save-label]").textContent = "Save Rule";
        $("#dr-priority").value = "1";

        // Start with one empty condition to guide the user.
        conditions.push(createCondition("", ""));
        renderConditions();

        openModal($("[data-dr-modal]"));
        $("#dr-name").focus();
    }

    function openEdit (id) {

        resetTestRulePreview();

        const rule = rules.find(function (item) {
            return item.id === id;
        });

        if (!rule) {
            return;
        }

        editingId = id;
        resetForm();

        $("#dr-name").value = rule.name;
        $("#dr-key").value = rule.key;
        $("#dr-description").value = rule.description || "";
        populateRecommendationSelect(rule.recommendation);
        $("#dr-priority").value = rule.priority;
        $("[name='active']").checked = rule.status === "active";

        conditionLogic = rule.conditionLogic === "any" ? "any" : "all";

        conditions = Array.isArray(rule.conditionItems)
            ? rule.conditionItems.map(function (condition) {
                return {
                    id: nextConditionId++,
                    questionId: condition.questionId || "",
                    answerValue: condition.answerValue || ""
                };
            })
            : [];

        $("[data-dr-modal-title]").textContent = "Edit Decision Rule";
        $("[data-dr-save-label]").textContent = "Save Changes";

        renderConditions();
        openModal($("[data-dr-modal]"));

        $("#dr-name").focus();
    }

    function makeUniqueKey (baseKey) {
        let key = baseKey;
        let suffix = 2;

        while (rules.some(function (rule) {
            return rule.key.toLowerCase() === key.toLowerCase();
        })) {
            key = baseKey + "_" + suffix++;
        }

        return key;
    }

    async function handleSave (event) {
        event.preventDefault();

        const form = $("[data-dr-form]");
        const validation = $("[data-dr-validation]");

        validation.classList.remove("is-visible");

        if (!form.reportValidity()) {
            return;
        }

        if (!validateConditions()) {
            return;
        }

        const name = $("#dr-name").value.trim();
        const key = $("#dr-key").value.trim();
        const description = $("#dr-description").value.trim();
        const recommendation = $("#dr-recommendation").value;
        const priority = Number($("#dr-priority").value);
        const active = $("[name='active']").checked;

        if (!name || !key || !recommendation ||
            !Number.isInteger(priority) ||
            priority < 1 || priority > 999) {

            validation.textContent =
                "Please complete the required fields correctly.";

            validation.classList.add("is-visible");
            return;
        }

        const duplicateKey = rules.some(function (rule) {
            return rule.key.toLowerCase() === key.toLowerCase() &&
                rule.id !== editingId;
        });

        if (duplicateKey) {
            validation.textContent =
                "This unique identifier is already in use.";

            validation.classList.add("is-visible");
            $("#dr-key").focus();
            return;
        }

        const existing = editingId
            ? rules.find(function (rule) {
                return rule.id === editingId;
            })
            : null;

        const savedConditions = conditions.map(function (condition) {
            return {
                id: condition.id,
                questionId: condition.questionId,
                answerValue: condition.answerValue
            };
        });

        const record = {
            id: editingId !== null ? editingId : nextId++,
            name: name,
            key: key,
            description: description,
            recommendation: recommendation,
            conditions: savedConditions.length,
            conditionLogic: conditionLogic,
            conditionItems: savedConditions,
            priority: priority,
            status: active ? "active" : "inactive"
        };

        const nextRules = editingId !== null
            ? rules.map(function (rule) {
                return rule.id === editingId ? record : rule;
            })
            : [ ...rules, record ];

        const wasEditing = editingId !== null;
        try {
            await persistRules(nextRules);
            closeModal($("[data-dr-modal]"));
            editingId = null;
            showToast(wasEditing ? "Rule saved to the widget draft." : "Rule added to the widget draft.");
        } catch (error) {
            validation.textContent = error.message;
            validation.classList.add("is-visible");
        }
    }

    async function duplicateRule (id) {
        const source = rules.find(function (rule) {
            return rule.id === id;
        });

        if (!source) {
            return;
        }

        const copy = {
            ...source,
            id: `new-${ Date.now() }`,
            name: source.name + " (Copy)",
            key: makeUniqueKey(source.key + "_copy"),
            conditionItems: Array.isArray(source.conditionItems)
                ? source.conditionItems.map(function (condition) {
                    return {
                        ...condition,
                        id: nextConditionId++
                    };
                })
                : [],
            status: "draft",
            priority: Math.max(
                1,
                ...rules.map(function (rule) {
                    return rule.priority;
                })
            ) + 1
        };

        copy.conditions = copy.conditionItems.length;

        try { await persistRules([ ...rules, copy ]); showToast("Rule duplicated to the widget draft."); }
        catch (error) { showApiMessage(error.message, true); }
    }

    async function toggleRule (id) {
        const rule = rules.find(function (item) {
            return item.id === id;
        });

        if (!rule) {
            return;
        }

        const nextRules = rules.map(item => item.id === id ? { ...item, status: item.status === "active" ? "inactive" : "active" } : item);
        try { await persistRules(nextRules); showToast(nextRules.find(item => item.id === id).status === "active" ? "Rule activated." : "Rule deactivated."); }
        catch (error) { showApiMessage(error.message, true); }
    }

    function askDelete (id) {
        const rule = rules.find(function (item) {
            return item.id === id;
        });

        if (!rule) {
            return;
        }

        deletingId = id;

        $("[data-dr-delete-message]").textContent =
            `Delete "${ rule.name }" from the saved draft?`;

        openModal($("[data-dr-delete-modal]"));
    }

    async function confirmDelete () {
        if (deletingId === null) {
            return;
        }

        const nextRules = rules.filter(rule => rule.id !== deletingId);
        try {
            await persistRules(nextRules);
            deletingId = null;
            closeModal($("[data-dr-delete-modal]"));
            showToast("Rule deleted from the widget draft.");
        } catch (error) { showApiMessage(error.message, true); }
    }



    function resetTestRulePreview () {
        const result = $("[data-dr-test-result]");

        // Hide and clear the previous result.
        if (result) {
            result.hidden = true;
            result.classList.remove("is-match", "is-no-match");
            result.innerHTML = "";
        }

        root.querySelectorAll("[data-dr-test-question]").forEach(select => { select.selectedIndex = 0; });
    }




    function testCurrentRule () {
        const result = $("[data-dr-test-result]");

        if (!result) {
            console.error("Decision Rules: Test Rule result container was not found.");
            return;
        }

        const visitorAnswers = {};
        root.querySelectorAll("[data-dr-test-question]").forEach(select => {
            visitorAnswers[ select.dataset.drTestQuestion ] = select.value;
        });

        // A condition can only match when its answer has been selected.
        const conditionResults = conditions.map(function (condition, index) {
            const question = getQuestion(condition.questionId);
            const expectedAnswer = condition.answerValue || "";
            const visitorAnswer = visitorAnswers[ condition.questionId ] || "";

            const questionLabel = question
                ? question.text
                : getQuestionText(condition.questionId);

            const expectedLabel = expectedAnswer
                ? getAnswerText(condition.questionId, expectedAnswer)
                : "No answer configured";

            const visitorLabel = visitorAnswer
                ? getAnswerText(condition.questionId, visitorAnswer)
                : "Not answered";

            const isAnswered = visitorAnswer !== "";
            const matches = isAnswered &&
                expectedAnswer !== "" &&
                visitorAnswer === expectedAnswer;

            return {
                index: index + 1,
                questionLabel: questionLabel,
                expectedLabel: expectedLabel,
                visitorLabel: visitorLabel,
                isAnswered: isAnswered,
                matches: matches
            };
        });

        // Do not claim that an empty rule matches.
        const hasConditions = conditionResults.length > 0;
        const logic = conditionLogic === "any" ? "ANY" : "ALL";

        const ruleMatches = hasConditions && (
            logic === "ALL"
                ? conditionResults.every(item => item.matches)
                : conditionResults.some(item => item.matches)
        );

        const recommendationSelect = $("#dr-recommendation");
        const recommendation = recommendationSelect
            ? recommendationSelect.options[
                recommendationSelect.selectedIndex
            ]?.text || "No recommendation selected"
            : "No recommendation selected";

        const resultRows = conditionResults.map(function (item) {
            let status;
            let statusClass;

            if (!item.isAnswered) {
                status = "UNANSWERED";
                statusClass = "dr-test-unanswered";
            } else if (item.matches) {
                status = "MATCH";
                statusClass = "dr-test-match";
            } else {
                status = "NO MATCH";
                statusClass = "dr-test-no-match";
            }

            return `
                <div class="dr-test-condition ${ statusClass }">
                    <strong>${ status } — Condition ${ item.index }</strong>
                    <div>
                        ${ escapeHtml(item.questionLabel) }:
                        expected
                        <strong>${ escapeHtml(item.expectedLabel) }</strong>;
                        visitor answered
                        <strong>${ escapeHtml(item.visitorLabel) }</strong>.
                    </div>
                </div>
            `;
        }).join("");

        let heading;
        let explanation;
        let resultClass;

        if (!hasConditions) {
            heading = "No conditions configured";
            explanation = "Add at least one condition before testing this rule.";
            resultClass = "is-no-match";
        } else if (ruleMatches) {
            heading = "Rule matches";
            explanation = "This rule qualifies for the selected visitor answers.";
            resultClass = "is-match";
        } else {
            heading = "Rule does not match";
            explanation = logic === "ALL"
                ? "Every condition must match, but one or more conditions failed or were unanswered."
                : "At least one condition must match, but none of the configured conditions matched.";
            resultClass = "is-no-match";
        }

        result.classList.remove("is-match", "is-no-match");
        result.classList.add(resultClass);
        result.hidden = false;

        result.innerHTML = `
            <div class="dr-test-result-heading">
                <strong>${ heading }</strong>
            </div>

            <p>
                Logic: <strong>${ logic }</strong>
            </p>

            <p>${ explanation }</p>

            <p>
                Configured recommendation:
                <strong>${ escapeHtml(recommendation) }</strong>
            </p>

            <div class="dr-test-condition-results">
                <strong>Condition-by-condition results</strong>
                ${ resultRows || "<p>No conditions available to evaluate.</p>" }
            </div>

            <p class="dr-test-preview-note">
                Preview only. No rule was saved or changed.
            </p>
        `;
    }


    // =========================================
    // Event handlers
    // =========================================


    root.addEventListener("click", function (event) {
        const button = event.target.closest("[data-dr-action]");

        if (!button || !root.contains(button)) {
            return;
        }

        const action = button.dataset.drAction;
        const id = button.dataset.id;

        switch (action) {

            case "test-rule":
                testCurrentRule();
                break;

            case "add":
                openCreate();
                break;

            case "edit":
                openEdit(id);
                break;

            case "duplicate":
                duplicateRule(id);
                break;

            case "toggle":
                toggleRule(id);
                break;

            case "delete":
                askDelete(id);
                break;

            case "close":
                closeModal($("[data-dr-modal]"));
                editingId = null;
                break;

            case "cancel-delete":
                closeModal($("[data-dr-delete-modal]"));
                deletingId = null;
                break;

            case "confirm-delete":
                confirmDelete();
                break;

            case "add-condition":
                addCondition();
                break;

            case "remove-condition":
                removeCondition(Number(button.dataset.conditionId));
                break;
        }
    });

    // ALL / ANY condition logic
    root.querySelectorAll("[data-dr-logic]").forEach(function (button) {
        button.addEventListener("click", function () {
            setConditionLogic(button.dataset.drLogic);
        });
    });

    // Question selection changes
    root.addEventListener("change", function (event) {
        const questionSelect = event.target.closest("[data-dr-question]");

        if (questionSelect) {
            const conditionId = Number(questionSelect.dataset.drQuestion);
            updateConditionQuestion(conditionId, questionSelect.value);
            return;
        }

        // Answer selection changes
        const answerSelect = event.target.closest("[data-dr-answer]");

        if (answerSelect) {
            const conditionId = Number(answerSelect.dataset.drAnswer);
            updateConditionAnswer(conditionId, answerSelect.value);
        }
    });

    // Form submission
    $("[data-dr-form]").addEventListener("submit", handleSave);

    // Search and status filtering
    $("[data-dr-search]").addEventListener("input", renderRows);
    $("[data-dr-filter]").addEventListener("change", renderRows);

    // Close a modal when clicking its backdrop
    root.querySelectorAll(".dr-modal-overlay").forEach(function (modal) {
        modal.addEventListener("click", function (event) {
            if (event.target !== modal) {
                return;
            }

            closeModal(modal);

            if (modal === $("[data-dr-delete-modal]")) {
                deletingId = null;
            }

            if (modal === $("[data-dr-modal]")) {
                editingId = null;
            }
        });
    });

    // Escape key closes open modals
    document.addEventListener("keydown", function (event) {
        if (event.key !== "Escape") {
            return;
        }

        root.querySelectorAll(".dr-modal-overlay.is-open").forEach(function (modal) {
            closeModal(modal);
        });

        deletingId = null;
        editingId = null;
    });

    // =========================================
    // Initialize the page
    // =========================================

    root.querySelector("#drWidgetSelect").addEventListener("change", async function () {
        selectedWidgetKey = this.value;
        localStorage.setItem("askpearls.activeWidgetPublicKey", selectedWidgetKey);
        try { await loadWidgetData(); }
        catch (error) { rules = []; questionOptions = []; recommendationOptions = []; renderTestQuestions(); populateRecommendationSelect(); renderRows(); showApiMessage(error.message, true); }
    });
    loadRulesPage();

})();
