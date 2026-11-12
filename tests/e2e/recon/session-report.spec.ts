import { test } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, gridReady, openRoute } from '../support/app';

// Diagnostic, counts only: for every trainer, how many rows does the admin's
// "pay trainer" list hold? Its filter is "this trainer, up to today, not yet paid".
// A handful to a few hundred is plausible. Thousands would mean the "not yet
// paid" part of the filter is being ignored by this build.
test('unpaid-session counts per trainer', async ({ page }) => {
  acceptDialogs(page);
  await login(page);

  await openRoute(page, 'trainerlist');
  await gridReady(page, 'trainerlist');
  const ids = await page.locator('#trainerlist .ui-jqgrid-btable tr.jqgrow').evaluateAll((rows) => rows.map((r) => r.id).filter((id) => /^\d+$/.test(id)));
  console.log(`\ntrainers listed: ${ids.length}`);

  // learn the list's data address from one real load, then ask it for each trainer
  const first = page.waitForResponse((r) => r.url().includes('PayTrainerList/TrainerPayments'), { timeout: 60_000 });
  await openRoute(page, `paytrainerlist/${ids[0]}`);
  const sample = new URL((await first).url());
  console.log(`data address shape: ${sample.pathname}?${Array.from(sample.searchParams.keys()).join('&')}`);

  for (const id of ids) {
    const url = new URL(sample.toString());
    url.searchParams.set('ParentId', id);
    url.searchParams.set('rows', '5');
    const response = await page.request.get(url.toString(), { failOnStatusCode: false });
    let records = '?';
    try { records = String(JSON.parse(await response.text()).records); } catch { /* not JSON */ }
    console.log(`  trainer ${id}: unpaid sessions listed = ${records}  (HTTP ${response.status()})`);
  }
});
