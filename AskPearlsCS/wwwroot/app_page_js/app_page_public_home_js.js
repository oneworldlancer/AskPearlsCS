(() => {
    'use strict';
    const root = document.documentElement;
    const home = document.querySelector('[data-home-panel]');
    const inner = document.querySelector('[data-inner-panel]');
    const content = document.querySelector('[data-inner-scroll]');
    const menuPanel = document.querySelector('[data-menu-panel]');
    const mobileMenu = document.querySelector('[data-mobile-menu]');
    const mobileToggle = document.querySelector('[data-mobile-toggle]');
    const themeToggle = document.querySelector('[data-theme-toggle]');
    const sections = {
        product: [ [ 'AI Chat Assistant', 'Answer visitor questions instantly', 'how' ], [ 'Lead Capture', 'Turn inquiries into qualified leads', 'how' ], [ 'Widget Builder', 'Create and customize your AI assistant', 'how' ], [ 'Automation', 'Keep customer conversations moving', 'how' ] ],
        solutions: [ [ 'Small Businesses', 'Engage visitors and answer questions', 'how' ], [ 'Startups', 'Launch an intelligent customer experience', 'how' ], [ 'E-commerce', 'Help shoppers find answers', 'how' ], [ 'Professional Services', 'Qualify client inquiries', 'how' ] ],
        cases: [ [ 'Customer Support', 'Respond to common inquiries', 'how' ], [ 'Lead Qualification', 'Capture relevant visitor details', 'how' ], [ 'Website Engagement', 'Guide visitors to next steps', 'how' ], [ 'Follow-Up Workflows', 'Build stronger customer relationships', 'how' ] ],
        resources: [ [ 'Getting Started', 'Create your first widget', 'how' ], [ 'Portal Walkthroughs', 'Learn the AskPearls workspace', 'how' ], [ 'Developer Documentation', 'Technical integration guides', 'developers' ], [ 'Contact Us', 'Get in touch', 'contact' ] ]
    };
    // These public-page sections are starter content. Replace legal copy with reviewed policies before publishing.
    const pages = {
        how: `<h2>How AskPearls Works</h2><p>Start with a free account, build your widget, and add it to your website.</p><div class="ap-guide"><strong>01 — Join AskPearls</strong><p>Open the Portal and continue with the existing sign-in process.</p></div><div class="ap-guide"><strong>02 — Create Your Widget</strong><p>Choose its name, customize its design, and configure questions and recommendations.</p></div><div class="ap-guide"><strong>03 — Publish & Embed</strong><p>Publish a version and copy the installation snippet from the Portal. Follow the installation instructions for your website.</p></div><p><a href="https://portal.askpearls.com/">Join AskPearls →</a></p>`,
        about: `<h2>About AskPearls</h2><p>AskPearls is an AI customer engagement platform designed to help businesses connect with visitors, answer questions, and turn conversations into opportunities.</p><h3>Our focus</h3><p>Simple setup, useful customer interactions, and a flexible website widget that businesses can configure and manage from their Portal.</p>`,
        contact: `<h2>Contact Us</h2><p>Have a question about AskPearls or need help getting started?</p><div class="ap-guide"><strong>Support</strong><p>Our contact form and support details will be connected when the public contact workflow is finalized.</p></div><p>For technical guides, visit the <a href="https://developer.askpearls.com/">Developer Center</a>.</p>`,
        privacy: `<h2>Privacy Policy</h2><p>The official AskPearls Privacy Policy is being prepared. This page is a placeholder and must not be presented as the final legal policy.</p><h3>Before publication</h3><p>Document the actual information collected, lawful purposes, cookies, Google authentication, retention, third-party services, and user rights. Have the policy reviewed for applicable legal requirements.</p>`,
        terms: `<h2>Terms of Service</h2><p>The official AskPearls Terms of Service are being prepared. This page is a placeholder and must not be presented as the final legal terms.</p><h3>Before publication</h3><p>Define account responsibilities, acceptable use, service availability, permitted integrations, intellectual property, and other applicable terms.</p>`,
        pricing: `<h2>Get Started Free</h2><p>AskPearls currently offers a free account for getting started with widget creation and testing.</p><div class="ap-guide"><strong>Free Account</strong><p>Create and configure your AskPearls widget in the Portal. Published usage limits and paid subscriptions will be described when those features are available.</p><a href="https://portal.askpearls.com/">Join AskPearls →</a></div>`
    };
    function setTheme (value) {
        const theme = value === 'dark' ? 'dark' : 'light';
        root.dataset.theme = theme;
        themeToggle?.setAttribute('aria-label', theme === 'dark' ? 'Switch to light theme' : 'Switch to dark theme');
        document.querySelector('meta[name="theme-color"]')?.setAttribute('content', theme === 'dark' ? '#091411' : '#f6f8fb');
        try { localStorage.setItem('askpearls-public-theme', theme); } catch (_) { }
    }
    setTheme(root.dataset.theme);
    themeToggle?.addEventListener('click', () => setTheme(root.dataset.theme === 'dark' ? 'light' : 'dark'));


    function closeMenus () {

        menuPanel.hidden = true;
        mobileMenu.hidden = true;

        // Remove expanded mobile submenus.
        mobileMenu.querySelectorAll('.ap-mobile-submenu')
            .forEach(submenu => submenu.remove());

        mobileToggle?.setAttribute('aria-expanded', 'false');

        document.querySelectorAll('[data-menu][aria-expanded]')
            .forEach(button => {
                button.setAttribute('aria-expanded', 'false');
            });
    } 

    function showView (name, push = true) {
        closeMenus();
        const isHome = name === 'home' || !pages[ name ];
        home.hidden = !isHome;
        inner.hidden = isHome;
        if (!isHome) { content.innerHTML = pages[ name ]; content.scrollTop = 0; }
        if (push) history.pushState({ view: isHome ? 'home' : name }, '', isHome ? '/' : `/#${ name }`);
    }


    function openMenu (name, trigger) {

        if (!sections[ name ]) return;

        const isMobile = window.matchMedia('(max-width: 780px)').matches;

        if (isMobile) {

            // Ensure desktop dropdown is closed.
            menuPanel.hidden = true;

            const existing = mobileMenu.querySelector('.ap-mobile-submenu');
            const alreadyOpen =
                existing?.dataset.section === name;

            // Remove any existing mobile submenu.
            existing?.remove();

            // Reset expanded states.
            mobileMenu.querySelectorAll('[data-menu]').forEach(button => {
                button.setAttribute('aria-expanded', 'false');
            });

            // Tapping the same parent closes its submenu.
            if (alreadyOpen) return;

            const submenu = document.createElement('div');
            submenu.className = 'ap-mobile-submenu';
            submenu.dataset.section = name;

            submenu.innerHTML = sections[ name ]
                .map(([ title, desc, view ]) => `
                <button type="button" data-view="${ view }">
                    <strong>${ title }</strong>
                    <small>${ desc }</small>
                </button>
            `)
                .join('');

            trigger.insertAdjacentElement('afterend', submenu);
            trigger.setAttribute('aria-expanded', 'true');

            return;
        }

        // Desktop dropdown
        const isOpen =
            !menuPanel.hidden &&
            menuPanel.dataset.active === name;

        menuPanel.hidden = true;

        document.querySelectorAll('.ap-nav [data-menu]').forEach(button => {
            button.setAttribute('aria-expanded', 'false');
        });

        if (isOpen) return;

        menuPanel.innerHTML = sections[ name ]
            .map(([ title, desc, view ]) => `
            <button type="button" data-view="${ view }">
                ${ title }
                <small>${ desc }</small>
            </button>
        `)
            .join('');

        menuPanel.dataset.active = name;
        menuPanel.hidden = false;

        trigger.setAttribute('aria-expanded', 'true');
    }



    document.addEventListener('click', e => {
        const view = e.target.closest('[data-view]');
        if (view) { showView(view.dataset.view); return; }
        const nav = e.target.closest('[data-menu]');
        if (nav) { openMenu(nav.dataset.menu, nav); return; }
        if (e.target.closest('[data-home]')) {
            e.preventDefault();

            // Navigate to homepage and reload it.
            window.location.assign('/');

            return;
        }
        if (!e.target.closest('[data-menu-panel]') && !e.target.closest('.ap-nav')) menuPanel.hidden = true;
    });
    mobileToggle?.addEventListener('click', () => {
        const opening = mobileMenu.hidden;
        closeMenus();
        mobileMenu.hidden = !opening;
        mobileToggle.setAttribute('aria-expanded', String(opening));
    });
    document.addEventListener('keydown', e => { if (e.key === 'Escape') closeMenus(); });
    window.addEventListener('popstate', () => showView(location.hash.slice(1) || 'home', false));
    showView(location.hash.slice(1) || 'home', false);
})();
