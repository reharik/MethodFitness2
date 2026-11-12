import { test, type Page } from '@playwright/test';
import { login } from '../support/session';
import { outline } from '../support/outline';

async function settle(page: Page) {
  await page.waitForLoadState('networkidle').catch(() => {});
  await page.waitForTimeout(1200);
}

async function show(page: Page, title: string, scope = 'body') {
  console.log(`\n===== ${title}  [${page.url().replace(/^https?:\/\/[^/]+/, '')}]`);
  console.log(await outline(page, scope));
}

test('outline the main screens', async ({ page }) => {
  page.on('pageerror', (e) => console.log(`[pageerror] ${e.message.slice(0, 200)}`));
  page.on('dialog', (d) => { console.log(`[dialog] ${d.type()}: ${d.message().slice(0, 120)}`); d.dismiss().catch(() => {}); });
  await login(page);
  await settle(page);

  const menu = page.locator('#left-navigation');
  for (const item of ['Clients', 'Session Report', 'Payment History', 'Session Verification']) {
    await menu.getByRole('link', { name: item, exact: true }).click();
    await settle(page);
    await show(page, `menu: ${item}`, '#main-content');
  }

  await menu.getByRole('link', { name: 'Admin Tools', exact: true }).click();
  await settle(page);
  await show(page, 'menu: Admin Tools (submenu open)', '#left-navigation');

  for (const item of ['Locations', 'Base Rates', 'Trainers']) {
    const link = menu.getByRole('link', { name: item, exact: true });
    if (await link.count()) {
      await link.first().click();
      await settle(page);
      await show(page, `admin: ${item}`, '#main-content');
      await menu.getByRole('link', { name: 'Admin Tools', exact: true }).click().catch(() => {});
      await settle(page);
    } else console.log(`\n(no link named ${item})`);
  }

  const reports = menu.getByRole('link', { name: 'Reports', exact: true });
  if (await reports.count()) {
    await reports.first().click();
    await settle(page);
    await show(page, 'menu: Reports (submenu open)', '#left-navigation');
    for (const item of ['Payments', 'Productivity', 'Activity']) {
      const link = menu.getByRole('link', { name: item, exact: true });
      if (await link.count()) {
        await link.first().click();
        await settle(page);
        await show(page, `report: ${item}`, '#main-content');
        await menu.getByRole('link', { name: 'Admin Tools', exact: true }).click().catch(() => {});
        await menu.getByRole('link', { name: 'Reports', exact: true }).first().click().catch(() => {});
        await settle(page);
      } else console.log(`\n(no link named ${item})`);
    }
  } else console.log('\n(no Reports link)');

  // Clients: what does "add" open?
  await menu.getByRole('link', { name: 'Clients', exact: true }).click();
  await settle(page);
  const add = page.locator('#main-content').locator('button, a, input[type=button]').filter({ hasText: /new|add/i });
  console.log(`\nadd-like controls on Clients: ${await add.count()}`);
  if (await add.count()) {
    await add.first().click();
    await settle(page);
    await show(page, 'clients: after clicking add');
  }

  // Calendar: what does clicking an empty slot open?
  await page.goto('/Scheduler#calendar');
  await settle(page);
  const slot = page.locator('table.fc-agenda-slots tr').nth(30).locator('td').first();
  const box = await slot.boundingBox();
  if (box) {
    // far right of the slot row = the last day column (Sunday), which is in the future this week
    await page.mouse.click(box.x + box.width - 40, box.y + box.height / 2);
    await settle(page);
    await show(page, 'calendar: after clicking an empty slot');
  }
});
