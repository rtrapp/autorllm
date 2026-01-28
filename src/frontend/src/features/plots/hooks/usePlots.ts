import { useState, useEffect } from "react";
import { api } from "@/lib/api";
import type { Plot, CreatePlotInput, UpdatePlotInput } from "../types/plot";

interface UsePlotsResult {
  plots: Plot[];
  isLoading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
  createPlot: (input: CreatePlotInput) => Promise<Plot>;
  updatePlot: (input: UpdatePlotInput) => Promise<void>;
  deletePlot: (plotId: string) => Promise<void>;
}

export function usePlots(projectId: string | undefined): UsePlotsResult {
  const [plots, setPlots] = useState<Plot[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchPlots = async () => {
    if (!projectId) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.get<Plot[]>(
        `/projects/${projectId}/plots`
      );
      setPlots(response.data);
    } catch (err) {
      console.error("Error fetching plots:", err);
      setError(err instanceof Error ? err : new Error("Failed to fetch plots"));
    } finally {
      setIsLoading(false);
    }
  };

  const createPlot = async (input: CreatePlotInput): Promise<Plot> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      const response = await api.post<Plot>(
        `/projects/${projectId}/plots`,
        input
      );
      await fetchPlots(); // Refresh list
      return response.data;
    } catch (err) {
      console.error("Error creating plot:", err);
      throw err instanceof Error ? err : new Error("Failed to create plot");
    }
  };

  const updatePlot = async (input: UpdatePlotInput): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.put(
        `/projects/${projectId}/plots/${input.plotId}`,
        input
      );
      await fetchPlots(); // Refresh list
    } catch (err) {
      console.error("Error updating plot:", err);
      throw err instanceof Error ? err : new Error("Failed to update plot");
    }
  };

  const deletePlot = async (plotId: string): Promise<void> => {
    if (!projectId) {
      throw new Error("Project ID is required");
    }

    try {
      await api.delete(`/projects/${projectId}/plots/${plotId}`);
      await fetchPlots(); // Refresh list
    } catch (err) {
      console.error("Error deleting plot:", err);
      throw err instanceof Error ? err : new Error("Failed to delete plot");
    }
  };

  useEffect(() => {
    fetchPlots();
  }, [projectId]);

  return {
    plots,
    isLoading,
    error,
    refetch: fetchPlots,
    createPlot,
    updatePlot,
    deletePlot,
  };
}
