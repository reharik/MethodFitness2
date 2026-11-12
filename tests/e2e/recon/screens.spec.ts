import { test, type Page } from '@playwright/test';
import { login } from '../support/session';
import { outline } from '../support/outline';
import { openRoute, setField, settle, stamp, submitAndRead, usDate } from '../support/app';

async function show(page: Page, title: string, scope = 'body') {
  console.log(`\n===== ${title}  [${page.url().replace(/^https?:\/\/[^/]+/, '')}]`);
  console.log(await outline(page, scope));
}

// Errors are reported by name only: Playwright's full message quotes the
// element it matched, which in a grid can be a real person's name.
async function attempt(label: string, fn: () => Promise<void>) {
  try { await fn(); } catch (e) { console.log(`\n(${label} failed: ${(e as Error).name})`); }
}

test('outline the appointment, payment and pay-trainer screens', async ({ page }) => {
  page.on('pageerror', (e) => console.log(`[pageerror] ${e.message.slice(0, 200)}`));
  page.on('dialog', (d) => { console.log(`[dialog] ${d.type()}: ${d.message().slice(0, 80)}`); d.accept().catch(() => {}); });
  await login(page);
  await settle(page);

  await attempt('appointment popup', async () => {
    await openRoute(page, 'calendar');
    await page.locator('.fc-header .fc-button-next').click();
    await settle(page, 1000);
    const row = page.locator('table.fc-agenda-slots tr').nth(30);
    await row.scrollIntoViewIfNeeded();
    const box = await row.boundingBox();
    if (!box) throw new Error('no slot row');
    await page.mouse.click(box.x + box.width * 0.5, box.y + box.height / 2);
    await settle(page, 1500);
    const info = await page.evaluate(() => {
      const opts = (name: string) => Array.from(document.querySelectorAll(`#editModule [name="${name}"] option`)).map((o) => (o.textContent ?? '').trim());
      const trainers = opts('TrainerEntityId');
      return {
        appointmentTypes: opts('AppointmentType'),                       // interface values, not personal data
        startTimes: opts('StartTimeString').slice(0, 6),
        locationCount: opts('LocationEntityId').length,
        trainerCount: trainers.length,
        adminIsATrainerOption: trainers.some((t) => /admin/i.test(t)),
        selectedTrainerIsAdmin: /admin/i.test((document.querySelector('#editModule [name="TrainerEntityId"] option:checked')?.textContent ?? '')),
        dateValue: (document.querySelector('#editModule [name="Date"]') as HTMLInputElement | null)?.value,
        startValue: (document.querySelector('#editModule [name="StartTimeString"]') as HTMLSelectElement | null)?.value,
        dialogTitle: document.querySelector('.ui-dialog-title')?.textContent,
      };
    });
    console.log('\n===== appointment popup facts\n' + JSON.stringify(info, null, 2));
    // what does the client picker offer when asked for a name that cannot exist?
    await page.locator('#editModule #token-input-ClientsDtos').fill('zzqx');
    await settle(page, 1500);
    console.log('client picker dropdown for a nonsense search: ' + await page.evaluate(() => {
      const d = document.querySelector('.token-input-dropdown');
      return d ? `${d.querySelectorAll('li').length} item(s), classes: ${d.className}` : 'no dropdown element';
    }));
    await page.locator('.ui-dialog-buttonpane button', { hasText: 'Cancel' }).click();
  });

  const lastName = `E2ERecon${stamp()}`;
  await attempt('client payments', async () => {
    await openRoute(page, 'clientlist');
    await page.locator('#clientlist #new').click();
    await setField(page, '#client', 'FirstName', 'Auto');
    await setField(page, '#client', 'LastName', lastName);
    await setField(page, '#client', 'Email', `${lastName.toLowerCase()}@example.invalid`);
    await setField(page, '#client', 'MobilePhone', '512-555-0100');
    await setField(page, '#client', 'StartDate', usDate());
    const saved = await submitAndRead(page, () => page.locator('#client #save').click());
    console.log(`\nrecon client saved: ${saved.Success} ${saved.Message ?? ''} ${(saved.Errors ?? []).map((e) => e.ErrorMessage).join(' | ')}`);
    await settle(page, 800);
    const search = page.locator('#clientlist input.search');
    await search.fill(lastName);
    await search.press('Enter');
    await settle(page, 1000);
    await page.locator('#gridContainer tr.jqgrow', { hasText: lastName }).locator('a').first().click();
    await settle(page, 1000);
    await page.locator('#client button.client_payment').click();
    await settle(page, 1500);
    await show(page, 'client: after clicking the payment button', '#main-content');
    const add = page.locator('#main-content #new, #main-content button.new');
    console.log(`new-buttons on that screen: ${await add.count()}`);
    if (await add.count()) {
      await add.first().click();
      await settle(page, 1500);
      await show(page, 'client payment: after #new', '#main-content');
    }
  });

  await attempt('delete recon client', async () => {
    await openRoute(page, 'clientlist');
    const search = page.locator('#clientlist input.search');
    await search.fill(lastName);
    await search.press('Enter');
    await settle(page, 1000);
    const row = page.locator('#gridContainer tr.jqgrow', { hasText: lastName });
    console.log(`\nrecon client rows found for cleanup: ${await row.count()}`);
    await row.locator('input.cbox').check();
    const deleted = await submitAndRead(page, () => page.locator('#clientlist button.delete').click(), 'GET', 'Delete');
    console.log(`recon client deleted: ${deleted.Success} ${deleted.Message ?? ''}`);
  });

  await attempt('pay trainer screens', async () => {
    await openRoute(page, 'trainerlist');
    await settle(page, 1000);
    const adminRow = page.locator('#gridContainer tr.jqgrow', { hasText: /admin/i });
    console.log(`\ntrainer rows matching "admin": ${await adminRow.count()}`);
    await adminRow.first().locator('a').first().click();
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
