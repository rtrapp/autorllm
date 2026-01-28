export type PlotType = 'Main' | 'Subplot' | 'Character Arc' | 'Romance' | 'Mystery';

export interface Plot {
  id: string;
  projectId: string;
  title: string;
  description: string;
  type: PlotType;
  resolution?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreatePlotInput {
  projectId: string;
  title: string;
  description: string;
  type: PlotType;
  resolution?: string;
}

export interface UpdatePlotInput {
  projectId: string;
  plotId: string;
  title: string;
  description: string;
  type: PlotType;
  resolution?: string;
}
