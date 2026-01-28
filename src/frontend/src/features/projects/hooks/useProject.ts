import { useState, useEffect } from "react";
import { api } from "@/lib/api";
import type { Project } from "./useProjects";

export function useProject(id: string | undefined) {
  const [project, setProject] = useState<Project | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const loadProject = async () => {
    if (!id) return;
    
    try {
      setIsLoading(true);
      setError(null);
      const response = await api.get<Project>(`/projects/${id}`);
      setProject(response.data);
    } catch (err) {
      setError(err instanceof Error ? err : new Error("Erro ao carregar projeto"));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadProject();
  }, [id]);

  return {
    project,
    isLoading,
    error,
    refetch: loadProject,
  };
}
