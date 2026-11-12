import { test } from '@playwright/test';
import { login } from '../support/session';
import { deleteFromList } from '../support/app';

// Runs first (files run in name order). Removes anything an earlier, interrupted
// run left behind. Every record these tests create has a name starting "E2E".
test('remove leftovers from earlier runs', async ({ page }) => {
  await login(page);
  const clients = await deleteFromList(page, 'clientlist', 'E2E');
  console.log(`leftover E2E clients removed: ${clients}`);
});
