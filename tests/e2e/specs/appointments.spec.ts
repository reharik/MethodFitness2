import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, expectSaved, openRoute, setField, settle, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer } from '../support/world';
import { clickSlot, dialogButton, saveAppointment } from '../support/flow';

test.describe('appointments', () => {
  test('book, change and delete an appointment', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);

    await login(page);
    const client = await createClient(page);
    const trainer = await ensureTrainer(page, [client]);

    try {
      // book it next week, so the slot is in the future
      await openRoute(page, 'calendar');
      await page.locator('#calendar .fc-button-next').click();
      await settle(page, 800);
      await clickSlot(page, 30, 0.5);
      await saveAppointment(page, trainer, client, 'created by the browser tests');
      const form = '#editModule';

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
      const cleanup = await archiveClient(page, client.lastName).catch(() => 'failed');
      console.log(`test client afterwards: ${cleanup}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }

    expect(problems, problems.join('\n')).toEqual([]);
  });
});
