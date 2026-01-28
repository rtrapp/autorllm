import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Spinner } from "@/components/ui/spinner";
import { Users, Plus, Edit2, Trash2 } from "lucide-react";
import { CharacterFormDialog } from "./CharacterFormDialog";
import { DeleteCharacterDialog } from "./DeleteCharacterDialog";
import { useCharacters } from "../hooks/useCharacters";
import type { Character } from "../types";
import { CHARACTER_ROLES } from "../types";

interface CharactersListProps {
  projectId: string;
}

export function CharactersList({ projectId }: CharactersListProps) {
  const { characters, isLoading, refetch } = useCharacters(projectId);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [editingCharacter, setEditingCharacter] = useState<Character | null>(null);
  const [deletingCharacter, setDeletingCharacter] = useState<Character | null>(null);

  const getRoleLabel = (role: string) => {
    return CHARACTER_ROLES.find((r) => r.value === role)?.label || role;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Spinner className="h-8 w-8" />
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Users className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-xl font-semibold">Personagens</h2>
          <span className="text-sm text-muted-foreground">({characters.length})</span>
        </div>
        <Button onClick={() => setIsCreateDialogOpen(true)} size="sm" className="gap-2">
          <Plus className="h-4 w-4" />
          Novo Personagem
        </Button>
      </div>

      {/* Lista de personagens */}
      {characters.length === 0 ? (
        <Card className="p-8 text-center">
          <Users className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
          <h3 className="text-lg font-semibold mb-2">Nenhum personagem criado</h3>
          <p className="text-sm text-muted-foreground mb-4">
            Comece criando os personagens da sua história
          </p>
          <Button onClick={() => setIsCreateDialogOpen(true)} className="gap-2">
            <Plus className="h-4 w-4" />
            Criar Primeiro Personagem
          </Button>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {characters.map((character) => (
            <Card key={character.id} className="p-4 hover:shadow-md transition-shadow">
              <div className="space-y-3">
                {/* Nome e Papel */}
                <div>
                  <h3 className="font-semibold text-lg">{character.name}</h3>
                  <p className="text-sm text-muted-foreground">{getRoleLabel(character.role)}</p>
                </div>

                {/* Descrição */}
                {character.description && (
                  <p className="text-sm text-muted-foreground line-clamp-3">
                    {character.description}
                  </p>
                )}

                {/* Traits resumidos */}
                <div className="text-xs text-muted-foreground space-y-1">
                  {character.backstory && (
                    <p className="line-clamp-2">
                      <span className="font-medium">Backstory:</span> {character.backstory}
                    </p>
                  )}
                  {character.appearance && (
                    <p className="line-clamp-1">
                      <span className="font-medium">Aparência:</span> {character.appearance}
                    </p>
                  )}
                  {character.personality && (
                    <p className="line-clamp-1">
                      <span className="font-medium">Personalidade:</span> {character.personality}
                    </p>
                  )}
                </div>

                {/* Ações */}
                <div className="flex gap-2 pt-2 border-t">
                  <Button
                    variant="outline"
                    size="sm"
                    className="flex-1 gap-2"
                    onClick={() => setEditingCharacter(character)}
                  >
                    <Edit2 className="h-3 w-3" />
                    Editar
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    className="gap-2 text-destructive hover:bg-destructive hover:text-destructive-foreground"
                    onClick={() => setDeletingCharacter(character)}
                  >
                    <Trash2 className="h-3 w-3" />
                    Deletar
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Dialogs */}
      <CharacterFormDialog
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
        mode="create"
        projectId={projectId}
        onSuccess={refetch}
      />

      <CharacterFormDialog
        open={!!editingCharacter}
        onOpenChange={(open) => !open && setEditingCharacter(null)}
        mode="edit"
        projectId={projectId}
        character={editingCharacter || undefined}
        onSuccess={() => {
          refetch();
          setEditingCharacter(null);
        }}
      />

      <DeleteCharacterDialog
        open={!!deletingCharacter}
        onOpenChange={(open) => !open && setDeletingCharacter(null)}
        character={deletingCharacter}
        projectId={projectId}
        onSuccess={() => {
          refetch();
          setDeletingCharacter(null);
        }}
      />
    </div>
  );
}
