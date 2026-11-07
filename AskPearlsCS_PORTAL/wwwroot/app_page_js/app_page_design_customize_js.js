/* AskPearls Design & Customize — Dashboard partial-safe, authenticated portal API */
(function () {
    'use strict';
    const root = document.querySelector('.design-customize-page[data-ap-page="design-customize"]');
    if (!root || root.dataset.dcInitialized === 'true') return;
    root.dataset.dcInitialized = 'true';
    const el = id => root.querySelector('#' + id);
    const apiBase = (root.dataset.apiBase || '/Widget/Api').replace(/\/+$/, '');
    const defaults = {
        primary: '#0DA88B', background: '#FFFFFF', text: '#17212B', theme: 'custom',
        font: 'Inter, Arial, sans-serif', fontSize: 14, radius: 12, shadow: true,
        width: 360, position: 'bottom-right', buttonShape: 'rounded',
        heading: 'Welcome to our assistant',
        description: 'Answer a few quick questions to find the right recommendation for you.',
        buttonText: 'Get started', footer: 'Powered by AskPearls',
        brandName: '', headerTitle: 'AskPearls Assistant', logoDataUrl: '', logoUrl: '', brandIcon: 'stars', hideAskPearlsBranding: false
    };
    const fields = [ 'dcPrimaryColor', 'dcBackgroundColor', 'dcTextColor', 'dcTheme', 'dcFontFamily',
        'dcFontSize', 'dcRadius', 'dcShadow', 'dcWidth', 'dcPosition', 'dcButtonShape',
        'dcHeading', 'dcDescription', 'dcButtonText', 'dcFooterText',
        'dcBrandName', 'dcHeaderTitle', 'dcBrandIcon', 'dcLogoUrl', 'dcHideBranding' ];
    let widgets = [], savedSettings = { ...defaults }, selectedKey = '', requestId = 0;
    let busy = false, toastTimer = null, activeMode = 'light';

    function readSettings () {
        return {
            primary: el('dcPrimaryColor').value.toUpperCase(),
            background: el('dcBackgroundColor').value.toUpperCase(),
            text: el('dcTextColor').value.toUpperCase(),
            theme: el('dcTheme').value, font: el('dcFontFamily').value,
            fontSize: Number(el('dcFontSize').value), radius: Number(el('dcRadius').value),
            shadow: el('dcShadow').checked, width: Number(el('dcWidth').value),
            position: el('dcPosition').value, buttonShape: el('dcButtonShape').value,
            heading: el('dcHeading').value, description: el('dcDescription').value,
            buttonText: el('dcButtonText').value, footer: el('dcFooterText').value,
            brandName: el('dcBrandName').value.trim(), headerTitle: el('dcHeaderTitle').value.trim(),
            logoDataUrl: '', logoUrl: el('dcLogoUrl').value.trim(), brandIcon: el('dcBrandIcon').value, hideAskPearlsBranding: el('dcHideBranding').checked
        };
    }
    function normalize (source) {
        const s = source && typeof source === 'object' ? source : {};
        const pick = (key, fallback) => s[ key ] ?? s[ key[ 0 ].toUpperCase() + key.slice(1) ] ?? fallback;
        const out = {};
        for (const [ key, value ] of Object.entries(defaults)) out[ key ] = pick(key, value);
        for (const key of [ 'primary', 'background', 'text' ]) {
            if (!/^#[0-9a-f]{6}$/i.test(String(out[ key ]))) out[ key ] = defaults[ key ];
            out[ key ] = out[ key ].toUpperCase();
        }
        for (const [ key, min, max ] of [ [ 'fontSize', 12, 18 ], [ 'radius', 0, 28 ], [ 'width', 300, 520 ] ]) {
            const n = Number(out[ key ]); out[ key ] = Number.isFinite(n) ? Math.min(max, Math.max(min, n)) : defaults[ key ];
        }
        out.shadow = out.shadow === true || out.shadow === 'true';
        out.hideAskPearlsBranding = out.hideAskPearlsBranding === true || out.hideAskPearlsBranding === 'true';
        for (const [ key, values ] of Object.entries({
            theme: [ 'custom', 'light', 'dark' ], position: [ 'bottom-right', 'bottom-left', 'center' ],
            buttonShape: [ 'rounded', 'pill', 'square' ],
            font: [ 'system', 'Inter, Arial, sans-serif', 'Arial, sans-serif', 'Georgia, serif', 'Verdana, sans-serif' ]
        })) if (!values.includes(out[ key ])) out[ key ] = defaults[ key ];
        if (out.font === 'system') out.font = defaults.font;
        for (const key of [ 'heading', 'description', 'buttonText', 'footer', 'brandName', 'headerTitle', 'logoDataUrl', 'logoUrl', 'brandIcon' ]) out[ key ] = String(out[ key ] ?? '');
        return out;
    }
    function applySettings (raw) {
        const s = normalize(raw);
        for (const [ id, key ] of [ [ 'dcPrimaryColor', 'primary' ], [ 'dcBackgroundColor', 'background' ],
        [ 'dcTextColor', 'text' ], [ 'dcTheme', 'theme' ], [ 'dcFontFamily', 'font' ],
        [ 'dcFontSize', 'fontSize' ], [ 'dcRadius', 'radius' ], [ 'dcWidth', 'width' ],
        [ 'dcPosition', 'position' ], [ 'dcButtonShape', 'buttonShape' ], [ 'dcHeading', 'heading' ],
        [ 'dcDescription', 'description' ], [ 'dcButtonText', 'buttonText' ], [ 'dcFooterText', 'footer' ],
        [ 'dcBrandName', 'brandName' ], [ 'dcHeaderTitle', 'headerTitle' ], [ 'dcBrandIcon', 'brandIcon' ] ]) el(id).value = s[ key ];
        el('dcShadow').checked = s.shadow;
        el('dcHideBranding').checked = s.hideAskPearlsBranding === true;
        el('dcLogoUrl').value = s.logoUrl || '';
        el('dcLogoThumbnail').hidden = !s.logoUrl; if (s.logoUrl) el('dcLogoThumbnail').src = s.logoUrl;
        syncHexInputs(); renderPreview();
    }
    function syncHexInputs () {
        for (const [ picker, hex ] of [ [ 'dcPrimaryColor', 'dcPrimaryHex' ], [ 'dcBackgroundColor', 'dcBackgroundHex' ], [ 'dcTextColor', 'dcTextHex' ] ]) el(hex).value = el(picker).value.toUpperCase();
    }
    function dirty () { return JSON.stringify(readSettings()) !== JSON.stringify(savedSettings); }
    function updateDirty () {
        const changed = dirty();
        el('dcDraftIndicator').textContent = changed ? '● Unsaved changes' : '✓ No unsaved changes';
        el('dcSave').disabled = busy || !selectedKey || !changed;
        el('dcReset').disabled = busy || !selectedKey || !changed;
    }
    function notice (message, type = 'danger') {
        const box = el('dcPageAlert');
        box.textContent = message || '';
        box.className = message ? 'alert alert-' + type : 'alert d-none';
    }
    function toast (message) {
        const box = el('dcToast'); if (!box) return;
        box.textContent = message; box.classList.add('is-visible');
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => box.classList.remove('is-visible'), 3500);
    }
    function setBusy (value) { busy = value; el('dcWidgetSelect').disabled = value; updateDirty(); }
    function antiForgeryToken () {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value ||
            document.querySelector('meta[name="RequestVerificationToken"]')?.content ||
            document.querySelector('meta[name="request-verification-token"]')?.content || '';
    }
    async function api (path, options = {}) {
        const headers = { Accept: 'application/json', ...(options.body ? { 'Content-Type': 'application/json' } : {}) };
        const token = antiForgeryToken();
        if (token && options.method && options.method.toUpperCase() !== 'GET') headers.RequestVerificationToken = token;
        const response = await fetch(apiBase + path, { credentials: 'same-origin', ...options, headers: { ...headers, ...options.headers } });
        const text = await response.text(); let data;
        try { data = text ? JSON.parse(text) : {}; }
        catch { throw new Error('API returned an invalid JSON response (HTTP ' + response.status + ').'); }
        if (!response.ok || data.success === false) throw new Error(data.message || data.title || 'API request failed (HTTP ' + response.status + ').');
        return data;
    }
    function renderPreview () {
        const s = readSettings(); const widget = el('dcWidgetPreview');
        el('dcPreviewBrandTitle').textContent = s.headerTitle || 'AskPearls Assistant';
        const brandIcon = el('dcPreviewBrandIcon'); brandIcon.replaceChildren();
        const assistantIcon = document.createElement('i');
        assistantIcon.className = 'bi bi-' + ({ building: 'building', chat: 'chat-dots', robot: 'robot' }[ s.brandIcon ] || 'stars');
        brandIcon.append(assistantIcon);
        el('dcPreviewCompanyName').textContent = s.brandName && s.brandName !== s.headerTitle ? s.brandName : '';
        const companyLogo = el('dcPreviewCompanyLogo');
        companyLogo.replaceChildren();
        if (s.logoUrl && /^https:\/\//i.test(s.logoUrl)) {
            const img = document.createElement('img'); img.src = s.logoUrl;
            img.alt = (s.brandName || 'Company') + ' logo';
            img.referrerPolicy = 'no-referrer';
            img.onerror = () => { img.remove(); companyLogo.classList.remove('has-logo'); companyLogo.innerHTML = '<i class="bi bi-stars"></i>'; };
            companyLogo.classList.add('has-logo'); companyLogo.append(img);
        } else {
            companyLogo.classList.remove('has-logo');
            const fallback = document.createElement('i'); fallback.className = 'bi bi-stars'; companyLogo.append(fallback);
        }
        const mode = activeMode;
        const dark = mode === 'dark' || (mode === 'mobile' && s.theme === 'dark') || (mode === 'light' && s.theme === 'dark');
        const bg = dark ? '#15231E' : s.background;
        const color = dark && s.text.toUpperCase() === '#17212B' ? '#E7F0EC' : s.text;
        root.style.setProperty('--dc-primary', s.primary);
        root.style.setProperty('--dc-widget-bg', bg);
        root.style.setProperty('--dc-widget-text', color);
        widget.style.backgroundColor = bg; widget.style.color = color;
        widget.style.fontFamily = s.font; widget.style.fontSize = s.fontSize + 'px';
        widget.style.borderRadius = s.radius + 'px';
        widget.style.boxShadow = s.shadow ? '0 18px 55px rgba(20,35,30,.18)' : 'none';
        widget.style.width = 'min(100%, ' + (mode === 'mobile' ? Math.min(320, s.width) : s.width) + 'px)';
        el('dcPreviewHeading').textContent = s.heading || 'Welcome';
        el('dcPreviewHeading').style.fontSize = Math.max(16, s.fontSize + 6) + 'px';
        el('dcPreviewDescription').textContent = s.description;
        el('dcPreviewButton').replaceChildren(document.createTextNode((s.buttonText || 'Get started') + ' '));
        const icon = document.createElement('i'); icon.className = 'bi bi-arrow-right'; el('dcPreviewButton').append(icon);
        el('dcPreviewFooter').textContent = s.hideAskPearlsBranding ? s.footer :
            (s.footer.toLowerCase().includes('askpearls') ? s.footer : s.footer + ' · AskPearls');
        for (const id of [ 'dcPreviewHeading', 'dcPreviewDescription', 'dcPreviewFooter' ]) el(id).style.color = color;
        const radius = s.buttonShape === 'pill' ? '999px' : s.buttonShape === 'square' ? '2px' : s.radius + 'px';
        el('dcPreviewButton').style.borderRadius = radius;
        root.querySelectorAll('.dc-preview-answer').forEach(b => {
            b.style.borderRadius = radius; b.style.color = color;
            b.style.borderColor = dark ? 'rgba(231,240,236,.2)' : 'rgba(100,120,110,.2)';
        });
        el('dcPreviewPosition').dataset.position = s.position;
        el('dcPreviewCanvas').dataset.mode = mode;
        for (const [ id, value ] of [ [ 'dcFontSizeValue', s.fontSize ], [ 'dcRadiusValue', s.radius ], [ 'dcWidthValue', s.width ] ]) el(id).textContent = value + ' px';
        root.querySelectorAll('.dc-preview-mode').forEach(b => b.classList.toggle('active', b.dataset.dcMode === mode));
        updateDirty();
    }
    function versionText (widget, draft) {
        const dv = draft?.status === 'draft' ? draft.versionNumber : (widget?.draftVersionNumber ?? null);
        const live = widget?.publishedVersionNumber ?? '0';
        return 'Draft v' + (dv ?? '0') + ' · Live v' + (live || '0');
    }
    function widgetKey (w) { return String(w.publicKey ?? w.widgetPublicKey ?? w.PublicKey ?? ''); }
    async function loadWidgets () {
        const select = el('dcWidgetSelect'); select.disabled = true;
        try {
            const data = await api('/widgets');
            widgets = Array.isArray(data.widgets) ? data.widgets : Array.isArray(data.data?.widgets) ? data.data.widgets : [];
            select.replaceChildren(new Option('-- Select a widget --', ''));
            for (const w of widgets) {
                const key = widgetKey(w); if (!key) continue;
                select.add(new Option(w.name || w.widgetName || w.title || key, key));
            }
            select.disabled = false;
            const preferred = new URLSearchParams(location.search).get('publicKey') || root.dataset.widgetPublicKey || '';
            if (preferred && widgets.some(w => widgetKey(w) === preferred)) select.value = preferred;
            else if (widgets.length === 1) select.value = widgetKey(widgets[ 0 ]);
            if (select.value) await selectWidget(select.value);
            else { el('dcWidgetMessage').textContent = widgets.length ? 'Select a widget to load its design.' : 'No widgets found. Create a widget first.'; updateDirty(); }
        } catch (err) {
            select.replaceChildren(new Option('Unable to load widgets', ''));
            el('dcWidgetMessage').textContent = 'Could not retrieve widgets.';
            notice(err.message); console.error('Design & Customize: widget list failed', err);
        }
    }
    async function selectWidget (key) {
        const seq = ++requestId; selectedKey = key; notice('');
        el('dcWidgetVersion').textContent = '—';
        if (!key) { el('dcWidgetMessage').textContent = 'Choose a widget to customize.'; applySettings(defaults); savedSettings = readSettings(); updateDirty(); return; }
        setBusy(true);
        try {
            const data = await api('/widgets/' + encodeURIComponent(key) + '/draft/configuration');
            if (seq !== requestId) return;
            applySettings(data.configuration || defaults); savedSettings = readSettings();
            const w = widgets.find(w => widgetKey(w) === key);
            el('dcWidgetVersion').textContent = versionText(w, data);
            el('dcWidgetMessage').textContent = (w?.name || 'Widget') + ' · ' + (data.status === 'draft' ? 'Draft loaded' : 'Published/default design loaded');
            updateDirty();
        } catch (err) {
            if (seq !== requestId) return;
            applySettings(defaults); savedSettings = readSettings();
            el('dcWidgetMessage').textContent = 'Could not load design configuration.';
            notice(err.message); console.error('Design & Customize: load failed', err);
        } finally { if (seq === requestId) setBusy(false); }
    }
    async function save () {
        if (!selectedKey || busy) return;
        const key = selectedKey, payload = readSettings();
        if (payload.logoUrl && !/^https:\/\/[^\s/]+/i.test(payload.logoUrl)) {
            notice('Company logo must be a public HTTPS URL.', 'warning'); return;
        }
        setBusy(true); notice('');
        try {
            const data = await api('/widgets/' + encodeURIComponent(key) + '/draft/configuration',
                { method: 'PUT', body: JSON.stringify(payload) });
            if (selectedKey !== key) return;
            savedSettings = { ...payload };
            const w = widgets.find(w => widgetKey(w) === key);
            if (w && data.versionNumber != null) w.draftVersionNumber = data.versionNumber;
            if (w && data.publishedVersionNumber != null) w.publishedVersionNumber = data.publishedVersionNumber;
            el('dcWidgetVersion').textContent = versionText(w, { status: 'draft', versionNumber: data.versionNumber ?? w?.draftVersionNumber });
            el('dcWidgetMessage').textContent = 'Draft saved successfully.';
            toast('Draft saved' + (data.versionNumber != null ? ' as v' + data.versionNumber : '') + '.');
        } catch (err) { notice(err.message); console.error('Design & Customize: save failed', err); }
        finally { setBusy(false); }
    }
    function changed () { renderPreview(); }
    for (const id of fields) { const node = el(id); node.addEventListener('input', changed); node.addEventListener('change', changed); }
    for (const [ picker, hex ] of [ [ 'dcPrimaryColor', 'dcPrimaryHex' ], [ 'dcBackgroundColor', 'dcBackgroundHex' ], [ 'dcTextColor', 'dcTextHex' ] ]) {
        el(picker).addEventListener('input', () => {
            if (picker === 'dcBackgroundColor' || picker === 'dcTextColor') el('dcTheme').value = 'custom';
            syncHexInputs(); renderPreview();
        });
        el(hex).addEventListener('change', () => {
            const v = el(hex).value.trim();
            if (!/^#[0-9a-f]{6}$/i.test(v)) { syncHexInputs(); notice('Use a six-digit hex color such as #0DA88B.', 'warning'); return; }
            el(picker).value = v; if (picker !== 'dcPrimaryColor') el('dcTheme').value = 'custom';
            notice(''); changed();
        });
    }
    el('dcTheme').addEventListener('change', () => {
        const theme = el('dcTheme').value;
        if (theme === 'dark') { el('dcBackgroundColor').value = '#15231E'; el('dcTextColor').value = '#E7F0EC'; }
        if (theme === 'light') { el('dcBackgroundColor').value = '#FFFFFF'; el('dcTextColor').value = '#17212B'; }
        syncHexInputs(); changed();
    });
    root.querySelectorAll('.dc-preview-mode').forEach(b => b.addEventListener('click', () => { activeMode = b.dataset.dcMode; renderPreview(); }));
    el('dcLogoUrl').addEventListener('input', () => {
        const url = el('dcLogoUrl').value.trim();
        const thumbnail = el('dcLogoThumbnail');
        thumbnail.hidden = !/^https:\/\//i.test(url);
        if (!thumbnail.hidden) thumbnail.src = url;
        changed();
    });
    el('dcRemoveLogo').addEventListener('click', () => {
        el('dcLogoUrl').value = '';
        el('dcLogoThumbnail').hidden = true;
        changed();
    });

    // Use the portal's existing Widget Builder navigation and Create Widget action.
    function createWidgetFromDesigner () {
        if (busy) return;
        if (dirty() && !window.confirm('Discard unsaved design changes and create a new widget?')) return;

        const findBuilderButton = () => document.querySelector(
            '.page-widget-builder[data-ap-page="widget-builder"] #wbCreateWidgetButton'
        );
        const startCreateMode = () => {
            const button = findBuilderButton();
            if (!button) return false;
            button.click(); // Reuses Widget Builder's existing createMode() handler.
            return true;
        };
        if (startCreateMode()) return;

        // Dashboard loads its pages dynamically; navigate using its own sidebar link.
        const links = Array.from(document.querySelectorAll('a[href], [data-page], [data-ap-target]'));
        const builderLink = links.find(link => {
            if (root.contains(link)) return false;
            const label = (link.textContent || '').trim().replace(/\s+/g, ' ');
            return /^Widget Builder$/i.test(label) ||
                /widget[-_]?builder/i.test(link.getAttribute('data-page') || '') ||
                /widget[-_]?builder/i.test(link.getAttribute('data-ap-target') || '');
        });
        if (!builderLink) {
            notice('Widget Builder navigation was not found. Open Widget Builder from the left menu and click +.', 'warning');
            return;
        }

        // Register before clicking because the page may be injected immediately.
        const observer = new MutationObserver(() => {
            if (startCreateMode()) observer.disconnect();
        });
        observer.observe(document.body, { childList: true, subtree: true });
        builderLink.click();
        if (startCreateMode()) observer.disconnect();
        // Prevent an observer from persisting indefinitely if navigation fails.
        window.setTimeout(() => observer.disconnect(), 10000);
    }
    el('dcCreateWidget').addEventListener('click', createWidgetFromDesigner);

    el('dcSave').addEventListener('click', save);
    el('dcReset').addEventListener('click', () => { applySettings(savedSettings); toast('Restored the last loaded or saved settings.'); });
    el('dcWidgetSelect').addEventListener('change', async e => {
        const next = e.target.value, previous = selectedKey;
        if (dirty() && !window.confirm('Discard unsaved design changes and switch widgets?')) { e.target.value = previous; return; }
        // clear the dirty state before loading the next widget
        savedSettings = readSettings();
        await selectWidget(next);
    });
    applySettings(defaults); savedSettings = readSettings(); updateDirty();
    loadWidgets();
})();
