import { test, type Page } from '@playwright/test';
import { login, logout } from '../support/session';
import { acceptDialogs, openRoute, settle } from '../support/app';
import { ensureTrainer } from '../support/world';

// Diagnostic: the test trainer has no sessions of its own on QA, so every list of
// "this trainer's sessions" should be empty for it. Prints counts only.
async function gridCount(page: Page, route: string, urlPart: string): Promise<string> {
  const wait = page.waitForResponse((r) => r.url().includes(urlPart) && r.request().resourceType() !== 'document', { timeout: 60_000 });
  await openRoute(page, route);
  const response = await wait;
  let summary = `HTTP ${response.status()}`;
  try {
    const body = JSON.parse(await response.text());
    const rows = Array.isArray(body.items) ? body.items.length : Array.isArray(body.rows) ? body.rows.length : -1;
    summary += ` keys=[${Object.keys(body).join(',')}] rowsOnPage=${rows} records=${body.records ?? '?'} total=${body.total ?? '?'}`;
  } catch { summary += ' (not JSON)'; }
  await settle(page, 500);
  const shown = await page.locator(`#${route.split('/')[0]} .ui-jqgrid-btable tr.jqgrow`).count();
  return `${summary} rowsRendered=${shown}`;
}

test('how many sessions do the "my sessions" lists show for a trainer with none?', async ({ page }) => {
  acceptDialogs(page);
  await login(page);
  const trainer = await ensureTrainer(page);
  console.log(`\ntest trainer id: ${trainer.id}`);
  console.log(`as admin    pay-trainer list for the test trainer : ${await gridCount(page, `paytrainerlist/${trainer.id}`, 'PayTrainerList/TrainerPayments')}`);
  console.log(`as admin    session report (admin's own)          : ${await gridCount(page, 'trainersessionview', 'TrainerSessionView/TrainerSessions')}`);

  await logout(page);
  await login(page, { user: trainer.loginName, pass: trainer.password });
  console.log(`as trainer  session report                        : ${await gridCount(page, 'trainersessionview', 'TrainerSessionView/TrainerSessions')}`);
  console.log(`as trainer  session verification                  : ${await gridCount(page, 'trainersessionverification', 'TrainerSessionVerification/TrainerSessions')}`);
});
