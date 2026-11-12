import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { openRoute, settle, watchForTrouble } from '../support/app';

test.describe('calendar', () => {
  test('loads the week view and switches views', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    await login(page);
    await openRoute(page, 'calendar');

    const calendar = page.locator('#calendar');
    await expect(calendar.locator('table.fc-agenda-slots')).toBeVisible();
    await expect(calendar.locator('table.fc-agenda-days th.fc-widget-header:not(.fc-agenda-axis):not(.fc-agenda-gutter)')).toHaveCount(7);

    await calendar.locator('.fc-button-month').click();
    await settle(page);
    await expect(calendar.locator('.fc-view-month')).toBeVisible();

    await calendar.locator('.fc-button-agendaDay').click();
    await settle(page);
    await expect(calendar.locator('.fc-view-agendaDay')).toBeVisible();

    await calendar.locator('.fc-button-agendaWeek').click();
    await calendar.locator('.fc-button-next').click();
    await settle(page);
    await expect(calendar.locator('.fc-view-agendaWeek')).toBeVisible();

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
