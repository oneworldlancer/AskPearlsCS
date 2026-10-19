
/* ============================================================
   AskPearls DecisionOS Widget
   Framework-independent | Shadow DOM | Server-side scoring
   ============================================================ */

(function (window, document) {
    "use strict";

    // ----------------------------------------------------------
    // 1. Read configuration from the script element
    // ----------------------------------------------------------

    const script = document.currentScript;

    if (!script) {
        console.error("AskPearls: Unable to locate widget script.");
        return;
    }

    const publicKey = (script.dataset.widget || "").trim();

    const apiUrl = (
        script.dataset.apiUrl || "https://api.askpearls.com"
    ).replace(/\/+$/, "");

    if (!publicKey || !apiUrl) {
        console.error(
            "AskPearls: data-widget and data-api-url are required."
        );
        return;
    }

    // Prevent accidental duplicate initialization.
    const instanceKey = "__ASKPEARLS_DECISIONOS_" + publicKey;

    if (window[ instanceKey ]) {
        return;
    }

    window[ instanceKey ] = true;

    // ----------------------------------------------------------
    // 2. Create isolated widget container
    // ----------------------------------------------------------

    const host = document.createElement("div");
    host.id = "askpearls-decisionos-" +
        publicKey.replace(/[^a-zA-Z0-9_-]/g, "-");

    document.body.appendChild(host);

    const root = host.attachShadow
        ? host.attachShadow({ mode: "open" })
        : host;

    // ----------------------------------------------------------
    // 3. Widget styles
    // ----------------------------------------------------------

    const style = document.createElement("style");

    style.textContent = `
        :host { all: initial; }

        * { box-sizing: border-box; }

        .ap-card {
            position: fixed;
            right: 24px;
            bottom: 24px;
            width: 360px;
            max-width: calc(100vw - 32px);
            max-height: calc(100vh - 32px);
            overflow-y: auto;
            background: #fff;
            color: #172033;
            border: 1px solid #e5e7eb;
            border-radius: 18px;
            box-shadow: 0 18px 50px rgba(0,0,0,.16);
            font-family: Arial, Helvetica, sans-serif;
            z-index: 2147483647;
        }

        .ap-head {
            padding: 20px;
            background: #0d6efd;
            color: #fff;
        }

        .ap-brand {
            font-size: 12px;
            font-weight: 700;
            opacity: .92;
            margin-bottom: 5px;
            letter-spacing: .3px;
        }

        .ap-title {
            font-size: 20px;
            line-height: 1.3;
            font-weight: 700;
        }

        .ap-subtitle {
            margin-top: 7px;
            font-size: 13px;
            line-height: 1.45;
        }

        .ap-body { padding: 20px; }

        .ap-progress {
            height: 5px;
            background: #e9ecef;
            border-radius: 10px;
            overflow: hidden;
            margin-bottom: 18px;
        }

        .ap-progress-bar {
            height: 100%;
            background: #0d6efd;
            transition: width .3s ease;
        }

        .ap-question-count {
            margin-bottom: 9px;
            color: #667085;
            font-size: 12px;
        }

        .ap-question {
            font-size: 17px;
            line-height: 1.45;
            font-weight: 700;
            margin-bottom: 17px;
        }

        .ap-options {
            display: flex;
            flex-direction: column;
            gap: 9px;
        }

        .ap-answer {
            width: 100%;
            padding: 13px 14px;
            border: 1px solid #d0d5dd;
            border-radius: 10px;
            background: #fff;
            color: #172033;
            text-align: left;
            font-size: 14px;
            cursor: pointer;
            transition: background .15s, border-color .15s;
        }

        .ap-answer:hover,
        .ap-answer:focus-visible {
            background: #eff6ff;
            border-color: #0d6efd;
            outline: none;
        }

        .ap-answer:disabled {
            opacity: .6;
            cursor: wait;
        }

        .ap-loading,
        .ap-error {
            padding: 22px 20px;
            font-size: 14px;
            line-height: 1.5;
        }

        .ap-error { color: #b42318; }

        .ap-result-image {
            display: block;
            width: 100%;
            max-height: 190px;
            object-fit: cover;
            background: #f2f4f7;
        }

        .ap-result-content { padding: 20px; }

        .ap-match {
            display: inline-block;
            margin-bottom: 12px;
            padding: 6px 10px;
            border-radius: 20px;
            background: #e8f7ed;
            color: #198754;
            font-size: 13px;
            font-weight: 700;
        }

        .ap-result-label {
            color: #667085;
            font-size: 12px;
            margin-bottom: 5px;
        }

        .ap-result-name {
            font-size: 21px;
            line-height: 1.3;
            font-weight: 700;
            margin-bottom: 10px;
        }

        .ap-result-short {
            font-size: 14px;
            line-height: 1.55;
            color: #475467;
        }

        .ap-score {
            margin-top: 12px;
            color: #667085;
            font-size: 12px;
        }

        .ap-section-title {
            margin: 18px 0 8px;
            font-size: 14px;
            font-weight: 700;
        }

        .ap-reasons {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .ap-reason {
            display: flex;
            gap: 8px;
            font-size: 13px;
            line-height: 1.45;
            color: #344054;
        }

        .ap-check { color: #198754; font-weight: 700; }

        .ap-highlights {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
        }

        .ap-highlight {
            padding: 5px 8px;
            border-radius: 6px;
            background: #f2f4f7;
            color: #475467;
            font-size: 11px;
        }

        .ap-result-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 20px;
        }

        .ap-cta,
        .ap-restart {
            display: inline-block;
            padding: 11px 13px;
            border-radius: 9px;
            font-size: 13px;
            font-weight: 700;
            text-decoration: none;
            cursor: pointer;
        }

        .ap-cta {
            background: #0d6efd;
            color: #fff;
            border: 1px solid #0d6efd;
        }

        .ap-cta:hover { background: #0b5ed7; }

        .ap-restart {
            background: #fff;
            color: #475467;
            border: 1px solid #d0d5dd;
        }

        .ap-restart:hover { background: #f9fafb; }

        @media (max-width: 480px) {
            .ap-card {
                right: 12px;
                bottom: 12px;
                width: calc(100vw - 24px);
                max-width: none;
                max-height: calc(100vh - 24px);
            }
        }
    `;

    root.appendChild(style);

    // ----------------------------------------------------------
    // 4. State
    // ----------------------------------------------------------

    let config = null;
    let questions = [];
    let questionIndex = 0;
    let isSubmitting = false;

    const answers = {};

    const sessionId =
        window.crypto && window.crypto.randomUUID
            ? window.crypto.randomUUID()
            : "session-" + Date.now() + "-" +
            Math.random().toString(36).slice(2);

    // ----------------------------------------------------------
    // 5. Safe HTML helpers
    // ----------------------------------------------------------

    function escapeHtml (value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function escapeAttr (value) {
        return escapeHtml(value);
    }

    function safeHttpUrl (value) {
        if (!value) return "";

        try {
            const url = new URL(value, window.location.href);

            if (url.protocol === "https:" ||
                url.protocol === "http:") {
                return url.href;
            }
        } catch (_) {
            // Invalid URL.
        }

        return "";
    }

    function getWidgetTitle () {
        return config?.widget?.title ||
            config?.widget?.name ||
            "Find Your Perfect Match";
    }

    function getWidgetSubtitle () {
        return config?.widget?.subtitle ||
            config?.widget?.description ||
            "";
    }

    // ----------------------------------------------------------
    // 6. Analytics
    // ----------------------------------------------------------

    function sendEvent (eventType, payload) {
        fetch(`${ apiUrl }/api/events`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                publicKey: publicKey,
                sessionId: sessionId,
                eventType: eventType,
                payload: payload || {}
            })
        }).then(response => {
            if (!response.ok) {
                console.warn(
                    "AskPearls analytics returned HTTP",
                    response.status
                );
            }
        }).catch(error => {
            // Analytics failure must not block the widget.
            console.warn(
                "AskPearls analytics could not be sent.",
                error
            );
        });
    }

    // ----------------------------------------------------------
    // 7. Loading and error views
    // ----------------------------------------------------------


    function showLoading (message) {
        const existingCard = root.querySelector(".ap-card");

        if (existingCard) {
            existingCard.remove();
        }

        const card = document.createElement("div");
        card.className = "ap-card";
        card.setAttribute("role", "status");
        card.setAttribute("aria-live", "polite");

        const header = document.createElement("div");
        header.className = "ap-head";

        const brand = document.createElement("div");
        brand.className = "ap-brand";
        brand.textContent = "AskPearls DecisionOS";

        const title = document.createElement("div");
        title.className = "ap-title";
        title.textContent = "Loading...";

        header.appendChild(brand);
        header.appendChild(title);

        const body = document.createElement("div");
        body.className = "ap-loading";
        body.textContent = message || "Loading DecisionOS...";

        card.appendChild(header);
        card.appendChild(body);

        root.appendChild(card);
    }



    function showError (message, retry) {
        const existingCard = root.querySelector(".ap-card");

        if (existingCard) {
            existingCard.remove();
        }

        const card = document.createElement("div");
        card.className = "ap-card";
        card.setAttribute("role", "alert");

        const header = document.createElement("div");
        header.className = "ap-head";

        const brand = document.createElement("div");
        brand.className = "ap-brand";
        brand.textContent = "AskPearls DecisionOS";

        const title = document.createElement("div");
        title.className = "ap-title";
        title.textContent = getWidgetTitle();

        header.appendChild(brand);
        header.appendChild(title);

        const errorBody = document.createElement("div");
        errorBody.className = "ap-error";
        errorBody.textContent = message;

        card.appendChild(header);
        card.appendChild(errorBody);

        if (retry) {
            const actions = document.createElement("div");
            actions.className = "ap-body";

            const button = document.createElement("button");
            button.type = "button";
            button.className = "ap-restart";
            button.textContent = "Try Again";
            button.addEventListener("click", retry);

            actions.appendChild(button);
            card.appendChild(actions);
        }

        root.appendChild(card);
    }


    // ----------------------------------------------------------
    // 8. Normalize question and answer API data
    // ----------------------------------------------------------

    function getQuestionText (question) {
        return question.text ||
            question.title ||
            question.name ||
            question.key ||
            "Choose an answer";
    }

    function getAnswerKey (answer) {
        return answer.key ||
            answer.code ||
            answer.value ||
            "";
    }

    function getAnswerText (answer) {
        return answer.title ||
            answer.text ||
            answer.name ||
            getAnswerKey(answer);
    }

    function getQuestionAnswers (question) {
        if (Array.isArray(question.answers)) {
            return question.answers;
        }

        // Supports an API that calls them options.
        if (Array.isArray(question.options)) {
            return question.options;
        }

        return [];
    }

    // ----------------------------------------------------------
    // 9. Render current question
    // ----------------------------------------------------------

    function renderQuestion () {
        if (!questions.length) {
            showError("This widget has no published questions.");
            return;
        }

        if (questionIndex >= questions.length) {
            submitAnswers();
            return;
        }

        const question = questions[ questionIndex ];
        const questionAnswers = getQuestionAnswers(question);

        if (!question.key || questionAnswers.length === 0) {
            showError(
                "A published question is missing its key or answers."
            );
            return;
        }

        const progress = Math.round(
            (questionIndex / questions.length) * 100
        );

        const title = getWidgetTitle();
        const subtitle = getWidgetSubtitle();

        root.querySelector(".ap-card")?.remove();

        const card = document.createElement("div");
        card.className = "ap-card";

        card.innerHTML = `
            <div class="ap-head">
                <div class="ap-brand">AskPearls DecisionOS</div>
                <div class="ap-title">${ escapeHtml(title) }</div>
                ${ subtitle ? `
                    <div class="ap-subtitle">
                        ${ escapeHtml(subtitle) }
                    </div>
                ` : "" }
            </div>

            <div class="ap-body">
                <div class="ap-progress">
                    <div class="ap-progress-bar"
                         style="width:${ progress }%"></div>
                </div>

                <div class="ap-question-count">
                    Question ${ questionIndex + 1 } of ${ questions.length }
                </div>

                <div class="ap-question">
                    ${ escapeHtml(getQuestionText(question)) }
                </div>

                <div class="ap-options"></div>
            </div>
        `;

        const optionsContainer = card.querySelector(".ap-options");

        questionAnswers.forEach(answer => {
            const key = getAnswerKey(answer);

            if (!key) return;

            const button = document.createElement("button");
            button.type = "button";
            button.className = "ap-answer";
            button.textContent = getAnswerText(answer);

            button.addEventListener("click", () => {
                if (isSubmitting) return;

                // Store the answer key expected by DecisionOS.
                answers[ question.key ] = key;

                sendEvent("question_answered", {
                    questionKey: question.key,
                    answerValue: key,
                    questionIndex: questionIndex
                });

                questionIndex++;

                renderQuestion();
            });

            optionsContainer.appendChild(button);
        });

        if (!optionsContainer.children.length) {
            showError("No usable answers were returned for this question.");
            return;
        }

        root.appendChild(card);
    }

    // ----------------------------------------------------------
    // 10. Submit answers to server-side DecisionOS
    // ----------------------------------------------------------

    async function submitAnswers () {
        if (isSubmitting) return;

        isSubmitting = true;
        showLoading("Calculating your recommendation...");

        sendEvent("evaluation_started", {
            questionCount: questions.length
        });

        try {
            const response = await fetch(
                `${ apiUrl }/api/decisions/evaluate`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({
                        widgetKey: publicKey,
                        answers: answers
                    })
                }
            );

            if (!response.ok) {
                throw new Error(
                    `Decision API returned HTTP ${ response.status }`
                );
            }

            const decision = await response.json();

            if (!decision || decision.success !== true) {
                throw new Error(
                    "DecisionOS could not evaluate these answers."
                );
            }

            // API normally serializes this property as camelCase.
            const best = decision.bestRecommendation ||
                decision.BestRecommendation;

            if (!best) {
                throw new Error(
                    "The API did not return a best recommendation."
                );
            }

            sendEvent("recommendation_generated", {
                recommendationKey: best.key,
                score: best.score,
                maximumScore: best.maximumScore ||
                    decision.maximumScore,
                percentage: best.percentage
            });

            renderRecommendation(
                best,
                decision.maximumScore
            );

        } catch (error) {
            console.error(
                "AskPearls DecisionOS evaluation failed.",
                error
            );

            showError(
                "We couldn't calculate your recommendation. " +
                "Please try again.",
                submitAnswers
            );
        } finally {
            isSubmitting = false;
        }
    }

    // ----------------------------------------------------------
    // 11. Render server recommendation
    // ----------------------------------------------------------

    function renderRecommendation (recommendation, decisionMaximumScore) {
        const score = Number(recommendation.score ?? 0);

        const maximumScore = Number(
            recommendation.maximumScore ??
            decisionMaximumScore ??
            0
        );

        let percentage = recommendation.percentage;

        if (percentage === undefined || percentage === null) {
            percentage = maximumScore > 0
                ? Math.round((score / maximumScore) * 100)
                : 0;
        }

        percentage = Math.max(
            0,
            Math.min(100, Number(percentage) || 0)
        );

        const imageUrl = safeHttpUrl(recommendation.imageUrl);
        const productUrl = safeHttpUrl(recommendation.url);

        const name = recommendation.name ||
            "Your Recommended Product";

        const description = recommendation.shortDescription ||
            recommendation.description ||
            "This recommendation matches your preferences.";

        const ctaText = recommendation.ctaText ||
            "View Recommendation";

        const highlights = Array.isArray(recommendation.highlights)
            ? recommendation.highlights
            : [];

        const reasons = Array.isArray(recommendation.reasons)
            ? recommendation.reasons
            : [];

        root.querySelector(".ap-card")?.remove();

        const card = document.createElement("div");
        card.className = "ap-card";

        card.innerHTML = `
            <div class="ap-head">
                <div class="ap-brand">AskPearls DecisionOS</div>
                <div class="ap-title">Your Recommendation</div>
            </div>

            ${ imageUrl ? `
                <img class="ap-result-image"
                     src="${ escapeAttr(imageUrl) }"
                     alt="${ escapeAttr(name) }"
                     loading="lazy">
            ` : "" }

            <div class="ap-result-content">
                <div class="ap-match">
                    ${ percentage }% match
                </div>

                <div class="ap-result-label">We recommend:</div>

                <div class="ap-result-name">
                    ${ escapeHtml(name) }
                </div>

                <div class="ap-result-short">
                    ${ escapeHtml(description) }
                </div>

                <div class="ap-score">
                    Decision score: ${ score } / ${ maximumScore }
                </div>

                ${ reasons.length ? `
                    <div class="ap-section-title">
                        Why it matches you
                    </div>
                    <div class="ap-reasons">
                        ${ reasons.map(reason => `
                            <div class="ap-reason">
                                <span class="ap-check">✓</span>
                                <span>${ escapeHtml(reason) }</span>
                            </div>
                        `).join("") }
                    </div>
                ` : "" }

                ${ highlights.length ? `
                    <div class="ap-section-title">Highlights</div>
                    <div class="ap-highlights">
                        ${ highlights.map(item => `
                            <span class="ap-highlight">
                                ${ escapeHtml(item) }
                            </span>
                        `).join("") }
                    </div>
                ` : "" }

                <div class="ap-result-actions">
                    ${ productUrl ? `
                        <a class="ap-cta"
                           id="ap-result-cta"
                           href="${ escapeAttr(productUrl) }"
                           target="_blank"
                           rel="noopener noreferrer">
                            ${ escapeHtml(ctaText) }
                        </a>
                    ` : "" }

                    <button type="button"
                            class="ap-restart"
                            id="ap-restart">
                        Start Again
                    </button>
                </div>
            </div>
        `;

        root.appendChild(card);

        const cta = card.querySelector("#ap-result-cta");

        if (cta) {
            cta.addEventListener("click", () => {
                sendEvent("recommendation_clicked", {
                    recommendationKey: recommendation.key,
                    score: score,
                    maximumScore: maximumScore,
                    matchPercentage: percentage,
                    url: productUrl
                });
            });
        }

        card.querySelector("#ap-restart")?.addEventListener(
            "click",
            () => {
                questionIndex = 0;
                isSubmitting = false;

                Object.keys(answers).forEach(key => {
                    delete answers[ key ];
                });

                sendEvent("widget_restarted", {});
                renderQuestion();
            }
        );

        sendEvent("recommendation_displayed", {
            recommendationKey: recommendation.key,
            percentage: percentage
        });
    }

    // ----------------------------------------------------------
    // 12. Load published questions from the API
    // ----------------------------------------------------------

    async function load () {
        showLoading("Loading published questions...");

        const url =
            `${ apiUrl }/api/widgets/` +
            `${ encodeURIComponent(publicKey) }/questions`;

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error(
                `Questions API returned HTTP ${ response.status }`
            );
        }

        const data = await response.json();

        if (!data || !Array.isArray(data.questions)) {
            throw new Error(
                "The questions API did not return a questions array."
            );
        }

        questions = data.questions.filter(question =>
            question &&
            question.key &&
            question.isActive !== false &&
            question.isVisible !== false
        );

        // API versions may return widget metadata separately.
        config = {
            widget: data.widget || {
                key: publicKey,
                name: "DecisionOS Widget",
                title: "Find Your Perfect Match"
            },
            questions: questions
        };

        if (!questions.length) {
            throw new Error(
                "No active published questions were returned."
            );
        }

        sendEvent("widget_loaded", {
            widgetKey: publicKey,
            questionCount: questions.length
        });

        renderQuestion();
    }

    // ----------------------------------------------------------
    // 13. Initialize
    // ----------------------------------------------------------

    load().catch(error => {
        console.error("AskPearls widget initialization failed.", error);

        showError(
            "Unable to load the AskPearls widget. " +
            "Please try again later.",
            load
        );
    });

})(window, document);
