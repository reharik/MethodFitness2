import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, cellsUnder, gridReady, openRoute, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer } from '../support/world';
import { bookPastAppointment, payEverythingPayable, purchaseSingles, runSessionCompletion, SessionType } from '../support/flow';

// A purchase pays off only as many sessions in arrears as were bought.
//
// Until November 2026 buying one single half-hour or pair session cleared every
// session of that kind the client had in arrears. Hours were always counted.
const NEEDS_ACTION = 'this site has no /Billing/SessionCompletion/Run action yet; deploy a build that includes it';

async function arrearsFlags(page: import('@playwright/test').Page, trainerId: string, clientName: string): Promise<string[]> {
  await openRoute(page, `paytrainerlist/${trainerId}`);
  await gridReady(page, 'paytrainerlist');
  return (await cellsUnder(page, 'paytrainerlist', clientName, 'In Arrears')).sort();
}

test.describe('arrears', () => {
  for (const type of ['Half Hour', 'Pair', 'Hour'] as SessionType[]) {
    test(`three ${type} sessions in arrears: buying one clears one, buying two more clears the rest`, async ({ page }) => {
      test.setTimeout(300_000);
      const { problems } = watchForTrouble(page);
      const notices = acceptDialogs(page);
      await login(page);
      const client = await createClient(page);          // has bought nothing
      const trainer = await ensureTrainer(page, [client]);

      try {
        for (let n = 0; n < 3; n++) await bookPastAppointment(page, trainer, client, type, n);

        const run = await runSessionCompletion(page);
        test.skip(run === undefined, NEEDS_ACTION);
        expect(run!.Success, run!.Message).toBe(true);
        const ours = run!.Appointments?.filter((a) => String(a.ClientId) === client.id) ?? [];
        expect(ours.map((a) => a.InArrears), 'all three appointments completed in arrears').toEqual([true, true, true]);
        expect(await arrearsFlags(page, trainer.id, client.lastName), 'three sessions owed').toEqual(['True', 'True', 'True']);

        await purchaseSingles(page, client, type, 1);
        expect(await arrearsFlags(page, trainer.id, client.lastName), 'buying one clears exactly one').toEqual(['False', 'True', 'True']);

        await purchaseSingles(page, client, type, 2);
        expect(await arrearsFlags(page, trainer.id, client.lastName), 'buying two more clears the rest').toEqual(['False', 'False', 'False']);
      } finally {
        await payEverythingPayable(page, trainer.id).catch(() => 0);
        console.log(`test client afterwards: ${await archiveClient(page, client.lastName).catch(() => 'failed')}`);
        if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
      }
      expect(problems, problems.join('\n')).toEqual([]);
    });
  }
});
