import { TabBar } from "@/components/TabBar";

/** Screens with the bottom tab bar. Log sleep lives outside this group, so it gets a full-screen layout like the Figma. */
export default function TabsLayout({ children }: LayoutProps<"/">) {
  return (
    <>
      <main className="flex flex-1 flex-col gap-3 px-4 pb-6 pt-8">{children}</main>
      <TabBar />
    </>
  );
}
