import { useState } from "react";
import { api } from "@/lib/api";
import type { UpdateCharacterRequest } from "../types";

interface UseUpdateCharacterResult {
  updateCharacter: (projectId: string, characterId: string, data: UpdateCharacterRequest) => Promise<void>;
  isLoading: boolean;
  error: Error | null;
}

export function useUpdateCharacter(): UseUpdateCharacterResult {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const updateCharacter = async (
    projectId: string,
    characterId: string,
    data: UpdateCharacterRequest
  ): Promise<void> => {
    setIsLoading(true);
    setError(null);

    try {
      await api.put(
        `/projects/${projectId}/characters/${characterId}`,
        {
          ...data,
          projectId,
          characterId,
        }
      );
    } catch (err) {
      console.error("Error updating character:", err);
      const error = err instanceof Error ? err : new Error("Failed to update character");
      setError(error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    updateCharacter,
    isLoading,
    error,
  };
}
