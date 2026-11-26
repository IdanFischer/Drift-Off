import { formatDuration } from "@/lib/format";
import { MoonIcon } from "./icons";

/** The big ring on Home: fills toward your nightly goal, full when you hit it. */
export function DurationRing({ minutes, goalMinutes }: { minutes: number | null; goalMinutes: number }) {
  const r = 84;
  const circumference = 2 * Math.PI * r;
  const progress = minutes === null ? 0 : Math.min(minutes / goalMinutes, 1);

  return (
    <div className="relative mx-auto h-52 w-52">
      <svg viewBox="0 0 200 200" className="h-full w-full -rotate-90" aria-hidden>
        <circle cx="100" cy="100" r={r} fill="none" strokeWidth="12" className="stroke-line" />
        <circle
          cx="100"
          cy="100"
          r={r}
          fill="none"
          strokeWidth="12"
          strokeLinecap="round"
          className="stroke-sun transition-[stroke-dashoffset] duration-700"
          strokeDasharray={circumference}
          strokeDashoffset={circumference * (1 - progress)}
        />
      </svg>
      <div className="absolute inset-0 flex flex-col items-center justify-center">
        <MoonIcon className="mb-1 h-5 w-5 text-sun" />
        <p className="tabular text-4xl font-bold tracking-tight">{minutes === null ? "–" : formatDuration(minutes)}</p>
        <p className="mt-1 text-xs text-haze">{minutes === null ? "no sleep logged yet" : "slept last night"}</p>
      </div>
    </div>
  );
}
