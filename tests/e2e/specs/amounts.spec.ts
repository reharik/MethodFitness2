import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, amountsUnder, expectSaved, gridReady, listRows, money, openRoute, settle, shownAmount, watchForTrouble } from '../support/app';
import { archiveClient, createClient, ensureTrainer, type TestClient } from '../support/world';
import { fillPurchase, openPayList, openPurchaseForm, payEverythingPayable, payTicked, runSessionCompletion, savePurchase } from '../support/flow';
import { calendarInfo, offsiteLocation, PROVIDENCE, saveBooking } from '../support/api';
import { addDays, at, easternNow } from '../support/eastern';

// The money, read off the screens the way the administrator sees it:
//   - the purchase form prices each line and totals them, and the saved payment keeps that total
//   - a ten-pack makes ten sessions, each worth a tenth of the pack price
//   - the pay screen prices each session at what the client paid for it and gives the
//     trainer their percentage of that
//   - the amount paid to the trainer is the sum of the ticked sessions, and it is what
//     lands on the trainer's payment list
//
// The test clients are given their own prices, so the expected amounts are written
// here in full and do not move when the studio changes its price list.

test.use({ timezoneId: 'America/New_York' });

const RATES = { FullHour: 83, HalfHour: 47, FullHourTenPack: 710 };
const cents = (n: number) => Math.round(n * 100) / 100;

test('amounts: purchase totals, ten-pack session price, trainer pay and the payout total', async ({ page }) => {
  test.setTimeout(480_000);
  const { problems } = watchForTrouble(page);
  const notices = acceptDialogs(page);
  await login(page);

  const singles = await createClient(page, 'E2EClient', RATES);   // buys two hours and a half hour
  const packs = await createClient(page, 'E2EClient', RATES);     // buys one ten-pack of hours
  const clients = [singles, packs];
  const trainer = await ensureTrainer(page, clients);

  try {
    // ---- buying: the form's arithmetic, and what is stored
    let form = await openPurchaseForm(page, singles);
    await fillPurchase(page, form, { FullHour: 2, HalfHour: 1 });
    expect(await shownAmount(page, 'FullHourPrice'), 'two hours at 83').toBe(166);
    expect(await shownAmount(page, 'HalfHourPrice'), 'one half hour at 47').toBe(47);
    expect(await shownAmount(page, 'PaymentTotal'), 'the form totals the lines').toBe(213);
    await savePurchase(page, form);
    expect(await amountsUnder(page, 'clientpurchaselist', '', 'total'), "the client's payment list shows the saved total").toEqual([213]);

    // the saved payment, opened read-only
    await page.locator('#clientpurchaselist .ui-jqgrid-btable tr.jqgrow a').first().click();
    await expect(page.locator('.content-header button.delete:visible').last()).toBeVisible();
    await settle(page);
    expect(await shownAmount(page, 'PaymentTotal'), 'the saved payment shows its total').toBe(213);
    expect(await shownAmount(page, 'FullHour'), 'the saved payment shows two hours').toBe(2);
    expect(await shownAmount(page, 'HalfHour'), 'the saved payment shows one half hour').toBe(1);

    form = await openPurchaseForm(page, packs);
    await fillPurchase(page, form, { FullHourTenPack: 1 });
    expect(await shownAmount(page, 'FullHourTenPackPrice'), 'one ten-pack at 710').toBe(710);
    expect(await shownAmount(page, 'PaymentTotal'), 'the form totals the ten-pack').toBe(710);
    await savePurchase(page, form);
    expect(await amountsUnder(page, 'clientpurchaselist', '', 'total'), "the ten-pack client's payment list shows the saved total").toEqual([710]);

    // a ten-pack is ten sessions: the next purchase form shows them as the client's inventory
    await openPurchaseForm(page, packs);
    expect(money(await page.locator('#fullHour:visible').first().innerText()), 'a ten-pack gives the client ten hour sessions').toBe(10);

    // ---- using the sessions
    console.log(`rows on the test trainer's pay screen from earlier runs: ${await payEverythingPayable(page, trainer.id)}`);
    const info = await calendarInfo(page);
    const where = offsiteLocation(info) ?? PROVIDENCE;
    const today = easternNow();
    const book = async (client: TestClient, when: Date, type: 'Hour' | 'Half Hour') =>
      expectSaved(await saveBooking(page, info, { when, type, locationId: where, trainerId: trainer.id, clientIds: [client.id] }));
    await book(singles, at(addDays(today, -3), 6), 'Hour');
    await book(singles, at(addDays(today, -4), 6), 'Half Hour');
    await book(packs, at(addDays(today, -5), 6), 'Hour');
    const run = await runSessionCompletion(page);
    expect(run, 'this site has the session completion action').toBeDefined();
    expect(run!.Success, run!.Message).toBe(true);
    expect(run!.Appointments?.filter((a) => String(a.ClientId) === singles.id) ?? [], "completion picked up the singles client's two appointments").toHaveLength(2);
    expect(run!.Appointments?.filter((a) => String(a.ClientId) === packs.id) ?? [], "completion picked up the ten-pack client's appointment").toHaveLength(1);
    expect(run!.Appointments?.filter((a) => clients.some((c) => String(a.ClientId) === c.id) && a.InArrears) ?? [], 'every test appointment was covered by a purchase').toHaveLength(0);

    // ---- the pay screen: price per session, the trainer's percentage, the trainer's pay
    await openPayList(page, trainer.id);
    const column = (client: TestClient, heading: string) => amountsUnder(page, 'paytrainerlist', client.lastName, heading);
    const sorted = (values: number[]) => [...values].sort((a, b) => a - b);

    expect(sorted(await column(singles, 'price')), 'single sessions are priced at what the client paid for each').toEqual([47, 83]);
    expect(await column(packs, 'price'), 'a ten-pack session is priced at a tenth of the pack').toEqual([71]);

    const percentages = [...(await column(singles, 'percentage')), ...(await column(packs, 'percentage'))];
    expect(new Set(percentages).size, 'one trainer percentage across the test clients').toBe(1);
    const percent = percentages[0];
    expect(percent > 0 && percent <= 100, 'the trainer percentage is between 1 and 100').toBe(true);
    console.log(`trainer percentage on the pay screen: ${percent}`);
    const share = (price: number) => cents(price * percent / 100);

    expect(sorted(await column(singles, 'trainer pay')), "the trainer's pay is their percentage of each session's price").toEqual(sorted([share(47), share(83)]));
    expect(await column(packs, 'trainer pay'), "the trainer's pay on a ten-pack session is their percentage of a tenth of the pack").toEqual([share(71)]);

    // ---- paying: the running total, the confirmation, and the trainer's payment list
    const expectedPayout = cents(share(47) + share(83) + share(71));
    await openRoute(page, `trainerpaymentlist/${trainer.id}`);
    await gridReady(page, 'trainerpaymentlist');
    const paidBefore = await amountsUnder(page, 'trainerpaymentlist', '', 'total');

    await openPayList(page, trainer.id);
    for (const client of clients) {
      const boxes = listRows(page, 'paytrainerlist', client.lastName).locator('input.cbox');
      for (let i = 0, n = await boxes.count(); i < n; i++) await boxes.nth(i).check();
    }
    await settle(page, 300);
    expect(money(await page.locator('#paytrainerlist .paymentAmount:visible').first().innerText()), 'the pay screen adds up the ticked sessions').toBe(expectedPayout);

    await payTicked(page, async () => {
      const question = await page.locator('.ui-dialog:visible').last().innerText();
      const asked = question.match(/\$\s*([0-9][0-9,]*(?:\.[0-9]+)?)/);
      expect(asked ? money(asked[1]) : NaN, 'the confirmation asks to pay the same amount').toBe(expectedPayout);
    });
    for (const client of clients) await expect(listRows(page, 'paytrainerlist', client.lastName), 'paid sessions leave the pay screen').toHaveCount(0);

    await openRoute(page, `trainerpaymentlist/${trainer.id}`);
    await gridReady(page, 'trainerpaymentlist');
    const paidAfter = await amountsUnder(page, 'trainerpaymentlist', '', 'total');
    const times = (list: number[]) => list.filter((total) => total === expectedPayout).length;
    expect(paidAfter.length, "the trainer's payment list has one more payment").toBe(paidBefore.length + 1);
    expect(times(paidAfter), 'and that payment is for the amount confirmed').toBe(times(paidBefore) + 1);
  } finally {
    await payEverythingPayable(page, trainer.id).catch(() => {});
    for (const c of clients) console.log(`test client afterwards: ${await archiveClient(page, c.lastName).catch(() => 'failed')}`);
    if (notices.length) console.log(`messages the app showed: ${JSON.stringify(notices)}`);
  }
  expect(problems, problems.join('\n')).toEqual([]);
});
