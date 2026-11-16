// Load after app_page_account_settings_js.js.
(function () {
    "use strict";
    const root = document.querySelector('.account-settings-page');
    if (!root || root.dataset.apCompaniesInitialized === 'true') return;
    root.dataset.apCompaniesInitialized = 'true';
    const el = id => root.querySelector('#' + id);
    const url = '/Account/Api/companies';
    const fields = {
        companyName: 'asBusinessName', companyWebsiteUrl: 'asBusinessWebsite',
        companyLogoUrl: 'asBusinessLogo', companyIndustry: 'asBusinessIndustry',
        companySize: 'asBusinessSize', companyCountry: 'asBusinessCountry',
        companyCity: 'asBusinessCity', emailAddress: 'asBusinessEmail',
        phoneNumber: 'asBusinessPhone'
    };
    let companies = [], selectedToken = '';
    const clean = value => value == null || String(value).trim() === '0' ? '' : String(value).trim();
    const status = message => { const target = el('asBusinessStatus'); if (target) target.textContent = message; };
    function headers (write) {
        const h = { Accept: 'application/json' };
        if (write) {
            h[ 'Content-Type' ] = 'application/json';
            const token = root.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (token) h.RequestVerificationToken = token;
        }
        return h;
    }
    async function request (path, method = 'GET', data) {
        const response = await fetch(url + path, {
            method, credentials: 'same-origin', cache: 'no-store', headers: headers(method !== 'GET'),
            ...(data !== undefined ? { body: JSON.stringify(data) } : {})
        });
        const body = await response.json().catch(() => ({}));
        if (!response.ok || body.success === false)
            throw Error(body.message || `Company request failed (${ response.status }).`);
        return body;
    }
    function reset () {
        selectedToken = '';
        if (el('asCompanySelect')) el('asCompanySelect').value = '';
        Object.values(fields).forEach(id => { if (el(id)) el(id).value = ''; });
        status('New company');
    }
    function show (token) {
        const c = companies.find(x => x.companyTokenId === token);
        if (!c) { reset(); return; }
        selectedToken = token;
        for (const [ key, id ] of Object.entries(fields)) {
            const control = el(id);
            if (!control) continue;
            const value = clean(c[ key ]);
            if (control.tagName === 'SELECT' && value && !Array.from(control.options).some(o => o.value === value))
                control.add(new Option(value + ' (existing)', value));
            control.value = value;
        }
        status('Editing ' + clean(c.companyName));
    }
    async function loadCompanies (preferredToken = '') {
        const org = el('asCompanyOrganization')?.value;
        reset();
        const select = el('asCompanySelect');
        if (select) select.replaceChildren(new Option('New company', ''));
        companies = [];
        if (!org) { status('Select an organization first.'); return; }
        const result = await request('?organizationTokenId=' + encodeURIComponent(org));
        if (!Array.isArray(result)) throw Error('Invalid companies response.');
        companies = result;
        if (select) companies.forEach(c => select.add(new Option(clean(c.companyName) || '(Unnamed)', c.companyTokenId)));
        if (preferredToken && companies.some(c => c.companyTokenId === preferredToken)) {
            if (select) select.value = preferredToken;
            show(preferredToken);
        } else status(companies.length + ' companies loaded.');
    }
    async function loadOrganizations (preferred = '') {
        const orgs = await request('/organizations');
        if (!Array.isArray(orgs)) throw Error('Invalid organizations response.');
        const select = el('asCompanyOrganization');
        if (!select) return;
        select.replaceChildren(new Option('Select organization', ''));
        orgs.forEach(o => select.add(new Option(clean(o.organizationName) || '(Unnamed)', o.organizationTokenId)));
        if (orgs.length) {
            select.value = orgs.some(o => o.organizationTokenId === preferred) ? preferred : orgs[ 0 ].organizationTokenId;
            await loadCompanies();
        } else { reset(); status('Save your organization in the Organization tab first.'); }
    }
    el('asCompanyOrganization')?.addEventListener('change', () => loadCompanies().catch(e => status(e.message)));
    el('asCompanySelect')?.addEventListener('change', e => show(e.target.value));
    el('asBusinessNew')?.addEventListener('click', reset);
    el('asBusinessSave')?.addEventListener('click', async () => {
        const organizationTokenId = el('asCompanyOrganization')?.value;
        const data = { organizationTokenId };
        for (const [ key, id ] of Object.entries(fields)) data[ key ] = clean(el(id)?.value);
        if (!organizationTokenId || !data.companyName) {
            status('Select an organization and enter a company name.'); return;
        }
        const btn = el('asBusinessSave'); if (btn) btn.disabled = true;
        try {
            const saved = await request(selectedToken ? '/' + encodeURIComponent(selectedToken) : '',
                selectedToken ? 'PUT' : 'POST', data);
            await loadCompanies(saved.companyTokenId);
            status('Company saved successfully.');
        } catch (e) { status(e.message); }
        finally { if (btn) btn.disabled = false; }
    });
    // Organizations may be created after initial load without refreshing the entire dashboard.
    root.querySelector('[data-as-section="companies"]')?.addEventListener('click', () => {
        loadOrganizations(el('asCompanyOrganization')?.value).catch(e => status(e.message));
    });
    loadOrganizations().catch(e => status(e.message));
})();
