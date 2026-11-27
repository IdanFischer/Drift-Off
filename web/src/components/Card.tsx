import type { ComponentProps } from "react";

/** The rounded navy card every Figma screen is built from. */
export function Card({ className = "", ...props }: ComponentProps<"section">) {
  return <section className={`rounded-card border border-line bg-dusk p-4 ${className}`} {...props} />;
}

/** Small pill used for "7h goal reached", "Goal met", etc. */
export function Pill({ children, tone = "sun" }: { children: React.ReactNode; tone?: "sun" | "muted" }) {
  const styles = tone === "sun" ? "bg-sun text-night" : "bg-dusk-raised text-haze";
  return (
    <span className={`inline-flex items-center gap-1 rounded-full px-2.5 py-1 text-[11px] font-semibold ${styles}`}>
      {children}
    </span>
  );
}
