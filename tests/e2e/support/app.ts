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
 * Runs an action that calls the app and returns the app's JSON verdict.
 * The app answers every save and delete with { Success, Message, Errors },
 * though not always with a JSON content type, so the body is what is checked.
 */
export async function submitAndRead(page: Page, action: () => Promise<void>, method: 'POST' | 'GET' = 'POST'): Promise<SaveResult> {
  const origin = new URL(page.url()).origin;
  let verdict: SaveResult | undefined;
  const isVerdict = async (r: Response) => {
    if (r.request().method() !== method || !r.url().startsWith(origin)) return false;
    try {
      const body = JSON.parse(await r.text());
      if (body && typeof body === 'object' && 'Success' in body) { verdict = body as SaveResult; return true; }
    } catch { /* not JSON: a template, a grid page, a script */ }
    return false;
  };
  await Promise.all([page.waitForResponse(isVerdict, { timeout: 45_000 }), action()]);
  return verdict!;
}

export function expectSaved(result: SaveResult): void {
  const why = [result.Message, ...(result.Errors ?? []).map((e) => e.ErrorMessage)].filter(Boolean).join(' | ');
  expect(result.Success, `the app rejected the save: ${why || '(no message)'}`).toBe(true);
}

/** Collects script errors and failed app requests so a test can assert the screen is healthy. */
export function watchForTrouble(page: Page): { problems: string[] } {
  const problems: string[] = [];
  page.on('pageerror', (e) => {
    // Known, harmless: leaving a list's search box with text in it throws this
    // (mf.gridSearch.js calls submitSearch on the wrong object in its blur handler).
    if (e.message.includes('this.submitSearch is not a function')) return;
    problems.push(`script error: ${e.message.slice(0, 160)}`);
  });
  page.on('response', (r) => {
    const path = r.url().replace(/^https?:\/\/[^/]+/, '');
    // A stylesheet the layout references is missing from the site; that is long-standing and harmless.
    if (r.status() >= 500 || (r.status() >= 400 && !/\.css(\?|$)/.test(path))) problems.push(`HTTP ${r.status()} ${path.split('?')[0].slice(0, 80)}`);
  });
  return { problems };
}

/** Types into a list screen's search box and applies it (the box searches on Enter). */
export async function searchList(page: Page, screen: string, text: string): Promise<void> {
  const search = page.locator(`#${screen} input.search`);
  await search.click();
  await search.fill(text);
  await search.press('Enter');
  await settle(page, 800);
}

/** Deletes every row a list search finds. Only ever called with this suite's own "E2E" prefix. */
export async function deleteFromList(page: Page, route: string, searchText: string): Promise<number> {
  if (!searchText.startsWith('E2E')) throw new Error('refusing to bulk-delete anything not created by these tests');
  await openRoute(page, route);
  await searchList(page, route, searchText);
  const rows = page.locator('#gridContainer tr.jqgrow', { hasText: searchText });
  const count = await rows.count();
  if (count === 0) return 0;
  for (let i = 0; i < count; i++) await rows.nth(i).locator('input.cbox').check();
  page.once('dialog', (d) => d.accept());
  expectSaved(await submitAndRead(page, () => page.locator(`#${route} button.delete`).click(), 'GET'));
  await settle(page, 800);
  return count;
}
