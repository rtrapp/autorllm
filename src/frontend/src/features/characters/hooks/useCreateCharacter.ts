import { useState } from "react";
import { api } from "@/lib/api";
import type { CreateCharacterRequest, CreateCharacterResponse } from "../types";

interface UseCreateCharacterResult {
  createCharacter: (data: CreateCharacterRequest) => Promise<CreateCharacterResponse>;
  isLoading: boolean;
  error: Error | null;
}

export function useCreateCharacter(): UseCreateCharacterResult {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const createCharacter = async (data: CreateCharacterRequest): Promise<CreateCharacterResponse> => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await api.post<CreateCharacterResponse>(
        `/projects/${data.projectId}/characters`,
        data
      );
      return response.data;
    } catch (err) {
      console.error("Error creating character:", err);
      const error = err instanceof Error ? err : new Error("Failed to create character");
      setError(error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return {
    createCharacter,
    isLoading,
    error,
  };
}
