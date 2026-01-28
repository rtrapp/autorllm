import { useState, useEffect, useCallback } from "react";
import { api } from "@/lib/api";
import type { Chapter, CreateChapterInput, UpdateChapterInput } from "../types/chapter";

interface UseChaptersResult {
  chapters: Chapter[];
  isLoading: boolean;
  error: Error | null;
  createChapter: (input: CreateChapterInput) => Promise<Chapter>;
  updateChapter: (input: UpdateChapterInput) => Promise<void>;
  deleteChapter: (chapterId: string) => Promise<void>;
  reorderChapters: (chapterIds: string[]) => Promise<void>;
  refetch: () => Promise<void>;
}

export function useChapters(projectId: string | undefined): UseChaptersResult {
  const [chapters, setChapters] = useState<Chapter[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchChapters = useCallback(async () => {
    if (!projectId) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.get<Chapter[]>(
        `/projects/${projectId}/chapters`
      );
      // Sort by order to ensure correct sequence
      const sortedChapters = response.data.sort((a, b) => a.order - b.order);
      setChapters(sortedChapters);
    } catch (err) {
      console.error("Error fetching chapters:", err);
      setError(err instanceof Error ? err : new Error("Failed to fetch chapters"));
    } finally {
      setIsLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    fetchChapters();
  }, [fetchChapters]);

  const createChapter = async (input: CreateChapterInput): Promise<Chapter> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      const response = await api.post<Chapter>(
        `/projects/${projectId}/chapters`,
        input
      );
      
      await fetchChapters(); // Refresh list
      return response.data;
    } catch (err) {
      console.error("Error creating chapter:", err);
      throw err instanceof Error ? err : new Error("Failed to create chapter");
    }
  };

  const updateChapter = async (input: UpdateChapterInput): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.put(
        `/projects/${projectId}/chapters/${input.chapterId}`,
        input
      );
      
      await fetchChapters(); // Refresh list
    } catch (err) {
      console.error("Error updating chapter:", err);
      throw err instanceof Error ? err : new Error("Failed to update chapter");
    }
  };

  const deleteChapter = async (chapterId: string): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.delete(`/projects/${projectId}/chapters/${chapterId}`);
      
      await fetchChapters(); // Refresh list
    } catch (err) {
      console.error("Error deleting chapter:", err);
      throw err instanceof Error ? err : new Error("Failed to delete chapter");
    }
  };

  const reorderChapters = async (chapterIds: string[]): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.put(`/projects/${projectId}/chapters/reorder`, {
        chapterIds,
      });
      
      await fetchChapters(); // Refresh list
    } catch (err) {
      console.error("Error reordering chapters:", err);
      throw err instanceof Error ? err : new Error("Failed to reorder chapters");
    }
  };

  return {
    chapters,
    isLoading,
    error,
    createChapter,
    updateChapter,
    deleteChapter,
    reorderChapters,
    refetch: fetchChapters,
  };
}
