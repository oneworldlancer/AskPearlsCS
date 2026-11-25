// Account & Settings: read-only subscription summary; billing owns plan management.
(function () {
    'use strict';
    const root = document.querySelector('.account-settings-page');
    if (!root || root.dataset.subscriptionInitialized === 'true') return;
    root.dataset.subscriptionInitialized = 'true';
    const card = root.querySelector('.as-plan-card');
    if (!card) return;
    root.querySelector('#asPlansModal')?.remove();
    // Replace legacy nodes so the account settings prototype cannot overwrite billing data.
    card.innerHTML = `<div class="as-plan-top"><div class="as-plan-icon"><i class="bi bi-stars" aria-hidden="true"></i></div><div class="as-plan-title"><small>YOUR SUBSCRIPTION</small><h4><span data-summary-plan>Loading…</span> <span class="as-plan-badge" data-summary-status></span></h4><p>Manage your plan and resource usage.</p></div></div>
    <div class="as-billing-summary-meta"><div><small>Catalog monthly price</small><strong data-summary-price>—</strong></div><div><small>Current period ends</small><strong data-summary-period>—</strong></div></div>
    <div class="as-plan-usage"><div class="as-usage-heading"><span>Widgets created</span><strong data-summary-count>—</strong></div><div class="as-usage-track" data-summary-track hidden><div class="as-usage-fill" data-summary-fill style="width:0%"></div></div><div class="as-usage-foot"><span data-summary-usage></span><span data-summary-remaining></span></div></div>
    <div class="as-plan-bottom"><span>Subscription management</span><div class="as-billing-summary-actions"><button type="button" class="as-btn as-btn-outline" data-summary-page="subscription">Manage Subscription <i class="bi bi-arrow-right"></i></button><button type="button" class="as-btn as-btn-primary" data-summary-page="subscription-plans">Upgrade Plan <i class="bi bi-arrow-up-right"></i></button></div></div><div class="as-plan-disclaimer" data-summary-note role="status">Loading subscription information…</div>`;
    const put = (key, value) => { card.querySelector('[data-summary-' + key + ']').textContent = value; };
    card.addEventListener('click', event => {
        const button = event.target.closest('[data-summary-page]');
        if (button && typeof window.loadPortalSection === 'function') window.loadPortalSection(button.dataset.summaryPage);
    });
    async function refresh() {
        try {
            const response = await fetch('/Account/Api/billing/overview', {credentials:'same-origin',cache:'no-store',headers:{Accept:'application/json'}});
            if (!response.ok) throw new Error('Request failed (' + response.status + ')');
            const data = await response.json();
            if (!root.isConnected) return;
            const c = data.current;
            if (!c) throw new Error('Subscription information unavailable');
            const name = String(c.plan || 'Unknown');
            const plan = (data.plans || []).find(p => [p.name,p.code].some(v => String(v || '').toLowerCase() === name.toLowerCase()));
            put('plan', name);
            put('status', c.status || 'Unknown');
            put('price', plan?.monthlyPriceUsd == null ? 'Not configured' : new Intl.NumberFormat('en-US',{style:'currency',currency:'USD'}).format(plan.monthlyPriceUsd));
            put('period', c.currentPeriodEndUtc ? new Date(c.currentPeriodEndUtc).toLocaleDateString() : 'Not scheduled');
            const used = Number(c.widgetsCreated || 0), limit = c.widgetLimit;
            put('count', used + ' / ' + (limit == null ? 'Unlimited' : limit));
            put('remaining', c.widgetsRemaining == null ? 'Unlimited' : c.widgetsRemaining + ' remaining');
            const percent = limit == null ? null : limit > 0 ? Math.round(100 * used / limit) : 0;
            card.querySelector('[data-summary-track]').hidden = percent == null;
            card.querySelector('[data-summary-fill]').style.width = Math.min(100,Math.max(0,percent || 0)) + '%';
            put('usage', percent == null ? 'No widget limit' : limit === 0 ? 'No widget allowance' : percent + '% of plan allowance used');
            put('note', c.isProvisioned ? 'Plan and usage loaded from your account. Manage changes in Subscription & Billing.' : 'No subscription has been provisioned. Contact support if this persists.');
        } catch(error) { put('plan','Unavailable'); put('note','Could not load subscription: ' + error.message); }
    }
    // Existing account initialization provisions Free asynchronously; refresh after it settles.
    refresh();
    const save = root.querySelector('#asSave');
    if (save?.disabled) {
        const observer = new MutationObserver(() => { if (!save.disabled || !root.isConnected) { observer.disconnect(); if(root.isConnected) refresh(); } });
        observer.observe(save,{attributes:true,attributeFilter:['disabled']});
        setTimeout(() => observer.disconnect(),30000);
    }
})();
