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
import type { Location } from "../types/location";

interface DeleteLocationDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  location: Location | null;
  onDelete: () => Promise<void>;
  isLoading: boolean;
}

export function DeleteLocationDialog({
  open,
  onOpenChange,
  location,
  onDelete,
  isLoading,
}: DeleteLocationDialogProps) {
  const handleDelete = async () => {
    try {
      await onDelete();
      onOpenChange(false);
    } catch (err) {
      console.error("Erro ao deletar local:", err);
    }
  };

  if (!location) return null;

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Deletar Local</AlertDialogTitle>
          <AlertDialogDescription>
            Tem certeza que deseja deletar o local <strong>{location.name}</strong>?
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
