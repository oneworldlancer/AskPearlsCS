
/* AskPearls shared widget selector presentation only.
   Existing page JavaScript remains the owner of dropdown options, selection,
   version labels, navigation and API requests. */
(function () {
    'use strict';

    const pageSelectors = {
        'my-widgets': 'activeWidgetSelect',
        'widget-builder': 'wbWidgetSelect',
        'questions-answers': 'qnaWidgetSelect',
        'recommendation-items': 'riWidgetSelect',
        'decision-rules': 'drWidgetSelect',
        'scoring-preview': 'spWidgetSelect',
        'design-customize': 'dcWidgetSelect',
        'publish-embed': 'peWidgetSelect'
    };

    const pages = '.ap-page .ap-widget-selector';

    function normalize () {
        document.querySelectorAll(pages).forEach(function (selector) {
            if (selector.dataset.unifiedSelectorReady === 'true') return;

            const select = selector.querySelector('select');

            const info = select && select.closest(
                '.ap-widget-selector-info, .wb-selector-info, .sp-selector-info, .dc-selector-info, .pe-widget-info'
            );

            const icon = selector.querySelector(
                '.ap-widget-selector-icon, .wb-selector-icon, .sp-selector-icon, .dc-selector-icon, .pe-widget-icon'
            );

            const version = selector.querySelector(
                '.ap-widget-selector-version, .wb-selector-version, .sp-current-version, .dc-version, .pe-version'
            );

            if (!select || !info || !icon || !version) return;

            selector.classList.add('ap-unified-widget-selector');

            info.classList.add('ap-widget-selector-info');
            icon.classList.add('ap-widget-selector-icon');
            version.classList.add('ap-widget-selector-version');
            select.classList.add('ap-widget-selector-control');

            const help = info.querySelector('small');

            if (help) {
                help.classList.add('ap-widget-selector-help');
            }

            const versionLabel = version.querySelector('span');

            if (versionLabel) {
                versionLabel.textContent = 'Draft / Live';
            }

            // Hide duplicate create buttons only when
            // the global header button exists.
            if (document.querySelector(
                '.ap-global-new-widget, [data-global-new-widget]'
            )) {
                selector.classList.add('ap-global-create-present');
            }

            selector.dataset.unifiedSelectorReady = 'true';
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', normalize, {
            once: true
        });
    } else {
        normalize();
    }

    // Dashboard content can be injected after initial page load.
    const observer = new MutationObserver(function (records) {
        if (records.some(function (r) {
            return r.addedNodes.length > 0;
        })) {
            normalize();
        }
    });


    const start = function () {
        if (document.body) {
            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', start, {
            once: true
        });
    } else {
        start();
    }
})();
