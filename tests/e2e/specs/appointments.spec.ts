import { test, expect, type Page } from '@playwright/test';
import { login } from '../support/session';
import { expectSaved, openRoute, setField, settle, submitAndRead, watchForTrouble } from '../support/app';
import { createClient, deleteClient, ensureTrainer, pickToken } from '../support/world';

/** Clicks an empty slot around midday on the calendar's current week and waits for the appointment form. */
async function clickSlot(page: Page, dayFraction: number) {
  const row = page.locator('#calendar table.fc-agenda-slots tr').nth(30);
  await row.scrollIntoViewIfNeeded();
  const box = await row.boundingBox();
  if (!box) throw new Error('calendar slot row not found');
  await page.mouse.click(box.x + box.width * dayFraction, box.y + box.height / 2);
  await expect(page.locator('#editModule [name="TrainerEntityId"]')).toBeVisible();
  await settle(page);
}

const dialogButton = (page: Page, name: string) =>
  page.locator('.ui-dialog:visible .ui-dialog-buttonpane button', { hasText: new RegExp(`^${name}$`) });

test.describe('appointments', () => {
  test('book, change and delete an appointment', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    const notices: string[] = [];
    page.on('dialog', (d) => { notices.push(d.message().slice(0, 100)); d.accept().catch(() => {}); });

    await login(page);
    const client = await createClient(page);
    const trainer = await ensureTrainer(page, [client]);

    try {
      // book it next week, so the slot is in the future
      await openRoute(page, 'calendar');
      await page.locator('#calendar .fc-button-next').click();
      await settle(page, 800);
      await clickSlot(page, 0.5);

      const form = '#editModule';
      await page.locator(`${form} [name="LocationEntityId"]`).selectOption({ index: 1 });
      await page.locator(`${form} [name="TrainerEntityId"]`).selectOption(trainer.id);
      await pickToken(page, `${form} #token-input-ClientsDtos`, client.lastName.slice(0, 12), client.lastName);
      await page.locator(`${form} [name="AppointmentType"]`).selectOption({ label: 'Hour' });
      await setField(page, form, 'Notes', 'created by the browser tests');
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Save').click()));
      await settle(page, 1000);

      // it shows on the calendar, titled with the client's name
      const event = page.locator('#calendar .fc-event', { hasText: client.lastName });
      await expect(event, 'the new appointment is on the calendar').toHaveCount(1);

      // open it and change the notes
      await event.click();
      await expect(dialogButton(page, 'Edit')).toBeVisible();
      await dialogButton(page, 'Edit').click();
      await expect(page.locator(`${form} [name="Notes"]`)).toHaveValue('created by the browser tests');
      await setField(page, form, 'Notes', 'changed by the browser tests');
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Save').click()));
      await settle(page, 1000);

      // delete it
      await expect(event).toHaveCount(1);
      await event.click();
      await expect(dialogButton(page, 'Delete')).toBeVisible();
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Delete').click(), 'GET'));
      await settle(page, 1000);
      await expect(event, 'the appointment is gone from the calendar').toHaveCount(0);
    } finally {
      const cleanup = await deleteClient(page, client.lastName).catch(() => undefined);
      console.log(`test client removed afterwards: ${cleanup?.Success ?? false}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
