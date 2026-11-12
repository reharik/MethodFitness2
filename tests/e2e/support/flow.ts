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
  await expect(page.locator('#editModule [name="TrainerEntityId"]')).toBeVisible();
  await settle(page);
}

/** Fills in and saves the appointment form that clickSlot opened. */
export async function saveAppointment(page: Page, trainer: TestTrainer, client: TestClient, notes: string): Promise<void> {
  const form = '#editModule';
  await page.locator(`${form} [name="LocationEntityId"]`).selectOption({ index: 1 });
  await page.locator(`${form} [name="TrainerEntityId"]`).selectOption(trainer.id);
  await pickToken(page, `${form} #token-input-ClientsDtos`, client.lastName.slice(0, 12), client.lastName);
  await page.locator(`${form} [name="AppointmentType"]`).selectOption({ label: 'Hour' });
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
export async function bookPastAppointment(page: Page, trainer: TestTrainer, client: TestClient): Promise<void> {
  const seed = Number(client.id);
  await openRoute(page, 'calendar');
  await page.locator('#calendar .fc-button-next').click();
  await settle(page, 800);
  await clickSlot(page, 30, 0.5);

  const form = '#editModule';
  await setField(page, form, 'Date', usDate(-(2 + (seed % 26))));
  const times = page.locator(`${form} [name="StartTimeString"] option`);
  const count = await times.count();
  if (count > 4) await page.locator(`${form} [name="StartTimeString"]`).selectOption({ index: 2 + (seed % (count - 4)) });
  await saveAppointment(page, trainer, client, 'created by the browser tests');
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
export async function purchaseHours(page: Page, client: TestClient, count: number): Promise<void> {
  await openRoute(page, `clientpurchaselist/${client.id}`);
  await gridReady(page, 'clientpurchaselist');
  await page.locator('#clientpurchaselist #new').click();
  await expect(page.locator('[name="FullHour"]:visible')).toBeVisible();
  const form = '#' + new URL(page.url()).hash.slice(1).split('/')[0];
  await setField(page, form, 'FullHour', String(count));
  await setField(page, form, 'Notes', 'created by the browser tests');
  expectSaved(await submitAndRead(page, () => page.locator(`${form} #save`).click()));
  await gridReady(page, 'clientpurchaselist');
}
