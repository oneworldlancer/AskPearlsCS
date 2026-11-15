(function () {
    'use strict';
    window.AskPearlsSupport = {
        async submit (form, status) {
            const button = form.querySelector('[type="submit"]');
            const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value
                || document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (!token) { status.textContent = 'Security token unavailable. Refresh the page.'; return; }
            const data = new FormData(form);
            data.set('__RequestVerificationToken', token);
            if (button) button.disabled = true;
            status.textContent = 'Sending your request…';
            try {
                const response = await fetch('/support/contact', { method: 'POST', credentials: 'same-origin', body: data });
                const result = await response.json().catch(() => ({}));
                if (!response.ok) throw Error(result.message || 'Could not send your message.');
                status.textContent = result.message || 'Your support request has been sent.';
                form.querySelector('[name="message"]')?.value && (form.querySelector('[name="message"]').value = '');
            } catch (e) { status.textContent = e.message; }
            finally { if (button) button.disabled = false; }
        }
    };
})();
