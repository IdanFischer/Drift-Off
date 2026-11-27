"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { ChartIcon, HomeIcon, TargetIcon, TrophyIcon } from "./icons";

const tabs = [
  { href: "/", label: "Home", Icon: HomeIcon },
  { href: "/history", label: "History", Icon: ChartIcon },
  { href: "/rewards", label: "Rewards", Icon: TrophyIcon },
  { href: "/goals", label: "Goals", Icon: TargetIcon },
] as const;

/** Bottom navigation. A client component only because it reads the current path to highlight a tab. */
export function TabBar() {
  const pathname = usePathname();

  return (
    <nav
      aria-label="Main"
      className="sticky bottom-0 mt-auto grid grid-cols-4 border-t border-line bg-dusk/95 px-2 pb-[max(env(safe-area-inset-bottom),0.75rem)] pt-2 backdrop-blur"
    >
      {tabs.map(({ href, label, Icon }) => {
        const active = href === "/" ? pathname === "/" : pathname.startsWith(href);
        return (
          <Link
            key={href}
            href={href}
            aria-current={active ? "page" : undefined}
            className={`flex flex-col items-center gap-1 text-[11px] font-medium ${active ? "text-sun" : "text-haze hover:text-moonlight"}`}
          >
            <span className={`flex h-7 w-12 items-center justify-center rounded-full ${active ? "bg-sun/15" : ""}`}>
              <Icon className="h-5 w-5" />
            </span>
            {label}
          </Link>
        );
      })}
    </nav>
  );
}
