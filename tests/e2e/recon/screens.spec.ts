import { test, type Page } from '@playwright/test';
import { login } from '../support/session';
import { outline } from '../support/outline';
import { openRoute, searchList, setField, settle, stamp, submitAndRead, usDate, type SaveResult } from '../support/app';

async function show(page: Page, title: string, scope = 'body') {
  console.log(`\n===== ${title}  [${page.url().replace(/^https?:\/\/[^/]+/, '')}]`);
  console.log(await outline(page, scope));
}

// Errors are reported by name only: Playwright's full message quotes the
// element it matched, which in a grid can be a real person's name.
async function attempt(label: string, fn: () => Promise<void>) {
  try { await fn(); } catch (e) { console.log(`\n(${label} failed: ${(e as Error).name})`); }
}

const verdict = (r: SaveResult) => `${r.Success} ${r.Message ?? ''} ${(r.Errors ?? []).map((e) => e.ErrorMessage).join(' | ')}`;

test('outline the trainer form, client payments and pay-trainer screens', async ({ page }) => {
  page.on('pageerror', (e) => console.log(`[pageerror] ${e.message.slice(0, 200)}`));
  page.on('dialog', (d) => { console.log(`[dialog] ${d.type()}: ${d.message().slice(0, 80)}`); d.accept().catch(() => {}); });
  await login(page);
  await settle(page);
  const id = stamp();
  const clientLast = `E2ERecon${id}`;
  const trainerLast = `E2ETrainer${id}`;

  await attempt('create client', async () => {
    await openRoute(page, 'clientlist');
    await page.locator('#clientlist #new').click();
    await setField(page, '#client', 'FirstName', 'Auto');
    await setField(page, '#client', 'LastName', clientLast);
    await setField(page, '#client', 'Email', `${clientLast.toLowerCase()}@example.invalid`);
    await setField(page, '#client', 'MobilePhone', '512-555-0100');
    await setField(page, '#client', 'StartDate', usDate());
    console.log(`\nclient saved: ${verdict(await submitAndRead(page, () => page.locator('#client #save').click()))}`);
  });

  await attempt('client payments', async () => {
    await openRoute(page, 'clientlist');
    await searchList(page, 'clientlist', clientLast);
    await page.locator('#gridContainer tr.jqgrow', { hasText: clientLast }).locator('a').first().click();
    await settle(page, 1000);
    await page.locator('#client button.client_payment').click();
    await settle(page, 1500);
    await show(page, 'client: after the payment button', '#main-content');
    const add = page.locator('#main-content #new, #main-content button.new');
    console.log(`new-buttons there: ${await add.count()}`);
    if (await add.count()) {
      await add.first().click();
      await settle(page, 1500);
      await show(page, 'client payment: after #new', '#main-content');
    }
  });

  await attempt('create trainer', async () => {
    await openRoute(page, 'trainerlist');
    await page.locator('#trainerlist #new').click();
    await settle(page, 1500);
    await show(page, 'trainer: new form', '#main-content');
    await setField(page, '#trainer', 'FirstName', 'Auto');
    await setField(page, '#trainer', 'LastName', trainerLast);
    await setField(page, '#trainer', 'Email', `${trainerLast.toLowerCase()}@example.invalid`);
    await setField(page, '#trainer', 'PhoneMobile', '512-555-0101');
    await setField(page, '#trainer', 'UserLoginInfoLoginName', trainerLast.toLowerCase());
    await setField(page, '#trainer', 'Password', `Pw-${id}-x9`);
    await setField(page, '#trainer', 'PasswordConfirmation', `Pw-${id}-x9`);
    await setField(page, '#trainer', 'ClientRateDefault', '50');

    // roles: what does the picker offer?
    const roles = page.locator('#trainer #token-input-UserRolesDtos');
    await roles.click();
    await roles.pressSequentially('Tr', { delay: 80 });
    await settle(page, 1500);
    console.log('\nrole picker offers: ' + JSON.stringify(await page.evaluate(() =>
      Array.from(document.querySelectorAll('.token-input-dropdown li')).map((li) => (li.textContent ?? '').trim()))));
    const trainerRole = page.locator('.token-input-dropdown li', { hasText: /^Trainer$/ });
    if (await trainerRole.count()) await trainerRole.first().click();
    await settle(page, 500);
    console.log('selected role tokens: ' + await page.locator('#UserRolesDtos_container .selectedItem, #UserRolesDtos_container li.token-input-token').count());

    // clients: can the new client be found and attached?
    const clients = page.locator('#trainer #token-input-ClientsDtos');
    await clients.click();
    await clients.pressSequentially(clientLast.slice(0, 10), { delay: 60 });
    await settle(page, 1500);
    const offered = page.locator('.token-input-dropdown li', { hasText: clientLast });
    console.log(`client picker offers our client: ${await offered.count()}`);
    if (await offered.count()) await offered.first().click();
    await settle(page, 800);
    await show(page, 'trainer: form filled', '#main-content');

    console.log(`\ntrainer saved: ${verdict(await submitAndRead(page, () => page.locator('#trainer #save').click()))}`);
  });

  await attempt('pay trainer screens', async () => {
    await openRoute(page, 'trainerlist');
    await searchList(page, 'trainerlist', trainerLast);
    const row = page.locator('#gridContainer tr.jqgrow', { hasText: trainerLast });
    console.log(`\nour trainer rows: ${await row.count()}`);
    await row.locator('a').first().click();
    await settle(page, 1500);
    await page.locator('#trainer #payTrainer').click();
    await settle(page, 2000);
    await show(page, 'trainer: after #payTrainer', '#main-content');
    await page.goBack();
    await settle(page, 1500);
    await page.locator('#trainer #trainerPayments').click();
    await settle(page, 2000);
    await show(page, 'trainer: after #trainerPayments', '#main-content');
  });
});
