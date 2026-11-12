import { test } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, openRoute, settle } from '../support/app';

// Diagnostic, counts only. The database says trainers 3 and 6057 have unpaid
// sessions on QA. What does the pay-trainer screen itself load for them?
test('pay-trainer list: what the screen loads vs a direct request', async ({ page }) => {
  acceptDialogs(page);
  await login(page);
  for (const id of ['3', '6057']) {
    const seen: string[] = [];
    const listen = async (r: import('@playwright/test').Response) => {
      if (!r.url().includes('PayTrainerList')) return;
      const u = new URL(r.url());
      let info = `HTTP ${r.status()}`;
      try {
        const body = JSON.parse(await r.text());
        info += ` keys=[${Object.keys(body).slice(0, 8).join(',')}]`;
        if ('records' in body) info += ` records=${body.records} total=${body.total} page=${body.page} items=${Array.isArray(body.items) ? body.items.length : '?'}`;
      } catch { info += ' (not JSON)'; }
      seen.push(`${r.request().method()} ${u.pathname}?${Array.from(u.searchParams.entries()).map(([k, v]) => `${k}=${k === 'nd' ? '…' : v}`).join('&')} -> ${info}`);
    };
    page.on('response', listen);
    await openRoute(page, `paytrainerlist/${id}`);
    await settle(page, 2500);
    page.off('response', listen);
    console.log(`\ntrainer ${id}: requests the screen made`);
    for (const s of seen) console.log(`  ${s}`);
    console.log(`  rows rendered: ${await page.locator('#paytrainerlist .ui-jqgrid-btable tr.jqgrow').count()}`);

    for (const rows of ['5', '50', '1000']) {
      const direct = await page.request.get(`/Billing/PayTrainerList/TrainerPayments?ParentId=${id}&_search=false&rows=${rows}&page=1&sidx=&sord=asc`,
        { failOnStatusCode: false, headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      let records = '?';
      try { const b = JSON.parse(await direct.text()); records = `records=${b.records} items=${Array.isArray(b.items) ? b.items.length : '?'}`; } catch { records = '(not JSON)'; }
      console.log(`  direct request rows=${rows}: HTTP ${direct.status()} ${records}`);
    }
  }
});
