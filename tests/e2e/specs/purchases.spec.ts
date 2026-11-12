import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, expectSaved, gridReady, openRoute, setField, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient } from '../support/world';

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

      await page.locator('#clientpurchaselist #new').click();
      await expect(page.locator('[name="FullHour"]:visible')).toBeVisible();
      // the form's container is named after its route, e.g. "#clientpurchase/0/123"
      const form = '#' + new URL(page.url()).hash.slice(1).split('/')[0];
      await setField(page, form, 'FullHour', '2');
      await setField(page, form, 'Notes', 'created by the browser tests');
      expectSaved(await submitAndRead(page, () => page.locator(`${form} #save`).click()));

      // back on the client's purchase list, with the purchase on it
      await gridReady(page, 'clientpurchaselist');
      await expect(purchases).toHaveCount(1);
    } finally {
      const cleanup = await archiveClient(page, client.lastName).catch(() => undefined);
      console.log(`test client archived afterwards: ${cleanup?.Success ?? false}`);
    }

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
