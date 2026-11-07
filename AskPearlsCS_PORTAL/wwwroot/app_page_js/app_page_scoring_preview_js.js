(function () {
    "use strict";
    // Dashboard injects Razor partials via AJAX. Never initialize against an
    // outdated/partially inserted Scoring & Preview DOM.
    const roots = Array.from(document.querySelectorAll('.sp-page'));
    const root = roots.findLast ? roots.findLast(el => el.isConnected) : roots.filter(el => el.isConnected).pop();
    if (!root || root.dataset.initialized === 'true') return;

    const requiredIds = [
        'spWidgetSelect', 'spWidgetMessage', 'spWidgetVersion', 'spPageAlert',
        'spWorkspace', 'spEditQuestion', 'spEditAnswer', 'spAllocationEditor',
        'spAllocationTotal', 'spAllocationStatus', 'spAllocationBar',
        'spResetAnswers', 'spResetAllocations', 'spVisitorAnswers',
        'spQuestionCount', 'spMaximum', 'spRanking', 'spRankingEmpty',
        'spBreakdownHead', 'spBreakdown', 'spBreakdownFooter'
    ];
    const missing = requiredIds.filter(id => !root.querySelector('#' + id));
    if (missing.length) {
        const msg = 'Scoring & Preview: outdated/incomplete Razor partial. Missing: ' + missing.join(', ');
        console.error(msg);
        const selector = root.querySelector('#spWidgetSelect');
        const help = root.querySelector('#spWidgetMessage');
        if (selector) {
            selector.disabled = true;
            selector.innerHTML = '<option value="">Page update required</option>';
        }
        if (help) help.textContent = 'The Dashboard loaded an outdated Scoring & Preview view. Replace the partial and reload.';
        return; // Do not mark initialized; a later correct partial can initialize.
    }
    root.dataset.initialized = 'true';
    const $ = id => root.querySelector('#' + id);
    const MAX = 50;
    const apiBase = (root.dataset.apiBase || '/Widget/Api').replace(/\/+$/, '');
    const owner = (root.dataset.demoOwnerUserToken || '').trim();
    const headers = { Accept: 'application/json' };
    // Only the authenticated portal proxy should be used for production.
    if (owner && !apiBase.startsWith('/Widget/Api')) headers[ 'X-Demo-Owner-User-Token' ] = owner;
    let widgets = [], questions = [], recommendations = [], allocations = {}, visitorAnswers = {};
    let requestSequence = 0;
    const value = (obj, key) => obj?.[ key ] ?? obj?.[ key[ 0 ].toUpperCase() + key.slice(1) ];
    const esc = input => String(input ?? '').replace(/[&<>"']/g, ch => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[ ch ]));
    const option = (id, text) => `<option value="${ esc(id) }">${ esc(text) }</option>`;
    function alertMessage (message, kind = 'info') {
        const el = $('spPageAlert'); el.className = `alert alert-${ kind }`; el.textContent = message;
    }
    function clearAlert () { $('spPageAlert').className = 'alert d-none'; $('spPageAlert').textContent = ''; }
    async function getJson (path) {
        const response = await fetch(apiBase + path, { headers, credentials: 'same-origin', cache: 'no-store' });
        const data = await response.json().catch(() => ({}));
        if (!response.ok || data.success === false) throw new Error(data.message || `API request failed (${ response.status }).`);
        return data;
    }
    function setVersion (widget) {
        $('spWidgetVersion').textContent = widget ? (
            widget.hasDraft ? `Draft v${ widget.draftVersionNumber || widget.versionNumber || '?' }` +
                (widget.hasPublishedVersion ? ` · Live v${ widget.publishedVersionNumber }` : '') :
                widget.hasPublishedVersion ? `Live v${ widget.publishedVersionNumber || widget.versionNumber }` : '—'
        ) : '—';
    }
    async function loadWidgets () {
        const select = $('spWidgetSelect'); select.disabled = true;
        try {
            const body = await getJson('/widgets');
            const payload = body.data && typeof body.data === 'object' ? body.data : body;
            widgets = (Array.isArray(payload.widgets) ? payload.widgets : []).map(w => ({
                publicKey: String(value(w, 'publicKey') || ''),
                name: String(value(w, 'name') || 'Untitled widget'),
                versionNumber: value(w, 'versionNumber'),
                draftVersionNumber: value(w, 'draftVersionNumber'),
                publishedVersionNumber: value(w, 'publishedVersionNumber'),
                hasDraft: Boolean(value(w, 'hasDraft')),
                hasPublishedVersion: Boolean(value(w, 'hasPublishedVersion'))
            })).filter(w => w.publicKey);
            select.innerHTML = option('', '-- Select a widget --') + widgets.map(w => option(w.publicKey, w.name)).join('');
            select.value = ''; select.disabled = widgets.length === 0;
            $('spWidgetMessage').textContent = widgets.length ? 'Choose a widget to load its questions and recommendations.' : 'No widgets found. Create a widget first.';
        } catch (error) {
            select.innerHTML = option('', 'Widgets unavailable');
            alertMessage(error.message, 'danger');
        }
    }
    function normalizeQuestions (data) {
        const payload = data.data && typeof data.data === 'object' ? data.data : data;
        return (Array.isArray(payload.questions) ? payload.questions : []).map((q, i) => {
            const rawOptions = value(q, 'options') || [];
            const answers = (Array.isArray(rawOptions) ? rawOptions : []).map((a, j) => {
                const text = typeof a === 'string' ? a : String(value(a, 'text') || value(a, 'label') || '');
                return { id: String(j), text };
            }).filter(a => a.text);
            return { id: String(value(q, 'id') || value(q, 'key') || i), text: String(value(q, 'text') || `Question ${ i + 1 }`), answers };
        }).filter(q => q.answers.length);
    }
    function normalizeRecommendations (data) {
        const payload = data.data && typeof data.data === 'object' ? data.data : data;
        return (Array.isArray(payload.items) ? payload.items : []).filter(r => value(r, 'active') === true).map((r, i) => ({
            id: String(value(r, 'key') || value(r, 'id') || i),
            name: String(value(r, 'name') || 'Untitled recommendation'),
            description: String(value(r, 'description') || '')
        })).filter(r => r.id);
    }
    function equalSplit () {
        const n = recommendations.length;
        if (!n) return [];
        const base = Math.floor(MAX / n), remainder = MAX % n;
        return recommendations.map((_, i) => base + (i < remainder ? 1 : 0));
    }
    function resetAllocations () {
        allocations = {};
        for (const q of questions) {
            allocations[ q.id ] = {};
            for (const a of q.answers) allocations[ q.id ][ a.id ] = equalSplit();
        }
        renderAllocationEditor(); renderResults();
    }
    function resetVisitorAnswers () {
        visitorAnswers = {};
        for (const q of questions) visitorAnswers[ q.id ] = q.answers[ 0 ].id;
        renderVisitorAnswers(); renderResults();
    }
    function selectedQuestion () { return questions.find(q => q.id === $('spEditQuestion').value); }
    function renderAnswerOptions () {
        const q = selectedQuestion();
        $('spEditAnswer').innerHTML = q ? q.answers.map(a => option(a.id, a.text)).join('') : '';
        renderAllocationEditor();
    }
    function renderAllocationEditor () {
        const q = selectedQuestion(), a = $('spEditAnswer').value;
        const current = q && allocations[ q.id ]?.[ a ];
        $('spAllocationEditor').innerHTML = current ? recommendations.map((r, i) => `<div class="col-12 col-md-4"><div class="sp-allocation-card"><label for="spPoints${ i }">${ esc(r.name) } points</label><input id="spPoints${ i }" class="form-control sp-point-input" type="number" min="0" max="${ MAX }" step="1" data-index="${ i }" value="${ current[ i ] }"><small class="text-muted d-block mt-2">${ esc(r.description) }</small></div></div>`).join('') : '';
        root.querySelectorAll('.sp-point-input').forEach(input => input.addEventListener('input', () => {
            const raw = input.value.trim(), number = Number(raw);
            const valid = raw !== '' && Number.isInteger(number) && number >= 0 && number <= MAX;
            input.classList.toggle('is-invalid', !valid);
            current[ Number(input.dataset.index) ] = valid ? number : null;
            updateAllocationStatus(); renderResults();
        }));
        updateAllocationStatus();
    }
    function validValues (values) {
        return Array.isArray(values) && values.length === recommendations.length &&
            values.every(v => Number.isInteger(v) && v >= 0 && v <= MAX) &&
            values.reduce((sum, v) => sum + v, 0) === MAX;
    }
    function allValid () { return questions.every(q => q.answers.every(a => validValues(allocations[ q.id ]?.[ a.id ]))); }
    function updateAllocationStatus () {
        const q = selectedQuestion(), values = q && allocations[ q.id ]?.[ $('spEditAnswer').value ];
        const validNumbers = values && values.every(v => Number.isInteger(v) && v >= 0 && v <= MAX);
        const total = validNumbers ? values.reduce((s, n) => s + n, 0) : null;
        $('spAllocationTotal').textContent = `${ total === null ? '—' : total } / ${ MAX }`;
        $('spAllocationStatus').textContent = total === MAX ? '✓ Valid allocation' : total === null ? 'Enter whole numbers between 0 and 50.' : total < MAX ? `Add ${ MAX - total } points.` : `Remove ${ total - MAX } points.`;
        $('spAllocationStatus').className = total === MAX ? 'sp-valid' : 'sp-invalid';
        $('spAllocationBar').style.width = `${ total === null ? 0 : Math.min(100, total / MAX * 100) }%`;
        $('spAllocationBar').classList.toggle('invalid', total !== MAX);
    }
    function renderVisitorAnswers () {
        $('spVisitorAnswers').innerHTML = questions.map((q, i) => `<div class="col-12 col-md-4"><div class="sp-answer-card"><label for="spVisitor${ i }">${ esc(q.text) }</label><select id="spVisitor${ i }" class="form-select" data-question="${ esc(q.id) }">${ q.answers.map(a => option(a.id, a.text)).join('') }</select></div></div>`).join('');
        root.querySelectorAll('#spVisitorAnswers select').forEach(select => {
            select.value = visitorAnswers[ select.dataset.question ];
            select.addEventListener('change', () => { visitorAnswers[ select.dataset.question ] = select.value; renderResults(); });
        });
    }
    function renderResults () {
        const valid = questions.length > 0 && recommendations.length > 0 && allValid() &&
            questions.every(q => q.answers.some(a => a.id === visitorAnswers[ q.id ]));
        $('spRankingEmpty').classList.toggle('d-none', valid);
        $('spRanking').classList.toggle('d-none', !valid);
        if (!valid) { $('spRanking').innerHTML = ''; $('spBreakdownHead').innerHTML = ''; $('spBreakdown').innerHTML = ''; $('spBreakdownFooter').innerHTML = ''; return; }
        const max = questions.length * MAX;
        const results = recommendations.map((r, i) => {
            const score = questions.reduce((sum, q) => sum + allocations[ q.id ][ visitorAnswers[ q.id ] ][ i ], 0);
            return { ...r, score, percentage: Math.round(score / max * 100), originalIndex: i };
        }).sort((a, b) => b.score - a.score || a.originalIndex - b.originalIndex);
        $('spRanking').innerHTML = results.map((r, i) => `<div class="sp-result ${ i === 0 ? 'sp-winner' : '' }"><div class="sp-result-top"><div class="d-flex align-items-center gap-3"><span class="sp-rank">${ i + 1 }</span><div><div class="fw-bold">${ esc(r.name) } ${ i === 0 ? '<span class="sp-valid ms-2">Top score</span>' : '' }</div><div class="text-muted small">${ esc(r.description) }</div></div></div><div class="text-end"><div class="sp-score">${ r.score } / ${ max }</div><div class="text-muted small">${ r.percentage }% of maximum</div></div></div><div class="sp-match-track"><div style="width:${ r.percentage }%"></div></div></div>`).join('');
        $('spBreakdownHead').innerHTML = `<tr><th>Selected answer</th>${ recommendations.map(r => `<th>${ esc(r.name) }</th>`).join('') }</tr>`;
        $('spBreakdown').innerHTML = questions.map(q => {
            const answer = q.answers.find(a => a.id === visitorAnswers[ q.id ]);
            const points = allocations[ q.id ][ visitorAnswers[ q.id ] ];
            return `<tr><td><strong>${ esc(q.text) }</strong><div class="text-muted small">${ esc(answer?.text) }</div></td>${ points.map(n => `<td><span class="sp-points">${ n }</span> <small class="text-muted">pts</small></td>`).join('') }</tr>`;
        }).join('');
        const scores = recommendations.map((_, i) => questions.reduce((sum, q) => sum + allocations[ q.id ][ visitorAnswers[ q.id ] ][ i ], 0));
        $('spBreakdownFooter').innerHTML = `<tr><th>Total score</th>${ scores.map(n => `<th>${ n } / ${ max }</th>`).join('') }</tr>`;
    }
    async function selectWidget () {
        const key = $('spWidgetSelect').value;
        const sequence = ++requestSequence;
        $('spWorkspace').classList.add('d-none'); clearAlert();
        const widget = widgets.find(w => w.publicKey === key);
        setVersion(widget);
        if (!widget) { $('spWidgetMessage').textContent = 'Choose the widget you want to preview.'; return; }
        $('spWidgetMessage').textContent = `Loading ${ widget.name }…`;
        try {
            const encoded = encodeURIComponent(key);
            const [ qData, rData ] = await Promise.all([
                getJson(`/widgets/${ encoded }/draft/questions`),
                getJson(`/widgets/${ encoded }/draft/recommendations`)
            ]);
            if (sequence !== requestSequence) return;
            questions = normalizeQuestions(qData);
            recommendations = normalizeRecommendations(rData);
            if (!questions.length || !recommendations.length) {
                alertMessage(!questions.length ? 'This widget has no answer options to score. Add questions and answers first.' : 'This widget has no active recommendations. Activate a recommendation first.', 'warning');
                $('spWidgetMessage').textContent = widget.name;
                return;
            }
            $('spQuestionCount').textContent = questions.length;
            $('spMaximum').textContent = questions.length * MAX;
            $('spEditQuestion').innerHTML = questions.map(q => option(q.id, q.text)).join('');
            visitorAnswers = {};
            for (const q of questions) visitorAnswers[ q.id ] = q.answers[ 0 ]?.id ?? '';
            resetAllocations(); renderAnswerOptions(); renderVisitorAnswers(); renderResults();
            $('spWorkspace').classList.remove('d-none');
            $('spWidgetMessage').textContent = `${ questions.length } questions · ${ recommendations.length } active recommendations`;
            alertMessage('Default allocations are evenly distributed for preview. They are not stored scoring rules.', 'info');
        } catch (error) {
            if (sequence !== requestSequence) return;
            $('spWidgetMessage').textContent = 'Could not load widget data.';
            alertMessage(error.message, 'danger');
        }
    }
    // Create widget: use the Dashboard's existing Widget Builder navigation.
    const createButton = $('spCreateWidgetButton');
    if (createButton) {
        createButton.addEventListener('click', () => {
            const links = Array.from(document.querySelectorAll('a[href]'));
            const builderLink = links.find(link =>
                link.closest('aside, nav, .sidebar, .left-sidebar, .app-sidebar, .ap-sidebar') &&
                link.textContent.trim().replace(/\s+/g, ' ').toLowerCase() === 'widget builder'
            ) || links.find(link => link.textContent.trim().replace(/\s+/g, ' ').toLowerCase() === 'widget builder');
            if (builderLink) {
                builderLink.click();
            } else {
                // Safe fallback to Dashboard; do not assume an unknown builder URL.
                window.location.assign('/Dashboard');
            }
        });
    }

    $('spWidgetSelect').addEventListener('change', selectWidget);
    $('spEditQuestion').addEventListener('change', renderAnswerOptions);
    $('spEditAnswer').addEventListener('change', renderAllocationEditor);
    $('spResetAnswers').addEventListener('click', resetVisitorAnswers);
    $('spResetAllocations').addEventListener('click', resetAllocations);
    loadWidgets();
})();
