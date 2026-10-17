    (() => {
        "use strict";

        const root = document.querySelector('[data-ap-page="questions"]');
        if (!root || root.dataset.initialized === "true") return;
        root.dataset.initialized = "true";

        const $ = id => root.querySelector(`#${id}`);

        let selectedId = 1;
        let editingId = null;
        let nextId = 4;
        let previousFocus = null;
        let toastTimer = null;

        let questions = [
            {
                id: 1,
                text: "How many people will use the sofa?",
                key: "users",
                helper: "Choose the option that best matches your needs.",
                required: true,
                options: ["1–2 people", "3–4 people", "5 or more people"]
            },
            {
                id: 2,
                text: "What matters most to you?",
                key: "priority",
                helper: "Choose what matters most.",
                required: true,
                options: ["Comfort", "Style", "Space"]
            },
            {
                id: 3,
                text: "How much space do you have?",
                key: "space",
                helper: "Choose the option closest to your available space.",
                required: true,
                options: ["Small", "Medium", "Large"]
            }
        ];

        const backdrop = $("qnaModalBackdrop");
        const form = $("qnaQuestionForm");
        const answerList = $("qnaAnswerEditorList");
        const modalBody = backdrop.querySelector(".qna-modal-body");

        function escapeHtml(value) {
            return String(value).replace(/[&<>"']/g, character => ({
                "&": "&amp;",
                "<": "&lt;",
                ">": "&gt;",
                '"': "&quot;",
                "'": "&#39;"
            })[character]);
        }

        function selectedQuestion() {
            return questions.find(question => question.id === selectedId) || null;
        }

        function markDirty() {
            $("draftStatus").textContent = "Unsaved draft";
        }

        function render() {
            $("questionCount").textContent = questions.length;

            $("questionList").innerHTML = questions.length
                ? questions.map((question, index) => `
                    <button type="button"
                        class="question-item ${question.id === selectedId ? "active" : ""}"
                        data-select-question="${question.id}">
                        <span class="question-number">${index + 1}</span>
                        <span class="question-label">
                            <strong>${escapeHtml(question.text)}</strong>
                            <small>${escapeHtml(question.key)}</small>
                        </span>
                    </button>
                `).join("")
                : `<p class="empty-state">No questions yet. Select Add to create your first question.</p>`;

            const question = selectedQuestion();
            $("editQuestionButton").disabled = !question;

            if (!question) {
                $("questionOverview").innerHTML =
                    `<p class="empty-state">Select Add Question to get started.</p>`;
                renderPreview(null);
                return;
            }

            $("questionOverview").innerHTML = `
                <div class="field">
                    <div class="field-label">Question text</div>
                    <div class="readonly-value">${escapeHtml(question.text)}</div>
                </div>
                <div class="field">
                    <div class="field-label">Question identifier</div>
                    <div class="readonly-value">${escapeHtml(question.key)}</div>
                </div>
                <div class="field">
                    <div class="field-label">Helper text</div>
                    <div class="readonly-value">${escapeHtml(question.helper || "—")}</div>
                </div>
                <div class="field">
                    <div class="field-label">Required question</div>
                    <div class="readonly-value">${question.required ? "Yes" : "No"}</div>
                </div>
                <div class="field">
                    <div class="field-label">Answer options (${question.options.length})</div>
                    ${question.options.map((option, index) => `
                        <div class="option-display">
                            <span>${index + 1}</span>
                            <span>${escapeHtml(option)}</span>
                        </div>
                    `).join("")}
                </div>
            `;

            renderPreview(question);
        }

        function renderPreview(question) {
            const total = questions.length;
            const index = question ? questions.findIndex(item => item.id === question.id) : 0;
            const number = question ? index + 1 : 0;
            const percent = total ? Math.round(number / total * 100) : 0;

            $("previewProgressLabel").textContent =
                total ? `Question ${number} of ${total}` : "No questions";
            $("previewProgressPercent").textContent = `${percent}%`;
            $("previewProgressFill").style.width = `${percent}%`;
            $("previewQuestion").textContent =
                question ? question.text : "Your question will appear here.";
            $("previewHelper").textContent = question ? question.helper : "";

            $("previewOptions").innerHTML = question
                ? question.options.map(option =>
                    `<div class="preview-option">${escapeHtml(option)}</div>`
                  ).join("")
                : "";
        }

        function showToast(message) {
            const toast = $("toast");
            toast.textContent = message;
            toast.hidden = false;

            clearTimeout(toastTimer);
            toastTimer = setTimeout(() => {
                toast.hidden = true;
            }, 2600);
        }

        function clearError() {
            $("qnaModalError").textContent = "";
            $("qnaModalError").classList.remove("visible");
        }

        function showError(message) {
            $("qnaModalError").textContent = message;
            $("qnaModalError").classList.add("visible");
        }

        function addOption(value = "") {
            const count = answerList.children.length;

            if (count >= 30) {
                showError("You can add up to 30 answer options per question.");
                return;
            }

            clearError();

            const row = document.createElement("div");
            row.className = "qna-answer-row";
            row.innerHTML = `
                <span class="qna-answer-number"></span>
                <input class="qna-form-control qna-answer-input"
                       maxlength="150" required
                       placeholder="Enter answer option">
                <button type="button" class="qna-remove-option"
                        aria-label="Remove answer option">×</button>
            `;

            row.querySelector("input").value = value;

            row.querySelector(".qna-remove-option").addEventListener("click", () => {
                if (answerList.children.length <= 2) {
                    showError("Each question must have at least two answer options.");
                    return;
                }

                row.remove();
                updateOptionNumbers();
                clearError();
            });

            answerList.appendChild(row);
            updateOptionNumbers();
        }

        function updateOptionNumbers() {
            [...answerList.children].forEach((row, index) => {
                row.querySelector(".qna-answer-number").textContent = index + 1;
            });
        }

        function openModal(question = null) {
            previousFocus = document.activeElement;
            editingId = question ? question.id : null;

            $("qnaModalTitle").textContent = question ? "Edit Question" : "Add Question";
            $("qnaInputText").value = question ? question.text : "";
            $("qnaInputKey").value = question ? question.key : "";
            $("qnaInputHelper").value = question ? question.helper : "";
            $("qnaInputRequired").checked = question ? question.required : true;
            $("qnaSaveQuestionButton").textContent =
                question ? "✓ Save Changes" : "✓ Save Question";

            answerList.replaceChildren();

            const options = question ? question.options : ["", ""];
            options.forEach(option => addOption(option));

            clearError();

            backdrop.hidden = false;
            backdrop.setAttribute("aria-hidden", "false");
            document.body.style.overflow = "hidden";

            modalBody.scrollTop = 0;

            requestAnimationFrame(() => {
                $("qnaInputText").focus();
            });
        }

        function closeModal() {
            backdrop.hidden = true;
            backdrop.setAttribute("aria-hidden", "true");
            document.body.style.overflow = "";
            clearError();

            if (previousFocus && typeof previousFocus.focus === "function") {
                previousFocus.focus();
            }
        }

        function saveQuestion(event) {
            event.preventDefault();
            clearError();

            const text = $("qnaInputText").value.trim();
            const key = $("qnaInputKey").value.trim();
            const helper = $("qnaInputHelper").value.trim();
            const required = $("qnaInputRequired").checked;
            const options = [...answerList.querySelectorAll(".qna-answer-input")]
                .map(input => input.value.trim());

            if (!text) {
                showError("Enter the question text.");
                $("qnaInputText").focus();
                return;
            }

            if (!/^[a-zA-Z][a-zA-Z0-9_]*$/.test(key)) {
                showError("The identifier must start with a letter and contain only letters, numbers, or underscores.");
                $("qnaInputKey").focus();
                return;
            }

            const duplicateKey = questions.some(question =>
                question.key.toLowerCase() === key.toLowerCase() &&
                question.id !== editingId
            );

            if (duplicateKey) {
                showError("That question identifier is already in use. Choose a unique identifier.");
                $("qnaInputKey").focus();
                return;
            }

            if (options.length < 2 || options.some(option => !option)) {
                showError("Enter at least two answer options, and make sure none are empty.");
                return;
            }

            if (new Set(options.map(option => option.toLowerCase())).size !== options.length) {
                showError("Answer options must be distinct. Remove duplicate options.");
                return;
            }

            if (editingId !== null) {
                const index = questions.findIndex(question => question.id === editingId);

                if (index !== -1) {
                    questions[index] = {
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
                    id: nextId++,
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
            showToast("Question saved in this page's temporary UI state.");
        }

        $("addQuestionButton").addEventListener("click", () => openModal(null));

        $("editQuestionButton").addEventListener("click", () => {
            const question = selectedQuestion();
            if (question) openModal(question);
        });

        $("qnaModalClose").addEventListener("click", closeModal);
        $("qnaCancelButton").addEventListener("click", closeModal);
        $("qnaAddOptionButton").addEventListener("click", () => addOption());
        form.addEventListener("submit", saveQuestion);

        backdrop.addEventListener("click", event => {
            if (event.target === backdrop) closeModal();
        });

        root.addEventListener("keydown", event => {
            if (event.key === "Escape" && !backdrop.hidden) closeModal();
        });

        $("questionList").addEventListener("click", event => {
            const button = event.target.closest("[data-select-question]");
            if (!button) return;

            selectedId = Number(button.dataset.selectQuestion);
            render();
        });

        $("saveDraftButton").addEventListener("click", () => {
            $("draftStatus").textContent = "Draft saved locally for this session";
            showToast("Demo only: connect persistence after the UI is complete.");
        });

        $("previewButton").addEventListener("click", () => {
            root.querySelector(".preview-column").scrollIntoView({
                behavior: "smooth",
                block: "start"
            });
        });

        render();
    })();
