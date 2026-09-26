(function () {
    "use strict";

    /*
     * =========================================================
     * AskPearls Widget
     * =========================================================
     *
     * Customer integration:
     *
     * <script
     *     src="https://cdn.askpearls.com/widget.js?tenantId=abc123">
     * </script>
     *
     * The JavaScript file is public.
     * No secrets must ever be stored here.
     *
     * Authentication is performed through a short-lived
     * Widget Session JWT issued by AskPearls API.
     */

    // ---------------------------------------------------------
    // Prevent duplicate initialization
    // ---------------------------------------------------------

    if (window.__ASKPEARLS_WIDGET__) {
        return;
    }

    window.__ASKPEARLS_WIDGET__ = true;


    // ---------------------------------------------------------
    // Configuration
    // ---------------------------------------------------------

    const ASKPEARLS = {

        cdnUrl: "https://cdn.askpearls.com",

        apiUrl: "https://api.askpearls.com",

        sessionEndpoint:
            "/api/widget/session",

        chatEndpoint:
            "/api/widget/chat",

        version:
            "1.0.0"

    };


    // ---------------------------------------------------------
    // Find current script
    // ---------------------------------------------------------

    const currentScript =
        document.currentScript;

    if (!currentScript) {

        console.error(
            "[AskPearls] Unable to detect widget script."
        );

        return;
    }


    // ---------------------------------------------------------
    // Read script URL
    // ---------------------------------------------------------

    let scriptUrl;

    try {

        scriptUrl =
            new URL(currentScript.src);

    } catch (error) {

        console.error(
            "[AskPearls] Invalid script URL.",
            error
        );

        return;
    }


    // ---------------------------------------------------------
    // Read tenantId
    // ---------------------------------------------------------

    const tenantId =
        scriptUrl.searchParams.get("tenantId");


    if (!tenantId) {

        console.error(
            "[AskPearls] Missing tenantId."
        );

        return;
    }


    // ---------------------------------------------------------
    // Widget state
    // ---------------------------------------------------------

    let sessionToken = null;

    let sessionExpiresAt = 0;

    let isInitialized = false;

    let isOpen = false;


    // ---------------------------------------------------------
    // Create root container
    // ---------------------------------------------------------

    const container =
        document.createElement("div");

    container.id =
        "askpearls-widget-container";

    document.body.appendChild(container);


    // ---------------------------------------------------------
    // Create Shadow DOM
    // ---------------------------------------------------------

    const shadowRoot =
        container.attachShadow({
            mode: "open"
        });


    // ---------------------------------------------------------
    // Load CSS
    // ---------------------------------------------------------

    const stylesheet =
        document.createElement("link");

    stylesheet.rel =
        "stylesheet";

    stylesheet.href =
        `${ASKPEARLS.cdnUrl}/widget.css?v=${ASKPEARLS.version}`;

    shadowRoot.appendChild(stylesheet);


    // ---------------------------------------------------------
    // Create widget markup
    // ---------------------------------------------------------

    const widget =
        document.createElement("div");

    widget.className =
        "askpearls-widget";

    widget.innerHTML = `

        <!-- Launcher -->

        <button
            type="button"
            class="askpearls-launcher"
            aria-label="Open AskPearls chat"
        >
            <span class="askpearls-launcher-icon">
                ✦
            </span>
        </button>


        <!-- Chat -->

        <div
            class="askpearls-chat"
            hidden
        >

            <!-- Header -->

            <div class="askpearls-header">

                <div class="askpearls-header-info">

                    <div class="askpearls-avatar">
                        ✦
                    </div>

                    <div>

                        <div class="askpearls-title">
                            AskPearls
                        </div>

                        <div class="askpearls-status">
                            <span></span>
                            Online
                        </div>

                    </div>

                </div>


                <button
                    type="button"
                    class="askpearls-close"
                    aria-label="Close chat"
                >
                    ×
                </button>

            </div>


            <!-- Messages -->

            <div
                class="askpearls-messages"
                aria-live="polite"
            >

                <div class="
                    askpearls-message
                    askpearls-message-bot
                ">
                    Hi! 👋
                    <br>
                    How can I help you today?
                </div>

            </div>


            <!-- Input -->

            <div class="askpearls-input-area">

                <input
                    type="text"
                    class="askpearls-input"
                    placeholder="Type your message..."
                    autocomplete="off"
                />

                <button
                    type="button"
                    class="askpearls-send"
                    aria-label="Send message"
                >
                    ➤
                </button>

            </div>

        </div>
    `;


    shadowRoot.appendChild(widget);


    // ---------------------------------------------------------
    // Get DOM elements
    // ---------------------------------------------------------

    const launcher =
        shadowRoot.querySelector(
            ".askpearls-launcher"
        );

    const chat =
        shadowRoot.querySelector(
            ".askpearls-chat"
        );

    const closeButton =
        shadowRoot.querySelector(
            ".askpearls-close"
        );

    const input =
        shadowRoot.querySelector(
            ".askpearls-input"
        );

    const sendButton =
        shadowRoot.querySelector(
            ".askpearls-send"
        );

    const messages =
        shadowRoot.querySelector(
            ".askpearls-messages"
        );


    // =========================================================
    // SESSION AUTHENTICATION
    // =========================================================

    /**
     * Get a short-lived Widget Session JWT.
     *
     * IMPORTANT:
     *
     * tenantId is NOT considered sufficient authentication.
     *
     * The API must validate:
     *
     * - tenant
     * - website origin
     * - allowed domain
     * - tenant status
     * - subscription
     * - rate limits
     */

    async function createSession() {

        try {

            const response =
                await fetch(
                    `${ASKPEARLS.apiUrl}${ASKPEARLS.sessionEndpoint}`,
                    {
                        method: "POST",

                        headers: {
                            "Content-Type":
                                "application/json"
                        },

                        credentials: "omit",

                        body: JSON.stringify({

                            tenantId:
                                tenantId,

                            pageUrl:
                                window.location.href

                        })
                    }
                );


            if (!response.ok) {

                throw new Error(
                    `Session request failed: ${response.status}`
                );

            }


            const data =
                await response.json();


            if (!data.token) {

                throw new Error(
                    "AskPearls API did not return a session token."
                );

            }


            // Keep JWT only in memory.

            sessionToken =
                data.token;


            const expiresIn =
                Number(data.expiresIn || 900);


            sessionExpiresAt =
                Date.now() +
                (expiresIn * 1000);


            return sessionToken;

        } catch (error) {

            console.error(
                "[AskPearls] Unable to create widget session.",
                error
            );

            throw error;
        }
    }


    // ---------------------------------------------------------
    // Make sure session is valid
    // ---------------------------------------------------------

    async function ensureSession() {

        const refreshBuffer =
            30 * 1000;


        if (
            sessionToken &&
            Date.now() <
            sessionExpiresAt - refreshBuffer
        ) {

            return sessionToken;
        }


        return await createSession();
    }


    // =========================================================
    // API REQUEST
    // =========================================================

    async function apiRequest(
        endpoint,
        options = {}
    ) {

        const token =
            await ensureSession();


        const headers = {

            ...(options.headers || {}),

            "Authorization":
                `Bearer ${token}`,

            "Content-Type":
                "application/json"

        };


        let response =
            await fetch(
                `${ASKPEARLS.apiUrl}${endpoint}`,
                {
                    ...options,
                    headers
                }
            );


        // -----------------------------------------------------
        // JWT expired
        // -----------------------------------------------------

        if (response.status === 401) {

            sessionToken = null;

            sessionExpiresAt = 0;


            // Create a new session

            const newToken =
                await createSession();


            headers.Authorization =
                `Bearer ${newToken}`;


            response =
                await fetch(
                    `${ASKPEARLS.apiUrl}${endpoint}`,
                    {
                        ...options,
                        headers
                    }
                );
        }


        return response;
    }


    // =========================================================
    // UI
    // =========================================================

    function openWidget() {

        chat.hidden = false;

        launcher.classList.add(
            "askpearls-launcher-hidden"
        );

        isOpen = true;


        setTimeout(function () {

            input.focus();

        }, 50);
    }


    function closeWidget() {

        chat.hidden = true;

        launcher.classList.remove(
            "askpearls-launcher-hidden"
        );

        isOpen = false;
    }


    // ---------------------------------------------------------
    // Launcher
    // ---------------------------------------------------------

    launcher.addEventListener(
        "click",
        openWidget
    );


    // ---------------------------------------------------------
    // Close
    // ---------------------------------------------------------

    closeButton.addEventListener(
        "click",
        closeWidget
    );


    // =========================================================
    // MESSAGES
    // =========================================================

    function addMessage(
        text,
        type
    ) {

        const message =
            document.createElement("div");


        message.className =
            `askpearls-message askpearls-message-${type}`;


        // textContent prevents HTML injection.

        message.textContent =
            text;


        messages.appendChild(
            message
        );


        scrollMessages();
    }


    function scrollMessages() {

        messages.scrollTop =
            messages.scrollHeight;
    }


    // =========================================================
    // SEND MESSAGE
    // =========================================================

    async function sendMessage() {

        const text =
            input.value.trim();


        if (!text) {
            return;
        }


        // Add user's message

        addMessage(
            text,
            "user"
        );


        input.value = "";


        // Disable input while waiting

        setInputState(false);


        try {

            const response =
                await apiRequest(
                    ASKPEARLS.chatEndpoint,
                    {

                        method: "POST",

                        body:
                            JSON.stringify({

                                tenantId:
                                    tenantId,

                                message:
                                    text,

                                pageUrl:
                                    window.location.href

                            })
                    }
                );


            if (!response.ok) {

                throw new Error(
                    `Chat request failed: ${response.status}`
                );

            }


            const data =
                await response.json();


            addMessage(
                data.message ||
                "Thanks! How else can I help?",
                "bot"
            );


        } catch (error) {

            console.error(
                "[AskPearls] Chat error.",
                error
            );


            addMessage(
                "Sorry, something went wrong. Please try again.",
                "bot"
            );


        } finally {

            setInputState(true);

            input.focus();
        }
    }


    // =========================================================
    // INPUT STATE
    // =========================================================

    function setInputState(
        enabled
    ) {

        input.disabled =
            !enabled;

        sendButton.disabled =
            !enabled;
    }


    // ---------------------------------------------------------
    // Send button
    // ---------------------------------------------------------

    sendButton.addEventListener(
        "click",
        sendMessage
    );


    // ---------------------------------------------------------
    // Enter key
    // ---------------------------------------------------------

    input.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Enter" &&
                !event.shiftKey
            ) {

                event.preventDefault();

                sendMessage();
            }

        }
    );


    // =========================================================
    // INITIALIZATION
    // =========================================================

    async function initialize() {

        if (isInitialized) {
            return;
        }


        try {

            /*
             * We don't necessarily need to create the JWT
             * immediately when the page loads.
             *
             * For better performance, the session can be
             * created when the user first opens the widget.
             */

            isInitialized = true;


        } catch (error) {

            console.error(
                "[AskPearls] Initialization failed.",
                error
            );

        }
    }


    // =========================================================
    // PUBLIC API
    // =========================================================

    window.AskPearls = {

        open: function () {
            openWidget();
        },

        close: function () {
            closeWidget();
        },

        isOpen: function () {
            return isOpen;
        },

        getTenantId: function () {
            return tenantId;
        }

    };


    // =========================================================
    // Start
    // =========================================================

    initialize();

})();