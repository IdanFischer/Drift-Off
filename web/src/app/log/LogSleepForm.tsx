"use client";

import { useActionState, useRef, useState } from "react";
import { Card, Pill } from "@/components/Card";
import { CheckIcon, MinusIcon, MoonIcon, PlusIcon, SunIcon } from "@/components/icons";
import { formatClock, formatDuration, toMinutes, toTimeString } from "@/lib/format";
import { logSleep, type LogSleepState } from "./actions";

const STEP = 5; // minutes per tap on − / +

type Props = {
  nightOf: string;
  targetBedtime: string;
  targetWakeTime: string;
  targetHours: number;
};

export function LogSleepForm({ nightOf, targetBedtime, targetWakeTime, targetHours }: Props) {
  // Start from the targets: most nights are close to them, so it's a few taps, not typing.
  const [bed, setBed] = useState(() => toMinutes(targetBedtime));
  const [wake, setWake] = useState(() => toMinutes(targetWakeTime));
  const [state, formAction, pending] = useActionState<LogSleepState, FormData>(logSleep, { error: null });

  // Same rule as the API: wake-up is the first time it comes round after bedtime.
  const duration = ((wake - bed + 1440) % 1440) || 1440;
  const goal = Math.round(targetHours * 60);
  const scaleMax = goal + 120;
  const goalMet = duration >= goal;

  return (
    <form action={formAction} className="flex flex-1 flex-col gap-3">
      <input type="hidden" name="nightOf" value={nightOf} />
      <input type="hidden" name="bedtime" value={toTimeString(bed)} />
      <input type="hidden" name="wakeTime" value={toTimeString(wake)} />

      <div className="grid grid-cols-2 gap-3">
        <TimeStepper
          label="Went to bed"
          icon={<MoonIcon className="h-3.5 w-3.5 text-lavender" />}
          minutes={bed}
          onChange={setBed}
          target={targetBedtime}
        />
        <TimeStepper
          label="Woke up"
          icon={<SunIcon className="h-3.5 w-3.5 text-sun" />}
          minutes={wake}
          onChange={setWake}
          target={targetWakeTime}
        />
      </div>

      <Card>
        <div className="flex items-start justify-between">
          <div>
            <p className="text-xs text-haze">Total sleep</p>
            <p className="tabular mt-1 text-4xl font-bold text-sun" aria-live="polite">{formatDuration(duration)}</p>
          </div>
          {goalMet ? (
            <Pill><CheckIcon className="h-3 w-3" />Goal met</Pill>
          ) : (
            <Pill tone="muted">{formatDuration(goal - duration)} to go</Pill>
          )}
        </div>

        <div className="relative mt-4 h-2 rounded-full bg-line" role="presentation">
          <div className="h-full rounded-full bg-sun" style={{ width: `${Math.min(duration / scaleMax, 1) * 100}%` }} />
          <div className="absolute -top-1 h-4 w-0.5 rounded bg-moonlight" style={{ left: `${(goal / scaleMax) * 100}%` }} />
        </div>
        <div className="relative mt-2 h-4 text-[10px] text-haze">
          <span className="absolute left-0">0h</span>
          <span className="absolute -translate-x-1/2 font-semibold text-moonlight" style={{ left: `${(goal / scaleMax) * 100}%` }}>
            Goal {formatDuration(goal)}
          </span>
          <span className="absolute right-0">{formatDuration(scaleMax)}</span>
        </div>
      </Card>

      {state.error && (
        <p role="alert" className="rounded-xl border border-red-400/30 bg-red-400/10 px-4 py-3 text-sm text-red-200">
          {state.error}
        </p>
      )}

      <button
        type="submit"
        disabled={pending}
        className="mt-auto rounded-full bg-sun py-4 font-semibold text-night transition hover:bg-sun-deep disabled:opacity-60 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-sun"
      >
        {pending ? "Saving…" : "Save sleep log"}
      </button>
    </form>
  );
}

function TimeStepper({ label, icon, minutes, onChange, target }: {
  label: string;
  icon: React.ReactNode;
  minutes: number;
  onChange: (m: number) => void;
  target: string;
}) {
  const picker = useRef<HTMLInputElement>(null);
  const wrap = (m: number) => (m + 1440) % 1440;

  return (
    <Card>
      <p className="flex items-center gap-1.5 text-xs text-haze">{icon}{label}</p>

      {/* Tapping the time opens the native picker for big jumps; − / + nudge by 5 minutes. */}
      <button
        type="button"
        onClick={() => picker.current?.showPicker?.()}
        className="tabular mt-1 text-[26px] font-bold leading-tight"
        aria-label={`${label}: ${formatClock(minutes)}. Change time`}
      >
        {formatClock(minutes)}
      </button>
      <input
        ref={picker}
        type="time"
        tabIndex={-1}
        aria-hidden
        className="sr-only"
        value={toTimeString(minutes)}
        onChange={(e) => e.target.value && onChange(toMinutes(e.target.value))}
      />

      <div className="mt-2 flex gap-2">
        <StepButton label={`${label} 5 minutes earlier`} onClick={() => onChange(wrap(minutes - STEP))}><MinusIcon className="h-4 w-4" /></StepButton>
        <StepButton label={`${label} 5 minutes later`} onClick={() => onChange(wrap(minutes + STEP))}><PlusIcon className="h-4 w-4" /></StepButton>
      </div>
      <p className="mt-3 text-[11px] text-haze">Target {formatClock(target)}</p>
    </Card>
  );
}

function StepButton({ label, onClick, children }: { label: string; onClick: () => void; children: React.ReactNode }) {
  return (
    <button
      type="button"
      onClick={onClick}
      aria-label={label}
      className="flex h-8 w-8 items-center justify-center rounded-full bg-dusk-raised text-moonlight transition hover:bg-line"
    >
      {children}
    </button>
  );
}
