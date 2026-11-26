"use server";

import { redirect } from "next/navigation";
import { api } from "@/lib/api/client";

export type LogSleepState = { error: string | null };

/**
 * Server Action behind the Log sleep form. It runs on the Next.js server, which calls the C# API,
 * so validation errors from the API come back to the form as a plain message.
 */
export async function logSleep(_prev: LogSleepState, form: FormData): Promise<LogSleepState> {
  const body = {
    nightOf: String(form.get("nightOf") ?? ""),
    bedtime: String(form.get("bedtime") ?? ""),
    wakeTime: String(form.get("wakeTime") ?? ""),
  };

  const { error, response } = await api.POST("/api/sleep-logs", { body });

  if (response.status === 201) redirect("/");
  if (response.status === 409) return { error: "You've already logged that night." };

  // ASP.NET Core returns validation problems as { errors: { Field: ["message", ...] } }.
  const problem = error as { title?: string; errors?: Record<string, string[]> } | undefined;
  const messages = problem?.errors ? Object.values(problem.errors).flat() : [];
  return { error: messages[0] ?? problem?.title ?? "Something went wrong. Please try again." };
}
