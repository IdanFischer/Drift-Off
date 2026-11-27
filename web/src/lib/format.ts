/** Small display helpers shared by server and client components. */

/** "22:48:00" → minutes since midnight (1368). */
export function toMinutes(time: string): number {
  const [h, m] = time.split(":").map(Number);
  return h * 60 + m;
}

/** 1368 → "22:48", the wire format the API accepts. */
export function toTimeString(minutes: number): string {
  const m = ((minutes % 1440) + 1440) % 1440;
  return `${String(Math.floor(m / 60)).padStart(2, "0")}:${String(m % 60).padStart(2, "0")}`;
}

/** "22:48:00" or 1368 → "10:48 PM". */
export function formatClock(time: string | number): string {
  const total = typeof time === "number" ? ((time % 1440) + 1440) % 1440 : toMinutes(time);
  const h = Math.floor(total / 60);
  const m = total % 60;
  const suffix = h < 12 ? "AM" : "PM";
  return `${h % 12 === 0 ? 12 : h % 12}:${String(m).padStart(2, "0")} ${suffix}`;
}

/** 462 → "7h 42m". */
export function formatDuration(minutes: number): string {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  return m === 0 ? `${h}h` : `${h}h ${m}m`;
}

/** Minutes from a target → "12 min early", "Right on target", "1h 5m late". */
export function formatOffset(delta: number, onTimeWithin = 5): string {
  if (Math.abs(delta) <= onTimeWithin) return "Right on target";
  const size = Math.abs(delta) >= 60 ? formatDuration(Math.abs(delta)) : `${Math.abs(delta)} min`;
  return `${size} ${delta < 0 ? "early" : "late"}`;
}

/** "2026-11-26" → "Thursday, Nov 26". Parsed as UTC so the server's time zone can't shift the day. */
export function formatLongDate(isoDate: string): string {
  return new Date(`${isoDate}T00:00:00Z`).toLocaleDateString("en-US", {
    weekday: "long",
    month: "short",
    day: "numeric",
    timeZone: "UTC",
  });
}
