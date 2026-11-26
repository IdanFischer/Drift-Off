import Link from "next/link";
import { Suspense } from "react";
import { Card, Pill } from "@/components/Card";
import { DurationRing } from "@/components/DurationRing";
import { CheckIcon, MoonIcon, SunIcon } from "@/components/icons";
import { getTodaySummary } from "@/lib/api/client";
import { formatClock, formatDuration, formatLongDate, formatOffset } from "@/lib/format";

export default function HomePage() {
  // The summary is fresh data from the API, so it streams in behind a skeleton instead of blocking the page.
  return (
    <Suspense fallback={<HomeSkeleton />}>
      <Home />
    </Suspense>
  );
}

async function Home() {
  const summary = await getTodaySummary();
  const night = summary.lastNight;
  const goalMinutes = Math.round(summary.goal.targetHours * 60);

  return (
    <>
      <header className="mb-1">
        <p className="text-xs text-haze">{formatLongDate(summary.today)}</p>
        <h1 className="text-xl font-semibold">Rise and shine, {summary.displayName}</h1>
      </header>

      <Card className="py-6">
        <DurationRing minutes={night?.durationMinutes ?? null} goalMinutes={goalMinutes} />
        <div className="mt-4 flex justify-center gap-2">
          {night?.hoursGoalMet && (
            <Pill>
              <CheckIcon className="h-3 w-3" />
              {formatDuration(goalMinutes)} goal reached
            </Pill>
          )}
          {night && !night.hoursGoalMet && (
            <Pill tone="muted">{formatDuration(goalMinutes - night.durationMinutes)} short of your goal</Pill>
          )}
        </div>
        {night && !summary.lastNightLogged && (
          <p className="mt-3 text-center text-xs text-haze">Showing your last logged night, {formatLongDate(night.nightOf)}.</p>
        )}
      </Card>

      <div className="grid grid-cols-2 gap-3">
        <TimeCard
          icon={<MoonIcon className="h-3.5 w-3.5 text-lavender" />}
          label="Bedtime"
          time={night?.bedtime}
          note={night ? formatOffset(night.bedtimeMinutesFromTarget) : `Target ${formatClock(summary.goal.targetBedtime)}`}
          good={night?.bedtimeOnTarget}
        />
        <TimeCard
          icon={<SunIcon className="h-3.5 w-3.5 text-sun" />}
          label="Wake up"
          time={night?.wakeTime}
          note={night ? formatOffset(night.wakeMinutesFromTarget) : `Target ${formatClock(summary.goal.targetWakeTime)}`}
          good={night ? Math.abs(night.wakeMinutesFromTarget) <= summary.goal.bedtimeWindowMinutes : undefined}
        />
      </div>

      {summary.lastNightLogged ? (
        <p className="mt-auto flex items-center justify-center gap-2 rounded-full border border-line py-4 text-sm text-haze">
          <CheckIcon className="h-4 w-4 text-sun" />
          Last night is logged. See you tonight.
        </p>
      ) : (
        <Link
          href="/log"
          className="mt-auto rounded-full bg-sun py-4 text-center font-semibold text-night transition hover:bg-sun-deep focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-sun"
        >
          Log last night&apos;s sleep
        </Link>
      )}
    </>
  );
}

function TimeCard({ icon, label, time, note, good }: { icon: React.ReactNode; label: string; time?: string; note: string; good?: boolean }) {
  return (
    <Card>
      <p className="flex items-center gap-1.5 text-xs text-haze">
        {icon}
        {label}
      </p>
      <p className="tabular mt-1 text-2xl font-bold">{time ? formatClock(time) : "–"}</p>
      <p className={`mt-1 flex items-center gap-1 text-[11px] ${good ? "text-sun" : "text-haze"}`}>
        {good && <CheckIcon className="h-3 w-3" />}
        {note}
      </p>
    </Card>
  );
}

function HomeSkeleton() {
  return (
    <div className="flex flex-1 animate-pulse flex-col gap-3" aria-busy="true" aria-label="Loading">
      <div className="h-12 w-2/3 rounded-lg bg-dusk" />
      <div className="h-80 rounded-card bg-dusk" />
      <div className="grid grid-cols-2 gap-3">
        <div className="h-28 rounded-card bg-dusk" />
        <div className="h-28 rounded-card bg-dusk" />
      </div>
    </div>
  );
}
