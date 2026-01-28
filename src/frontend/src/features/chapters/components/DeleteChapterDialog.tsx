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
import type { Chapter } from "../types/chapter";

interface DeleteChapterDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  chapter: Chapter | null;
  onDelete: (chapterId: string) => Promise<void>;
  isLoading?: boolean;
}

export function DeleteChapterDialog({
  open,
  onOpenChange,
  chapter,
  onDelete,
  isLoading = false,
}: DeleteChapterDialogProps) {
  const handleDelete = async () => {
    if (!chapter) return;

    try {
      await onDelete(chapter.id);
      onOpenChange(false);
    } catch (err) {
      console.error("Erro ao deletar capítulo:", err);
    }
  };

  if (!chapter) return null;

  const hasContent = chapter.content && chapter.content.trim().length > 0;

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Deletar Capítulo</AlertDialogTitle>
          <AlertDialogDescription>
            {hasContent ? (
              <>
                ⚠️ <strong>Atenção:</strong> O capítulo <strong>"{chapter.title}"</strong> contém{" "}
                <strong>{chapter.wordCount} palavras</strong> de conteúdo escrito.
                <br />
                <br />
                Tem certeza que deseja deletar este capítulo? Esta ação não pode ser desfeita e
                todo o conteúdo será permanentemente perdido.
              </>
            ) : (
              <>
                Tem certeza que deseja deletar o capítulo <strong>"{chapter.title}"</strong>?
                Esta ação não pode ser desfeita.
              </>
            )}
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
