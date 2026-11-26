/** Inline SVG icons (stroke style to match the Figma). They inherit size and colour from CSS. */
import type { SVGProps } from "react";

type IconProps = SVGProps<SVGSVGElement>;

const base = {
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeWidth: 1.8,
  strokeLinecap: "round",
  strokeLinejoin: "round",
  "aria-hidden": true,
} as const;

export const MoonIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="M20 14.5A8.5 8.5 0 0 1 9.5 4a8.5 8.5 0 1 0 10.5 10.5Z" /></svg>
);
export const SunIcon = (p: IconProps) => (
  <svg {...base} {...p}>
    <circle cx="12" cy="12" r="4" />
    <path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4" />
  </svg>
);
export const HomeIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="M4 10.5 12 4l8 6.5V19a1 1 0 0 1-1 1h-4.5v-5.5h-5V20H5a1 1 0 0 1-1-1Z" /></svg>
);
export const ChartIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="M6 19V11M12 19V5M18 19v-6" /></svg>
);
export const TrophyIcon = (p: IconProps) => (
  <svg {...base} {...p}>
    <path d="M8 4h8v5a4 4 0 0 1-8 0Z" />
    <path d="M8 6H5a3 3 0 0 0 3 4M16 6h3a3 3 0 0 1-3 4M12 13v4M8.5 20h7" />
  </svg>
);
export const TargetIcon = (p: IconProps) => (
  <svg {...base} {...p}><circle cx="12" cy="12" r="8" /><circle cx="12" cy="12" r="4" /><circle cx="12" cy="12" r="0.8" fill="currentColor" /></svg>
);
export const ChevronLeftIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="m14.5 6-6 6 6 6" /></svg>
);
export const CheckIcon = (p: IconProps) => (
  <svg {...base} strokeWidth={2.4} {...p}><path d="m5 12.5 4.5 4.5L19 7.5" /></svg>
);
export const MinusIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="M6 12h12" /></svg>
);
export const PlusIcon = (p: IconProps) => (
  <svg {...base} {...p}><path d="M12 6v12M6 12h12" /></svg>
);
