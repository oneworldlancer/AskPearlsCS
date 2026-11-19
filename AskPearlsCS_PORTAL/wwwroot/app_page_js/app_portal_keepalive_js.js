
(function () {
    "use strict";

    const KEEP_ALIVE_INTERVAL = 5 * 60 * 1000;

    async function keepAlive () {
        try {
            const response = await fetch("/Portal/KeepAlive", {
                method: "GET",
                credentials: "same-origin",
                cache: "no-store",
                headers: {
                    "Accept": "application/json"
                }
            });

            if (response.status === 401 ||
                response.redirected ||
                !response.ok) {
                console.warn(
                    "[AskPearls] Authentication may have expired."
                );
                return;
            }

            console.debug(
                "[AskPearls] Session keep-alive successful."
            );

        } catch (error) {
            console.warn(
                "[AskPearls] Keep-alive failed:",
                error
            );
        }
    }

    // Only one timer per browser tab.
    if (window.__askPearlsKeepAliveStarted) {
        return;
    }

    window.__askPearlsKeepAliveStarted = true;

    setInterval(keepAlive, KEEP_ALIVE_INTERVAL);
})();
