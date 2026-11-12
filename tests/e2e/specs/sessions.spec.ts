import { test, expect } from '@playwright/test';
import { login, logout } from '../support/session';
import { acceptDialogs, cellUnder, expectSaved, gridReady, listRows, openRoute, settle, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer } from '../support/world';
import { bookPastAppointment, dialogButton, purchaseHours, runSessionCompletion } from '../support/flow';

// The money path: an appointment ends, session completion turns it into a used
// session, the trainer signs off, and the administrator pays the trainer.
//
// In production a scheduled Lambda does the completion step. On QA nothing is
// scheduled, so these tests trigger it through the app's admin-only action. A
// site built before that action existed cannot run them, and they are skipped.
const NEEDS_ACTION = 'this site has no /Billing/SessionCompletion/Run action yet; deploy a build that includes it';

test.describe('sessions', () => {
  test('an appointment with no session to cover it goes into arrears, and a purchase clears it', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);          // has bought nothing
    const trainer = await ensureTrainer(page, [client]);

    try {
      await bookPastAppointment(page, trainer, client);

      const run = await runSessionCompletion(page);
      test.skip(run === undefined, NEEDS_ACTION);
      expect(run!.Success, run!.Message).toBe(true);
      const ours = run!.Appointments?.filter((a) => String(a.ClientId) === client.id) ?? [];
      expect(ours, 'completion picked up the test appointment').toHaveLength(1);
      expect(ours[0].InArrears, 'with no purchased session, it is recorded in arrears').toBe(true);

      // the trainer's unpaid sessions show it, flagged in arrears
      await openRoute(page, `paytrainerlist/${trainer.id}`);
      await gridReady(page, 'paytrainerlist');
      await expect(listRows(page, 'paytrainerlist', client.lastName)).toHaveCount(1);
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'In Arrears')).toBe('True');

      // buying a session pays off the arrears
      await purchaseHours(page, client, 1);
      await openRoute(page, `paytrainerlist/${trainer.id}`);
      await gridReady(page, 'paytrainerlist');
      await expect(listRows(page, 'paytrainerlist', client.lastName)).toHaveCount(1);
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'In Arrears')).toBe('False');
    } finally {
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });

  test('a completed session is verified by its trainer and then paid by the administrator', async ({ page }) => {
    test.setTimeout(300_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);
    await purchaseHours(page, client, 2);
    const trainer = await ensureTrainer(page, [client]);

    try {
      await bookPastAppointment(page, trainer, client);

      const run = await runSessionCompletion(page);
      test.skip(run === undefined, NEEDS_ACTION);
      expect(run!.Success, run!.Message).toBe(true);
      const ours = run!.Appointments?.filter((a) => String(a.ClientId) === client.id) ?? [];
      expect(ours, 'completion picked up the test appointment').toHaveLength(1);
      expect(ours[0].InArrears, 'a purchased session covers it').toBe(false);

      // the trainer signs off on it
      await logout(page);
      await login(page, { user: trainer.loginName, pass: trainer.password });
      await openRoute(page, 'trainersessionverification');
      await gridReady(page, 'trainersessionverification');
      await expect(listRows(page, 'trainersessionverification', client.lastName)).toHaveCount(1);
      // Signing off also emails the admin address, which can take a while to give up on QA.
      expectSaved(await submitAndRead(page, () => page.locator('#trainersessionverification #acceptSessionsButton').click(), 'POST', 150_000));
      await gridReady(page, 'trainersessionverification');
      await expect(listRows(page, 'trainersessionverification', client.lastName)).toHaveCount(0);

      // the administrator pays the trainer for it
      await logout(page);
      await login(page);
      await openRoute(page, `paytrainerlist/${trainer.id}`);
      await gridReady(page, 'paytrainerlist');
      const row = listRows(page, 'paytrainerlist', client.lastName);
      await expect(row).toHaveCount(1);
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'Trainer Verified')).toBe('True');
      await row.locator('input.cbox').check();
      await expect(page.locator('#payTrainerButton')).toBeVisible();
      await page.locator('#payTrainerButton').click();
      await expect(dialogButton(page, 'Ok')).toBeVisible();
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Ok').click()));
      await settle(page, 1000);
      await gridReady(page, 'paytrainerlist');
      await expect(row, 'a paid session leaves the unpaid list').toHaveCount(0);
    } finally {
      // whoever is signed in at this point, finish as the administrator
      await logout(page).catch(() => {});
      await login(page).catch(() => {});
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });

  test("the trainer's session report lists an unverified, unpaid session and drops it once it is paid", async ({ page }) => {
    test.setTimeout(300_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);
    await purchaseHours(page, client, 2);
    const trainer = await ensureTrainer(page, [client]);
    const asTrainer = { user: trainer.loginName, pass: trainer.password };

    try {
      await bookPastAppointment(page, trainer, client);
      const run = await runSessionCompletion(page);
      test.skip(run === undefined, NEEDS_ACTION);
      expect(run!.Success, run!.Message).toBe(true);

      // not verified, not paid: the trainer sees it on the session report
      await logout(page);
      await login(page, asTrainer);
      await openRoute(page, 'trainersessionview');
      await gridReady(page, 'trainersessionview');
      await expect(listRows(page, 'trainersessionview', client.lastName), 'an unverified, unpaid session is on the report').toHaveCount(1);

      // the administrator pays it without the trainer having verified it
      await logout(page);
      await login(page);
      await openRoute(page, `paytrainerlist/${trainer.id}`);
      await gridReady(page, 'paytrainerlist');
      const row = listRows(page, 'paytrainerlist', client.lastName);
      await expect(row).toHaveCount(1);
      expect(await cellUnder(page, 'paytrainerlist', client.lastName, 'Trainer Verified')).toBe('False');
      await row.locator('input.cbox').check();
      await expect(page.locator('#payTrainerButton')).toBeVisible();
      await page.locator('#payTrainerButton').click();
      await expect(dialogButton(page, 'Ok')).toBeVisible();
      expectSaved(await submitAndRead(page, () => dialogButton(page, 'Ok').click()));
      await settle(page, 1000);

      // paid but never verified: it must not come back onto the trainer's lists.
      // (This is the condition that was missing from the repo in November 2026.)
      await logout(page);
      await login(page, asTrainer);
      await openRoute(page, 'trainersessionview');
      await gridReady(page, 'trainersessionview');
      await expect(listRows(page, 'trainersessionview', client.lastName), 'a paid session is off the session report').toHaveCount(0);
      await openRoute(page, 'trainersessionverification');
      await gridReady(page, 'trainersessionverification');
      await expect(listRows(page, 'trainersessionverification', client.lastName), 'a paid session is not offered for verification').toHaveCount(0);
    } finally {
      await logout(page).catch(() => {});
      await login(page).catch(() => {});
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });
});
