import { useState } from "react";
import { api } from "@/lib/api";

interface CreateProjectData {
  title: string;
  author: string;
  synopsis: string;
  genre?: string;
}

interface CreateProjectResult {
  projectId: string;
}

export function useCreateProject() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const createProject = async (data: CreateProjectData): Promise<CreateProjectResult> => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await api.post<CreateProjectResult>("/projects", data);

      return response.data;
    } catch (err) {
      const errorObj = err instanceof Error ? err : new Error("Erro ao criar projeto");
      setError(errorObj);
      throw errorObj;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    createProject,
    isLoading,
    error,
  };
}
