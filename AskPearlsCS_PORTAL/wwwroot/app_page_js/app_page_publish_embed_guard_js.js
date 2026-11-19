/* Publish eligibility guard for the existing Publish & Embed implementation.
   UI defense only: API must independently enforce verified-domain eligibility. */
(function () {
    'use strict';

    const root = document.querySelector('.publish-embed-page');
    if (!root || root.dataset.peGuardInitialized === 'true') return;

    const select = root.querySelector('#peWidgetSelect');
    const publishButton = root.querySelector('#pePublishBtn');
    const reason = root.querySelector('#pePublishReason');
    const domainList = root.querySelector('#peDomainList');
    if (!select || !publishButton || !reason || !domainList) return;

    root.dataset.peGuardInitialized = 'true';
    const endpoint = '/Widget/Api/widgets';
    let eligible = false;
    let pending = true;
    let requestNumber = 0;
    let refreshing = false;
    let refreshTimer;

    const normalized = value => String(value ?? '').trim().toLowerCase();
    const isVerified = domain => domain && typeof domain === 'object' &&
        normalized(domain.verificationStatus ?? domain.VerificationStatus) === 'verified';

    const numberOrZero = value => {
        const number = Number.parseInt(value ?? '0', 10);
        return Number.isFinite(number) && number > 0 ? number : 0;
    };

    const draftVersion = widget => numberOrZero(
        widget.draftVersionNumber ?? widget.DraftVersionNumber ??
        widget.widgetDraftVersion ?? widget.WidgetDraftVersion
    );

    const publishedVersion = widget => numberOrZero(
        widget.publishedVersionNumber ?? widget.PublishedVersionNumber ??
        widget.widgetPublishedVersion ?? widget.WidgetPublishedVersion
    );

    const hasDraft = widget => {
        const flag = widget.hasDraft ?? widget.HasDraft;
        // A positive version is authoritative even when an older API returns a stale flag.
        return draftVersion(widget) > 0 || flag === true || flag === 'true';
    };

    const hasPublishedVersion = widget => {
        const flag = widget.hasPublishedVersion ?? widget.HasPublishedVersion;
        // A positive live version is the strongest indicator of a published widget.
        // Do not use status alone: some APIs may return 'published' for a widget
        // that has subsequently acquired a new draft.
        return publishedVersion(widget) > 0 || flag === true || flag === 'true';
    };

    const setReason = (message, allowed) => {
        eligible = allowed;
        reason.textContent = message;
        reason.dataset.eligible = String(allowed);
        publishButton.disabled = !allowed;
        publishButton.setAttribute('aria-disabled', String(!allowed));
        publishButton.title = allowed ? 'Publish the selected draft' : message;
    };

    const getWidgets = async () => {
        const response = await fetch(endpoint, {
            credentials: 'same-origin',
            headers: { Accept: 'application/json' }
        });
        if (!response.ok) throw new Error('Unable to load widget publishing status.');
        const data = await response.json();
        const payload = data.data ?? data;
        return Array.isArray(payload) ? payload :
            Array.isArray(payload.widgets) ? payload.widgets :
                Array.isArray(payload.items) ? payload.items :
                    Array.isArray(payload.jsonlist) ? payload.jsonlist : [];
    };

    const keyOf = widget => String(
        widget.publicKey ?? widget.PublicKey ??
        widget.widgetPublicKey ?? widget.WidgetPublicKey ?? ''
    );

    async function refresh () {
        const key = select.value;
        const requestId = ++requestNumber;
        pending = true;
        setReason(
            key ? 'Checking saved draft and verified domains…' :
                'Select a widget to check publishing eligibility.',
            false
        );
        if (!key) {
            pending = false;
            return;
        }

        try {
            const [ widgets, domainResponse ] = await Promise.all([
                getWidgets(),
                fetch(endpoint + '/' + encodeURIComponent(key) + '/domains', {
                    credentials: 'same-origin',
                    headers: { Accept: 'application/json' }
                })
            ]);
            if (!domainResponse.ok) throw new Error('Unable to verify the domain list.');
            const domainData = await domainResponse.json();
            if (domainData.success === false) {
                throw new Error(domainData.message || 'Domain list request failed.');
            }
            if (requestId !== requestNumber || select.value !== key || !root.isConnected) return;

            const widget = widgets.find(w => keyOf(w) === key);
            const domains = Array.isArray(domainData.domains) ? domainData.domains : [];
            const verified = domains.filter(isVerified).length;
            const waiting = domains.length - verified;

            if (!widget) {
                setReason('Selected widget was not found. Refresh the page.', false);
            } else if (!hasDraft(widget)) {
                setReason(
                    hasPublishedVersion(widget)
                        ? 'Your widget is live. Create or save a new draft to publish an update.'
                        : 'Save a widget draft before publishing.',
                    false
                );
            } else if (verified === 0) {
                setReason(
                    'Waiting for verified domain. Verify at least one allowed domain before publishing.',
                    false
                );
            } else {
                setReason(
                    verified + ' verified domain' + (verified === 1 ? '' : 's') +
                    (waiting > 0 ? ' · ' + waiting + ' pending' : '') +
                    '. Ready to publish.',
                    true
                );
            }
        } catch (error) {
            if (requestId === requestNumber && select.value === key) {
                setReason(error.message || 'Unable to check publish eligibility.', false);
            }
        } finally {
            if (requestId === requestNumber) pending = false;
        }
    }

    // Capture phase prevents the existing handler from publishing when UI is ineligible.
    publishButton.addEventListener('click', event => {
        if (pending || !eligible) {
            event.preventDefault();
            event.stopImmediatePropagation();
            event.stopPropagation();
        }
        // The existing page handler owns the confirmation and publish request.
    }, true);

    select.addEventListener('change', () => { void refresh(); });

    // Observe existing domain UI updates (add/remove/verify), without rebuilding its DOM.
    const observer = new MutationObserver(() => {
        if (!select.value || refreshing) return;
        clearTimeout(refreshTimer);
        refreshTimer = setTimeout(() => {
            refreshing = true;
            void refresh().finally(() => { refreshing = false; });
        }, 350);
    });
    observer.observe(domainList, { childList: true, subtree: true, characterData: true });

    // Existing page JS may re-enable the button while rendering.
    const buttonObserver = new MutationObserver(() => {
        if (!eligible && !publishButton.disabled) publishButton.disabled = true;
    });
    buttonObserver.observe(publishButton, {
        attributes: true,
        attributeFilter: [ 'disabled' ]
    });

    // Refresh after publishing to reflect draft/live version changes.
    // This is a fallback; the page's own successful publish handler should update its UI.
    publishButton.addEventListener('click', () => {
        if (eligible) {
            setTimeout(() => { if (root.isConnected) void refresh(); }, 1500);
        }
    });

    void refresh();
})();
