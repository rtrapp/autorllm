import { useState, useEffect } from "react";
import { api } from "@/lib/api";
import type { Location, CreateLocationInput, UpdateLocationInput } from "../types/location";

interface UseLocationsResult {
  locations: Location[];
  isLoading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
  createLocation: (input: CreateLocationInput) => Promise<Location>;
  updateLocation: (input: UpdateLocationInput) => Promise<void>;
  deleteLocation: (locationId: string) => Promise<void>;
}

export function useLocations(projectId: string | undefined): UseLocationsResult {
  const [locations, setLocations] = useState<Location[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchLocations = async () => {
    if (!projectId) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.get<Location[]>(
        `/projects/${projectId}/locations`
      );
      setLocations(response.data);
    } catch (err) {
      console.error("Error fetching locations:", err);
      setError(err instanceof Error ? err : new Error("Failed to fetch locations"));
    } finally {
      setIsLoading(false);
    }
  };

  const createLocation = async (input: CreateLocationInput): Promise<Location> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      const response = await api.post<Location>(
        `/projects/${projectId}/locations`,
        input
      );
      await fetchLocations(); // Refresh list
      return response.data;
    } catch (err) {
      console.error("Error creating location:", err);
      throw err instanceof Error ? err : new Error("Failed to create location");
    }
  };

  const updateLocation = async (input: UpdateLocationInput): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.put(
        `/projects/${projectId}/locations/${input.locationId}`,
        input
      );
      await fetchLocations(); // Refresh list
    } catch (err) {
      console.error("Error updating location:", err);
      throw err instanceof Error ? err : new Error("Failed to update location");
    }
  };

  const deleteLocation = async (locationId: string): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.delete(`/projects/${projectId}/locations/${locationId}`);
      await fetchLocations(); // Refresh list
    } catch (err) {
      console.error("Error deleting location:", err);
      throw err instanceof Error ? err : new Error("Failed to delete location");
    }
  };

  useEffect(() => {
    fetchLocations();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [projectId]);

  return {
    locations,
    isLoading,
    error,
    refetch: fetchLocations,
    createLocation,
    updateLocation,
    deleteLocation,
  };
}
