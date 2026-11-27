import Link from "next/link";
import { Suspense } from "react";
import { ChevronLeftIcon } from "@/components/icons";
import { getTodaySummary } from "@/lib/api/client";
import { formatLongDate } from "@/lib/format";
import { LogSleepForm } from "./LogSleepForm";

export default function LogSleepPage() {
  return (
    <main className="flex flex-1 flex-col gap-3 px-4 pb-6 pt-6">
      <Suspense fallback={<div className="flex-1 animate-pulse rounded-card bg-dusk" aria-busy="true" />}>
        <LogSleep />
      </Suspense>
    </main>
  );
}

async function LogSleep() {
  const summary = await getTodaySummary();
  const nightOf = summary.nightToLog; // the API works out "last night" in the user's own time zone

  return (
    <>
      <header className="relative mb-2 flex items-center justify-center">
        <Link
          href="/"
          aria-label="Back to Home"
          className="absolute left-0 flex h-9 w-9 items-center justify-center rounded-full border border-line bg-dusk text-moonlight hover:bg-dusk-raised"
        >
          <ChevronLeftIcon className="h-5 w-5" />
        </Link>
        <div className="text-center">
          <h1 className="font-semibold">Log sleep</h1>
          <p className="text-[11px] text-haze">Night of {formatLongDate(nightOf)}</p>
        </div>
      </header>

      <LogSleepForm
        nightOf={nightOf}
        targetBedtime={summary.goal.targetBedtime}
        targetWakeTime={summary.goal.targetWakeTime}
        targetHours={summary.goal.targetHours}
      />
    </>
  );
}
