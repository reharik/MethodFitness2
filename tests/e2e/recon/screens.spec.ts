import { test, type Page } from '@playwright/test';
import { login } from '../support/session';
import { outline } from '../support/outline';

async function settle(page: Page, ms = 1500) {
  await page.waitForLoadState('networkidle').catch(() => {});
  await page.waitForTimeout(ms);
}

async function show(page: Page, title: string, scope = 'body') {
  console.log(`\n===== ${title}  [${page.url().replace(/^https?:\/\/[^/]+/, '')}]`);
  console.log(await outline(page, scope));
}

// Errors are reported by name only: Playwright's full message quotes the
// element it matched, which in a grid can be a real person's name.
async function attempt(label: string, fn: () => Promise<void>) {
  try { await fn(); } catch (e) { console.log(`\n(${label} failed: ${(e as Error).name})`); }
}

async function openRoute(page: Page, hash: string) {
  await page.goto(`/Scheduler#${hash}`);
  await page.evaluate((h) => { if (window.location.hash !== '#' + h) window.location.hash = h; }, hash);
  await settle(page);
}

test('outline forms and popups', async ({ page }) => {
  page.on('pageerror', (e) => console.log(`[pageerror] ${e.message.slice(0, 200)}`));
  page.on('dialog', (d) => { console.log(`[dialog] ${d.type()}`); d.dismiss().catch(() => {}); });
  page.on('response', (r) => { if (r.status() >= 400) console.log(`[http ${r.status()}] ${r.url().replace(/^https?:\/\/[^/]+/, '').slice(0, 100)}`); });
  await login(page);
  await settle(page);

  await attempt('client add form', async () => {
    await openRoute(page, 'clientlist');
    await page.locator('#clientlist #new').click();
    await settle(page);
    await show(page, 'clients: after #new');
  });

  await attempt('base rates', async () => {
    await openRoute(page, 'basesessionrate');
    await settle(page, 4000);
    await show(page, 'base rates', '#main-content');
  });

  await attempt('trainer row', async () => {
    await openRoute(page, 'trainerlist');
    const rowLinks = page.locator('#gridContainer tr.jqgrow a');
    console.log(`\ntrainer grid: ${await page.locator('#gridContainer tr.jqgrow').count()} rows, ${await rowLinks.count()} links in rows`);
    await rowLinks.first().click();
    await settle(page);
    await show(page, 'trainers: after clicking the first row link');
  });

  await attempt('client row', async () => {
    await openRoute(page, 'clientlist');
    const rowLinks = page.locator('#gridContainer tr.jqgrow a');
    console.log(`\nclient grid: ${await page.locator('#gridContainer tr.jqgrow').count()} rows, ${await rowLinks.count()} links in rows`);
    await rowLinks.first().click();
    await settle(page);
    await show(page, 'clients: after clicking the first row link');
  });

  await attempt('calendar slot', async () => {
    await openRoute(page, 'calendar');
    console.log(`\ncalendar header buttons: ${await page.locator('.fc-header .fc-button').evaluateAll((els) => els.map((e) => (e.className.match(/fc-button-\w+/) ?? [''])[0]).join(', '))}`);
    await page.locator('.fc-header .fc-button-next').click();   // next week: every slot is in the future
    await settle(page);
    const row = page.locator('table.fc-agenda-slots tr').nth(30);
    await row.scrollIntoViewIfNeeded();
    const box = await row.boundingBox();
    if (!box) throw new Error('no slot row');
    await page.mouse.click(box.x + box.width * 0.5, box.y + box.height / 2);
    await settle(page);
    await show(page, 'calendar: after clicking an empty slot next week');
  });
});
