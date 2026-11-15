import { test, expect } from '@playwright/test';
import { login, logout } from '../support/session';
import { acceptDialogs, cellUnder, expectSaved, gridReady, listRows, openRoute, settle, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer, type TestClient } from '../support/world';
import { filterPayList, openPayList, payEverythingPayable, payTicked, purchaseHours, runSessionCompletion, selectAllBox } from '../support/flow';
import { calendarInfo, offsiteLocation, PROVIDENCE, saveBooking } from '../support/api';
import { addDays, at, easternNow, lastSunday, usDay } from '../support/eastern';

// The weekly pay run, the way the administrator does it: open a trainer's pay
// screen, look for sessions in arrears, set the end date to last Sunday, select
// all, leave out anything the trainer has not verified, and pay.

test.use({ timezoneId: 'America/New_York' });

test('the pay run: up to last Sunday, verified sessions are paid; unverified and in-arrears sessions are left', async ({ page }) => {
  test.setTimeout(540_000);
  const { problems } = watchForTrouble(page);
  const notices = acceptDialogs(page);
  await login(page);

  const today = easternNow();
  const sunday = lastSunday(today);
  // a session after last Sunday only fits when today is Tuesday or later
  const roomAfterSunday = today.getUTCDay() >= 2;

  const verified = await createClient(page);
  await purchaseHours(page, verified, 2);
  const unverified = await createClient(page);
  await purchaseHours(page, unverified, 2);
  const arrears = await createClient(page);            // buys nothing
  let thisWeek: TestClient | undefined;
  if (roomAfterSunday) { thisWeek = await createClient(page); await purchaseHours(page, thisWeek, 2); }
  const clients = [verified, unverified, arrears, ...(thisWeek ? [thisWeek] : [])];
  const trainer = await ensureTrainer(page, clients);
  const asTrainer = { user: trainer.loginName, pass: trainer.password };

  try {
    console.log(`rows on the test trainer's pay screen from earlier runs: ${await payEverythingPayable(page, trainer.id)}`);
    const info = await calendarInfo(page);
    const where = offsiteLocation(info) ?? PROVIDENCE;
    const book = async (client: TestClient, when: Date) =>
      expectSaved(await saveBooking(page, info, { when, locationId: where, trainerId: trainer.id, clientIds: [client.id] }));
    const complete = async (expected: TestClient[]) => {
      const run = await runSessionCompletion(page);
      expect(run, 'this site has the session completion action').toBeDefined();
      expect(run!.Success, run!.Message).toBe(true);
      for (const c of expected) {
        expect(run!.Appointments?.filter((a) => String(a.ClientId) === c.id) ?? [], 'completion picked up a test appointment').toHaveLength(1);
      }
    };

    // sessions the trainer will sign off: one before last Sunday, one after
    await book(verified, at(addDays(sunday, -3), 10));
    if (thisWeek) await book(thisWeek, at(addDays(today, -1), 6));
    await complete(thisWeek ? [verified, thisWeek] : [verified]);

    await logout(page);
    await login(page, asTrainer);
    await openRoute(page, 'trainersessionverification');
    await gridReady(page, 'trainersessionverification');
    await expect(listRows(page, 'trainersessionverification', verified.lastName)).toHaveCount(1);
    // Signing off also emails the admin address, which can take a while to give up on QA.
    expectSaved(await submitAndRead(page, () => page.locator('#trainersessionverification #acceptSessionsButton').click(), 'POST', 150_000));
    await logout(page);
    await login(page);

    // sessions the trainer has not signed off: one covered by a purchase, one in arrears
    await book(unverified, at(addDays(sunday, -4), 10));
    await book(arrears, at(addDays(sunday, -5), 10));
    await complete([unverified, arrears]);

    // the pay screen, filtered to last Sunday
    await openPayList(page, trainer.id);
    await filterPayList(page, usDay(sunday));
    const row = (c: TestClient) => listRows(page, 'paytrainerlist', c.lastName);
    await expect(row(verified), 'the verified session is listed').toHaveCount(1);
    await expect(row(unverified), 'the unverified session is listed').toHaveCount(1);
    await expect(row(arrears), 'the in-arrears session is listed').toHaveCount(1);
    if (thisWeek) await expect(row(thisWeek), "this week's session is outside the end date").toHaveCount(0);
    expect(await cellUnder(page, 'paytrainerlist', verified.lastName, 'Trainer Verified')).toBe('True');
    expect(await cellUnder(page, 'paytrainerlist', unverified.lastName, 'Trainer Verified')).toBe('False');
    expect(await cellUnder(page, 'paytrainerlist', arrears.lastName, 'In Arrears')).toBe('True');
    await expect(row(arrears).locator('input.cbox'), 'an in-arrears session has no checkbox').toHaveCount(0);

    // select all, then leave the unverified one out
    await selectAllBox(page).check();
    await settle(page, 300);
    await expect(row(verified).locator('input.cbox')).toBeChecked();
    await expect(row(unverified).locator('input.cbox'), 'select all ticks an unverified session too; the screen allows it').toBeChecked();
    await row(unverified).locator('input.cbox').uncheck();
    await settle(page, 300);
    await payTicked(page);

    await expect(row(verified), 'the verified session was paid and left the list').toHaveCount(0);
    await expect(row(unverified), 'the unverified session was left unpaid').toHaveCount(1);
    await expect(row(arrears), 'the in-arrears session was left unpaid').toHaveCount(1);

    // moving the end date to today brings this week's session back, still unpaid
    if (thisWeek) {
      await filterPayList(page, usDay(today));
      await expect(row(thisWeek), "this week's session is still waiting to be paid").toHaveCount(1);
    }
  } finally {
    await logout(page).catch(() => {});
    await login(page).catch(() => {});
    // leave the test trainer's pay screen tidy for the next run
    await purchaseHours(page, arrears, 1).catch(() => {});
    await payEverythingPayable(page, trainer.id).catch(() => {});
    for (const c of clients) console.log(`test client afterwards: ${await archiveClient(page, c.lastName).catch(() => 'failed')}`);
    if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
  }
  expect(problems, problems.join('\n')).toEqual([]);
});
