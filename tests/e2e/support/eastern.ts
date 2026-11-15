// The business is in Providence, RI, and the app's time rules are in Eastern
// time. GitHub's machines run in UTC, so "now" is worked out for Eastern here.
//
// A "wall time" below is a Date whose UTC fields hold the Eastern wall-clock
// reading. Only its getUTC* fields mean anything.

export function easternNow(): Date {
  const parts = new Intl.DateTimeFormat('en-US', {
    timeZone: 'America/New_York', hourCycle: 'h23',
    year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit',
  }).formatToParts(new Date());
  const n = (type: string) => Number(parts.find((p) => p.type === type)!.value);
  return new Date(Date.UTC(n('year'), n('month') - 1, n('day'), n('hour'), n('minute')));
}

export const addMinutes = (wall: Date, minutes: number) => new Date(wall.getTime() + minutes * 60_000);
export const addHours = (wall: Date, hours: number) => addMinutes(wall, hours * 60);
export const addDays = (wall: Date, days: number) => addMinutes(wall, days * 24 * 60);

/** The next quarter hour at or after the given wall time. */
export function quarter(wall: Date): Date {
  const ms = 15 * 60_000;
  return new Date(Math.ceil(wall.getTime() / ms) * ms);
}

/** The same day at the given hour and minute. */
export function at(wall: Date, hour: number, minute = 0): Date {
  return new Date(Date.UTC(wall.getUTCFullYear(), wall.getUTCMonth(), wall.getUTCDate(), hour, minute));
}

const two = (n: number) => String(n).padStart(2, '0');

/** MM/dd/yyyy */
export const usDay = (wall: Date) => `${two(wall.getUTCMonth() + 1)}/${two(wall.getUTCDate())}/${wall.getUTCFullYear()}`;

/** h:mm AM, the way the app's time lists write it. */
export function clock(wall: Date): string {
  const h = wall.getUTCHours();
  return `${h % 12 === 0 ? 12 : h % 12}:${two(wall.getUTCMinutes())} ${h < 12 ? 'AM' : 'PM'}`;
}

/** M/d/yyyy h:mm AM, the way the app names a calendar slot. */
export const slotKey = (wall: Date) => `${wall.getUTCMonth() + 1}/${wall.getUTCDate()}/${wall.getUTCFullYear()} ${clock(wall)}`;

/** Reads the app's "11/20/2026 9:00:00 AM" into a wall time. */
export function parseAppTime(text: string): Date | undefined {
  const m = /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{1,2}):(\d{2})(?::\d{2})? (AM|PM)$/i.exec(text.trim());
  if (!m) return undefined;
  const hour = (Number(m[4]) % 12) + (m[6].toUpperCase() === 'PM' ? 12 : 0);
  return new Date(Date.UTC(Number(m[3]), Number(m[1]) - 1, Number(m[2]), hour, Number(m[5])));
}

/** The most recent Sunday strictly before today. */
export function lastSunday(wall: Date = easternNow()): Date {
  const day = wall.getUTCDay();
  return at(addDays(wall, -(day === 0 ? 7 : day)), 0);
}
