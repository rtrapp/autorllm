import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Spinner } from "@/components/ui/spinner";
import { useDeleteCharacter } from "../hooks/useDeleteCharacter";
import type { Character } from "../types";

interface DeleteCharacterDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  character: Character | null;
  projectId: string;
  onSuccess?: () => void;
}

export function DeleteCharacterDialog({
  open,
  onOpenChange,
  character,
  projectId,
  onSuccess,
}: DeleteCharacterDialogProps) {
  const { deleteCharacter, isLoading } = useDeleteCharacter();

  const handleDelete = async () => {
    if (!character) return;

    try {
      await deleteCharacter(projectId, character.id);
      onOpenChange(false);
      if (onSuccess) {
        onSuccess();
      }
    } catch (err) {
      console.error("Erro ao deletar personagem:", err);
    }
  };

  if (!character) return null;

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Deletar Personagem</AlertDialogTitle>
          <AlertDialogDescription>
            Tem certeza que deseja deletar o personagem <strong>{character.name}</strong>?
            Esta ação não pode ser desfeita.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={isLoading}>Cancelar</AlertDialogCancel>
          <AlertDialogAction
            onClick={handleDelete}
            disabled={isLoading}
            className="bg-destructive hover:bg-destructive/90"
          >
            {isLoading ? (
              <>
                <Spinner className="mr-2 h-4 w-4" />
                Deletando...
              </>
            ) : (
              "Deletar"
            )}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
