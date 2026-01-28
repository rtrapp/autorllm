import { useState, useEffect } from "react";
import { api } from "@/lib/api";
import type { Character } from "../types";

interface UseCharactersResult {
  characters: Character[];
  isLoading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
}

export function useCharacters(projectId: string | undefined): UseCharactersResult {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchCharacters = async () => {
    if (!projectId) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.get<Character[]>(
        `/projects/${projectId}/characters`
      );
      setCharacters(response.data);
    } catch (err) {
      console.error("Error fetching characters:", err);
      setError(err instanceof Error ? err : new Error("Failed to fetch characters"));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchCharacters();
  }, [projectId]);

  return {
    characters,
    isLoading,
    error,
    refetch: fetchCharacters,
  };
}
