import { expect, type Page } from '@playwright/test';
import type { SaveResult } from './app';
import { addDays, addMinutes, at, clock, parseAppTime, slotKey, usDay } from './eastern';

// The scheduling rules are checked by sending the app the same requests its
// calendar sends (open the form, save it, drag, delete), as whoever is signed
// in on the page. That exercises the server's rules exactly, without depending
// on where things happen to sit on a crowded calendar.
//
// PRIVACY: as an administrator these replies contain real names. Nothing in
// this file returns or prints a title or a name; only ids, times and counts.

export const PROVIDENCE = 2;
export const EAST_GREENWICH = 1;

export interface CalendarInfo {
  eventsUrl: string; addUpdateUrl: string; deleteUrl: string; eventChangedUrl: string;
  trainerId: number;
  canEnterRetroactive: boolean; canEditPast: boolean; canSeeOthers: boolean;
  /** ids of every location on the calendar's location list */
  locationIds: number[];
}

async function getJson(page: Page, url: string, params?: Record<string, string | number>): Promise<any> {
  const response = await page.request.get(url, { params, failOnStatusCode: false, timeout: 60_000 });
  try { return JSON.parse(await response.text()); }
  catch { throw new Error(`GET ${url.split('?')[0]} answered HTTP ${response.status()} with something other than JSON`); }
}

function verdict(body: any, what: string): SaveResult {
  if (!body || typeof body !== 'object' || !('Success' in body)) throw new Error(`${what}: the app's reply had no verdict in it`);
  return { Success: !!body.Success, Message: typeof body.Message === 'string' ? body.Message.slice(0, 120) : undefined, Errors: body.Errors ?? undefined };
}

/** What the calendar screen is told about the signed-in user, and where its actions live. */
export async function calendarInfo(page: Page): Promise<CalendarInfo> {
  const body = await getJson(page, '/Schedule/AppointmentCalendar/AppointmentCalendar');
  const def = body?.CalendarDefinition;
  if (!def) throw new Error('the calendar definition did not come back');
  const locationIds = ((body._LocationList ?? []) as { Value: string }[]).map((l) => Number(l.Value)).filter((n) => n > 0);
  return {
    eventsUrl: def.Url, addUpdateUrl: def.AddUpdateUrl, deleteUrl: def.DeleteUrl, eventChangedUrl: def.EventChangedUrl,
    trainerId: def.TrainerId,
    canEnterRetroactive: !!def.CanEnterRetroactiveAppointments, canEditPast: !!def.CanEditPastAppointments, canSeeOthers: !!def.CanSeeOthersAppointments,
    locationIds,
  };
}

/** A location with no capacity limit (anything that is not one of the two gyms), if there is one. */
export const offsiteLocation = (info: CalendarInfo): number | undefined => info.locationIds.find((id) => id !== PROVIDENCE && id !== EAST_GREENWICH);

export interface Booking {
  id?: number;                 // set to change an existing appointment
  when: Date;                  // Eastern wall time
  type?: 'Hour' | 'Half Hour' | 'Pair';
  locationId: number;
  trainerId: number | string;
  clientIds: string[];
  notes?: string;
}

/** Opens the appointment form the way the calendar does and returns the ids of the clients its picker offers. */
export async function pickerClientIds(page: Page, info: CalendarInfo, when: Date): Promise<string[]> {
  const model = await getJson(page, info.addUpdateUrl, { ScheduledDate: usDay(when), ScheduledStartTime: clock(when) });
  return ((model?.ClientsDtos?._availableItems ?? []) as { id: string }[]).map((c) => String(c.id));
}

/** Saves the appointment form: a new appointment, or a change to an existing one. Returns the app's verdict. */
export async function saveBooking(page: Page, info: CalendarInfo, b: Booking): Promise<SaveResult> {
  const model = await getJson(page, info.addUpdateUrl, b.id ? { EntityId: b.id } : { ScheduledDate: usDay(b.when), ScheduledStartTime: clock(b.when) });
  const saveUrl: string | undefined = model?._saveUrl;
  if (!saveUrl) throw new Error('the appointment form did not say where to save');
  const type = b.type ?? 'Hour';
  const response = await page.request.post(saveUrl, {
    failOnStatusCode: false, timeout: 60_000,
    data: {
      EntityId: b.id ?? 0, Copy: false,
      ClientsDtos: { selectedItems: b.clientIds.map((id) => ({ id, name: '' })) },
      LocationEntityId: b.locationId, TrainerEntityId: Number(b.trainerId), AppointmentType: type,
      Date: usDay(b.when), StartTimeString: clock(b.when), EndTimeString: clock(addMinutes(b.when, type === 'Half Hour' ? 30 : 60)),
      Notes: b.notes ?? 'created by the browser tests',
    },
  });
  let body: any;
  try { body = JSON.parse(await response.text()); } catch { throw new Error(`saving an appointment answered HTTP ${response.status()} with something other than JSON`); }
  return verdict(body, 'saving an appointment');
}

/** What dragging an appointment to a new start time sends. */
export async function dragTo(page: Page, info: CalendarInfo, id: number, to: Date, minutes = 60): Promise<SaveResult> {
  const stampOf = (w: Date) => `${usDay(w)} ${clock(w)}`;
  return verdict(await getJson(page, info.eventChangedUrl, { EntityId: id, ScheduledDate: stampOf(to), StartTime: stampOf(to), EndTime: stampOf(addMinutes(to, minutes)) }), 'moving an appointment');
}

export async function deleteBooking(page: Page, info: CalendarInfo, id: number): Promise<SaveResult> {
  return verdict(await getJson(page, info.deleteUrl, { EntityId: id }), 'deleting an appointment');
}

export interface Slot { id: number; start: Date; trainerId: number; locationId: number }

interface EventsReply { appointments: Slot[]; blocked: Record<number, Set<string>> }

/**
 * The calendar's data between two days, as the signed-in user gets it:
 * appointments (id, start, trainer, location only) and the slots the calendar
 * blocks at each location. A trainer is only sent their own appointments.
 */
export async function calendarData(page: Page, info: CalendarInfo, from: Date, to: Date, trainerIds?: (number | string)[]): Promise<EventsReply> {
  const params: Record<string, string | number> = { start: at(from, 0).getTime() / 1000, end: at(addDays(to, 1), 0).getTime() / 1000, Loc: 0 };
  if (trainerIds) params.TrainerIds = trainerIds.join(',');
  const body = await getJson(page, info.eventsUrl, params);
  const appointments: Slot[] = [];
  for (const e of (body?.Events ?? []) as any[]) {
    const start = typeof e.start === 'string' ? parseAppTime(e.start) : undefined;
    if (e.EntityId > 0 && start) appointments.push({ id: e.EntityId, start, trainerId: e.trainerId, locationId: e.locationId });
  }
  const blocked: Record<number, Set<string>> = {};
  for (const loc of (body?.BlockedSlotsByLocation?.Location ?? []) as any[]) {
    blocked[loc.LocationId] = new Set(((loc.TimeSlots ?? []) as any[]).map((s) => String(s.TimeSlot)));
  }
  return { appointments, blocked };
}

/** The signed-in user's view of one trainer's appointments starting exactly at the given time. */
export async function bookingsAt(page: Page, info: CalendarInfo, trainerId: number | string, when: Date): Promise<Slot[]> {
  const data = await calendarData(page, info, when, when, [trainerId]);
  return data.appointments.filter((a) => String(a.trainerId) === String(trainerId) && slotKey(a.start) === slotKey(when));
}

export async function onlyBookingAt(page: Page, info: CalendarInfo, trainerId: number | string, when: Date): Promise<number> {
  const found = await bookingsAt(page, info, trainerId, when);
  expect(found.length, 'exactly one appointment for the trainer at that time').toBe(1);
  return found[0].id;
}

/** Deletes the given trainers' appointments between two days. Run as an administrator; only for the test trainers. */
export async function clearBookings(page: Page, info: CalendarInfo, trainerIds: (number | string)[], from: Date, to: Date): Promise<number> {
  const data = await calendarData(page, info, from, to, trainerIds);
  const wanted = new Set(trainerIds.map(String));
  let removed = 0;
  for (const a of data.appointments) {
    if (!wanted.has(String(a.trainerId))) continue;
    const result = await deleteBooking(page, info, a.id).catch(() => undefined);
    if (result?.Success) removed++;
  }
  return removed;
}

/**
 * A quiet hour far ahead: a Sunday at 5 AM, at least `weeksOut` weeks away, on
 * which nobody has anything booked from 4 to 7 AM. Run as an administrator.
 */
export async function quietHour(page: Page, info: CalendarInfo, today: Date, weeksOut: number): Promise<Date> {
  for (let week = weeksOut; week < weeksOut + 12; week++) {
    const sunday = addDays(today, 7 * week - today.getUTCDay());
    const slot = at(sunday, 5);
    const data = await calendarData(page, info, slot, slot);
    const near = data.appointments.filter((a) => Math.abs(a.start.getTime() - slot.getTime()) < 2 * 3600_000).length;
    if (near === 0) return slot;
  }
  throw new Error('no quiet Sunday 5 AM hour found in a twelve-week window');
}
