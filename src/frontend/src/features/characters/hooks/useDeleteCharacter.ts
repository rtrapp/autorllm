import { useState } from "react";
import { api } from "@/lib/api";

interface UseDeleteCharacterResult {
  deleteCharacter: (projectId: string, characterId: string) => Promise<void>;
  isLoading: boolean;
  error: Error | null;
}

export function useDeleteCharacter(): UseDeleteCharacterResult {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const deleteCharacter = async (projectId: string, characterId: string): Promise<void> => {
    setIsLoading(true);
    setError(null);

    try {
      await api.delete(`/projects/${projectId}/characters/${characterId}`);
    } catch (err) {
      console.error("Error deleting character:", err);
      const error = err instanceof Error ? err : new Error("Failed to delete character");
      setError(error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    deleteCharacter,
    isLoading,
    error,
  };
}
