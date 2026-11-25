(() => {
'use strict';
const el=(tag,text,cls)=>{const n=document.createElement(tag);if(text!=null)n.textContent=text;if(cls)n.className=cls;return n;};
const card=(title,...nodes)=>{const n=el('article',null,'ap-card ap-billing-card');n.append(el('h3',title,'ap-card-title'),...nodes);return n;};
const grid=(...nodes)=>{const n=el('div',null,'ap-billing-grid');n.append(...nodes);return n;};
const date=v=>v?new Date(v).toLocaleDateString():'Not scheduled';
const button=(label,page)=>{const n=el('button',label);n.type='button';if(page)n.addEventListener('click',()=>window.loadPortalSection?.(page));else n.disabled=true;return n;};
const money=v=>v==null?'Price not configured':new Intl.NumberFormat('en-US',{style:'currency',currency:'USD'}).format(v);
function plans(body,d){
 const intro=el('div',null,'ap-billing-plans-intro');intro.append(el('h3','Grow at your own pace'),el('p','Compare your available AskPearls plans.'),el('p','Preview prices only — paid checkout is not enabled.'));body.append(intro);
 const list=el('div',null,'ap-billing-plan-grid');
 for(const p of d.plans){const current=d.current.isProvisioned && String(d.current.status).toLowerCase()==='active' && [p.name,p.code].some(v=>String(v).toLowerCase()===String(d.current.plan).toLowerCase());const n=el('article',null,'ap-card ap-billing-card ap-billing-plan');if(current)n.classList.add('is-current');n.append(el('h3',p.name,'ap-card-title'),el('span',current?'Current plan':'', 'ap-billing-plan-status'),el('strong',money(p.monthlyPriceUsd),'ap-billing-plan-price'),el('small',p.monthlyPriceUsd==null?'':'/ month'),el('p',p.description,'ap-billing-plan-description'),button(current?'Your current plan':'Checkout unavailable'));const features=el('ul',null,'ap-billing-plan-features');for(const l of p.limits){const item=el('li');const icon=el('i',null,'bi bi-check-circle');icon.setAttribute('aria-hidden','true');item.append(icon,document.createTextNode(' '+String(l.code).replace(/[_-]/g,' ')+': '+(l.value??'Not configured')));features.append(item);}n.append(features);list.append(n);}
 body.append(list);if(!d.plans.length)body.append(el('div','No active portal plans configured.','ap-empty-state'));
}
async function init(root){if(root.dataset.loaded)return;root.dataset.loaded='1';const status=root.querySelector('[data-billing-status]'),body=root.querySelector('[data-billing-content]');
try{const res=await fetch('/Account/Api/billing/overview',{credentials:'same-origin',cache:'no-store',headers:{Accept:'application/json'}});if(!res.ok)throw Error('Request failed ('+res.status+')');const d=await res.json();if(!root.isConnected)return;const c=d.current;status.textContent='';body.replaceChildren();
const usage=()=>{const p=el('progress');p.max=Math.max(1,c.widgetLimit??1);p.value=Math.min(p.max,c.widgetsCreated);return card('Widgets',el('strong',`${c.widgetsCreated} / ${c.widgetLimit??'Unlimited'}`),...(c.widgetLimit==null?[]:[p]),el('p',c.widgetsRemaining==null?'Unlimited':`${c.widgetsRemaining} remaining`));};
switch(root.dataset.billingSection){
case 'subscription':body.append(grid(card('Current plan',el('strong',c.plan),el('p',c.status),el('p','Period ends: '+date(c.currentPeriodEndUtc)),button('View plans','subscription-plans')),usage()),card('Subscription history',...d.history.map(h=>el('p',`${h.plan} · ${h.status} · ${date(h.createdAtUtc)}`))));break;
case 'subscription-plans':plans(body,d);break;
case 'subscription-usage':body.append(grid(usage()),card('Other limits',el('p','Domain and session usage enforcement is not connected to this module yet.')));break;
case 'billing-payments':body.append(grid(card('Payment provider',el('p',d.paymentStatus)),card('Current subscription',el('strong',c.plan),el('p','Period ends: '+date(c.currentPeriodEndUtc)))),card('Payment history',el('p','Provider transaction history is not connected.')));break;
case 'billing-invoices':body.append(card('Invoices & receipts',el('p','Provider invoice retrieval is not connected.')));break;
case 'billing-settings':body.append(card('Payment settings',el('p','Payment methods will be managed by the payment provider.'),button('Manage payment method')));break;
}
}catch(e){status.textContent='Unable to load billing information. '+e.message;/* keep loaded flag to prevent observer retry loops */}}
function scan(){const root=document.querySelector('#apMainContent [data-billing-section]');if(root&&!root.dataset.loaded){const collapse=document.getElementById('apBillingCollapse');if(collapse&&window.bootstrap?.Collapse)window.bootstrap.Collapse.getOrCreateInstance(collapse,{toggle:false}).show();init(root);}}
const start=()=>{const main=document.querySelector('#apMainContent');if(main){new MutationObserver(scan).observe(main,{childList:true,subtree:true});scan();}};
if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',start,{once:true});else start();
})();
