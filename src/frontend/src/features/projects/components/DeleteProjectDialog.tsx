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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useState } from "react";
import type { Project } from "../hooks/useProjects";

interface DeleteProjectDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  project: Project | null;
  onConfirm: () => void;
}

export function DeleteProjectDialog({
  open,
  onOpenChange,
  project,
  onConfirm,
}: DeleteProjectDialogProps) {
  const [confirmText, setConfirmText] = useState("");

  if (!project) return null;

  const hasContent = project.currentWordCount > 0;
  const isConfirmValid = confirmText === project.title;

  const handleConfirm = () => {
    // Se tem conteúdo, precisa confirmar digitando o título
    // Se não tem conteúdo, pode deletar direto
    if (!hasContent || isConfirmValid) {
      onConfirm();
      setConfirmText("");
      onOpenChange(false);
    }
  };

  const handleCancel = () => {
    setConfirmText("");
    onOpenChange(false);
  };

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle className="text-destructive">
            Deletar Projeto
          </AlertDialogTitle>
          <AlertDialogDescription asChild>
            <div className="space-y-4">
              {hasContent ? (
                <>
                  <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-md">
                    <p className="font-semibold text-destructive text-sm">
                      ⚠️ Atenção: Todos os dados serão perdidos permanentemente
                    </p>
                    <p className="text-sm text-muted-foreground mt-2">
                      Este projeto contém capítulos, personagens e outros dados que
                      serão deletados permanentemente. Esta ação não pode ser desfeita.
                    </p>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="confirm-title">
                      Digite o título do projeto para confirmar:
                    </Label>
                    <div className="font-mono text-sm bg-secondary px-2 py-1 rounded">
                      {project.title}
                    </div>
                    <Input
                      id="confirm-title"
                      value={confirmText}
                      onChange={(e) => setConfirmText(e.target.value)}
                      placeholder="Digite o título exato do projeto"
                      className={!isConfirmValid && confirmText ? "border-destructive" : ""}
                    />
                  </div>
                </>
              ) : (
                <div>
                  Tem certeza que deseja deletar o projeto{" "}
                  <span className="font-semibold">{project.title}</span>? Esta
                  ação não pode ser desfeita.
                </div>
              )}
            </div>
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={handleCancel}>Cancelar</AlertDialogCancel>
          <AlertDialogAction
            onClick={handleConfirm}
            disabled={hasContent && !isConfirmValid}
            className="bg-destructive hover:bg-destructive/90"
          >
            Confirmar Exclusão
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
