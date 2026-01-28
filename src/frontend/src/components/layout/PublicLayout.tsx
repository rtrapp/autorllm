import { Outlet } from "react-router-dom";
import { PublicHeader } from "./PublicHeader";

export function PublicLayout() {
  return (
    <div className="min-h-screen bg-secondary/20 font-sans">
      <PublicHeader />
      <Outlet />
    </div>
  );
}
