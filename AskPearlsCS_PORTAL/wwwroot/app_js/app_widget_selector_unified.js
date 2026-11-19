/* AskPearls global New Widget action. Does not alter page API handlers. */
(function () {
    'use strict';
    const URL = '/Dashboard/WidgetBuilder';
    const label = 'Create New Widget';
    function createButton () {
        const a = document.createElement('a');
        a.href = URL;
        a.className = 'ap-global-new-widget ap-widget-builder-link';
        a.setAttribute('data-page', 'widget-builder');
        a.setAttribute('data-widget-builder-create', '');
        a.setAttribute('title', label);
        a.setAttribute('aria-label', label);
        a.innerHTML = '<i class="bi bi-plus-lg" aria-hidden="true"></i><span class="ap-global-new-widget-label">New Widget</span>';
        return a;
    }
    function setup () {
        // Use actual separate desktop/mobile action containers from _Layout.cshtml.
        const desktop = document.querySelector('.header-actions-desktop');
        const mobile = document.querySelector('.header-actions-mobile');
        if (desktop && !desktop.querySelector('.ap-global-new-widget')) {
            // Existing top-header New Widget controls may have been injected by older scripts.
            const old = Array.from(desktop.querySelectorAll('a,button')).find(x =>
                /new widget/i.test((x.textContent || '').trim()) && !x.closest('.dropdown-menu'));
            if (old) {
                old.classList.add('ap-global-new-widget');
                old.setAttribute('aria-label', label);
                old.setAttribute('title', label);
                if (!old.querySelector('.ap-global-new-widget-label')) {
                    old.innerHTML = '<i class="bi bi-plus-lg" aria-hidden="true"></i><span class="ap-global-new-widget-label">New Widget</span>';
                }
            } else {
                desktop.prepend(createButton());
            }
        }
        if (mobile && !mobile.querySelector('.ap-global-new-widget')) {
            const button = createButton();
            const search = mobile.querySelector('.search-toggle');
            if (search) search.insertAdjacentElement('afterend', button);
            else mobile.prepend(button);
        }
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', setup, { once: true });
    else setup();
})();
