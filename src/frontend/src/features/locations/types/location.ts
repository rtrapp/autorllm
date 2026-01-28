export interface Location {
  id: string;
  projectId: string;
  name: string;
  description: string;
  geography?: string | null;
  culture?: string | null;
  significance?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateLocationInput {
  projectId: string;
  name: string;
  description: string;
  geography?: string;
  culture?: string;
  significance?: string;
}

export interface UpdateLocationInput {
  projectId: string;
  locationId: string;
  name: string;
  description: string;
  geography?: string;
  culture?: string;
  significance?: string;
}
