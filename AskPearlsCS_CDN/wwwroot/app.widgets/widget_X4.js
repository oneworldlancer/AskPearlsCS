/* =========================================================
   AskPearls DecisionOS Widget
   Framework-independent MVP
   ========================================================= */

(function (window, document) {

    "use strict";


    /* =========================================================
       1. READ SCRIPT CONFIGURATION
       ========================================================= */

    const currentScript = document.currentScript;

    if (!currentScript) {
        console.error("AskPearls: Unable to find the current script.");
        return;
    }

    const publicKey = currentScript.dataset.widget;

    const apiUrl =
        (currentScript.dataset.apiUrl || "https://api.askpearls.com")
            .replace(/\/+$/, "");


    if (!publicKey) {

        console.error(
            "AskPearls: data-widget is required."
        );

        return;
    }


    /* =========================================================
       2. CREATE WIDGET HOST
       ========================================================= */

    const host = document.createElement("div");

    host.id = "askpearls-widget-host";

    document.body.appendChild(host);


    /* =========================================================
       3. CREATE SHADOW DOM
       ========================================================= */

    const shadow = host.attachShadow
        ? host.attachShadow({ mode: "open" })
        : host;


    /* =========================================================
       4. WIDGET STYLES
       ========================================================= */

    const style = document.createElement("style");

    style.textContent = `

        :host {
            all: initial;
        }

        * {
            box-sizing: border-box;
        }

        .ap-card {

            position: fixed;

            right: 24px;
            bottom: 24px;

            width: 360px;
            max-width: calc(100vw - 32px);

            background: #ffffff;

            color: #172033;

            border: 1px solid #e5e7eb;

            border-radius: 18px;

            box-shadow:
                0 18px 50px rgba(0, 0, 0, 0.16);

            font-family:
                Arial,
                Helvetica,
                sans-serif;

            z-index: 2147483647;

            overflow: hidden;
        }


        /* Header */

        .ap-head {

            padding: 20px;

            background: #0d6efd;

            color: #ffffff;
        }


        .ap-brand {

            font-size: 12px;

            font-weight: 700;

            opacity: 0.9;

            margin-bottom: 5px;

            letter-spacing: 0.3px;
        }


        .ap-title {

            font-size: 20px;

            line-height: 1.3;

            font-weight: 700;
        }


        /* Body */

        .ap-body {

            padding: 20px;
        }


        /* Progress */

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

            transition: width 0.3s ease;
        }


        /* Question */

        .ap-question {

            font-size: 16px;

            line-height: 1.5;

            font-weight: 700;

            margin-bottom: 15px;
        }


        /* Answers */

        .ap-options {

            display: flex;

            flex-direction: column;

            gap: 10px;
        }


        .ap-answer {

            display: block;

            width: 100%;

            padding: 12px 14px;

            border: 1px solid #d0d5dd;

            border-radius: 10px;

            background: #ffffff;

            color: #172033;

            font-family: inherit;

            font-size: 15px;

            line-height: 1.4;

            text-align: left;

            cursor: pointer;

            transition:
                background 0.2s ease,
                border-color 0.2s ease,
                transform 0.1s ease;
        }


        .ap-answer:hover {

            border-color: #0d6efd;

            background: #f5f9ff;
        }


        .ap-answer:active {

            transform: scale(0.99);
        }


        /* Result */

        .ap-result-label {

            font-size: 13px;

            color: #667085;

            margin-bottom: 6px;
        }


        .ap-result-name {

            font-size: 22px;

            line-height: 1.3;

            font-weight: 700;

            color: #172033;

            margin-bottom: 10px;
        }


        .ap-result-text {

            font-size: 14px;

            line-height: 1.5;

            color: #475467;

            margin-bottom: 16px;
        }


        /* CTA */

        .ap-cta {

            display: inline-block;

            padding: 11px 16px;

            background: #0d6efd;

            color: #ffffff;

            border-radius: 9px;

            text-decoration: none;

            font-size: 14px;

            font-weight: 700;
        }


        .ap-cta:hover {

            background: #0b5ed7;

            color: #ffffff;
        }


        /* Loading */

        .ap-loading {

            text-align: center;

            padding: 30px 10px;

            color: #667085;

            font-size: 14px;
        }


        /* Error */

        .ap-error {

            padding: 20px;

            color: #b42318;

            background: #fef3f2;

            font-size: 14px;

            line-height: 1.5;
        }



        /* =========================================================
   Rich Recommendation
   ========================================================= */

.ap-result-image {
    width: 100%;
    height: 170px;
    object-fit: cover;
    display: block;
}

.ap-match {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 10px;
    padding: 6px 10px;
    border-radius: 20px;
    background: #e8f7ed;
    color: #198754;
    font-size: 13px;
    font-weight: 700;
}

.ap-result-label {
    font-size: 13px;
    color: #667085;
    margin-bottom: 6px;
}

.ap-result-name {
    font-size: 22px;
    line-height: 1.3;
    font-weight: 700;
    color: #172033;
    margin-bottom: 8px;
}

.ap-result-short {
    font-size: 14px;
    line-height: 1.5;
    color: #475467;
    margin-bottom: 14px;
}

.ap-section-title {
    font-size: 14px;
    font-weight: 700;
    color: #172033;
    margin: 16px 0 8px;
}

.ap-reasons {
    display: flex;
    flex-direction: column;
    gap: 7px;
}

.ap-reason {
    display: flex;
    align-items: flex-start;
    gap: 8px;
    font-size: 13px;
    line-height: 1.4;
    color: #344054;
}

.ap-check {
    flex: 0 0 auto;
    width: 18px;
    height: 18px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background: #e8f7ed;
    color: #198754;
    font-size: 11px;
    font-weight: 700;
}

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
    align-items: center;
    gap: 8px;
    margin-top: 18px;
}

.ap-restart {
    display: inline-block;
    padding: 11px 12px;
    background: #ffffff;
    color: #475467;
    border: 1px solid #d0d5dd;
    border-radius: 9px;
    font-size: 13px;
    font-weight: 700;
    cursor: pointer;
}

.ap-restart:hover {
    background: #f9fafb;
}




        /* Mobile */

        @media (max-width: 480px) {

            .ap-card {

                right: 12px;

                bottom: 12px;

                width: calc(100vw - 24px);

                max-width: none;
            }
        }

    `;


    shadow.appendChild(style);


    /* =========================================================
       5. ROOT CONTAINER
       ========================================================= */

    const root = document.createElement("div");

    shadow.appendChild(root);


    /* =========================================================
       6. WIDGET STATE
       ========================================================= */

    let config = null;

    let questionIndex = 0;

    const answers = {};

    const sessionId =
        window.crypto && window.crypto.randomUUID
            ? window.crypto.randomUUID()
            : "session-" + Date.now();


    /* =========================================================
       7. HTML ESCAPING
       ========================================================= */

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


    /* =========================================================
       8. SEND ANALYTICS EVENT
       ========================================================= */

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

        })
            .then(function (response) {

                if (!response.ok) {

                    console.warn(
                        "AskPearls: Event request failed:",
                        response.status
                    );
                }

            })
            .catch(function (error) {

                console.warn(
                    "AskPearls: Unable to send event.",
                    error
                );

            });

    }


    /* =========================================================
       9. SHOW LOADING
       ========================================================= */

    function showLoading () {

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">
                        AskPearls DecisionOS
                    </div>

                    <div class="ap-title">
                        Find Your Perfect Sofa
                    </div>

                </div>

                <div class="ap-loading">

                    Loading...

                </div>

            </div>

        `;
    }


    /* =========================================================
       10. SHOW ERROR
       ========================================================= */

    function showError (message) {

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">
                        AskPearls DecisionOS
                    </div>

                    <div class="ap-title">
                        Find Your Perfect Sofa
                    </div>

                </div>

                <div class="ap-error">

                    ${ escapeHtml(message) }

                </div>

            </div>

        `;
    }


    /* =========================================================
       11. RENDER CURRENT QUESTION
       ========================================================= */

    function render () {

        /*
         * Safety check.
         *
         * This prevents the exact ".map is undefined"
         * error we were getting before.
         */

        if (!config) {

            showError(
                "Widget configuration is not available."
            );

            return;
        }


        if (!Array.isArray(config.questions)) {

            console.error(
                "AskPearls: config.questions is not an array.",
                config
            );

            showError(
                "Widget configuration is invalid."
            );

            return;
        }


        /* -----------------------------------------------------
           Are we finished?
           ----------------------------------------------------- */

        if (questionIndex >= config.questions.length) {

            showRecommendation();

            return;
        }


        /* -----------------------------------------------------
           Get current question
           ----------------------------------------------------- */

        const question =
            config.questions[ questionIndex ];


        if (!question) {

            console.error(
                "AskPearls: Question not found.",
                questionIndex
            );

            showError(
                "Unable to load this question."
            );

            return;
        }


        /* -----------------------------------------------------
           Make sure options is an array
           ----------------------------------------------------- */

        const options =
            Array.isArray(question.options)
                ? question.options
                : [];


        if (options.length === 0) {

            console.error(
                "AskPearls: Question has no options.",
                question
            );

            showError(
                "This question does not have any answers."
            );

            return;
        }


        /* -----------------------------------------------------
           Progress
           ----------------------------------------------------- */

        const totalQuestions =
            config.questions.length;

        const currentNumber =
            questionIndex + 1;

        const progress =
            (currentNumber / totalQuestions) * 100;


        /* -----------------------------------------------------
           Render
           ----------------------------------------------------- */

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">
                        AskPearls DecisionOS
                    </div>

                    <div class="ap-title">
                        ${ escapeHtml(
            config.title ||
            "Find Your Perfect Match"
        ) }
                    </div>

                </div>


                <div class="ap-body">


                    <div class="ap-progress">

                        <div
                            class="ap-progress-bar"
                            style="width:${ progress }%">
                        </div>

                    </div>


                    <div
                        style="
                            font-size:12px;
                            color:#667085;
                            margin-bottom:8px;
                        "
                    >

                        Question ${ currentNumber }
                        of ${ totalQuestions }

                    </div>


                    <div class="ap-question">

                        ${ escapeHtml(
            question.text || ""
        ) }

                    </div>


                    <div class="ap-options">

                        ${ options.map(function (option) {

            return `

                                <button
                                    type="button"
                                    class="ap-answer"
                                    data-value="${ escapeAttr(
                option.value
            ) }"
                                >

                                    ${ escapeHtml(
                option.text
            ) }

                                </button>

                            `;

        }).join("") }

                    </div>


                </div>

            </div>

        `;


        /* =====================================================
           12. ANSWER BUTTON EVENTS
           ===================================================== */

        root
            .querySelectorAll(".ap-answer")
            .forEach(function (button) {

                button.addEventListener(
                    "click",
                    function () {

                        const value =
                            this.dataset.value;


                        /* Save answer */

                        answers[ question.key ] =
                            value;


                        /* Send analytics */

                        sendEvent(
                            "question_answered",
                            {
                                questionKey:
                                    question.key,

                                answerValue:
                                    value,

                                questionIndex:
                                    questionIndex
                            }
                        );


                        /* Next question */

                        questionIndex++;


                        render();

                    }
                );

            });

    }


    /* =========================================================
       13. DECISION / RECOMMENDATION
       ========================================================= */

    function showRecommendation () {

        /* =====================================================
           Get recommendations safely
           ===================================================== */

        const recommendations =
            Array.isArray(config.recommendations)
                ? config.recommendations
                : [];


        if (recommendations.length === 0) {

            showError(
                "No recommendations are configured."
            );

            return;
        }


        /* =====================================================
           DEMO DECISION LOGIC
           ===================================================== */

        let recommendation = null;


        /*
         * Style
         * → Luna
         */

        if (answers.priority === "style") {

            recommendation =
                recommendations.find(function (item) {

                    return item.key === "luna";

                });

        }


        /*
         * Large family
         * → Oslo
         */

        if (!recommendation &&
            answers.users === "large") {

            recommendation =
                recommendations.find(function (item) {

                    return item.key === "oslo";

                });

        }


        /*
         * Comfort
         * → Oslo
         */

        if (!recommendation &&
            answers.priority === "comfort") {

            recommendation =
                recommendations.find(function (item) {

                    return item.key === "oslo";

                });

        }


        /*
         * Space saving
         * → Luna
         */

        if (!recommendation &&
            answers.priority === "space") {

            recommendation =
                recommendations.find(function (item) {

                    return item.key === "luna";

                });

        }


        /*
         * Fallback
         */

        if (!recommendation) {

            recommendation =
                recommendations[ 0 ];

        }


        /* =====================================================
           Safe recommendation values
           ===================================================== */

        const matchScore =
            Number.isFinite(
                Number(recommendation.matchScore)
            )
                ? Number(recommendation.matchScore)
                : 0;


        const imageUrl =
            recommendation.imageUrl || "";


        const productUrl =
            recommendation.url || "#";


        const shortDescription =
            recommendation.shortDescription ||
            recommendation.description ||
            "This recommendation matches your preferences.";


        const reasons =
            Array.isArray(recommendation.reasons)
                ? recommendation.reasons
                : [];


        const highlights =
            Array.isArray(recommendation.highlights)
                ? recommendation.highlights
                : [];


        const ctaText =
            recommendation.ctaText ||
            "View Recommendation";


        /* =====================================================
           Build reasons HTML
           ===================================================== */

        const reasonsHtml =
            reasons.length > 0

                ? reasons.map(function (reason) {

                    return `

                    <div class="ap-reason">

                        <span class="ap-check">
                            ✓
                        </span>

                        <span>
                            ${ escapeHtml(reason) }
                        </span>

                    </div>

                `;

                }).join("")

                : "";


        /* =====================================================
           Build highlights HTML
           ===================================================== */

        const highlightsHtml =
            highlights.length > 0

                ? highlights.map(function (highlight) {

                    return `

                    <span class="ap-highlight">

                        ${ escapeHtml(highlight) }

                    </span>

                `;

                }).join("")

                : "";


        /* =====================================================
           Optional image
           ===================================================== */

        const imageHtml =
            imageUrl

                ? `

                <img
                    class="ap-result-image"
                    src="${ escapeAttr(imageUrl) }"
                    alt="${ escapeAttr(
                    recommendation.name
                ) }"
                >

              `

                : "";


        /* =====================================================
           Analytics
           ===================================================== */

        sendEvent(
            "recommendation_shown",
            {
                recommendationKey:
                    recommendation.key,

                matchScore:
                    matchScore,

                answers:
                    answers
            }
        );


        /* =====================================================
           Render rich recommendation
           ===================================================== */

        root.innerHTML = `

        <div class="ap-card">


            ${ imageHtml }


            <div class="ap-head">

                <div class="ap-brand">
                    AskPearls DecisionOS
                </div>

                <div class="ap-title">
                    Your Sofa Match
                </div>

            </div>


            <div class="ap-body">


                <!-- Match score -->

                <div class="ap-match">

                    ✓ ${ matchScore }% Match

                </div>


                <!-- Recommendation -->

                <div class="ap-result-label">

                    We recommend:

                </div>


                <div class="ap-result-name">

                    ${ escapeHtml(
            recommendation.name
        ) }

                </div>


                <!-- Description -->

                <div class="ap-result-short">

                    ${ escapeHtml(
            shortDescription
        ) }

                </div>


                <!-- Why -->

                ${ reasons.length > 0

                ? `

                            <div class="ap-section-title">

                                Why it matches you

                            </div>


                            <div class="ap-reasons">

                                ${ reasonsHtml }

                            </div>

                          `

                : ""
            }


                <!-- Highlights -->

                ${ highlights.length > 0

                ? `

                            <div class="ap-section-title">

                                Highlights

                            </div>


                            <div class="ap-highlights">

                                ${ highlightsHtml }

                            </div>

                          `

                : ""
            }


                <!-- Actions -->

                <div class="ap-result-actions">


                    <a
                        href="${ escapeAttr(productUrl) }"
                        class="ap-cta"
                        id="ap-result-cta"
                        target="_blank"
                        rel="noopener noreferrer"
                    >

                        ${ escapeHtml(ctaText) }

                    </a>


                    <button
                        type="button"
                        class="ap-restart"
                        id="ap-restart"
                    >

                        Start Again

                    </button>


                </div>


            </div>

        </div>

    `;


        /* =====================================================
           CTA tracking
           ===================================================== */

        const cta =
            root.querySelector("#ap-result-cta");


        if (cta) {

            cta.addEventListener(
                "click",
                function () {

                    sendEvent(
                        "recommendation_clicked",
                        {
                            recommendationKey:
                                recommendation.key,

                            matchScore:
                                matchScore,

                            url:
                                productUrl
                        }
                    );

                }
            );

        }


        /* =====================================================
           Restart
           ===================================================== */

        const restart =
            root.querySelector("#ap-restart");


        if (restart) {

            restart.addEventListener(
                "click",
                function () {

                    /* Reset state */

                    questionIndex = 0;


                    Object.keys(answers)
                        .forEach(function (key) {

                            delete answers[ key ];

                        });


                    sendEvent(
                        "widget_restarted",
                        {}
                    );


                    /* Render question 1 */

                    render();

                }
            );

        }

    }

    /* =========================================================
       14. LOAD CONFIGURATION FROM API
       ========================================================= */

    async function load () {

        showLoading();


        const url =
            `${ apiUrl }/api/widgets/` +
            encodeURIComponent(publicKey);


        console.log(
            "AskPearls: Loading widget configuration:",
            url
        );


        const response =
            await fetch(url);


        if (!response.ok) {

            throw new Error(
                `Widget API returned HTTP ${ response.status }`
            );

        }


        config =
            await response.json();


        /* -----------------------------------------------------
           DEBUG
        ----------------------------------------------------- */

        console.log(
            "AskPearls: Widget configuration loaded:",
            config
        );


        /* -----------------------------------------------------
           Validate configuration
        ----------------------------------------------------- */

        if (!config) {

            throw new Error(
                "Widget configuration is empty."
            );

        }


        if (!Array.isArray(config.questions)) {

            throw new Error(
                "Widget configuration does not contain a valid questions array."
            );

        }


        if (!Array.isArray(config.recommendations)) {

            throw new Error(
                "Widget configuration does not contain a valid recommendations array."
            );

        }


        /* -----------------------------------------------------
           Track widget loaded
        ----------------------------------------------------- */

        sendEvent(
            "widget_loaded",
            {
                title: config.title,
                questionCount:
                    config.questions.length,
                recommendationCount:
                    config.recommendations.length
            }
        );


        /* -----------------------------------------------------
           Render first question
        ----------------------------------------------------- */

        render();

    }


    /* =========================================================
       15. START WIDGET
       ========================================================= */

    load()
        .catch(function (error) {

            console.error(
                "AskPearls:",
                error
            );


            showError(
                "Unable to load the AskPearls widget. " +
                "Please try again later."
            );

        });


})(window, document);