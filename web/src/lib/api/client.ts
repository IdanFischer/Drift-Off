import "server-only";
import createClient from "openapi-fetch";
import type { components, paths } from "./schema";

/**
 * Typed client for the Drift API. The types in ./schema.d.ts are generated from the API's
 * OpenAPI spec (`npm run gen:api`), so a renamed field in C# becomes a TypeScript error here.
 *
 * It is server-only: pages fetch on the server and forms post through Server Actions,
 * so the browser never needs the API's address and the API needs no public CORS rule.
 */
export const api = createClient<paths>({
  baseUrl: process.env.API_URL ?? "http://localhost:5080",
});

export type SleepLog = components["schemas"]["SleepLogResponse"];
export type TodaySummary = components["schemas"]["TodaySummaryResponse"];
export type Goal = components["schemas"]["GoalResponse"];

export async function getTodaySummary(): Promise<TodaySummary> {
  const { data, error } = await api.GET("/api/summary/today");
  if (!data) throw new Error(`Could not load today's summary: ${JSON.stringify(error)}`);
  return data;
}
