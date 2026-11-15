import { expect, type Page } from '@playwright/test';
import { expectSaved, gridReady, openRoute, setField, settle, submitAndRead, usDate } from './app';
import { pickToken, type TestClient, type TestTrainer } from './world';

export const dialogButton = (page: Page, name: string) =>
  page.locator('.ui-dialog:visible .ui-dialog-buttonpane button', { hasText: new RegExp(`^${name}$`) });

/** Clicks a calendar slot on the week being shown and waits for the appointment form. */
export async function clickSlot(page: Page, slotRow: number, dayFraction: number): Promise<void> {
  const row = page.locator('#calendar table.fc-agenda-slots tr').nth(slotRow);
  await row.scrollIntoViewIfNeeded();
  const box = await row.boundingBox();
  if (!box) throw new Error('calendar slot row not found');
  await page.mouse.click(box.x + box.width * dayFraction, box.y + box.height / 2);
  // the location field: a trainer's form has no trainer field, an administrator's does
  await expect(page.locator('#editModule [name="LocationEntityId"]')).toBeVisible();
  await settle(page);
}

/** Fills in and saves the appointment form that clickSlot opened. */
export async function saveAppointment(page: Page, trainer: TestTrainer, client: TestClient, notes: string, type: SessionType = 'Hour', partners: TestClient[] = []): Promise<void> {
  const form = '#editModule';
  await page.locator(`${form} [name="LocationEntityId"]`).selectOption({ index: 1 });
  await page.locator(`${form} [name="TrainerEntityId"]`).selectOption(trainer.id);
  await pickToken(page, `${form} #token-input-ClientsDtos`, client.lastName.slice(0, 12), client.lastName);
  // the form only allows Pair once a second client is on the appointment, and forces it then
  for (const partner of partners) await pickToken(page, `${form} #token-input-ClientsDtos`, partner.lastName.slice(0, 12), partner.lastName);
  await page.locator(`${form} [name="AppointmentType"]`).selectOption({ label: type });
  await setField(page, form, 'Notes', notes);
  expectSaved(await submitAndRead(page, () => dialogButton(page, 'Save').click()));
  await settle(page, 1000);
}

/**
 * Books an hour-long appointment in the past, which only an administrator may do.
 *
 * Past weeks on the calendar are crowded with real appointments, and clicking one
 * opens it instead of the booking form. So this opens the form from an empty slot
 * next week and then moves the date back. The day and time vary with the client
 * so repeated runs do not land on the same slot for the test trainer.
 */
export async function bookPastAppointment(page: Page, trainer: TestTrainer, client: TestClient, type: SessionType = 'Hour', nth = 0, partners: TestClient[] = []): Promise<void> {
  // nth spreads several appointments for one client over different days
  const seed = Number(client.id) + nth * 5;
  await openRoute(page, 'calendar');
  await page.locator('#calendar .fc-button-next').click();
  await settle(page, 800);
  await clickSlot(page, 30, 0.5);

  const form = '#editModule';
  await setField(page, form, 'Date', usDate(-(2 + (seed % 26))));
  const times = page.locator(`${form} [name="StartTimeString"] option`);
  const count = await times.count();
  if (count > 4) await page.locator(`${form} [name="StartTimeString"]`).selectOption({ index: 2 + (seed % (count - 4)) });
  await saveAppointment(page, trainer, client, 'created by the browser tests', type, partners);
}

export interface CompletionRun { Success: boolean; Message?: string; Appointments?: { ClientId: number; InArrears: boolean }[] }

/**
 * Runs session completion through the app's admin action. Returns undefined when
 * the site under test does not have the action yet (builds before it was added).
 */
export async function runSessionCompletion(page: Page): Promise<CompletionRun | undefined> {
  const response = await page.request.post('/Billing/SessionCompletion/Run', { failOnStatusCode: false, timeout: 120_000 });
  try {
    const body = JSON.parse(await response.text());
    if (body && typeof body === 'object' && 'Success' in body) return body as CompletionRun;
  } catch { /* an error page rather than the action's JSON */ }
  return undefined;
}

/** Records a purchase of full-hour sessions for a client. */
/** The three kinds of session, as the appointment form labels them, and the purchase-form field that buys singles of each. */
export type SessionType = 'Hour' | 'Half Hour' | 'Pair';
export const SINGLE_FIELD: Record<SessionType, string> = { 'Hour': 'FullHour', 'Half Hour': 'HalfHour', 'Pair': 'Pair' };

/** Buys `count` single sessions of one kind for a client. */
export async function purchaseSingles(page: Page, client: TestClient, type: SessionType, count: number): Promise<void> {
  const field = SINGLE_FIELD[type];
  await openRoute(page, `clientpurchaselist/${client.id}`);
  await gridReady(page, 'clientpurchaselist');
  await page.locator('#clientpurchaselist #new').click();
  await expect(page.locator(`[name="${field}"]:visible`)).toBeVisible();
  const form = '#' + new URL(page.url()).hash.slice(1).split('/')[0];
  await setField(page, form, field, String(count));
  await setField(page, form, 'Notes', 'created by the browser tests');
  expectSaved(await submitAndRead(page, () => page.locator(`${form} #save`).click()));
  await gridReady(page, 'clientpurchaselist');
}

export const purchaseHours = (page: Page, client: TestClient, count: number) => purchaseSingles(page, client, 'Hour', count);

/**
 * Opens a trainer's pay screen from a fresh page load. The screen builds its
 * date filter once per page, so revisiting it without a reload can leave it out.
 */
export async function openPayList(page: Page, trainerId: string): Promise<void> {
  await page.goto(`/Scheduler?t=${Date.now()}#paytrainerlist/${trainerId}`);
  await expect(page.locator('#paytrainerlist')).toBeVisible({ timeout: 30_000 });
  await gridReady(page, 'paytrainerlist');
}

/** Applies the pay screen's End Date filter. */
export async function filterPayList(page: Page, endDate: string): Promise<void> {
  await setField(page, '#paytrainerlist', 'EndDate', endDate);
  await page.locator('#paytrainerlist #search').click();
  await gridReady(page, 'paytrainerlist');
  await settle(page, 500);
}

/** Presses the pay button and confirms. The rows to pay must already be ticked. */
export async function payTicked(page: Page): Promise<void> {
  await expect(page.locator('#payTrainerButton'), 'the pay button shows once something payable is ticked').toBeVisible();
  await page.locator('#payTrainerButton').click();
  await expect(dialogButton(page, 'Ok')).toBeVisible();
  expectSaved(await submitAndRead(page, () => dialogButton(page, 'Ok').click()));
  await settle(page, 1000);
  await gridReady(page, 'paytrainerlist');
}

const SELECT_ALL = '#paytrainerlist #cb_trainerPayment';
export const selectAllBox = (page: Page) => page.locator(SELECT_ALL);

/**
 * Housekeeping for the test trainer: pays everything on the pay screen that can
 * be paid, so sessions left by earlier runs do not pile up. Returns how many
 * rows were on the list beforehand.
 */
export async function payEverythingPayable(page: Page, trainerId: string): Promise<number> {
  await openPayList(page, trainerId);
  const rows = await page.locator('#paytrainerlist .ui-jqgrid-btable tr.jqgrow').count();
  const tickable = await page.locator('#paytrainerlist .ui-jqgrid-btable tr.jqgrow input.cbox').count();
  if (tickable === 0) return rows;
  await selectAllBox(page).check();
  await settle(page, 300);
  if (await page.locator('#payTrainerButton').isVisible()) await payTicked(page);
  return rows;
}
