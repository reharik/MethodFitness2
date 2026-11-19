import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { gridReady, openRoute, watchForTrouble } from '../support/app';
import { archiveClient, createClient } from '../support/world';

// Clients are never deleted, only archived. Neither the client list nor the
// client form offers a delete button.
test('a client can not be deleted, from the list or from its form', async ({ page }) => {
  const { problems } = watchForTrouble(page);
  await login(page);
  const client = await createClient(page);   // leaves the client's form open
  try {
    await expect(page.locator('#client [name="LastName"]')).toBeVisible();
    expect(await page.locator('#client button.delete:visible').count(), 'the client form has no delete button').toBe(0);

    const started = Date.now();
    await openRoute(page, 'clientlist');
    await gridReady(page, 'clientlist');
    await expect(page.locator('#clientlist .ui-jqgrid-btable tr.jqgrow').first()).toBeVisible();
    // a number only, for keeping an eye on how long this list takes
    console.log(`client list (administrator, archived hidden) showed its rows after ${Date.now() - started} ms`);
    expect(await page.locator('#clientlist button.delete:visible').count(), 'the client list has no delete button').toBe(0);
  } finally {
    console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
  }
  expect(problems, problems.join('\n')).toEqual([]);
});
