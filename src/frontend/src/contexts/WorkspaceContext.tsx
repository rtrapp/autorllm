import { createContext, useContext, useState, type ReactNode } from "react";
import type { Chapter } from "@/features/chapters/types/chapter";

interface WorkspaceContextType {
  selectedChapter: Chapter | null;
  setSelectedChapter: (chapter: Chapter | null) => void;
}

const WorkspaceContext = createContext<WorkspaceContextType | null>(null);

export function WorkspaceProvider({ children }: { children: ReactNode }) {
  const [selectedChapter, setSelectedChapter] = useState<Chapter | null>(null);

  return (
    <WorkspaceContext.Provider value={{ selectedChapter, setSelectedChapter }}>
      {children}
    </WorkspaceContext.Provider>
  );
}

export function useWorkspace() {
  const context = useContext(WorkspaceContext);
  if (!context) {
    throw new Error("useWorkspace must be used within WorkspaceProvider");
  }
  return context;
}
