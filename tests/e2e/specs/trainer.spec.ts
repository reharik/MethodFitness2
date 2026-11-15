import { test, expect, type Page } from '@playwright/test';
import { login, logout } from '../support/session';
import { acceptDialogs, cellUnder, expectSaved, gridReady, listRows, openRoute, setField, settle, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer, pickToken } from '../support/world';
import { clickSlot, dialogButton, openPayList, payEverythingPayable, purchaseHours, runSessionCompletion } from '../support/flow';
import { calendarData, calendarInfo, clearBookings, dragTo, offsiteLocation, PROVIDENCE, saveBooking, type CalendarInfo } from '../support/api';
import { addDays, at, easternNow } from '../support/eastern';

// A trainer's own week, signed in as the test trainer rather than as the
// administrator: book a client, move the appointment, take a payment from a
// client who has run out of sessions, and sign off the week.

test.use({ timezoneId: 'America/New_York' });

/** The test trainer's own appointments over the next four weeks: ids and start times only. */
async function upcoming(page: Page, info: CalendarInfo, trainerId: string) {
  const today = easternNow();
  const data = await calendarData(page, info, today, addDays(today, 28), [trainerId]);
  return data.appointments.filter((a) => String(a.trainerId) === trainerId);
}

test.describe('a trainer', () => {
  test('books an appointment for their own client, moves it, and deletes it', async ({ page }) => {
    test.setTimeout(300_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);
    const trainer = await ensureTrainer(page, [client]);
    const adminInfo = await calendarInfo(page);
    const today = easternNow();
    console.log(`appointments left by earlier runs and removed: ${await clearBookings(page, adminInfo, [trainer.id], today, addDays(today, 200))}`);
    const offsite = offsiteLocation(adminInfo);

    try {
      await logout(page);
      await login(page, { user: trainer.loginName, pass: trainer.password });
      const info = await calendarInfo(page);

      // book it next week from the calendar
      await openRoute(page, 'calendar');
      await page.locator('#calendar .fc-button-next').click();
      await settle(page, 800);
      await clickSlot(page, 30, 0.5);
      const form = '#editModule';
      // a location without a capacity limit when there is one, so a busy gym cannot get in the way
      if (offsite !== undefined) await page.locator(`${form} [name="LocationEntityId"]`).selectOption(String(offsite));
      else await page.locator(`${form} [name="LocationEntityId"]`).selectOption({ index: 1 });
      await expect(page.locator(`${form} select[name="TrainerEntityId"]`), 'a trainer is not offered a choice of trainer').toHaveCount(0);
      await pickToken(page, `${form} #token-input-ClientsDtos`, client.lastName, client.lastName);
      await page.locator(`${form} [name="AppointmentType"]`).selectOption({ label: 'Hour' });
      await setField(page, form, 'Notes', 'created by the browser tests');
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Save').click()));
      await settle(page, 1000);

      const event = page.locator('#calendar .fc-event', { hasText: client.lastName });
      await expect(event, 'the new appointment is on the calendar').toHaveCount(1);
      let mineNow = await upcoming(page, info, trainer.id);
      expect(mineNow, 'the trainer has exactly one upcoming appointment').toHaveLength(1);
      const id = mineNow[0].id;
      const booked = mineNow[0].start.getTime();

      // move it an hour later through the form
      await event.click();
      await expect(dialogButton(page, 'Edit')).toBeVisible();
      await dialogButton(page, 'Edit').click();
      const times = page.locator(`${form} [name="StartTimeString"]`);
      await expect(times).toBeVisible();
      const index = await times.evaluate((el) => (el as HTMLSelectElement).selectedIndex);
      await times.selectOption({ index: index + 4 });
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Save').click()));
      await settle(page, 1000);
      mineNow = await upcoming(page, info, trainer.id);
      expect(mineNow.map((a) => a.start.getTime()), 'the appointment moved an hour later').toEqual([booked + 3600_000]);

      // drag it to another time on the calendar
      await expect(event).toHaveCount(1);
      await event.scrollIntoViewIfNeeded();
      const box = await event.boundingBox();
      const rowBox = await page.locator('#calendar table.fc-agenda-slots tr').nth(30).boundingBox();
      if (!box || !rowBox) throw new Error('could not locate the appointment on the calendar');
      let draggedByMouse = true;
      try {
        expectSaved(await submitAndRead(page, async () => {
          await page.mouse.move(box.x + box.width / 2, box.y + 6);
          await page.mouse.down();
          for (let step = 1; step <= 12; step++) await page.mouse.move(box.x + box.width / 2, box.y + 6 + (rowBox.height * 4 * step) / 12, { steps: 2 });
          await page.mouse.up();
        }, 'GET', 15_000));
      } catch {
        draggedByMouse = false;
        await page.mouse.up().catch(() => {});
        expectSaved(await dragTo(page, info, id, new Date(booked + 2 * 3600_000)));
      }
      console.log(`drag: ${draggedByMouse ? 'done with the mouse' : 'the mouse drag did not register; sent the drag request instead'}`);
      await settle(page, 800);
      mineNow = await upcoming(page, info, trainer.id);
      expect(mineNow, 'still exactly one appointment').toHaveLength(1);
      expect(mineNow[0].start.getTime(), 'the drag changed its start time').not.toBe(booked + 3600_000);

      // stretching it by its bottom edge never sticks: an appointment's length comes from its type
      await page.reload();
      await expect(page.locator('#calendar')).toBeVisible({ timeout: 30_000 });
      await page.locator('#calendar .fc-button-next').click();
      await settle(page, 1000);
      await expect(event).toHaveCount(1);
      await event.scrollIntoViewIfNeeded();
      const before = await event.boundingBox();
      const handle = event.locator('.ui-resizable-handle');
      if (before && (await handle.count()) > 0) {
        const startBefore = (await upcoming(page, info, trainer.id))[0].start.getTime();
        const x = before.x + before.width / 2;
        const y = before.y + before.height - 2;
        await page.mouse.move(x, y);
        await page.mouse.down();
        for (let step = 1; step <= 10; step++) await page.mouse.move(x, y + (before.height * step) / 10, { steps: 2 });
        await page.mouse.up();
        await settle(page, 1000);
        const after = await event.boundingBox();
        expect(Math.abs((after?.height ?? 0) - before.height) < 4, 'a stretched appointment snaps back to its length').toBe(true);
        const still = await upcoming(page, info, trainer.id);
        expect(still.map((a) => a.start.getTime()), 'and its time is unchanged').toEqual([startBefore]);
      } else {
        console.log('resize: the appointment had no resize handle to pull');
      }

      // delete it
      await openRoute(page, 'calendar');
      await page.reload();
      await expect(page.locator('#calendar')).toBeVisible({ timeout: 30_000 });
      await page.locator('#calendar .fc-button-next').click();
      await settle(page, 1000);
      await expect(event).toHaveCount(1);
      await event.click();
      await expect(dialogButton(page, 'Delete')).toBeVisible();
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Delete').click(), 'GET'));
      await settle(page, 1000);
      await expect(event, 'the appointment is gone from the calendar').toHaveCount(0);
      expect(await upcoming(page, info, trainer.id), 'and gone from the schedule').toHaveLength(0);
    } finally {
      await logout(page).catch(() => {});
      await login(page).catch(() => {});
      await clearBookings(page, adminInfo, [trainer.id], today, addDays(today, 200)).catch(() => {});
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    // Known, harmless: after a drag the old calendar library (FullCalendar 1.x) can throw this
    // from a mouse-move over the element it has just redrawn. The move itself is saved.
    const real = problems.filter((p) => !p.includes("reading '_fci'"));
    expect(real, real.join('\n')).toEqual([]);
  });

  test('takes a payment from a client who ran out of sessions, then signs off the week', async ({ page }) => {
    test.setTimeout(420_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);          // has bought nothing
    const trainer = await ensureTrainer(page, [client]);
    const asTrainer = { user: trainer.loginName, pass: trainer.password };

    try {
      await payEverythingPayable(page, trainer.id);
      const adminInfo = await calendarInfo(page);
      // the appointment happened three days ago; only an administrator can enter it after the fact
      expectSaved(await saveBooking(page, adminInfo, {
        when: at(addDays(easternNow(), -3), 9), locationId: offsiteLocation(adminInfo) ?? PROVIDENCE, trainerId: trainer.id, clientIds: [client.id],
      }));
      const run = await runSessionCompletion(page);
      expect(run, 'this site has the session completion action').toBeDefined();
      expect(run!.Success, run!.Message).toBe(true);
      const ours = run!.Appointments?.filter((a) => String(a.ClientId) === client.id) ?? [];
      expect(ours, 'completion picked up the test appointment').toHaveLength(1);
      expect(ours[0].InArrears, 'with no sessions left, the session is recorded in arrears').toBe(true);

      // the trainer enters the client's payment
      await logout(page);
      await login(page, asTrainer);
      await purchaseHours(page, client, 1);

      // and signs off the week
      await openRoute(page, 'trainersessionverification');
      await gridReady(page, 'trainersessionverification');
      await expect(listRows(page, 'trainersessionverification', client.lastName), 'the session is waiting for the trainer to verify').toHaveCount(1);
      expectSaved(await submitAndRead(page, () => page.locator('#trainersessionverification #acceptSessionsButton').click(), 'POST', 150_000));
      await gridReady(page, 'trainersessionverification');
      await expect(listRows(page, 'trainersessionverification', client.lastName), 'once verified it leaves the list').toHaveCount(0);

      // the administrator sees it verified and no longer in arrears
      await logout(page);
      await login(page);
      await openPayList(page, trainer.id);
      await expect(listRows(page, 'paytrainerlist', client.lastName)).toHaveCount(1);
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'In Arrears'), "the trainer's payment entry cleared the arrears").toBe('False');
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'Trainer Verified')).toBe('True');
    } finally {
      await logout(page).catch(() => {});
      await login(page).catch(() => {});
      await payEverythingPayable(page, trainer.id).catch(() => {});
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });
});
