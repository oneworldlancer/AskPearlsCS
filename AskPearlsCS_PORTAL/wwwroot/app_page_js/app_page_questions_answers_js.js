
(() => {
    "use strict";

    const root = document.querySelector('[data-ap-page="questions"]');
    if (!root || root.dataset.initialized === "true") return;
    root.dataset.initialized = "true";

    const $ = id => root.querySelector(`#${ id }`);

    let selectedId = null;
    let editingId = null;
    let nextId = 1;
    let previousFocus = null;
    let toastTimer = null;
    let questions = [];
    let questionsLoaded = false;
    let draftQuestionsRouteAvailable = false;
    let widgets = [];

    const apiBase = (
        root.dataset.apiBase || "/Widget/Api"
    ).replace(/\/+$/, "");

    const backdrop = $("qnaModalBackdrop");
    const form = $("qnaQuestionForm");
    const answerList = $("qnaAnswerEditorList");
    const modalBody = backdrop.querySelector(".qna-modal-body");

    // =====================================================
    // API HELPERS
    // =====================================================

    function apiHeaders () {
        return { Accept: "application/json" };
    }

    async function readApiResponse (response) {
        const text = await response.text();
        let data = {};

        if (text) {
            try {
                data = JSON.parse(text);
            } catch {
                throw new Error(
                    "The API returned an unreadable response."
                );
            }
        }

        if (!response.ok) {
            throw new Error(
                data.message ||
                "API request failed (HTTP " + response.status + ")."
            );
        }

        return data;
    }

    function showApiMessage (message, isError = false) {
        const box = $("qnaApiMessage");

        box.textContent = message;
        box.hidden = !message;
        box.classList.toggle("is-error", Boolean(isError));
    }

    function antiforgeryToken () {
        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value ||
            document.querySelector(
                'meta[name="request-verification-token"]'
            )?.content ||
            document.querySelector(
                'meta[name="csrf-token"]'
            )?.content ||
            "";
    }

    // =====================================================
    // GLOBAL WIDGET VERSION HELPER
    // =====================================================

    function updateSelectedWidgetVersion (widget) {
        if (!widget) {
            $("qnaWidgetVersion").textContent = "—";
            return;
        }

        if (!window.AskPearlsWidgetVersion?.update) {
            console.error(
                "app_widget_version_helper.js must load " +
                "before Questions & Answers."
            );
            return;
        }

        window.AskPearlsWidgetVersion.update(
            "qnaWidgetVersion",
            widget
        );
    }

    // =====================================================
    // EDITOR STATE
    // =====================================================

    function setEditorEnabled (enabled) {
        $("addQuestionButton").disabled = !enabled;

        $("saveDraftButton").disabled =
            !enabled || !draftQuestionsRouteAvailable;

        $("previewButton").disabled = !enabled;

        $("editQuestionButton").disabled =
            !enabled || !selectedQuestion();
    }

    function resetSelection () {
        questions = [];
        selectedId = null;
        editingId = null;
        questionsLoaded = false;
        draftQuestionsRouteAvailable = false;

        $("qnaWidgetName").textContent = "No widget selected";

        $("qnaWidgetSummary").textContent =
            "Select a widget above to manage its questions.";

        updateSelectedWidgetVersion(null);

        $("draftStatus").textContent = "Select a widget";

        showApiMessage("");
        render();
        setEditorEnabled(false);
    }

    function currentWidget () {
        return widgets.find(
            widget =>
                widget.publicKey === $("qnaWidgetSelect").value
        ) || null;
    }

    // =====================================================
    // NORMALIZE QUESTIONS
    // =====================================================

    function normalizeQuestion (item, index) {
        let extra = {};

        const raw =
            item.configuration ??
            item.configurationJson ??
            item.widgetConfigurationJson;

        if (typeof raw === "string") {
            try {
                extra = JSON.parse(raw);
            } catch {
                extra = {};
            }
        } else if (raw && typeof raw === "object") {
            extra = raw;
        }

        const answers = Array.isArray(item.answers)
            ? item.answers
            : Array.isArray(item.options)
                ? item.options
                : [];

        const options = answers.map(answer =>
            typeof answer === "string"
                ? answer
                : String(
                    answer.text ??
                    answer.title ??
                    answer.name ??
                    answer.key ??
                    ""
                )
        ).filter(Boolean);

        return {
            id: String(
                item.id ??
                item.questionTokenId ??
                item.question_token_id ??
                index + 1
            ),

            text: String(
                item.text ??
                item.title ??
                item.questionText ??
                "Untitled question"
            ),

            key: String(
                item.key ??
                item.name ??
                item.code ??
                "question_" + (index + 1)
            ),

            helper: String(
                item.description ??
                item.helper ??
                extra.helper ??
                ""
            ),

            required:
                item.required ??
                extra.required ??
                true,

            options: options.length
                ? options
                : [ "Option 1", "Option 2" ]
        };
    }

    // =====================================================
    // LOAD QUESTIONS FOR SELECTED WIDGET
    // =====================================================

    async function loadQuestionsForWidget () {
        const widget = currentWidget();

        if (!widget) {
            resetSelection();
            return;
        }

        questions = [];
        selectedId = null;

        setEditorEnabled(false);

        questionsLoaded = false;
        draftQuestionsRouteAvailable = false;

        $("questionList").innerHTML =
            '<p class="empty-state">Loading questions…</p>';

        $("questionOverview").innerHTML =
            '<p class="empty-state">Loading question details…</p>';

        updateSelectedWidgetVersion(widget);

        try {
            const path =
                apiBase +
                "/widgets/" +
                encodeURIComponent(widget.publicKey);

            let questionResult;
            let usedPublishedFallback = false;

            try {
                questionResult = await readApiResponse(
                    await fetch(
                        path + "/draft/questions",
                        { headers: apiHeaders() }
                    )
                );

                draftQuestionsRouteAvailable = true;

            } catch (draftError) {
                if (
                    !String(
                        draftError.message || ""
                    ).includes("HTTP 404")
                ) {
                    throw draftError;
                }

                questionResult = await readApiResponse(
                    await fetch(
                        path + "/questions",
                        { headers: apiHeaders() }
                    )
                );

                usedPublishedFallback = true;
            }

            // Preserve the original version endpoint request.
            const versionResult = await fetch(
                path + "/version",
                { headers: apiHeaders() }
            ).then(readApiResponse).catch(() => ({}));

            if (
                currentWidget()?.publicKey !== widget.publicKey
            ) {
                return;
            }

            const rows = Array.isArray(
                questionResult.questions
            )
                ? questionResult.questions
                : [];

            questions = rows.map(normalizeQuestion);

            questionsLoaded = true;
            nextId = questions.length + 1;

            selectedId = questions.length
                ? questions[ 0 ].id
                : null;

            const version =
                questionResult.versionNumber ||
                (
                    questionResult.status === "draft"
                        ? widget.draftVersionNumber
                        : widget.publishedVersionNumber
                ) ||
                widget.versionNumber;

            // Only update the draft number if the
            // response explicitly represents a draft.
            if (
                questionResult.status === "draft" &&
                version
            ) {
                widget.hasDraft = true;
                widget.draftVersionNumber = String(version);
            }

            // One shared formatter for Draft and Live.
            updateSelectedWidgetVersion(widget);

            $("qnaWidgetName").textContent = widget.name;

            $("qnaWidgetSummary").textContent =
                questionResult.status === "draft"
                    ? "Questions loaded from the saved draft version. " +
                    "The live published version is unchanged."
                    : "Questions loaded from the live published version.";

            render();

            showApiMessage(
                usedPublishedFallback
                    ? "Showing published questions. The API " +
                    "draft-questions route is not available yet, " +
                    "so draft-only changes cannot load or save."
                    : questions.length
                        ? "Questions loaded from the selected widget. " +
                        "Add or edit questions, then save the draft to the API."
                        : "No questions exist yet. Add your first " +
                        "question and save the draft.",
                usedPublishedFallback
            );

            $("draftStatus").textContent =
                questionResult.status === "draft"
                    ? "Saved draft loaded"
                    : "Published questions loaded";

            setEditorEnabled(draftQuestionsRouteAvailable);

        } catch (error) {
            if (
                currentWidget()?.publicKey !== widget.publicKey
            ) {
                return;
            }

            questions = [];
            selectedId = null;

            render();

            showApiMessage(
                error.message ||
                "Could not load this widget's questions.",
                true
            );

            $("draftStatus").textContent =
                "Questions could not be loaded";

            setEditorEnabled(false);
        }
    }

    // =====================================================
    // LOAD WIDGETS
    // =====================================================

    async function loadWidgets () {
        const select = $("qnaWidgetSelect");
        select.disabled = true;

        try {
            const data = await readApiResponse(
                await fetch(
                    apiBase + "/widgets",
                    { headers: apiHeaders() }
                )
            );

            widgets = (
                Array.isArray(data.widgets)
                    ? data.widgets
                    : []
            ).map(item => ({
                publicKey: String(
                    item.publicKey ??
                    item.PublicKey ??
                    ""
                ),

                name: String(
                    item.name ??
                    item.Name ??
                    "Untitled widget"
                ),

                versionNumber: String(
                    item.versionNumber ??
                    item.VersionNumber ??
                    ""
                ),

                draftVersionNumber: String(
                    item.draftVersionNumber ??
                    item.DraftVersionNumber ??
                    ""
                ),

                publishedVersionNumber: String(
                    item.publishedVersionNumber ??
                    item.PublishedVersionNumber ??
                    ""
                ),

                hasDraft: Boolean(
                    item.hasDraft ??
                    item.HasDraft ??
                    (
                        String(
                            item.status ??
                            item.Status ??
                            ""
                        ).toLowerCase() === "draft"
                    )
                ),

                hasPublishedVersion: Boolean(
                    item.hasPublishedVersion ??
                    item.HasPublishedVersion ??
                    (
                        String(
                            item.status ??
                            item.Status ??
                            ""
                        ).toLowerCase() === "published"
                    )
                )
            })).filter(item => item.publicKey);

            select.replaceChildren(
                new Option("-- Select a widget --", "")
            );

            select.value = "";
            resetSelection();

            if (!widgets.length) {
                select.add(
                    new Option("No widgets available", "")
                );

                $("qnaWidgetName").textContent =
                    "No widget selected";

                $("qnaWidgetSummary").textContent =
                    "Create a widget before managing questions.";

                questions = [];
                render();

                showApiMessage(
                    "No widgets were returned by the API.",
                    true
                );

                return;
            }

            widgets.forEach(widget => {
                select.add(
                    new Option(
                        widget.name,
                        widget.publicKey
                    )
                );
            });

            select.disabled = false;

            // Require manual widget selection.

        } catch (error) {
            select.replaceChildren(
                new Option("Could not load widgets", "")
            );

            questions = [];
            resetSelection();

            showApiMessage(
                error.message ||
                "Could not load widgets from the API.",
                true
            );
        }
    }

    // =====================================================
    // HTML HELPERS
    // =====================================================

    function escapeHtml (value) {
        return String(value).replace(
            /[&<>"']/g,
            character => ({
                "&": "&amp;",
                "<": "&lt;",
                ">": "&gt;",
                '"': "&quot;",
                "'": "&#39;"
            })[ character ]
        );
    }

    function selectedQuestion () {
        return questions.find(
            question => question.id === selectedId
        ) || null;
    }

    function markDirty () {
        $("draftStatus").textContent = "Unsaved draft";
    }

    // =====================================================
    // RENDER QUESTION LIST
    // =====================================================

    function render () {
        $("questionCount").textContent = questions.length;

        $("questionList").innerHTML = questions.length
            ? questions.map((question, index) => `
                <button type="button"
                    class="question-item ${ question.id === selectedId ? "active" : "" }"
                    data-select-question="${ escapeHtml(question.id) }">

                    <span class="question-number">
                        ${ index + 1 }
                    </span>

                    <span class="question-label">
                        <strong>
                            ${ escapeHtml(question.text) }
                        </strong>
                        <small>
                            ${ escapeHtml(question.key) }
                        </small>
                    </span>
                </button>
            `).join("")
            : `<p class="empty-state">
                No questions yet. Select Add to create your first question.
               </p>`;

        const question = selectedQuestion();

        $("editQuestionButton").disabled =
            !question ||
            !currentWidget() ||
            !questionsLoaded;

        if (!question) {
            $("questionOverview").innerHTML =
                `<p class="empty-state">
                    Select Add Question to get started.
                 </p>`;

            renderPreview(null);
            return;
        }

        $("questionOverview").innerHTML = `
            <div class="field">
                <div class="field-label">Question text</div>
                <div class="readonly-value">
                    ${ escapeHtml(question.text) }
                </div>
            </div>

            <div class="field">
                <div class="field-label">Question identifier</div>
                <div class="readonly-value">
                    ${ escapeHtml(question.key) }
                </div>
            </div>

            <div class="field">
                <div class="field-label">Helper text</div>
                <div class="readonly-value">
                    ${ escapeHtml(question.helper || "—") }
                </div>
            </div>

            <div class="field">
                <div class="field-label">Required question</div>
                <div class="readonly-value">
                    ${ question.required ? "Yes" : "No" }
                </div>
            </div>

            <div class="field">
                <div class="field-label">
                    Answer options (${ question.options.length })
                </div>

                ${ question.options.map((option, index) => `
                    <div class="option-display">
                        <span>${ index + 1 }</span>
                        <span>${ escapeHtml(option) }</span>
                    </div>
                `).join("") }
            </div>
        `;

        renderPreview(question);
    }

    // =====================================================
    // PREVIEW
    // =====================================================

    function renderPreview (question) {
        const total = questions.length;

        const index = question
            ? questions.findIndex(
                item => item.id === question.id
            )
            : 0;

        const number = question ? index + 1 : 0;

        const percent = total
            ? Math.round(number / total * 100)
            : 0;

        $("previewProgressLabel").textContent =
            total
                ? `Question ${ number } of ${ total }`
                : "No questions";

        $("previewProgressPercent").textContent =
            `${ percent }%`;

        $("previewProgressFill").style.width =
            `${ percent }%`;

        $("previewQuestion").textContent =
            question
                ? question.text
                : "Your question will appear here.";

        $("previewHelper").textContent =
            question ? question.helper : "";

        $("previewOptions").innerHTML = question
            ? question.options.map(option =>
                `<div class="preview-option">
                    ${ escapeHtml(option) }
                 </div>`
            ).join("")
            : "";
    }

    // =====================================================
    // TOAST AND VALIDATION MESSAGES
    // =====================================================

    function showToast (message) {
        const toast = $("toast");

        toast.textContent = message;
        toast.hidden = false;

        clearTimeout(toastTimer);

        toastTimer = setTimeout(() => {
            toast.hidden = true;
        }, 2600);
    }

    function clearError () {
        $("qnaModalError").textContent = "";
        $("qnaModalError").classList.remove("visible");
    }

    function showError (message) {
        $("qnaModalError").textContent = message;
        $("qnaModalError").classList.add("visible");
    }

    // =====================================================
    // ANSWER OPTIONS
    // =====================================================

    function addOption (value = "") {
        const count = answerList.children.length;

        if (count >= 30) {
            showError(
                "You can add up to 30 answer options per question."
            );
            return;
        }

        clearError();

        const row = document.createElement("div");
        row.className = "qna-answer-row";

        row.innerHTML = `
            <span class="qna-answer-number"></span>

            <input
                class="qna-form-control qna-answer-input"
                maxlength="150"
                required
                placeholder="Enter answer option">

            <button
                type="button"
                class="qna-remove-option"
                aria-label="Remove answer option">
                ×
            </button>
        `;

        row.querySelector("input").value = value;

        row.querySelector(
            ".qna-remove-option"
        ).addEventListener("click", () => {
            if (answerList.children.length <= 2) {
                showError(
                    "Each question must have at least two answer options."
                );
                return;
            }

            row.remove();
            updateOptionNumbers();
            clearError();
        });

        answerList.appendChild(row);
        updateOptionNumbers();
    }

    function updateOptionNumbers () {
        [ ...answerList.children ].forEach(
            (row, index) => {
                row.querySelector(
                    ".qna-answer-number"
                ).textContent = index + 1;
            }
        );
    }

    // =====================================================
    // QUESTION MODAL
    // =====================================================

    function openModal (question = null) {
        previousFocus = document.activeElement;

        editingId = question
            ? question.id
            : null;

        $("qnaModalTitle").textContent =
            question ? "Edit Question" : "Add Question";

        $("qnaInputText").value =
            question ? question.text : "";

        $("qnaInputKey").value =
            question ? question.key : "";

        $("qnaInputHelper").value =
            question ? question.helper : "";

        $("qnaInputRequired").checked =
            question ? question.required : true;

        $("qnaSaveQuestionButton").textContent =
            question
                ? "✓ Save Changes"
                : "✓ Save Question";

        answerList.replaceChildren();

        const options = question
            ? question.options
            : [ "", "" ];

        options.forEach(option => addOption(option));

        clearError();

        backdrop.hidden = false;

        backdrop.setAttribute(
            "aria-hidden",
            "false"
        );

        document.body.style.overflow = "hidden";

        modalBody.scrollTop = 0;

        requestAnimationFrame(() => {
            $("qnaInputText").focus();
        });
    }

    function closeModal () {
        backdrop.hidden = true;

        backdrop.setAttribute(
            "aria-hidden",
            "true"
        );

        document.body.style.overflow = "";

        clearError();

        if (
            previousFocus &&
            typeof previousFocus.focus === "function"
        ) {
            previousFocus.focus();
        }
    }

    // =====================================================
    // SAVE QUESTION LOCALLY
    // =====================================================

    function saveQuestion (event) {
        event.preventDefault();
        clearError();

        const text = $("qnaInputText").value.trim();
        const key = $("qnaInputKey").value.trim();
        const helper = $("qnaInputHelper").value.trim();
        const required = $("qnaInputRequired").checked;

        const options = [
            ...answerList.querySelectorAll(
                ".qna-answer-input"
            )
        ].map(input => input.value.trim());

        if (!text) {
            showError("Enter the question text.");
            $("qnaInputText").focus();
            return;
        }

        if (
            !/^[a-zA-Z][a-zA-Z0-9_]*$/.test(key)
        ) {
            showError(
                "The identifier must start with a letter " +
                "and contain only letters, numbers, or underscores."
            );

            $("qnaInputKey").focus();
            return;
        }

        const duplicateKey = questions.some(
            question =>
                question.key.toLowerCase() ===
                key.toLowerCase() &&
                question.id !== editingId
        );

        if (duplicateKey) {
            showError(
                "That question identifier is already in use. " +
                "Choose a unique identifier."
            );

            $("qnaInputKey").focus();
            return;
        }

        if (
            options.length < 2 ||
            options.some(option => !option)
        ) {
            showError(
                "Enter at least two answer options, " +
                "and make sure none are empty."
            );
            return;
        }

        if (
            new Set(
                options.map(
                    option => option.toLowerCase()
                )
            ).size !== options.length
        ) {
            showError(
                "Answer options must be distinct. " +
                "Remove duplicate options."
            );
            return;
        }

        if (editingId !== null) {
            const index = questions.findIndex(
                question => question.id === editingId
            );

            if (index !== -1) {
                questions[ index ] = {
                    id: editingId,
                    text,
                    key,
                    helper,
                    required,
                    options
                };

                selectedId = editingId;
            }

        } else {
            const question = {
                id: String(nextId++),
                text,
                key,
                helper,
                required,
                options
            };

            questions.push(question);
            selectedId = question.id;
        }

        closeModal();
        render();
        markDirty();

        showToast(
            "Question added to the draft. " +
            "Save Draft to send it to the API."
        );
    }

    // =====================================================
    // MODAL EVENTS
    // =====================================================

    $("addQuestionButton").addEventListener(
        "click",
        () => openModal(null)
    );

    $("editQuestionButton").addEventListener(
        "click",
        () => {
            const question = selectedQuestion();
            if (question) openModal(question);
        }
    );

    $("qnaModalClose").addEventListener(
        "click",
        closeModal
    );

    $("qnaCancelButton").addEventListener(
        "click",
        closeModal
    );

    $("qnaAddOptionButton").addEventListener(
        "click",
        () => addOption()
    );

    form.addEventListener(
        "submit",
        saveQuestion
    );

    backdrop.addEventListener(
        "click",
        event => {
            if (event.target === backdrop) {
                closeModal();
            }
        }
    );

    root.addEventListener(
        "keydown",
        event => {
            if (
                event.key === "Escape" &&
                !backdrop.hidden
            ) {
                closeModal();
            }
        }
    );

    // =====================================================
    // QUESTION SELECTION
    // =====================================================

    $("questionList").addEventListener(
        "click",
        event => {
            const button = event.target.closest(
                "[data-select-question]"
            );

            if (!button) return;

            selectedId = String(
                button.dataset.selectQuestion
            );

            render();
        }
    );

    // =====================================================
    // WIDGET SELECTION
    // =====================================================

    $("qnaWidgetSelect").addEventListener(
        "change",
        loadQuestionsForWidget
    );

    // =====================================================
    // SAVE DRAFT TO API
    // =====================================================

    $("saveDraftButton").addEventListener(
        "click",
        async () => {
            const widget = currentWidget();

            if (!widget) return;

            if (
                !questionsLoaded ||
                !draftQuestionsRouteAvailable
            ) {
                const errorMessage = !questionsLoaded
                    ? "Questions have not loaded, so the draft " +
                    "was not saved. Reload the questions after " +
                    "the API issue is fixed."
                    : "The API draft-questions route is unavailable. " +
                    "The published questions are shown, but saving " +
                    "a draft is disabled until the API is updated.";

                showApiMessage(errorMessage, true);
                showToast("Question draft was not saved.");
                return;
            }

            const button = $("saveDraftButton");
            button.disabled = true;

            $("draftStatus").textContent =
                "Saving draft…";

            try {
                const response = await fetch(
                    apiBase +
                    "/widgets/" +
                    encodeURIComponent(widget.publicKey) +
                    "/draft/questions",
                    {
                        method: "PUT",

                        headers: {
                            ...apiHeaders(),
                            "Content-Type": "application/json",
                            "RequestVerificationToken":
                                antiforgeryToken()
                        },

                        body: JSON.stringify({
                            questions: questions.map(
                                question => ({
                                    text: question.text,
                                    key: question.key,
                                    helper: question.helper,
                                    required: Boolean(
                                        question.required
                                    ),
                                    options: question.options
                                })
                            )
                        })
                    }
                );

                const result = await readApiResponse(
                    response
                );

                $("draftStatus").textContent =
                    "Saved draft";

                widget.hasDraft = true;

                // Update the draft version only.
                // Keep the published version unchanged.
                if (result.versionNumber != null) {
                    widget.draftVersionNumber =
                        String(result.versionNumber);
                }

                updateSelectedWidgetVersion(widget);

                showApiMessage(
                    "Question draft saved to the API. " +
                    "Published questions are unchanged " +
                    "until you publish the draft."
                );

                showToast("Question draft saved.");

                await loadQuestionsForWidget();

            } catch (error) {
                $("draftStatus").textContent =
                    "Draft save failed";

                showApiMessage(
                    error.message ||
                    "Could not save the question draft.",
                    true
                );

                showToast(
                    "Could not save question draft."
                );

            } finally {
                setEditorEnabled(
                    Boolean(currentWidget()) &&
                    questionsLoaded &&
                    draftQuestionsRouteAvailable
                );
            }
        }
    );

    // =====================================================
    // PREVIEW BUTTON
    // =====================================================

    $("previewButton").addEventListener(
        "click",
        () => {
            root.querySelector(
                ".preview-column"
            ).scrollIntoView({
                behavior: "smooth",
                block: "start"
            });
        }
    );

    // =====================================================
    // INITIALIZATION
    // =====================================================

    resetSelection();
    loadWidgets();

})();
