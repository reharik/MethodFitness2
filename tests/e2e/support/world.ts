import { expect, type Page } from '@playwright/test';
import { expectSaved, openRoute, searchList, setField, settle, stamp, submitAndRead, usDate, type SaveResult } from './app';

// Everything these tests create is named with an "E2E" prefix, so it is easy to
// tell apart from real records and safe to find and remove.

export interface TestClient { lastName: string; id: string }
export interface TestTrainer { lastName: string; id: string; loginName: string; password: string }

/** The one trainer account these tests use. Trainers cannot be deleted in the app, so it is reused. */
export const TRAINER_LAST_NAME = 'E2ETrainerMain';

function idFromHash(page: Page, screen: string): string {
  const match = new URL(page.url()).hash.match(new RegExp(`^#${screen}/(\\d+)`));
  if (!match || match[1] === '0') throw new Error(`expected to be on an existing ${screen}, but the address is ${new URL(page.url()).hash}`);
  return match[1];
}

async function openFromList(page: Page, listRoute: string, formScope: string, text: string): Promise<boolean> {
  const row = page.locator('#gridContainer tr.jqgrow', { hasText: text });
  // The list reloads on its own after a save, which can swallow a search typed too soon.
  for (let attempt = 0; attempt < 3 && (await row.count()) === 0; attempt++) {
    await openRoute(page, listRoute);
    await searchList(page, listRoute, text);
  }
  if ((await row.count()) === 0) return false;
  await row.first().locator('a').first().click();
  await expect(page.locator(`${formScope} [name="LastName"]`)).toBeVisible();
  await settle(page);
  return true;
}

export async function createClient(page: Page, prefix = 'E2EClient'): Promise<TestClient> {
  const lastName = `${prefix}${stamp()}`;
  await openRoute(page, 'clientlist');
  await page.locator('#clientlist #new').click();
  await expect(page.locator('#client [name="FirstName"]')).toBeVisible();
  await setField(page, '#client', 'FirstName', 'Auto');
  await setField(page, '#client', 'LastName', lastName);
  await setField(page, '#client', 'Email', `${lastName.toLowerCase()}@example.invalid`);
  await setField(page, '#client', 'MobilePhone', '512-555-0100');
  await setField(page, '#client', 'StartDate', usDate(-60));
  expectSaved(await submitAndRead(page, () => page.locator('#client #save').click()));
  await expect(page.locator('#clientlist')).toBeVisible();
  expect(await openFromList(page, 'clientlist', '#client', lastName), 'the new client is in the client list').toBe(true);
  return { lastName, id: idFromHash(page, 'client') };
}

/** Deletes a client through its form. Returns the app's verdict rather than failing, so cleanup can be best-effort. */
export async function deleteClient(page: Page, lastName: string): Promise<SaveResult | undefined> {
  if (!lastName.startsWith('E2E')) throw new Error('refusing to delete a client these tests did not create');
  if (!(await openFromList(page, 'clientlist', '#client', lastName))) return undefined;
  page.once('dialog', (d) => d.accept());
  const result = await submitAndRead(page, () => page.locator('#client button.delete').click());
  await settle(page);
  return result;
}

/**
 * Makes sure the test trainer exists, has the given clients, and has a password
 * this run knows. Reuses the account from earlier runs when it is there.
 */
export async function ensureTrainer(page: Page, clients: TestClient[] = []): Promise<TestTrainer> {
  const password = `E2e-${stamp()}-pw9`;
  const loginName = TRAINER_LAST_NAME.toLowerCase();
  const exists = await openFromList(page, 'trainerlist', '#trainer', TRAINER_LAST_NAME);
  if (!exists) {
    await openRoute(page, 'trainerlist');
    await page.locator('#trainerlist #new').click();
    await expect(page.locator('#trainer [name="FirstName"]')).toBeVisible();
    await setField(page, '#trainer', 'FirstName', 'Auto');
    await setField(page, '#trainer', 'LastName', TRAINER_LAST_NAME);
    await setField(page, '#trainer', 'Email', `${loginName}@example.invalid`);
    await setField(page, '#trainer', 'PhoneMobile', '512-555-0101');
    await setField(page, '#trainer', 'UserLoginInfoLoginName', loginName);
    await setField(page, '#trainer', 'ClientRateDefault', '50');
    await pickToken(page, '#trainer #token-input-UserRolesDtos', 'Tr', /^Trainer$/);
  }
  await setField(page, '#trainer', 'Password', password);
  await setField(page, '#trainer', 'PasswordConfirmation', password);
  for (const c of clients) await pickToken(page, '#trainer #token-input-ClientsDtos', c.lastName.slice(0, 12), c.lastName);
  expectSaved(await submitAndRead(page, () => page.locator('#trainer #save').click()));
  await expect(page.locator('#trainerlist')).toBeVisible();
  expect(await openFromList(page, 'trainerlist', '#trainer', TRAINER_LAST_NAME), 'the test trainer is in the trainer list').toBe(true);
  return { lastName: TRAINER_LAST_NAME, id: idFromHash(page, 'trainer'), loginName, password };
}

/** Types into one of the app's token pickers and chooses the matching suggestion. */
export async function pickToken(page: Page, input: string, typed: string, choose: string | RegExp): Promise<void> {
  const box = page.locator(input);
  await box.click();
  await box.pressSequentially(typed, { delay: 60 });
  const suggestion = page.locator('.token-input-dropdown li', { hasText: choose });
  await expect(suggestion.first(), `the picker offers a match for "${typed}"`).toBeVisible();
  await suggestion.first().click();
  await settle(page);
}
