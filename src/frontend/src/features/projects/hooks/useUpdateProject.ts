import { useState } from "react";
import { api } from "@/lib/api";

export interface UpdateProjectData {
  title: string;
  author: string;
  synopsis: string;
  genre?: string;
}

export function useUpdateProject() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const updateProject = async (projectId: string, data: UpdateProjectData) => {
    try {
      setIsLoading(true);
      setError(null);
      
      await api.put(`/projects/${projectId}`, {
        projectId,
        ...data,
      });
      
      return { success: true };
    } catch (err) {
      const error = err instanceof Error ? err : new Error("Erro ao atualizar projeto");
      setError(error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    updateProject,
    isLoading,
    error,
  };
}
