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
import type { Plot } from "../types/plot";

interface DeletePlotDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  plot: Plot | null;
  onConfirm: () => void;
  isLoading: boolean;
}

export function DeletePlotDialog({
  open,
  onOpenChange,
  plot,
  onConfirm,
  isLoading,
}: DeletePlotDialogProps) {
  if (!plot) return null;

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Deletar Plot</AlertDialogTitle>
          <AlertDialogDescription>
            Tem certeza que deseja deletar o plot <strong>{plot.title}</strong>?
            Esta ação não pode ser desfeita.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={isLoading}>Cancelar</AlertDialogCancel>
          <AlertDialogAction
            onClick={(e) => {
              e.preventDefault();
              onConfirm();
            }}
            disabled={isLoading}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            {isLoading && <Spinner className="mr-2 size-4" />}
            Deletar
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
