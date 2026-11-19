
/* AskPearls — Unified Draft / Live selector labels
   Presentation-only enhancement. Does not replace page-specific widget logic.
   Source: authenticated GET /Widget/Api/widgets.
*/
(function () {
    'use strict';

    const pairs = [
        [ 'activeWidgetSelect', 'activeWidgetVersion' ],
        [ 'wbWidgetSelect', 'wbWidgetVersion' ],
        [ 'qnaWidgetSelect', 'qnaWidgetVersion' ],
        [ 'riWidgetSelect', 'riWidgetVersion' ],
        [ 'drWidgetSelect', 'drWidgetVersion' ],
        [ 'spWidgetSelect', 'spWidgetVersion' ],
        [ 'dcWidgetSelect', 'dcWidgetVersion' ],
        [ 'peWidgetSelect', 'peVersion' ]
    ];

    let widgets = [];
    let fetchedAt = 0;
    let pending = null;

    const cacheMs = 60000;
    const timers = new WeakMap();
    const registered = new WeakSet();

    function prop (obj, ...names) {
        for (const name of names) {
            if (
                obj &&
                obj[ name ] !== undefined &&
                obj[ name ] !== null
            ) {
                return obj[ name ];
            }
        }

        return undefined;
    }

    function version (value) {
        const n = Number.parseInt(
            String(value ?? '0'),
            10
        );

        return Number.isFinite(n) && n > 0 ? n : 0;
    }

    function keyOf (widget) {
        return String(
            prop(
                widget,
                'publicKey',
                'PublicKey',
                'widgetPublicKey',
                'WidgetPublicKey'
            ) ?? ''
        );
    }

    function getVersions (widget) {
        return {
            draft: version(
                prop(
                    widget,
                    'draftVersionNumber',
                    'DraftVersionNumber',
                    'widgetDraftVersion',
                    'WidgetDraftVersion'
                )
            ),

            live: version(
                prop(
                    widget,
                    'publishedVersionNumber',
                    'PublishedVersionNumber',
                    'widgetPublishedVersion',
                    'WidgetPublishedVersion'
                )
            )
        };
    }

    function format (widget) {
        const v = getVersions(widget);

        return `Draft v${ v.draft } · Live v${ v.live }`;
    }

    async function getWidgets (force) {
        if (
            !force &&
            widgets.length &&
            Date.now() - fetchedAt < cacheMs
        ) {
            return widgets;
        }

        if (pending) {
            return pending;
        }

        pending = (async function () {
            const response = await fetch(
                '/Widget/Api/widgets',
                {
                    credentials: 'same-origin',
                    headers: {
                        'Accept': 'application/json'
                    }
                }
            );

            if (!response.ok) {
                throw new Error(
                    `Widgets request failed: ${ response.status }`
                );
            }

            const json = await response.json();

            if (json.success === false) {
                throw new Error(
                    'Widgets request was not successful'
                );
            }

            const result = Array.isArray(json)
                ? json
                : (
                    Array.isArray(json.widgets)
                        ? json.widgets
                        : (
                            Array.isArray(json.data?.widgets)
                                ? json.data.widgets
                                : []
                        )
                );

            widgets = result;
            fetchedAt = Date.now();

            return widgets;
        })();

        try {
            return await pending;
        } finally {
            pending = null;
        }
    }

    function selectedWidget (select, list) {
        const value = String(
            select.value || ''
        ).trim();

        if (!value) {
            return null;
        }

        return list.find(
            w => keyOf(w) === value
        ) || null;
    }

    async function update (select, label, force) {
        if (
            !select.isConnected ||
            !label.isConnected
        ) {
            return;
        }

        const selectedValue = select.value;

        if (!selectedValue) {
            return;
        }

        try {
            const list = await getWidgets(force);

            if (
                !select.isConnected ||
                select.value !== selectedValue
            ) {
                return;
            }

            const widget = selectedWidget(
                select,
                list
            );

            if (!widget) {
                return;
            }

            const result = format(widget);

            if (label.textContent.trim() !== result) {
                label.textContent = result;
            }

        } catch (error) {
            console.warn(
                '[AskPearls unified versions]',
                error
            );
        }
    }

    function schedule (select, label, force) {
        const previous = timers.get(select);

        if (previous) {
            clearTimeout(previous);
        }

        timers.set(
            select,
            setTimeout(
                () => update(select, label, force),
                0
            )
        );
    }

    function register () {
        pairs.forEach(([ selectId, versionId ]) => {
            const select = document.getElementById(
                selectId
            );

            const label = document.getElementById(
                versionId
            );

            if (
                !select ||
                !label ||
                registered.has(select)
            ) {
                return;
            }

            registered.add(select);

            select.addEventListener(
                'change',
                () => {
                    // Let the page's existing change handlers run first.
                    schedule(select, label, false);
                }
            );

            // Handle options loaded asynchronously.
            const optionObserver = new MutationObserver(
                () => {
                    schedule(select, label, false);
                }
            );

            optionObserver.observe(select, {
                childList: true,
                subtree: true,
                attributes: true,
                attributeFilter: [
                    'value',
                    'selected'
                ]
            });

            // Normalize labels written by page-specific scripts.
            const labelObserver = new MutationObserver(
                () => {
                    if (!select.value) {
                        return;
                    }

                    const text = label.textContent.trim();

                    if (
                        /^Draft v\d+ · Live v\d+$/.test(text)
                    ) {
                        return;
                    }

                    schedule(select, label, true);
                }
            );

            labelObserver.observe(label, {
                childList: true,
                characterData: true,
                subtree: true
            });

            schedule(select, label, false);
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener(
            'DOMContentLoaded',
            register,
            { once: true }
        );
    } else {
        register();
    }

    // Dashboard swaps Razor partials without full navigation.
    const startObserver = () => {
        if (!document.body) {
            return;
        }

        const observer = new MutationObserver(
            records => {
                if (
                    records.some(
                        r => r.addedNodes.length
                    )
                ) {
                    register();
                }
            }
        );

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    };

    if (document.readyState === 'loading') {
        document.addEventListener(
            'DOMContentLoaded',
            startObserver,
            { once: true }
        );
    } else {
        startObserver();
    }

})();
