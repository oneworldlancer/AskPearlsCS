
(function () {
    'use strict';

    function initAskPearlsSidebar () {
        const sidebar = document.getElementById('askPearlsSidebar');
        const toggle = document.querySelector('#askPearlsSidebar')
            ? document.querySelector('.sidebar-toggle')
            : null;
        const backdrop = document.getElementById('apSidebarBackdrop');

        if (!sidebar || !toggle || !backdrop) {
            console.error('AskPearls sidebar initialization failed:', {
                sidebarFound: !!sidebar,
                toggleFound: !!toggle,
                backdropFound: !!backdrop
            });
            return;
        }

        const mobileQuery = window.matchMedia('(max-width: 1199.98px)');

        function openSidebar () {
            if (!mobileQuery.matches) return;

            document.body.classList.add('sidebar-open');
            toggle.setAttribute('aria-expanded', 'true');
            backdrop.setAttribute('aria-hidden', 'false');
        }

        function closeSidebar () {
            document.body.classList.remove('sidebar-open');
            toggle.setAttribute('aria-expanded', 'false');
            backdrop.setAttribute('aria-hidden', 'true');
        }

        toggle.setAttribute('aria-controls', 'askPearlsSidebar');
        toggle.setAttribute('aria-expanded', 'false');

        toggle.addEventListener('click', function (event) {
            event.preventDefault();

            if (!mobileQuery.matches) return;

            if (document.body.classList.contains('sidebar-open')) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });

        backdrop.addEventListener('click', closeSidebar);

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                closeSidebar();
            }
        });

        mobileQuery.addEventListener?.('change', closeSidebar);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAskPearlsSidebar);
    } else {
        initAskPearlsSidebar();
    }
})();
