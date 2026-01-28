export interface Character {
  id: string;
  projectId: string;
  name: string;
  description: string;
  role: CharacterRole;
  backstory?: string;
  appearance?: string;
  personality?: string;
  createdAt: string;
  updatedAt: string;
}

export type CharacterRole = "Protagonist" | "Antagonist" | "Supporting" | "Minor";

export const CHARACTER_ROLES: { value: CharacterRole; label: string }[] = [
  { value: "Protagonist", label: "Protagonista" },
  { value: "Antagonist", label: "Antagonista" },
  { value: "Supporting", label: "Suporte" },
  { value: "Minor", label: "Menor" },
];

export interface CreateCharacterRequest {
  projectId: string;
  name: string;
  role: string;
  description: string;
  backstory?: string | null;
  appearance?: string | null;
  personality?: string | null;
}

export interface UpdateCharacterRequest {
  name: string;
  role: string;
  description: string;
  backstory?: string | null;
  appearance?: string | null;
  personality?: string | null;
}

export interface CreateCharacterResponse {
  characterId: string;
}
