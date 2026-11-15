import { test, expect, type Page } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, expectSaved, openRoute, settle, type SaveResult } from '../support/app';
import { archiveClient, createClient, ensureTrainer, OTHER_TRAINER_LAST_NAME, type TestClient, type TestTrainer } from '../support/world';
import { addDays, addHours, addMinutes, easternNow, quarter, slotKey } from '../support/eastern';
import {
  calendarData, calendarInfo, clearBookings, deleteBooking, dragTo, EAST_GREENWICH, offsiteLocation,
  pickerClientIds, PROVIDENCE, quietHour, saveBooking, type CalendarInfo,
} from '../support/api';

// The scheduling rules a trainer works under. An administrator is exempt from
// most of them, so these run as the test trainer, with an administrator
// alongside to set the scene (other trainers' appointments, past bookings).
//
// The rules, as the business states them:
//   - Closed period: without a special permission, nothing that starts less
//     than six hours from now can be booked, changed, moved or deleted.
//   - Providence holds four appointments at once, East Greenwich two. Other
//     locations have no limit. A pair appointment counts once.
//   - A trainer books only their own clients.
//
// Most checks send the calendar's own requests (see support/api.ts) so they do
// not depend on what else is on the calendar that day.

test.use({ timezoneId: 'America/New_York' });
test.describe.configure({ mode: 'serial' });

const RETRO = 'the test trainer holds the retroactive permission, so the closed period does not apply to them';

let admin: Page;
let trainer: Page;
let trainerNotices: string[] = [];
let main: TestTrainer;
let other: TestTrainer;
let mine: TestClient;
let theirs: TestClient;
let theirs2: TestClient;
let adminInfo: CalendarInfo;
let trainerInfo: CalendarInfo;
let quiet: Date;            // a far-off hour with nothing booked
let anywhere: number;       // a location without a limit, or Providence if there is none

const refused = (result: SaveResult, why: string) => expect(result.Success, why).toBe(false);

/** How many appointments a trainer has starting at a time, at one location. Counts only. */
async function countAt(page: Page, info: CalendarInfo, trainerId: string, when: Date, locationId?: number): Promise<number[]> {
  const data = await calendarData(page, info, when, when, [trainerId]);
  return data.appointments
    .filter((a) => String(a.trainerId) === trainerId && slotKey(a.start) === slotKey(when) && (locationId === undefined || a.locationId === locationId))
    .map((a) => a.id);
}

async function sweep(): Promise<number> {
  const today = easternNow();
  return clearBookings(admin, adminInfo, [main.id, other.id], today, addDays(today, 200));
}

test.beforeAll(async ({ browser }, testInfo) => {
  test.setTimeout(420_000);
  const baseURL = testInfo.project.use.baseURL;
  admin = await (await browser.newContext({ baseURL, timezoneId: 'America/New_York' })).newPage();
  acceptDialogs(admin);
  await login(admin);
  mine = await createClient(admin);
  theirs = await createClient(admin);
  theirs2 = await createClient(admin);
  main = await ensureTrainer(admin, [mine]);
  other = await ensureTrainer(admin, [theirs, theirs2], OTHER_TRAINER_LAST_NAME);
  adminInfo = await calendarInfo(admin);
  anywhere = offsiteLocation(adminInfo) ?? PROVIDENCE;
  console.log(`appointments left by earlier runs and removed: ${await sweep()}`);
  quiet = await quietHour(admin, adminInfo, easternNow(), 8);

  trainer = await (await browser.newContext({ baseURL, timezoneId: 'America/New_York' })).newPage();
  trainerNotices = acceptDialogs(trainer);
  await login(trainer, { user: main.loginName, pass: main.password });
  trainerInfo = await calendarInfo(trainer);
  console.log(`test trainer: retroactive permission ${trainerInfo.canEnterRetroactive}, edit past ${trainerInfo.canEditPast}, see others ${trainerInfo.canSeeOthers}; `
    + `locations on the list ${adminInfo.locationIds.length}, one without a limit ${offsiteLocation(adminInfo) !== undefined}`);
});

test.afterAll(async () => {
  test.setTimeout(240_000);
  if (!admin || !adminInfo || !main || !other) return;
  console.log(`test appointments removed afterwards: ${await sweep().catch(() => 'failed')}`);
  for (const c of [mine, theirs, theirs2]) {
    if (c) console.log(`test client afterwards: ${await archiveClient(admin, c.lastName).catch(() => 'failed')}`);
  }
  await admin.context().close();
  await trainer?.context().close();
});

test.describe('scheduling rules for a trainer', () => {
  test('the client picker offers a trainer only their own clients', async () => {
    const offered = await pickerClientIds(trainer, trainerInfo, quiet);
    expect(offered.includes(mine.id), "the trainer's own client is offered").toBe(true);
    expect(offered.includes(theirs.id), "another trainer's client is not offered").toBe(false);

    const offeredToAdmin = await pickerClientIds(admin, adminInfo, quiet);
    expect(offeredToAdmin.includes(mine.id) && offeredToAdmin.includes(theirs.id), 'an administrator is offered both').toBe(true);
  });

  test('clicking a calendar slot inside the closed period is refused with a message', async () => {
    test.skip(trainerInfo.canEnterRetroactive, RETRO);
    await openRoute(trainer, 'calendar');
    await settle(trainer, 800);
    // the first time slot of today's column: either already past or under six hours away
    const today = trainer.locator('#calendar .fc-agenda-days td.fc-today').first();
    await expect(today, "today's column is on the calendar").toHaveCount(1);
    const column = await today.boundingBox();
    const row = trainer.locator('#calendar table.fc-agenda-slots tr').first();
    await row.scrollIntoViewIfNeeded();
    const first = await row.boundingBox();
    if (!column || !first) throw new Error('could not locate the first slot of today');
    const before = trainerNotices.length;
    await trainer.mouse.click(column.x + column.width / 2, first.y + first.height / 2);
    await settle(trainer, 800);
    const shown = trainerNotices.slice(before);
    expect(shown.some((m) => /period is closed/i.test(m)), 'the app says the period is closed').toBe(true);
    expect(await trainer.locator('#editModule [name="LocationEntityId"]:visible').count(), 'the booking form does not open').toBe(0);
  });

  test('the closed period is six hours for booking, changing, moving and deleting', async () => {
    test.skip(trainerInfo.canEnterRetroactive, RETRO);
    const now = easternNow();
    const inside = quarter(addHours(now, 3));      // well inside six hours
    const alsoInside = quarter(addHours(now, 4));
    const outside = quarter(addHours(now, 7));     // outside six hours, inside the eight the drag rule used to use
    const later = addHours(outside, 2);
    const booking = { locationId: anywhere, trainerId: main.id, clientIds: [mine.id] };

    // booking
    refused(await saveBooking(trainer, trainerInfo, { ...booking, when: inside }), 'booking three hours from now is refused');
    expect(await countAt(trainer, trainerInfo, main.id, inside), 'and nothing was booked').toHaveLength(0);
    expectSaved(await saveBooking(trainer, trainerInfo, { ...booking, when: outside }));
    let ids = await countAt(trainer, trainerInfo, main.id, outside);
    expect(ids, 'booking seven hours from now works').toHaveLength(1);
    const id = ids[0];

    // moving by drag: allowed outside six hours at both ends, refused when the new time is inside
    const moved = await dragTo(trainer, trainerInfo, id, later);
    expect(moved.Success, `dragging an appointment that is seven hours away to nine hours away is allowed (six-hour rule). The app said: ${moved.Message ?? ''}`).toBe(true);
    expect(await countAt(trainer, trainerInfo, main.id, later), 'it is now at the later time').toHaveLength(1);
    refused(await dragTo(trainer, trainerInfo, id, alsoInside), 'dragging it to four hours from now is refused');
    expect(await countAt(trainer, trainerInfo, main.id, later), 'and it stays where it was').toHaveLength(1);

    // changing through the form
    refused(await saveBooking(trainer, trainerInfo, { ...booking, id, when: alsoInside }), 'changing its time to four hours from now is refused');
    expect(await countAt(trainer, trainerInfo, main.id, later), 'and it stays where it was').toHaveLength(1);

    // an appointment that is already inside the closed period (an administrator can book one)
    expect(adminInfo.canEnterRetroactive, 'the administrator may book inside the closed period').toBe(true);
    expectSaved(await saveBooking(admin, adminInfo, { ...booking, when: inside }));
    ids = await countAt(trainer, trainerInfo, main.id, inside);
    expect(ids, 'the trainer has an appointment three hours from now').toHaveLength(1);
    const soon = ids[0];
    refused(await deleteBooking(trainer, trainerInfo, soon), 'the trainer cannot delete it');
    refused(await dragTo(trainer, trainerInfo, soon, addHours(later, 2)), 'the trainer cannot drag it out of the closed period');
    refused(await saveBooking(trainer, trainerInfo, { ...booking, id: soon, when: addHours(later, 2) }), 'the trainer cannot change it through the form');
    expect(await countAt(trainer, trainerInfo, main.id, inside), 'it is still there, unmoved').toHaveLength(1);
    expectSaved(await deleteBooking(admin, adminInfo, soon));

    // deleting outside the closed period is fine
    expectSaved(await deleteBooking(trainer, trainerInfo, id));
    expect(await countAt(trainer, trainerInfo, main.id, later), 'the trainer deleted their later appointment').toHaveLength(0);
  });

  test('Providence holds four appointments at once and refuses a fifth', async () => {
    const theirBooking = { locationId: PROVIDENCE, trainerId: other.id, when: quiet };
    const myBooking = { locationId: PROVIDENCE, trainerId: main.id, clientIds: [mine.id], when: quiet };
    const quarters = [0, 15, 30, 45].map((m) => slotKey(addMinutes(quiet, m)));

    // three appointments by another trainer, one of them a pair
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs.id] }));
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs2.id] }));
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs.id, theirs2.id], type: 'Pair' }));

    // three is not full: the calendar does not block the hour, and a fourth saves
    let view = await calendarData(trainer, trainerInfo, quiet, quiet);
    expect(quarters.some((q) => view.blocked[PROVIDENCE]?.has(q)), 'with three booked (a pair counting once) the hour is not blocked').toBe(false);
    expectSaved(await saveBooking(trainer, trainerInfo, myBooking));
    const fourth = await countAt(trainer, trainerInfo, main.id, quiet, PROVIDENCE);
    expect(fourth, 'the trainer booked the fourth').toHaveLength(1);

    // the fourth can still be edited while the hour is full
    const edited = await saveBooking(trainer, trainerInfo, { ...myBooking, id: fourth[0], notes: 'changed by the browser tests' });
    expect(edited.Success, `an appointment in a full hour can still be edited. The app said: ${edited.Message ?? ''}`).toBe(true);

    // a fifth is refused, here but not at a location without a limit
    const fifth = await saveBooking(trainer, trainerInfo, myBooking);
    expect(fifth.Success, 'a fifth appointment at Providence is refused').toBe(false);
    expect(await countAt(trainer, trainerInfo, main.id, quiet, PROVIDENCE), 'and was not booked').toHaveLength(1);
    if (offsiteLocation(adminInfo) !== undefined) {
      expectSaved(await saveBooking(trainer, trainerInfo, { ...myBooking, locationId: offsiteLocation(adminInfo)! }));
    }

    // with four held by other trainers the calendar blocks the hour for this trainer
    expectSaved(await deleteBooking(trainer, trainerInfo, fourth[0]));
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs.id] }));
    view = await calendarData(trainer, trainerInfo, quiet, quiet);
    expect(quarters.every((q) => view.blocked[PROVIDENCE]?.has(q)), 'the calendar blocks the whole hour').toBe(true);
    refused(await saveBooking(trainer, trainerInfo, myBooking), 'and saving there is refused');

    // an administrator is exempt
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs2.id] }));
    console.log(`test appointments removed: ${await sweep()}`);
  });

  test('East Greenwich holds two appointments at once and refuses a third', async () => {
    const theirBooking = { locationId: EAST_GREENWICH, trainerId: other.id, when: quiet };
    const myBooking = { locationId: EAST_GREENWICH, trainerId: main.id, clientIds: [mine.id], when: quiet };
    const quarters = [0, 15, 30, 45].map((m) => slotKey(addMinutes(quiet, m)));

    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs.id] }));
    let view = await calendarData(trainer, trainerInfo, quiet, quiet);
    expect(quarters.some((q) => view.blocked[EAST_GREENWICH]?.has(q)), 'with one booked the hour is not blocked').toBe(false);

    expectSaved(await saveBooking(trainer, trainerInfo, myBooking));
    const second = await countAt(trainer, trainerInfo, main.id, quiet, EAST_GREENWICH);
    expect(second, 'the trainer booked the second').toHaveLength(1);
    const edited = await saveBooking(trainer, trainerInfo, { ...myBooking, id: second[0], notes: 'changed by the browser tests' });
    expect(edited.Success, `an appointment in a full hour can still be edited. The app said: ${edited.Message ?? ''}`).toBe(true);
    refused(await saveBooking(trainer, trainerInfo, myBooking), 'a third appointment at East Greenwich is refused');
    expect(await countAt(trainer, trainerInfo, main.id, quiet, EAST_GREENWICH), 'and was not booked').toHaveLength(1);

    expectSaved(await deleteBooking(trainer, trainerInfo, second[0]));
    expectSaved(await saveBooking(admin, adminInfo, { ...theirBooking, clientIds: [theirs2.id] }));
    view = await calendarData(trainer, trainerInfo, quiet, quiet);
    expect(quarters.every((q) => view.blocked[EAST_GREENWICH]?.has(q)), 'with two held by other trainers the calendar blocks the hour').toBe(true);
    refused(await saveBooking(trainer, trainerInfo, myBooking), 'and saving there is refused');
    console.log(`test appointments removed: ${await sweep()}`);
  });
});
