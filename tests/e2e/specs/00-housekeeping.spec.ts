import { test } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, openRoute, searchList } from '../support/app';
import { archiveClient } from '../support/world';

// Runs first (files run in name order). Archives clients that earlier, interrupted
// runs left live. Every record these tests create has a name starting "E2E".
test('archive leftover test clients', async ({ page }) => {
  acceptDialogs(page);
  await login(page);
  await openRoute(page, 'clientlist');
  await searchList(page, 'clientlist', 'E2E');
  // rows whose last cell still says "Archive" are live
  const live = await page.locator('#clientlist .ui-jqgrid-btable tr.jqgrow').evaluateAll((rows) =>
    rows
      .map((r) => Array.from(r.querySelectorAll('td')).map((td) => (td.textContent ?? '').trim()))
      .filter((cells) => cells[cells.length - 1] === 'Archive')
      .map((cells) => cells.find((c) => /^E2E\w+$/.test(c)))
      .filter((name): name is string => Boolean(name)));
  let archived = 0;
  for (const name of live.slice(0, 30)) if ((await archiveClient(page, name)) === 'archived') archived++;
  console.log(`live leftover test clients: ${live.length}; archived now: ${archived}`);
});
