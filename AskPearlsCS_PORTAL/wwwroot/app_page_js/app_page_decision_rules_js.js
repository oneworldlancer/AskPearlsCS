
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

    /*
     * Mock questions and answers for the UI prototype.
     * These will later be replaced by real Questions & Answers data.
     */
    const questionOptions = [
    {
        id: "room_size",
    text: "How large is your living room?",
    answers: [
    { value: "small", text: "Small" },
    { value: "medium", text: "Medium" },
    { value: "large", text: "Large" }
    ]
        },
    {
        id: "budget",
    text: "What is your budget?",
    answers: [
    { value: "budget", text: "Budget-friendly" },
    { value: "mid_range", text: "Mid-range" },
    { value: "premium", text: "Premium" }
    ]
        },
    {
        id: "comfort",
    text: "What matters most to you?",
    answers: [
    { value: "comfort", text: "Comfort" },
    { value: "durability", text: "Durability" },
    { value: "style", text: "Style" }
    ]
        },
    {
        id: "style",
    text: "Which style do you prefer?",
    answers: [
    { value: "modern", text: "Modern" },
    { value: "classic", text: "Classic" },
    { value: "minimalist", text: "Minimalist" }
    ]
        }
    ];

    let rules = [
    {
        id: 1,
    name: "Small Space — Sofa Finder",
    key: "small_space_sofa",
    description: "Recommend a compact sofa for smaller living spaces.",
    recommendation: "Oslo Sofa",
    conditions: 2,
    conditionLogic: "all",
    conditionItems: [
    {
        id: 1,
    questionId: "room_size",
    answerValue: "small"
                },
    {
        id: 2,
    questionId: "budget",
    answerValue: "budget"
                }
    ],
    priority: 1,
    status: "active"
        },
    {
        id: 2,
    name: "Premium Comfort",
    key: "premium_comfort",
    description: "Recommend a comfort-focused option.",
    recommendation: "Luna Sofa",
    conditions: 3,
    conditionLogic: "all",
    conditionItems: [
    {
        id: 3,
    questionId: "budget",
    answerValue: "premium"
                },
    {
        id: 4,
    questionId: "comfort",
    answerValue: "comfort"
                },
    {
        id: 5,
    questionId: "style",
    answerValue: "modern"
                }
    ],
    priority: 2,
    status: "active"
        },
    {
        id: 3,
    name: "Large Living Room",
    key: "large_living_room",
    description: "Recommend an option suitable for larger living areas.",
    recommendation: "Haven Sofa",
    conditions: 1,
    conditionLogic: "all",
    conditionItems: [
    {
        id: 6,
    questionId: "room_size",
    answerValue: "large"
                }
    ],
    priority: 3,
    status: "draft"
        }
    ];

    const $ = function (selector) {
        return root.querySelector(selector);
    };

    function escapeHtml(value) {
        return String(value ?? "").replace(/[&<>"']/g, function (char) {
            return {
            "&": "&amp;",
        "<": "&lt;",
                ">": "&gt;",
        '"': "&quot;",
        "'": "&#39;"
            }[char];
        });
    }

        function showToast(message) {
        const toast = $("[data-dr-toast]");
        toast.textContent = message;
        toast.classList.add("is-visible");

        clearTimeout(toastTimer);

        toastTimer = setTimeout(function () {
            toast.classList.remove("is-visible");
        }, 2600);
    }

        function openModal(modal) {
            modal.classList.add("is-open");
        modal.setAttribute("aria-hidden", "false");
        document.body.style.overflow = "hidden";
    }

        function closeModal(modal) {

            resetTestRulePreview();


        modal.classList.remove("is-open");
        modal.setAttribute("aria-hidden", "true");

        if (!root.querySelector(".dr-modal-overlay.is-open")) {
            document.body.style.overflow = "";
        }
    }

        function getQuestion(questionId) {
        return questionOptions.find(function (question) {
            return question.id === questionId;
        });
    }

        function getAnswer(questionId, answerValue) {
        const question = getQuestion(questionId);

        if (!question) {
            return null;
        }

        return question.answers.find(function (answer) {
            return answer.value === answerValue;
        }) || null;
    }

        function getQuestionText(questionId) {
        const question = getQuestion(questionId);
        return question ? question.text : "Select a question";
    }

        function getAnswerText(questionId, answerValue) {
        const answer = getAnswer(questionId, answerValue);
        return answer ? answer.text : "Select an answer";
    }

        function createCondition(questionId, answerValue) {
        return {
            id: nextConditionId++,
        questionId: questionId || "",
        answerValue: answerValue || ""
        };
    }

        function getConditionSummary(condition) {
        if (!condition.questionId || !condition.answerValue) {
            return "Complete the question and answer selections.";
        }

        return getQuestionText(condition.questionId) +
        " is \"" +
        getAnswerText(condition.questionId, condition.answerValue) +
        "\"";
    }

        function updateConditionCount() {
        const count = conditions.length;

        $("[data-dr-condition-count]").textContent =
        count + " condition" + (count === 1 ? "" : "s");
    }

        function updateLogicButtons() {
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

        function updateRuleSummary() {
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

        function renderConditions() {
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
                rule.recommendation.toLowerCase().includes(search);

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
                    <td>${ escapeHtml(rule.recommendation) }</td>
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
    $("#dr-recommendation").value = rule.recommendation;
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

function handleSave (event) {
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
        status: active ? "active" : "draft"
    };

    if (editingId !== null) {
        rules = rules.map(function (rule) {
            return rule.id === editingId ? record : rule;
        });
    } else {
        rules.push(record);
    }

    const wasEditing = editingId !== null;

    closeModal($("[data-dr-modal]"));

    editingId = null;

    renderRows();

    showToast(
        wasEditing
            ? "Rule updated in the demo."
            : "Rule created in the demo."
    );
}

function duplicateRule (id) {
    const source = rules.find(function (rule) {
        return rule.id === id;
    });

    if (!source) {
        return;
    }

    const copy = {
        ...source,
        id: nextId++,
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

    rules.push(copy);

    renderRows();

    showToast("Rule duplicated as a draft.");
}

function toggleRule (id) {
    const rule = rules.find(function (item) {
        return item.id === id;
    });

    if (!rule) {
        return;
    }

    rule.status = rule.status === "active" ? "inactive" : "active";

    renderRows();

    showToast(
        rule.status === "active"
            ? "Rule activated."
            : "Rule deactivated."
    );
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
        `Delete "${ rule.name }"? This cannot be undone in this demo.`;

    openModal($("[data-dr-delete-modal]"));
}

function confirmDelete () {
    if (deletingId === null) {
        return;
    }

    rules = rules.filter(function (rule) {
        return rule.id !== deletingId;
    });

    deletingId = null;

    closeModal($("[data-dr-delete-modal]"));

    renderRows();

    showToast("Rule deleted from the demo.");
}



function resetTestRulePreview () {
    const result = $("[data-dr-test-result]");

    // Hide and clear the previous result.
    if (result) {
        result.hidden = true;
        result.classList.remove("is-match", "is-no-match");
        result.innerHTML = "";
    }

    // Reset all sample visitor answers.
    [
        "dr-test-room-size",
        "dr-test-budget",
        "dr-test-comfort",
        "dr-test-style"
    ].forEach(function (id) {
        const select = $("#" + id);

        if (select) {
            select.selectedIndex = 0;
        }
    });
}




function testCurrentRule () {
    const result = $("[data-dr-test-result]");

    if (!result) {
        console.error("Decision Rules: Test Rule result container was not found.");
        return;
    }

    // Read all four simulated visitor answers.
    const visitorAnswers = {
        room_size: $("#dr-test-room-size")?.value || "",
        budget: $("#dr-test-budget")?.value || "",
        comfort: $("#dr-test-comfort")?.value || "",
        style: $("#dr-test-style")?.value || ""
    };

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
        explanation = "This rule qualifies for the selected sample answers.";
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
    const id = Number(button.dataset.id);

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

renderRows();

}) ();
