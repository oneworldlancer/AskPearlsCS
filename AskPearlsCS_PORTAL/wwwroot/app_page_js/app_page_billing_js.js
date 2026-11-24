(() => {
'use strict';
const el=(tag,text,cls)=>{const n=document.createElement(tag);if(text!=null)n.textContent=text;if(cls)n.className=cls;return n;};
const card=(title,...nodes)=>{const n=el('article',null,'ap-billing-card');n.append(el('h3',title),...nodes);return n;};
const grid=(...nodes)=>{const n=el('div',null,'ap-billing-grid');n.append(...nodes);return n;};
const date=v=>v?new Date(v).toLocaleDateString():'Not scheduled';
const button=(label,page)=>{const n=el('button',label);n.type='button';if(page)n.dataset.page=page;else n.disabled=true;return n;};
async function init(root){if(root.dataset.loaded)return;root.dataset.loaded='1';const status=root.querySelector('[data-billing-status]'), body=root.querySelector('[data-billing-content]');
try{const res=await fetch('/Account/Api/billing/overview',{credentials:'same-origin',cache:'no-store',headers:{Accept:'application/json'}});if(!res.ok)throw Error('Request failed ('+res.status+')');const d=await res.json();if(!root.isConnected)return;const c=d.current;status.textContent='';
const usage=()=>{const p=el('progress');p.max=Math.max(1,c.widgetLimit??1);p.value=Math.min(p.max,c.widgetsCreated);return card('Widgets',el('strong',`${c.widgetsCreated} / ${c.widgetLimit??'Unlimited'}`),...(c.widgetLimit==null?[]:[p]),el('p',c.widgetsRemaining==null?'Unlimited':`${c.widgetsRemaining} remaining`));};
switch(root.dataset.billingSection){
case 'subscription':body.append(grid(card('Current plan',el('strong',c.plan),el('p',c.status),el('p','Period ends: '+date(c.currentPeriodEndUtc)),button('View plans','subscription-plans')),usage()),card('Subscription history',...d.history.map(h=>el('p',`${h.plan} · ${h.status} · ${date(h.createdAtUtc)}`))));break;
case 'subscription-plans':body.append(el('p','Catalog prices are previews. Checkout is disabled.'),grid(...d.plans.map(p=>card(p.name,el('strong',p.monthlyPriceUsd==null?'Price not configured':new Intl.NumberFormat('en-US',{style:'currency',currency:'USD'}).format(p.monthlyPriceUsd)+' / month'),el('p',p.description),...p.limits.map(l=>el('p',`${l.code}: ${l.value??'Not configured'}`)),button('Checkout unavailable')))));if(!d.plans.length)body.append(el('p','No active portal plans configured.'));break;
case 'subscription-usage':body.append(grid(usage()),card('Other limits',el('p','Domain and session usage enforcement is not connected to this module yet.')));break;
case 'billing-payments':body.append(grid(card('Payment provider',el('p',d.paymentStatus)),card('Current subscription',el('strong',c.plan),el('p','Period ends: '+date(c.currentPeriodEndUtc)))),card('Payment history',el('p','Provider transaction history is not connected.')));break;
case 'billing-invoices':body.append(card('Invoices & receipts',el('p','Provider invoice retrieval is not connected.')));break;
case 'billing-settings':body.append(card('Payment settings',el('p','Payment methods will be managed by the payment provider.'),button('Manage payment method')));break;
}
}catch(e){status.textContent='Unable to load billing information. '+e.message;root.dataset.loaded='';}}
function scan(){const root=document.querySelector('#apMainContent [data-billing-section]');if(root){document.querySelector('#apBillingAccordion')?.setAttribute('open','');init(root);}}
const start=()=>{const main=document.querySelector('#apMainContent');if(main){new MutationObserver(scan).observe(main,{childList:true,subtree:true});scan();}};
if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',start,{once:true});else start();
})();
