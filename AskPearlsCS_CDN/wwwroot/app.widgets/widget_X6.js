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

        console.error(
            "AskPearls: Unable to find the current script."
        );

        return;
    }


    const publicKey =
        currentScript.dataset.widget;


    const apiUrl =
        (currentScript.dataset.apiUrl ||
            "https://api.askpearls.com")
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

    const host =
        document.createElement("div");

    host.id =
        "askpearls-widget-host";

    document.body.appendChild(host);


    /* =========================================================
       3. CREATE SHADOW DOM
       ========================================================= */

    const shadow =
        host.attachShadow
            ? host.attachShadow({
                mode: "open"
            })
            : host;


    /* =========================================================
       4. WIDGET STYLES
       ========================================================= */

    const style =
        document.createElement("style");

    style.textContent = `

        :host {
            all: initial;
        }

        * {
            box-sizing: border-box;
        }


        /* =====================================================
           CARD
           ===================================================== */

        .ap-card {

            position: fixed;

            right: 24px;
            bottom: 24px;

            width: 360px;

            max-width:
                calc(100vw - 32px);

            background: #ffffff;

            color: #172033;

            border:
                1px solid #e5e7eb;

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


        /* =====================================================
           HEADER
           ===================================================== */

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


        .ap-subtitle {

            margin-top: 7px;

            font-size: 13px;

            line-height: 1.45;

            opacity: 0.92;
        }


        /* =====================================================
           BODY
           ===================================================== */

        .ap-body {

            padding: 20px;
        }


        /* =====================================================
           PROGRESS
           ===================================================== */

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

            transition:
                width 0.3s ease;
        }


        /* =====================================================
           QUESTION
           ===================================================== */

        .ap-question {

            font-size: 16px;

            line-height: 1.5;

            font-weight: 700;

            margin-bottom: 15px;
        }


        /* =====================================================
           ANSWERS
           ===================================================== */

        .ap-options {

            display: flex;

            flex-direction: column;

            gap: 10px;
        }


        .ap-answer {

            display: block;

            width: 100%;

            padding: 12px 14px;

            border:
                1px solid #d0d5dd;

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

            border-color:
                #0d6efd;

            background:
                #f5f9ff;
        }


        .ap-answer:active {

            transform:
                scale(0.99);
        }


        /* =====================================================
           RESULT
           ===================================================== */

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


        .ap-result-short {

            font-size: 14px;

            line-height: 1.5;

            color: #475467;

            margin-bottom: 14px;
        }


        /* =====================================================
           MATCH
           ===================================================== */

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


        /* =====================================================
           RESULT IMAGE
           ===================================================== */

        .ap-result-image {

            width: 100%;

            height: 170px;

            object-fit: cover;

            display: block;
        }


        /* =====================================================
           SECTION TITLE
           ===================================================== */

        .ap-section-title {

            font-size: 14px;

            font-weight: 700;

            color: #172033;

            margin:
                16px 0 8px;
        }


        /* =====================================================
           REASONS
           ===================================================== */

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

            flex:
                0 0 auto;

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


        /* =====================================================
           HIGHLIGHTS
           ===================================================== */

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


        /* =====================================================
           ACTIONS
           ===================================================== */

        .ap-result-actions {

            display: flex;

            align-items: center;

            gap: 8px;

            margin-top: 18px;
        }


        .ap-cta {

            display: inline-block;

            padding:
                11px 16px;

            background:
                #0d6efd;

            color:
                #ffffff;

            border-radius: 9px;

            text-decoration:
                none;

            font-size: 14px;

            font-weight: 700;
        }


        .ap-cta:hover {

            background:
                #0b5ed7;

            color:
                #ffffff;
        }


        .ap-restart {

            display: inline-block;

            padding:
                11px 12px;

            background:
                #ffffff;

            color:
                #475467;

            border:
                1px solid #d0d5dd;

            border-radius:
                9px;

            font-size:
                13px;

            font-weight:
                700;

            cursor:
                pointer;
        }


        .ap-restart:hover {

            background:
                #f9fafb;
        }


        /* =====================================================
           LOADING
           ===================================================== */

        .ap-loading {

            text-align:
                center;

            padding:
                30px 10px;

            color:
                #667085;

            font-size:
                14px;
        }


        /* =====================================================
           ERROR
           ===================================================== */

        .ap-error {

            padding:
                20px;

            color:
                #b42318;

            background:
                #fef3f2;

            font-size:
                14px;

            line-height:
                1.5;
        }


        /* =====================================================
           MOBILE
           ===================================================== */

        @media (max-width: 480px) {

            .ap-card {

                right:
                    12px;

                bottom:
                    12px;

                width:
                    calc(100vw - 24px);

                max-width:
                    none;
            }
        }

    `;


    shadow.appendChild(style);


    /* =========================================================
       5. ROOT CONTAINER
       ========================================================= */

    const root =
        document.createElement("div");

    shadow.appendChild(root);


    /* =========================================================
       6. WIDGET STATE
       ========================================================= */

    let config = null;

    let questionIndex = 0;

    const answers = {};


    const sessionId =
        window.crypto &&
            window.crypto.randomUUID

            ? window.crypto.randomUUID()

            : "session-" + Date.now();


    /* =========================================================
       7. HTML ESCAPING
       ========================================================= */

    function escapeHtml (value) {

        return String(value ?? "")

            .replace(
                /&/g,
                "&amp;"
            )

            .replace(
                /</g,
                "&lt;"
            )

            .replace(
                />/g,
                "&gt;"
            )

            .replace(
                /"/g,
                "&quot;"
            )

            .replace(
                /'/g,
                "&#039;"
            );
    }


    function escapeAttr (value) {

        return escapeHtml(value);
    }


    /* =========================================================
       8. GET WIDGET INFORMATION
       ========================================================= */

    function getWidgetTitle () {

        if (
            config &&
            config.widget &&
            config.widget.title
        ) {

            return config.widget.title;
        }


        return "Find Your Perfect Match";
    }


    function getWidgetSubtitle () {

        if (
            config &&
            config.widget &&
            config.widget.subtitle
        ) {

            return config.widget.subtitle;
        }


        return "";
    }


    function getWidgetName () {

        if (
            config &&
            config.widget &&
            config.widget.name
        ) {

            return config.widget.name;
        }


        return "DecisionOS Widget";
    }


    /* =========================================================
       9. SEND ANALYTICS EVENT
       ========================================================= */

    function sendEvent (
        eventType,
        payload
    ) {

        fetch(
            `${ apiUrl }/api/events`,
            {

                method:
                    "POST",

                headers:
                {
                    "Content-Type":
                        "application/json"
                },

                body:
                    JSON.stringify({

                        publicKey:
                            publicKey,

                        sessionId:
                            sessionId,

                        eventType:
                            eventType,

                        payload:
                            payload || {}

                    })

            }
        )

            .then(
                function (response) {

                    if (!response.ok) {

                        console.warn(
                            "AskPearls: Event request failed:",
                            response.status
                        );

                    }

                }
            )

            .catch(
                function (error) {

                    console.warn(
                        "AskPearls: Unable to send event.",
                        error
                    );

                }
            );
    }


    /* =========================================================
       10. SHOW LOADING
       ========================================================= */

    function showLoading () {

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">
                        AskPearls DecisionOS
                    </div>

                    <div class="ap-title">
                        Loading...
                    </div>

                </div>

                <div class="ap-loading">

                    Loading DecisionOS...

                </div>

            </div>

        `;
    }


    /* =========================================================
       11. SHOW ERROR
       ========================================================= */

    function showError (message) {

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">
                        AskPearls DecisionOS
                    </div>

                    <div class="ap-title">
                        ${ escapeHtml(
            getWidgetTitle()
        ) }
                    </div>

                </div>

                <div class="ap-error">

                    ${ escapeHtml(message) }

                </div>

            </div>

        `;
    }


    /* =========================================================
       12. RENDER CURRENT QUESTION
       ========================================================= */

    function render () {


        /*
         * Safety check.
         */

        if (!config) {

            showError(
                "Widget configuration is not available."
            );

            return;
        }


        /*
         * Validate questions.
         */

        if (
            !Array.isArray(
                config.questions
            )
        ) {

            console.error(
                "AskPearls: config.questions is not an array.",
                config
            );

            showError(
                "Widget configuration is invalid."
            );

            return;
        }


        /*
         * Are we finished?
         */

        if (
            questionIndex >=
            config.questions.length
        ) {

            showRecommendation();

            return;
        }


        /*
         * Get current question.
         */

        const question =
            config.questions[
            questionIndex
            ];


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


        /*
         * Make sure options is an array.
         */

        const options =
            Array.isArray(
                question.options
            )

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


        /*
         * Progress.
         */

        const totalQuestions =
            config.questions.length;


        const currentNumber =
            questionIndex + 1;


        const progress =
            (
                currentNumber /
                totalQuestions
            ) * 100;


        /*
         * Widget information.
         */

        const title =
            getWidgetTitle();


        const subtitle =
            getWidgetSubtitle();


        const subtitleHtml =
            subtitle

                ? `

                    <div class="ap-subtitle">

                        ${ escapeHtml(
                    subtitle
                ) }

                    </div>

                `

                : "";


        /*
         * Render.
         */

        root.innerHTML = `

            <div class="ap-card">

                <div class="ap-head">

                    <div class="ap-brand">

                        AskPearls DecisionOS

                    </div>

                    <div class="ap-title">

                        ${ escapeHtml(
            title
        ) }

                    </div>

                    ${ subtitleHtml }

                </div>


                <div class="ap-body">


                    <!-- PROGRESS -->

                    <div class="ap-progress">

                        <div
                            class="ap-progress-bar"
                            style="
                                width:${ progress }%;
                            "
                        >
                        </div>

                    </div>


                    <!-- QUESTION NUMBER -->

                    <div
                        style="
                            font-size:12px;
                            color:#667085;
                            margin-bottom:8px;
                        "
                    >

                        Question
                        ${ currentNumber }
                        of
                        ${ totalQuestions }

                    </div>


                    <!-- QUESTION -->

                    <div class="ap-question">

                        ${ escapeHtml(
            question.text || ""
        ) }

                    </div>


                    <!-- OPTIONS -->

                    <div class="ap-options">

                        ${ options
                .map(
                    function (option) {

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

                    }
                )
                .join("") }

                    </div>


                </div>

            </div>

        `;


        /* =====================================================
           ANSWER BUTTON EVENTS
           ===================================================== */

        root
            .querySelectorAll(
                ".ap-answer"
            )
            .forEach(
                function (button) {

                    button.addEventListener(
                        "click",
                        function () {

                            const value =
                                this.dataset.value;


                            /*
                             * Save answer.
                             */

                            answers[
                                question.key
                            ] = value;


                            /*
                             * Send analytics.
                             */

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


                            /*
                             * Next question.
                             */

                            questionIndex++;


                            render();

                        }
                    );

                }
            );

    }


    /* =========================================================
       13. DECISION ENGINE
       ========================================================= */


    /*
     * ============================================================
     *
     * DECISIONOS MVP SCORING MODEL
     *
     * Every question has a maximum possible score of 50.
     *
     * Example:
     *
     * 2 questions
     *
     * 2 × 50
     *
     * = 100 maximum possible points
     *
     *
     * Example visitor:
     *
     * users    = large
     * priority = comfort
     *
     *
     * Oslo:
     *
     * large   = 40
     * comfort = 45
     *
     * total = 85
     *
     * match = 85 / 100 = 85%
     *
     * ============================================================
     */


    function calculateRecommendationScores () {


        /*
         * Get recommendations.
         */

        const recommendations =
            Array.isArray(
                config.recommendations
            )

                ? config.recommendations

                : [];


        /*
         * Maximum points allowed
         * per question.
         */

        const MAX_SCORE_PER_QUESTION =
            50;


        /*
         * Calculate maximum possible
         * score for this widget.
         *
         * Example:
         *
         * 2 questions × 50 = 100
         */

        const maximumScore =
            Array.isArray(
                config.questions
            )

                ? config.questions.length *
                MAX_SCORE_PER_QUESTION

                : 0;


        /*
         * Calculate every recommendation.
         */

        return recommendations.map(
            function (recommendation) {

                let score = 0;

                const matchedReasons = [];


                /*
                 * Get rules.
                 */

                const rules =
                    Array.isArray(
                        recommendation.rules
                    )

                        ? recommendation.rules

                        : [];


                /*
                 * Evaluate rules.
                 */

                rules.forEach(
                    function (rule) {


                        /*
                         * Validate rule.
                         */

                        if (
                            !rule ||
                            !rule.questionKey
                        ) {

                            return;
                        }


                        /*
                         * Visitor answer.
                         */

                        const visitorAnswer =
                            answers[
                            rule.questionKey
                            ];


                        /*
                         * No answer.
                         */

                        if (
                            visitorAnswer ===
                            undefined ||

                            visitorAnswer ===
                            null
                        ) {

                            return;
                        }


                        /*
                         * Does rule match?
                         */

                        if (
                            String(
                                visitorAnswer
                            ) ===
                            String(
                                rule.answerValue
                            )
                        ) {


                            /*
                             * Get points.
                             */

                            const points =
                                Number(
                                    rule.score
                                );


                            /*
                             * Add points.
                             */

                            if (
                                !isNaN(
                                    points
                                )
                            ) {

                                score +=
                                    points;

                            }


                            /*
                             * Add reason.
                             */

                            if (
                                rule.reason
                            ) {

                                matchedReasons.push(
                                    rule.reason
                                );

                            }

                        }

                    }
                );


                /*
                 * Calculate match percentage.
                 */

                let percentage = 0;


                if (
                    maximumScore > 0
                ) {

                    percentage =
                        Math.round(
                            (
                                score /
                                maximumScore
                            ) * 100
                        );

                }


                /*
                 * Protect percentage.
                 *
                 * It should never be
                 * below 0 or above 100.
                 */

                percentage =
                    Math.max(
                        0,
                        Math.min(
                            100,
                            percentage
                        )
                    );


                /*
                 * Return calculation.
                 */

                return {

                    recommendation:
                        recommendation,

                    score:
                        score,

                    maximumScore:
                        maximumScore,

                    percentage:
                        percentage,

                    matchedReasons:
                        matchedReasons

                };

            }
        );

    }


    /* =========================================================
       14. GET BEST RECOMMENDATION
       ========================================================= */

    function getBestRecommendation () {


        const results =
            calculateRecommendationScores();


        if (
            !results ||
            results.length === 0
        ) {

            return null;
        }


        /*
         * Sort highest score first.
         */

        results.sort(
            function (a, b) {

                return (
                    b.score -
                    a.score
                );

            }
        );


        /*
         * Development debug.
         */

        console.log(
            "AskPearls DecisionOS - Recommendation Scores:",
            results
        );


        return results[ 0 ];

    }


    /* =========================================================
       15. SHOW RECOMMENDATION
       ========================================================= */

    function showRecommendation () {


        /*
         * Calculate best result.
         */

        const result =
            getBestRecommendation();


        if (!result) {

            showError(
                "Unable to determine a recommendation."
            );

            return;
        }


        /*
         * Result.
         */

        const recommendation =
            result.recommendation;


        const score =
            result.score;


        const maximumScore =
            result.maximumScore;


        const matchPercentage =
            result.percentage;


        const matchedReasons =
            Array.isArray(
                result.matchedReasons
            )

                ? result.matchedReasons

                : [];


        /*
         * Safe recommendation values.
         */

        const recommendationName =
            recommendation.name ||
            "Recommended Product";


        const imageUrl =
            recommendation.imageUrl ||
            "";


        const productUrl =
            recommendation.url ||
            "#";


        const shortDescription =
            recommendation.shortDescription ||

            recommendation.description ||

            "This recommendation matches your preferences.";


        const ctaText =
            recommendation.ctaText ||

            "View Recommendation";


        const highlights =
            Array.isArray(
                recommendation.highlights
            )

                ? recommendation.highlights

                : [];


        /*
         * Widget title.
         */

        const widgetTitle =
            getWidgetTitle();


        /* =====================================================
           BUILD REASONS
           ===================================================== */

        let reasonsHtml = "";


        if (
            matchedReasons.length > 0
        ) {

            reasonsHtml =
                matchedReasons
                    .map(
                        function (reason) {

                            return `

                                <div class="ap-reason">

                                    <span class="ap-check">

                                        ✓

                                    </span>

                                    <span>

                                        ${ escapeHtml(
                                reason
                            ) }

                                    </span>

                                </div>

                            `;

                        }
                    )
                    .join("");

        }

        else {

            reasonsHtml = `

                <div class="ap-reason">

                    <span class="ap-check">

                        ✓

                    </span>

                    <span>

                        Good overall match

                    </span>

                </div>

            `;

        }


        /* =====================================================
           BUILD HIGHLIGHTS
           ===================================================== */

        let highlightsHtml = "";


        if (
            highlights.length > 0
        ) {

            highlightsHtml =
                highlights
                    .map(
                        function (highlight) {

                            return `

                                <span class="ap-highlight">

                                    ${ escapeHtml(
                                highlight
                            ) }

                                </span>

                            `;

                        }
                    )
                    .join("");

        }


        /* =====================================================
           BUILD IMAGE
           ===================================================== */

        let imageHtml = "";


        if (imageUrl) {

            imageHtml = `

                <img
                    class="ap-result-image"
                    src="${ escapeAttr(
                imageUrl
            ) }"
                    alt="${ escapeAttr(
                recommendationName
            ) }"
                >

            `;

        }


        /* =====================================================
           ANALYTICS
           ===================================================== */

        sendEvent(
            "recommendation_shown",
            {

                recommendationKey:
                    recommendation.key,

                score:
                    score,

                maximumScore:
                    maximumScore,

                matchPercentage:
                    matchPercentage,

                answers:
                    answers,

                matchedReasons:
                    matchedReasons

            }
        );


        /* =====================================================
           RENDER RESULT
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


                    <!-- MATCH -->

                    <div class="ap-match">

                        ✓
                        ${ matchPercentage }%
                        Match

                    </div>


                    <!-- SCORE -->

                    <div
                        style="
                            font-size:12px;
                            color:#667085;
                            margin-bottom:12px;
                        "
                    >

                        Decision score:

                        <strong>

                            ${ score }

                        </strong>

                        /

                        ${ maximumScore }

                    </div>


                    <!-- RECOMMENDATION -->

                    <div class="ap-result-label">

                        We recommend:

                    </div>


                    <div class="ap-result-name">

                        ${ escapeHtml(
            recommendationName
        ) }

                    </div>


                    <!-- DESCRIPTION -->

                    <div class="ap-result-short">

                        ${ escapeHtml(
            shortDescription
        ) }

                    </div>


                    <!-- WHY -->

                    ${ matchedReasons.length > 0

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


                    <!-- HIGHLIGHTS -->

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


                    <!-- ACTIONS -->

                    <div class="ap-result-actions">


                        <a
                            href="${ escapeAttr(
                productUrl
            ) }"
                            class="ap-cta"
                            id="ap-result-cta"
                            target="_blank"
                            rel="noopener noreferrer"
                        >

                            ${ escapeHtml(
                ctaText
            ) }

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
           CTA TRACKING
           ===================================================== */

        const cta =
            root.querySelector(
                "#ap-result-cta"
            );


        if (cta) {

            cta.addEventListener(
                "click",
                function () {

                    sendEvent(
                        "recommendation_clicked",
                        {

                            recommendationKey:
                                recommendation.key,

                            score:
                                score,

                            maximumScore:
                                maximumScore,

                            matchPercentage:
                                matchPercentage,

                            url:
                                productUrl

                        }
                    );

                }
            );

        }


        /* =====================================================
           RESTART
           ===================================================== */

        const restart =
            root.querySelector(
                "#ap-restart"
            );


        if (restart) {

            restart.addEventListener(
                "click",
                function () {


                    /*
                     * Reset question.
                     */

                    questionIndex =
                        0;


                    /*
                     * Clear answers.
                     */

                    Object.keys(
                        answers
                    )
                        .forEach(
                            function (key) {

                                delete
                                    answers[ key ];

                            }
                        );


                    /*
                     * Track restart.
                     */

                    sendEvent(
                        "widget_restarted",
                        {}
                    );


                    /*
                     * Render first question.
                     */

                    render();

                }
            );

        }

    }


    /* =========================================================
       16. LOAD CONFIGURATION FROM API
       ========================================================= */

    async function load () {


        showLoading();


        /*
         * API URL.
         */

        const url =
            `${ apiUrl }/api/widgets/` +
            encodeURIComponent(
                publicKey
            );


        console.log(
            "AskPearls: Loading widget configuration:",
            url
        );


        /*
         * Request configuration.
         */

        const response =
            await fetch(url);


        if (!response.ok) {

            throw new Error(
                `Widget API returned HTTP ${ response.status }`
            );

        }


        /*
         * Parse JSON.
         */

        config =
            await response.json();


        /*
         * Debug.
         */

        console.log(
            "AskPearls: Widget configuration loaded:",
            config
        );


        /* =====================================================
           VALIDATE CONFIGURATION
           ===================================================== */

        if (!config) {

            throw new Error(
                "Widget configuration is empty."
            );

        }


        /*
         * Widget object.
         */

        if (
            !config.widget ||
            typeof config.widget !== "object"
        ) {

            throw new Error(
                "Widget configuration does not contain a valid widget object."
            );

        }


        /*
         * Questions.
         */

        if (
            !Array.isArray(
                config.questions
            )
        ) {

            throw new Error(
                "Widget configuration does not contain a valid questions array."
            );

        }


        /*
         * Recommendations.
         */

        if (
            !Array.isArray(
                config.recommendations
            )
        ) {

            throw new Error(
                "Widget configuration does not contain a valid recommendations array."
            );

        }


        /*
         * Validate widget key.
         *
         * We don't require it to be identical
         * to the script publicKey yet, but we
         * can warn during development.
         */

        if (
            config.widget.key &&
            config.widget.key !== publicKey
        ) {

            console.warn(
                "AskPearls: Widget configuration key does not match the script public key.",
                {
                    scriptKey:
                        publicKey,

                    configurationKey:
                        config.widget.key
                }
            );

        }


        /* =====================================================
           TRACK WIDGET LOADED
           ===================================================== */

        sendEvent(
            "widget_loaded",
            {

                widgetKey:
                    config.widget.key,

                widgetName:
                    config.widget.name,

                title:
                    config.widget.title,

                questionCount:
                    config.questions.length,

                recommendationCount:
                    config.recommendations.length

            }
        );


        /* =====================================================
           RENDER FIRST QUESTION
           ===================================================== */

        render();

    }


    /* =========================================================
       17. START WIDGET
       ========================================================= */

    load()

        .catch(
            function (error) {

                console.error(
                    "AskPearls:",
                    error
                );


                showError(
                    "Unable to load the AskPearls widget. " +
                    "Please try again later."
                );

            }
        );


})(window, document);