import { AppSidebar } from "./AppSidebar"
import { AssistantSidebar } from "./AssistantSidebar"
import { AppHeader } from "./AppHeader"
import { Outlet } from "react-router-dom"
import { WorkspaceProvider } from "@/contexts/WorkspaceContext"

export function WorkspaceLayout() {
  return (
    <WorkspaceProvider>
      <div className="h-screen flex flex-col overflow-hidden bg-secondary/30 font-sans">
        <AppHeader />
        <main className="flex-1 flex overflow-hidden relative">
           <AppSidebar />
           <section className="flex-1 overflow-y-auto relative bg-secondary/30 flex justify-center">
               <Outlet />
           </section>
           <AssistantSidebar />
        </main>
      </div>
    </WorkspaceProvider>
  )
}
