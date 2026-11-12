import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { expectSaved, openRoute, setField, settle, submitAndRead, watchForTrouble } from '../support/app';
import { createClient, deleteClient } from '../support/world';

test.describe('client purchases', () => {
  test('record a purchase of sessions for a client', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    page.on('dialog', (d) => d.accept().catch(() => {}));
    await login(page);
    const client = await createClient(page);

    try {
      await openRoute(page, `clientpurchaselist/${client.id}`);
      const list = page.locator('#main-content');
      await expect(list.locator('#new')).toBeVisible();
      await expect(list.locator('.ui-jqgrid-btable tr.jqgrow')).toHaveCount(0);

      await list.locator('#new').click();
      await expect(list.locator('[name="FullHour"]')).toBeVisible();
      await setField(page, '#main-content', 'FullHour', '2');
      await setField(page, '#main-content', 'Notes', 'created by the browser tests');
      expectSaved(await submitAndRead(page, () => list.locator('#save').click()));
      await settle(page, 1000);

      // back on the client's purchase list, with the purchase on it
      await expect(list.locator('.ui-jqgrid-btable tr.jqgrow')).toHaveCount(1);
    } finally {
      const cleanup = await deleteClient(page, client.lastName).catch(() => undefined);
      console.log(`test client removed afterwards: ${cleanup?.Success ?? false}`);
    }

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
