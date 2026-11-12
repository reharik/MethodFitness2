import { test } from '@playwright/test';
import { login } from '../support/session';
import { outline } from '../support/outline';

test('outline the main screens', async ({ page }) => {
  page.on('pageerror', (e) => console.log(`[pageerror] ${e.message.slice(0, 200)}`));

  await page.goto('/');
  console.log(`\n===== login page: ${page.url()}`);
  console.log(await outline(page));

  await login(page);
  await page.waitForLoadState('networkidle');
  console.log(`\n===== after login: ${page.url()}`);
  console.log(await outline(page));

  // Top-level menu entries, by position rather than text, so the log shows what they are.
  const menuLinks = await page.evaluate(() =>
    Array.from(document.querySelectorAll('a'))
      .filter((a) => !a.closest('table, .fc-event'))
      .map((a) => ({ text: (a.textContent ?? '').trim().slice(0, 40), href: a.getAttribute('href') ?? '' }))
      .filter((a) => a.href.includes('#') && a.href.length > 1),
  );
  console.log('\n===== hash links found');
  for (const l of menuLinks) console.log(`  "${l.text}" -> ${l.href}`);

  const seen = new Set<string>();
  for (const l of menuLinks) {
    const hash = l.href.slice(l.href.indexOf('#'));
    if (seen.has(hash) || hash === '#') continue;
    seen.add(hash);
    await page.evaluate((h) => { window.location.hash = h; }, hash);
    await page.waitForLoadState('networkidle').catch(() => {});
    await page.waitForTimeout(1500);
    console.log(`\n===== screen "${l.text}" (${hash})`);
    console.log(await outline(page));
  }
});
