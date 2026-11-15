// Include AFTER app_page_account_settings_js.js on Account Settings page.
(function () {
    "use strict";
    const root = document.querySelector('.account-settings-page');
    if (!root || root.dataset.apCompaniesInitialized === 'true') return;
    root.dataset.apCompaniesInitialized = 'true';
    const el = id => root.querySelector('#' + id);
    const url = '/Account/Api/companies';
    let companies = [];
    let selectedToken = '';
    const fields = { companyName: 'asBusinessName', companyWebsiteUrl: 'asBusinessWebsite', companyLogoUrl: 'asBusinessLogo', companyIndustry: 'asBusinessIndustry', companySize: 'asBusinessSize', companyCountry: 'asBusinessCountry', companyCity: 'asBusinessCity', emailAddress: 'asBusinessEmail', phoneNumber: 'asBusinessPhone' };
    const status = message => { if (el('asBusinessStatus')) el('asBusinessStatus').textContent = message; };
    function headers (write) {
        const h = { Accept: 'application/json' };
        if (write) { h[ 'Content-Type' ] = 'application/json'; const token = root.querySelector('input[name="__RequestVerificationToken"]')?.value; if (token) h.RequestVerificationToken = token; }
        return h;
    }
    async function request (path, method = 'GET', data) {
        const response = await fetch(url + path, { method, credentials: 'same-origin', headers: headers(method !== 'GET'), ...(data ? { body: JSON.stringify(data) } : {}) });
        const body = await response.json().catch(() => ({}));
        if (!response.ok) throw Error(body.message || 'Company request failed.');
        return body;
    }
    function reset () {
        selectedToken = '';
        if (el('asCompanySelect')) el('asCompanySelect').value = '';
        Object.values(fields).forEach(id => { if (el(id)) el(id).value = ''; });
        status('New company');
    }
    function show (token) {
        selectedToken = token;
        const c = companies.find(x => x.companyTokenId === token);
        if (!c) { reset(); return; }
        for (const [ key, id ] of Object.entries(fields)) if (el(id)) el(id).value = c[ key ] || '';
        status('Editing ' + c.companyName);
    }
    async function loadCompanies () {
        const org = el('asCompanyOrganization')?.value;
        reset();
        if (!org) { companies = []; return; }
        companies = await request('?organizationTokenId=' + encodeURIComponent(org));
        const select = el('asCompanySelect');
        if (select) {
            select.replaceChildren(new Option('New company', ''));
            companies.forEach(c => select.add(new Option(c.companyName || '(Unnamed)', c.companyTokenId)));
        }
        status(companies.length + ' companies loaded.');
    }
    async function loadOrganizations () {
        const orgs = await request('/organizations');
        const select = el('asCompanyOrganization');
        if (!select) return;
        select.replaceChildren(new Option('Select organization', ''));
        orgs.forEach(o => select.add(new Option(o.organizationName || '(Unnamed)', o.organizationTokenId)));
        if (orgs.length) { select.value = orgs[ 0 ].organizationTokenId; await loadCompanies(); }
        else status('Save your organization in the Organization tab first.');
    }
    el('asCompanyOrganization')?.addEventListener('change', () => loadCompanies().catch(e => status(e.message)));
    el('asCompanySelect')?.addEventListener('change', e => show(e.target.value));
    el('asBusinessNew')?.addEventListener('click', reset);
    el('asBusinessSave')?.addEventListener('click', async () => {
        const organizationTokenId = el('asCompanyOrganization')?.value;
        const data = { organizationTokenId };
        for (const [ key, id ] of Object.entries(fields)) data[ key ] = el(id)?.value.trim() || '';
        if (!organizationTokenId || !data.companyName) { status('Select an organization and enter a company name.'); return; }
        const btn = el('asBusinessSave'); if (btn) btn.disabled = true;
        try {
            const saved = await request(selectedToken ? '/' + encodeURIComponent(selectedToken) : '', selectedToken ? 'PUT' : 'POST', data);
            await loadCompanies();
            if (el('asCompanySelect')) el('asCompanySelect').value = saved.companyTokenId;
            show(saved.companyTokenId); status('Company saved successfully.');
        } catch (e) { status(e.message); } finally { if (btn) btn.disabled = false; }
    });
    // Existing Account Settings navigation is responsible for switching panels.
    // A nav button with data-as-section="companies" is required.
    loadOrganizations().catch(e => status(e.message));
})();
