// Load Paddle.js v2 once, then call from a sandbox-only test button.
window.askPearlsPaddleCheckout = async function (planCode, billingInterval, antiforgeryToken) {
    const response = await fetch('/Account/Api/paddle/checkout', {method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/json','RequestVerificationToken':antiforgeryToken},body:JSON.stringify({planCode,billingInterval})});
    if (!response.ok) throw new Error('Checkout unavailable');
    const data = await response.json();
    if (data.environment !== 'Sandbox') throw new Error('Sandbox only');
    if (!window.Paddle) throw new Error('Paddle.js is not loaded');
    if (!window.askPearlsPaddleInitialized) { Paddle.Environment.set('sandbox'); Paddle.Initialize({token:data.clientSideToken}); window.askPearlsPaddleInitialized=true; }
    Paddle.Checkout.open({transactionId:data.transactionId});
};
