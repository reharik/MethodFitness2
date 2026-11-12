import { expect, type Page, type Response } from '@playwright/test';

/** A short unique suffix so each run's records are distinguishable and searchable. */
export function stamp(): string {
  return Date.now().toString(36).toUpperCase();
}

/** MM/dd/yyyy, the format the app's date pickers use. Offset is in days from today. */
export function usDate(offsetDays = 0): string {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return `${String(d.getMonth() + 1).padStart(2, '0')}/${String(d.getDate()).padStart(2, '0')}/${d.getFullYear()}`;
}

export async function settle(page: Page, ms = 400): Promise<void> {
  await page.waitForLoadState('networkidle').catch(() => {});
  await page.waitForTimeout(ms);
}

/** Opens one of the app's screens by its client-side route, e.g. "clientlist". */
export async function openRoute(page: Page, hash: string): Promise<void> {
  await page.goto(`/Scheduler#${hash}`);
  await page.evaluate((h) => { if (window.location.hash !== '#' + h) window.location.hash = h; }, hash);
  await settle(page);
}

/** Sets a knockout-bound text or date field and makes sure the binding sees the change. */
export async function setField(page: Page, scope: string, name: string, value: string): Promise<void> {
  const field = page.locator(`${scope} [name="${name}"]`);
  await field.fill(value);
  await field.dispatchEvent('change');
  await field.blur();
  // jQuery UI date pickers stay open after typing; close them so they do not cover the next control.
  await page.evaluate(() => { const $ = (window as any).jQuery; if ($ && $.datepicker) $('.hasDatepicker').datepicker('hide'); });
}

export interface SaveResult { Success: boolean; Message?: string; Errors?: { ErrorMessage: string }[]; EntityId?: number }

/**
 * Runs an action that posts a form and returns the app's JSON verdict.
 * The app answers every save with { Success, Message, Errors }.
 */
export async function submitAndRead(page: Page, action: () => Promise<void>, method: 'POST' | 'GET' = 'POST', urlPart = ''): Promise<SaveResult> {
  const isAppCall = (r: Response) =>
    r.request().method() === method && r.url().startsWith(new URL(page.url()).origin) && r.url().includes(urlPart) &&
    (r.headers()['content-type'] ?? '').includes('json');
  const [response] = await Promise.all([page.waitForResponse(isAppCall, { timeout: 45_000 }), action()]);
  return (await response.json()) as SaveResult;
}

export function expectSaved(result: SaveResult): void {
  const why = [result.Message, ...(result.Errors ?? []).map((e) => e.ErrorMessage)].filter(Boolean).join(' | ');
  expect(result.Success, `the app rejected the save: ${why || '(no message)'}`).toBe(true);
}

/** Collects script errors and failed app requests so a test can assert the screen is healthy. */
export function watchForTrouble(page: Page): { problems: string[] } {
  const problems: string[] = [];
  page.on('pageerror', (e) => problems.push(`script error: ${e.message.slice(0, 160)}`));
  page.on('response', (r) => {
    const path = r.url().replace(/^https?:\/\/[^/]+/, '');
    // A stylesheet the layout references is missing from the site; that is long-standing and harmless.
    if (r.status() >= 500 || (r.status() >= 400 && !/\.css(\?|$)/.test(path))) problems.push(`HTTP ${r.status()} ${path.split('?')[0].slice(0, 80)}`);
  });
  return { problems };
}
