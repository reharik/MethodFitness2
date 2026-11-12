import { test } from '@playwright/test';
import { login } from '../support/session';
import { openRoute, searchList } from '../support/app';
import { deleteClient } from '../support/world';

// Runs first (files run in name order). Removes clients that earlier, interrupted
// runs left behind. Every record these tests create has a name starting "E2E".
// Best-effort: a client the app will not delete (for example one with session
// history) is left alone and reported.
test('remove leftover test clients', async ({ page }) => {
  await login(page);
  let removed = 0;
  const kept: string[] = [];
  for (let i = 0; i < 25; i++) {
    await openRoute(page, 'clientlist');
    await searchList(page, 'clientlist', 'E2E');
    const names = await page.locator('#gridContainer tr.jqgrow td', { hasText: /^E2E\w+$/ }).allTextContents();
    const next = names.map((n) => n.trim()).find((n) => !kept.includes(n));
    if (!next) break;
    const result = await deleteClient(page, next);
    if (result?.Success) removed++; else kept.push(next);
  }
  console.log(`leftover test clients removed: ${removed}; could not be removed: ${kept.length}`);
});
