import { useState, useEffect } from "react";
import { api } from "@/lib/api";

export interface Project {
  id: string;
  title: string;
  author: string;
  synopsis: string;
  genre?: string;
  targetWordCount: number;
  currentWordCount: number;
  createdAt: string;
  updatedAt: string;
}

export function useProjects() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    loadProjects();
  }, []);

  const loadProjects = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const response = await api.get<Project[]>("/projects");
      setProjects(response.data);
    } catch (err) {
      setError(err instanceof Error ? err : new Error("Erro ao carregar projetos"));
    } finally {
      setIsLoading(false);
    }
  };

  const deleteProject = async (projectId: string) => {
    try {
      await api.delete(`/projects/${projectId}`);
      // Atualiza a lista local removendo o projeto deletado
      setProjects((prev) => prev.filter((p) => p.id !== projectId));
    } catch (err) {
      throw err instanceof Error ? err : new Error("Erro ao deletar projeto");
    }
  };

  return {
    projects,
    isLoading,
    error,
    refetch: loadProjects,
    deleteProject,
  };
}
