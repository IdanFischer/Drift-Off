import type { Metadata, Viewport } from "next";
import { Plus_Jakarta_Sans } from "next/font/google";
import "./globals.css";

const jakarta = Plus_Jakarta_Sans({
  variable: "--font-jakarta",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Drift – Sleep tracker",
  description: "Log your sleep, hit your bedtime, earn rewards.",
};

export const viewport: Viewport = {
  themeColor: "#0b0d17",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en" className={`${jakarta.variable} h-full antialiased`}>
      <body className="min-h-full font-sans">
        <div className="mx-auto flex min-h-dvh w-full max-w-[440px] flex-col">{children}</div>
      </body>
    </html>
  );
}
