import { test, expect, Page } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, cellsUnder, gridReady, openRoute, SaveResult, settle, submitAndRead, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer, TestClient } from '../support/world';
import { bookPastAppointment, payEverythingPayable, purchaseSingles, runSessionCompletion } from '../support/flow';

// Deleting a payment that was entered by mistake.
//
// A payment is opened read-only from the client's payment list and has a delete
// button. Deleting it puts the sessions it paid for back in arrears and removes the
// ones nobody has used yet. It is refused once a trainer has been paid for one of
// its sessions.
const NEEDS_ACTION = 'this site has no /Billing/SessionCompletion/Run action yet; deploy a build that includes it';

async function paymentCount(page: Page, client: TestClient): Promise<number> {
  await openRoute(page, `clientpurchaselist/${client.id}`);
  await gridReady(page, 'clientpurchaselist');
  await settle(page, 500);
  return page.locator('#clientpurchaselist .ui-jqgrid-btable tr.jqgrow').count();
}

/** Opens the client's only payment and presses delete. Returns the app's verdict. */
async function deleteOnlyPayment(page: Page, client: TestClient): Promise<SaveResult> {
  expect(await paymentCount(page, client), 'the client has exactly one payment').toBe(1);
  await page.locator('#clientpurchaselist .ui-jqgrid-btable tr.jqgrow a').first().click();
  const button = page.locator('.content-header button.delete:visible').last();
  await expect(button, 'a payment opens with a delete button').toBeVisible();
  expect(await page.locator('#save:visible').count(), 'an existing payment opens read-only, with no save button').toBe(0);
  return submitAndRead(page, () => button.click());
}

async function arrearsFlags(page: Page, trainerId: string, clientName: string): Promise<string[]> {
  await openRoute(page, `paytrainerlist/${trainerId}`);
  await gridReady(page, 'paytrainerlist');
  return (await cellsUnder(page, 'paytrainerlist', clientName, 'In Arrears')).sort();
}

test.describe('deleting a payment', () => {
  test('puts paid-for sessions back in arrears and removes unused ones', async ({ page }) => {
    test.setTimeout(300_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);          // has bought nothing
    const trainer = await ensureTrainer(page, [client]);

    try {
      await bookPastAppointment(page, trainer, client, 'Hour', 0);
      const first = await runSessionCompletion(page);
      test.skip(first === undefined, NEEDS_ACTION);
      expect(await arrearsFlags(page, trainer.id, client.lastName), 'one session owed').toEqual(['True']);

      // three hours: one pays off the session owed, two are left to use
      await purchaseSingles(page, client, 'Hour', 3);
      expect(await arrearsFlags(page, trainer.id, client.lastName), 'the payment cleared it').toEqual(['False']);

      const verdict = await deleteOnlyPayment(page, client);
      expect(verdict.Success, `the delete was refused: ${verdict.Message ?? ''}`).toBe(true);
      await expect(page.locator('#clientpurchaselist'), 'after deleting, the screen goes back to the payment list').toBeVisible();
      expect(await paymentCount(page, client), 'the payment is gone from the list').toBe(0);
      const after = await arrearsFlags(page, trainer.id, client.lastName);
      expect(after, `the session it paid for is owed again (flags were ${after.join(',')})`).toEqual(['True']);

      // if the two unused hours had survived, this appointment would use one
      await bookPastAppointment(page, trainer, client, 'Hour', 1);
      const second = await runSessionCompletion(page);
      const ours = second!.Appointments?.filter((a) => String(a.ClientId) === client.id) ?? [];
      expect(ours.map((a) => a.InArrears), 'the unused hours were removed with the payment').toEqual([true]);
    } finally {
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });

  test('is refused once a trainer has been paid for one of its sessions', async ({ page }) => {
    test.setTimeout(300_000);
    const { problems } = watchForTrouble(page);
    const notices = acceptDialogs(page);
    await login(page);
    const client = await createClient(page);
    const trainer = await ensureTrainer(page, [client]);

    try {
      await bookPastAppointment(page, trainer, client, 'Hour', 0);
      const run = await runSessionCompletion(page);
      test.skip(run === undefined, NEEDS_ACTION);
      await purchaseSingles(page, client, 'Hour', 1);
      await payEverythingPayable(page, trainer.id);
      expect((await arrearsFlags(page, trainer.id, client.lastName)).length, 'the session was paid out and left the pay list').toBe(0);

      const verdict = await deleteOnlyPayment(page, client);
      expect(verdict.Success, 'deleting a payment with a paid-out session is refused').toBe(false);
      await expect(page.locator('.noty_bar:visible').first(), 'the reason for the refusal is shown on screen').toBeVisible();
      expect(await paymentCount(page, client), 'the payment is still there').toBe(1);
    } finally {
      console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
      if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
    }
    expect(problems, problems.join('\n')).toEqual([]);
  });
});
