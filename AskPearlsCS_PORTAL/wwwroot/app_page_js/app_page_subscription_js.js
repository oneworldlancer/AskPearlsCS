// Load AFTER app_page_account_settings_js.js in the Account Settings partial.
// Enhances the existing plan modal without changing the existing page controller.
(function () {
    'use strict';
    const root = document.querySelector('.account-settings-page');
    if (!root || root.dataset.subscriptionInitialized === 'true') return;
    root.dataset.subscriptionInitialized = 'true';
    const byId = id => root.querySelector('#' + id);
    const endpoint = '/Account/Api/subscription/';
    let current = null;
    let plans = [];
    let chosen = null;

    async function get (path) {
        const response = await fetch(endpoint + path, {
            method: 'GET', credentials: 'same-origin', cache: 'no-store',
            headers: { Accept: 'application/json' }
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) throw new Error(data.message || `Subscription request failed (${ response.status }).`);
        return data;
    }

    function renderCurrent () {
        if (!current) return;
        const put = (id, value) => { if (byId(id)) byId(id).textContent = value; };
        put('asCurrentPlanName', current.plan + ' Plan');
        put('asCurrentPlanBadge', current.status === 'active' ? 'Current' : current.status);
        put('asWidgetsUsed', current.widgetsCreated);
        put('asWidgetsLimit', current.widgetLimit == null ? 'Unlimited' : current.widgetLimit);
        put('asWidgetsRemaining', current.widgetsRemaining == null ? 'Unlimited' : current.widgetsRemaining + ' remaining');
        put('asUsageSummary', current.widgetLimit == null ? 'No widget limit' :
            `${ Math.round(100 * current.widgetsCreated / Math.max(1, current.widgetLimit)) }% of your plan allowance used`);
        if (byId('asUsageFill')) byId('asUsageFill').style.width =
            `${ current.widgetLimit == null ? 0 : Math.min(100, 100 * current.widgetsCreated / Math.max(1, current.widgetLimit)) }%`;
        put('asSubscriptionNote', current.isProvisioned
            ? 'Plan and usage loaded from your account. Paid checkout is not yet available.'
            : 'No subscription has been provisioned. Contact support if this persists.');
    }

    function renderPlans () {
        for (const plan of plans) {
            const card = root.querySelector(`.as-plan-option[data-plan="${ plan.name }"]`);
            if (!card) continue;
            const price = card.querySelector('.as-plan-price strong');
            if (price) price.textContent = '$' + plan.monthlyPriceUsd;
            const isCurrent = current && current.plan.toLowerCase() === plan.code && current.status === 'active';
            const badge = card.querySelector('.as-plan-current');
            if (badge) badge.hidden = !isCurrent;
            const button = card.querySelector('[data-select-plan]');
            if (button) {
                button.disabled = isCurrent;
                button.textContent = isCurrent ? 'Your current plan' : `View ${ plan.name }`;
            }
        }
        const note = root.querySelector('.as-plans-demo-note');
        if (note) note.textContent = 'Preview prices only — paid checkout is not enabled.';
    }

    async function refresh () {
        try {
            [ current, plans ] = await Promise.all([ get('current'), get('plans') ]);
            renderCurrent(); renderPlans();
        } catch (error) {
            console.error('Subscription loading failed:', error);
            const note = byId('asSubscriptionNote');
            if (note) note.textContent = 'Could not load subscription: ' + error.message;
        }
    }

    // Capture clicks before the original prototype handlers so we do not show a
    // misleading "selected successfully" demo confirmation.
    root.addEventListener('click', event => {
        const select = event.target.closest('[data-select-plan]');
        if (select && root.contains(select)) {
            event.preventDefault(); event.stopPropagation();
            chosen = plans.find(p => p.name.toLowerCase() === select.dataset.selectPlan.toLowerCase());
            if (!chosen) return;
            const confirmation = byId('asPlanConfirmation');
            if (byId('asPlanConfirmationTitle')) byId('asPlanConfirmationTitle').textContent = chosen.name + ' plan';
            if (byId('asPlanConfirmationText')) byId('asPlanConfirmationText').textContent =
                'Paid upgrades are not available until billing is connected. No payment or plan change will occur.';
            if (byId('asPlanConfirm')) {
                byId('asPlanConfirm').textContent = 'Checkout unavailable';
                byId('asPlanConfirm').disabled = true;
            }
            if (confirmation) confirmation.hidden = false;
            root.querySelectorAll('.as-plan-option').forEach(card =>
                card.classList.toggle('as-plan-selected', card.dataset.plan === chosen.name));
            return;
        }
        if (event.target.closest('#asPlanConfirm')) {
            event.preventDefault(); event.stopPropagation();
        }
    }, true);

    // Refresh whenever the modal is opened, including after account changes.
    byId('asUpgradePlan')?.addEventListener('click', refresh);
    refresh();
})();
