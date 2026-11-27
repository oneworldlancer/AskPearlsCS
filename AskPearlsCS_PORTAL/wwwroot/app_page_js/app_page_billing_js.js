(() => {
'use strict';
const el=(tag,text,cls)=>{const n=document.createElement(tag);if(text!=null)n.textContent=text;if(cls)n.className=cls;return n;};
const card=(title,...nodes)=>{const n=el('article',null,'ap-card ap-billing-card');n.append(el('h3',title,'ap-card-title'),...nodes);return n;};
const grid=(...nodes)=>{const n=el('div',null,'ap-billing-grid');n.append(...nodes);return n;};
const date=v=>v&&Number.isFinite(Date.parse(v))?new Date(v).toLocaleDateString():'Not scheduled';
const money=v=>v==null?'Not configured':new Intl.NumberFormat('en-US',{style:'currency',currency:'USD'}).format(v);
const btn=(label,fn)=>{const n=el('button',label,'btn btn-outline-secondary');n.type='button';n.disabled=!fn;if(fn)n.addEventListener('click',fn);return n;};
const nav=(label,page)=>btn(label,()=>window.loadPortalSection?.(page));
const line=(label,value)=>el('p',label+': '+(value??'Unavailable'));
function usage(c){const nodes=[line('Widgets',`${c.widgetsCreated??'—'} / ${c.widgetLimit??'Unlimited'}`)];if(c.widgetLimit>0){const p=el('progress');p.max=c.widgetLimit;p.value=Math.min(p.max,Math.max(0,c.widgetsCreated||0));nodes.push(p);}nodes.push(line('Remaining',c.widgetsRemaining??'Unlimited'));return card('Widget usage',...nodes);}
function plans(root,body,d){const list=el('div',null,'ap-billing-plan-grid');for(const p of d.plans||[]){const current=d.current.isProvisioned&&String(d.current.status).toLowerCase()==='active'&&[p.name,p.code].some(v=>String(v).toLowerCase()===String(d.current.plan).toLowerCase());const n=card(p.name,el('strong',money(p.monthlyPriceUsd),'ap-billing-plan-price'),el('small',' / month'),el('p',p.description));n.classList.add('ap-billing-plan');if(current)n.classList.add('is-current');const features=el('ul',null,'ap-billing-plan-features');for(const l of p.limits||[])features.append(el('li',String(l.code).replace(/[_-]/g,' ')+': '+(l.value??'Unlimited')));n.append(features);const allowed=!current&&d.checkoutEnabled===true&&p.monthlyPriceUsd>0&&String(d.current.plan).toLowerCase()==='free'&&typeof window.askPearlsPaddleCheckout==='function'&&!!window.Paddle;const action=btn(current?'Current plan':allowed?'Select plan':'Checkout unavailable',allowed?async()=>{const buttons=list.querySelectorAll('button:not(:disabled)');buttons.forEach(b=>b.disabled=true);try{const token=root.querySelector('input[name="__RequestVerificationToken"]')?.value;if(!token)throw Error('Missing security token');await window.askPearlsPaddleCheckout(p.code,'month',token);root.querySelector('[data-billing-status]').textContent='Checkout opened. Refresh after payment confirmation. Your plan changes only after server verification.';}catch(e){root.querySelector('[data-billing-status]').textContent=e.message;buttons.forEach(b=>b.disabled=false);}}:null);n.append(action);list.append(n);}body.append(list);if(!list.children.length)body.append(el('p','No plans configured.'));}
async function load(root){if(root.dataset.busy==='1')return;root.dataset.busy='1';const status=root.querySelector('[data-billing-status]'),body=root.querySelector('[data-billing-content]'),refresh=root.querySelector('[data-billing-refresh]');if(refresh)refresh.disabled=true;status.textContent='Loading account information…';try{const r=await fetch('/Account/Api/billing/overview',{credentials:'same-origin',cache:'no-store',headers:{Accept:'application/json'}});if(!r.ok)throw Error('Request failed ('+r.status+')');const d=await r.json();if(!root.isConnected)return;const c=d.current;if(!c)throw Error('Subscription unavailable');body.replaceChildren();status.textContent='';switch(root.dataset.billingSection){case 'subscription':body.append(grid(card('Current plan',el('strong',c.plan),line('Status',c.status),line('Period ends',date(c.currentPeriodEndUtc)),nav('View plans','subscription-plans')),usage(c)),card('Subscription history',...(d.history||[]).map(h=>el('p',`${h.plan} · ${h.status} · ${date(h.createdAtUtc)}`))));break;case 'subscription-plans':plans(root,body,d);break;case 'subscription-usage':body.append(grid(usage(c)),card('Additional allowances',el('p','Live domain and session counters are not available from the current API.')));break;case 'billing-payments':body.append(grid(card('Payment provider',el('p',d.paymentStatus)),card('Subscription',line('Plan',c.plan),line('Period ends',date(c.currentPeriodEndUtc)))),card('Payment history',el('p','Payment history is not available from the current API.')));break;case 'billing-invoices':body.append(card('Invoices & receipts',el('p','Invoice retrieval is not available from the current API.')));break;case 'billing-settings':body.append(card('Payment settings',el('p','Secure customer portal access is not available from the current API.'),btn('Manage payment method',null)));break;}}catch(e){status.textContent='Unable to load billing information. '+e.message;}finally{root.dataset.busy='0';if(refresh)refresh.disabled=false;}}
const initialized = new WeakSet();
function scan(){
 document.querySelectorAll('[data-billing-section]').forEach(root=>{
  if(initialized.has(root))return;
  if(!root.querySelector('[data-billing-status]')||!root.querySelector('[data-billing-content]'))return;
  initialized.add(root);
  root.querySelector('[data-billing-refresh]')?.addEventListener('click',()=>load(root));
  load(root);
 });
}
function start(){
 if(window.__askPearlsBillingObserver)return;
 window.__askPearlsBillingObserver=new MutationObserver(scan);
 window.__askPearlsBillingObserver.observe(document.body,{childList:true,subtree:true});
 scan();
}
if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',start,{once:true});else start();
})();
