import { test, expect, type Page } from '@playwright/test';
import { login } from '../support/session';
import { openRoute, setField, usDate, watchForTrouble } from '../support/app';

// Each report is a form in the app that opens the report itself in a new window.
async function runReport(page: Page, route: string, expectInUrl: string[], prepare?: () => Promise<void>) {
  await openRoute(page, route);
  const form = `#${route}`;
  await expect(page.locator(`${form} #viewReport`)).toBeVisible();
  await setField(page, form, 'StartDate', usDate(-30));
  await setField(page, form, 'EndDate', usDate());
  if (prepare) await prepare();

  const [report] = await Promise.all([page.waitForEvent('popup'), page.locator(`${form} #viewReport`).click()]);
  const { problems } = watchForTrouble(report);
  await report.waitForLoadState('domcontentloaded');
  for (const part of expectInUrl) expect(report.url()).toContain(part);

  // The viewer renders its frame first and then fetches the report body.
  await expect(report.locator('[id^="ReportViewer1"]').first()).toBeAttached({ timeout: 60_000 });
  await report.waitForLoadState('networkidle').catch(() => {});
  // Checked inside the page and reported as yes/no: an assertion on the body would
  // print the report's contents (real names and amounts) into the log when it fails.
  const showsError = await report.evaluate(() =>
    /Server Error in|An error has occurred during report processing|could not be found/i.test(document.body.innerText));
  expect(showsError, 'the report window shows an error message').toBe(false);
  expect(problems, problems.join('\n')).toEqual([]);
  await report.close();
}

test.describe('reports', () => {
  test.beforeEach(async ({ page }) => { await login(page); });

  test('Payments report runs for a date range', async ({ page }) => {
    await runReport(page, 'dailypayments', ['DailyPayments.aspx', 'StartDate=', 'EndDate=']);
  });

  test('Activity report runs for a date range', async ({ page }) => {
    await runReport(page, 'activity', ['Activity.aspx', 'StartDate=', 'LocationId=']);
  });

  test('Productivity report runs for a trainer and date range', async ({ page }) => {
    await runReport(page, 'trainermetric', ['TrainerMetric.aspx', 'TrainerId='], async () => {
      // any trainer will do; pick by position so no name appears in the test or its output
      await page.locator('#trainermetric [name="TrainerEntityId"]').selectOption({ index: 1 });
    });
  });
});
