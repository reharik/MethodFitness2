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
  const current = page.url().includes('/Scheduler') ? new URL(page.url()).hash : null;
  if (current === null) await page.goto(`/Scheduler#${hash}`);
  else if (current !== `#${hash}`) await page.evaluate((h) => { window.location.hash = h; }, hash);
  await expect(page.locator(`#${hash.split('/')[0]}`)).toBeVisible({ timeout: 30_000 });
  await settle(page);
}

/** Waits until the list on screen has finished loading its rows. */
export async function gridReady(page: Page): Promise<void> {
  await expect(page.locator('.ui-jqgrid-btable').first()).toBeVisible();
  await expect(page.locator('.ui-jqgrid .loading:visible')).toHaveCount(0);
  await settle(page, 300);
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
  const seen: string[] = [];
  const isVerdict = async (r: Response) => {
    if (!r.url().startsWith(origin)) return false;
    const path = r.url().slice(origin.length).split('?')[0].slice(0, 70);
    if (!/\.(js|css|png|jpg|gif|ico)$/i.test(path)) seen.push(`${r.request().method()} ${r.status()} ${path}`);
    if (r.request().method() !== method) return false;
    try {
      const body = JSON.parse(await r.text());
      if (body && typeof body === 'object' && 'Success' in body) { verdict = body as SaveResult; return true; }
    } catch { /* not JSON: a template, a grid page, a script */ }
    return false;
  };
  try {
    await Promise.all([page.waitForResponse(isVerdict, { timeout: 30_000 }), action()]);
  } catch (e) {
    // Paths only, no bodies: enough to see what the app did instead of answering.
    throw new Error(`the app gave no ${method} verdict. Requests it made meanwhile: ${seen.slice(-8).join(' ; ') || 'none'}`);
  }
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
  await gridReady(page);
  const search = page.locator(`#${screen} input.search`);
  await search.click();
  await search.fill(text);
  await search.press('Enter');
  await gridReady(page);
}
