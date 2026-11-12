import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, gridReady, openRoute, watchForTrouble } from '../support/app';
import { archiveClient, createClient } from '../support/world';
import { purchaseHours } from '../support/flow';

test.describe('client purchases', () => {
  test('record a purchase of sessions for a client', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    acceptDialogs(page);
    await login(page);
    const client = await createClient(page);

    try {
      await openRoute(page, `clientpurchaselist/${client.id}`);
      const purchases = page.locator('#clientpurchaselist .ui-jqgrid-btable tr.jqgrow');
      await gridReady(page, 'clientpurchaselist');
      await expect(purchases).toHaveCount(0);

      await purchaseHours(page, client, 2);

      // back on the client's purchase list, with the purchase on it
      await expect(purchases).toHaveCount(1);
    } finally {
      const cleanup = await archiveClient(page, client.lastName).catch(() => 'failed');
      console.log(`test client afterwards: ${cleanup}`);
    }

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
