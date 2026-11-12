import { test } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, gridReady, openRoute, searchList } from '../support/app';
import { archiveClient } from '../support/world';

// Runs first (files run in name order). Archives clients that earlier, interrupted
// runs left in the list. Every record these tests create has a name starting "E2E".
test('archive leftover test clients', async ({ page }) => {
  acceptDialogs(page);
  await login(page);
  let archived = 0;
  for (let i = 0; i < 25; i++) {
    await openRoute(page, 'clientlist');
    await searchList(page, 'clientlist', 'E2E');
    await gridReady(page, 'clientlist');
    const names = await page.locator('#clientlist .ui-jqgrid-btable tr.jqgrow td', { hasText: /^E2E\w+$/ }).allTextContents();
    if (names.length === 0) break;
    const result = await archiveClient(page, names[0].trim());
    if (!result?.Success) break;
    archived++;
  }
  console.log(`leftover test clients archived: ${archived}`);
});
