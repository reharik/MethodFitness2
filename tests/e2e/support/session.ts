import { expect, type Page } from '@playwright/test';

export function credentials(): { user: string; pass: string } {
  const user = process.env.E2E_USER;
  const pass = process.env.E2E_PASS;
  if (!user || !pass) throw new Error('Set E2E_USER and E2E_PASS to a QA login.');
  return { user, pass };
}

/** Signs in as the QA admin from the environment, or as the given account. */
export async function login(page: Page, account?: { user: string; pass: string }): Promise<void> {
  const { user, pass } = account ?? credentials();
  await page.goto('/');
  await page.locator('input[name="UserName"]').fill(user);
  await page.locator('input[name="Password"]').fill(pass);
  await page.locator('#save, .kyt_submitButton').first().click();
  await expect(page.locator('body#signin')).toHaveCount(0, { timeout: 60_000 });
  await expect(page.locator('#left-navigation')).toBeVisible({ timeout: 60_000 });
}

export async function logout(page: Page): Promise<void> {
  await page.goto('/Login/Logout');
  await expect(page.locator('input[name="UserName"]')).toBeVisible({ timeout: 60_000 });
}
