import type { ReactNode } from "react";
import { Navbar } from "./Navbar";

interface PageLayoutProps {
  children: ReactNode;
  title?: string;
}

export const PageLayout = ({ children, title }: PageLayoutProps) => (
  <div className="min-h-screen bg-gray-50">
    <Navbar title={title} />
    <main className="max-w-7xl mx-auto px-6 py-8">{children}</main>
  </div>
);
