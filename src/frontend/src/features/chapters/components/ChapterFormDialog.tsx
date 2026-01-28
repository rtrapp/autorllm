import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Spinner } from "@/components/ui/spinner";
import type { Chapter, CreateChapterInput, UpdateChapterInput } from "../types/chapter";

interface ChapterFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  chapter?: Chapter;
  onSubmit: (data: CreateChapterInput | UpdateChapterInput) => Promise<void>;
  isLoading?: boolean;
}

export function ChapterFormDialog({
  open,
  onOpenChange,
  mode,
  chapter,
  onSubmit,
  isLoading = false,
}: ChapterFormDialogProps) {
  const [formData, setFormData] = useState({
    title: chapter?.title || "",
    summary: chapter?.summary || "",
  });

  const [validationError, setValidationError] = useState("");

  // Atualizar formData quando o capítulo mudar (modo edit)
  useEffect(() => {
    if (mode === "edit" && chapter) {
      setFormData({
        title: chapter.title,
        summary: chapter.summary || "",
      });
    }
  }, [mode, chapter]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validação
    if (!formData.title.trim()) {
      setValidationError("O título é obrigatório");
      return;
    }

    if (formData.title.length > 200) {
      setValidationError("O título deve ter no máximo 200 caracteres");
      return;
    }

    if (formData.summary && formData.summary.length > 1000) {
      setValidationError("O resumo deve ter no máximo 1000 caracteres");
      return;
    }

    setValidationError("");

    try {
      if (mode === "create") {
        await onSubmit({
          title: formData.title.trim(),
          summary: formData.summary?.trim() || undefined,
        });

        // Resetar formulário
        setFormData({
          title: "",
          summary: "",
        });
      } else {
        // Modo edit
        if (!chapter) return;

        await onSubmit({
          chapterId: chapter.id,
          title: formData.title.trim(),
          summary: formData.summary?.trim() || undefined,
        });
      }

      // Fechar dialog
      onOpenChange(false);
    } catch (err) {
      console.error(`Erro ao ${mode === "create" ? "criar" : "atualizar"} capítulo:`, err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      setValidationError("");
      if (mode === "create") {
        setFormData({
          title: "",
          summary: "",
        });
      }
      onOpenChange(false);
    }
  };

  const dialogTitle = mode === "create" ? "Criar Novo Capítulo" : "Editar Capítulo";
  const dialogDescription =
    mode === "create"
      ? "Defina o título e um resumo opcional para o capítulo."
      : "Atualize as informações do capítulo.";
  const submitButtonText = mode === "create" ? "Criar Capítulo" : "Salvar Alterações";
  const loadingText = mode === "create" ? "Criando..." : "Salvando...";

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[550px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-serif">{dialogTitle}</DialogTitle>
          <DialogDescription>{dialogDescription}</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit}>
          <div className="space-y-6 py-4">
            {/* Título */}
            <div className="space-y-2">
              <Label htmlFor="title" className="text-base">
                Título <span className="text-destructive">*</span>
              </Label>
              <Input
                id="title"
                placeholder="Ex: A Vila Silenciosa"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                maxLength={200}
                disabled={isLoading}
                className="text-base"
                autoFocus
              />
              <p className="text-xs text-muted-foreground">
                {formData.title.length}/200 caracteres
              </p>
            </div>

            {/* Resumo */}
            <div className="space-y-2">
              <Label htmlFor="summary" className="text-base">
                Resumo <span className="text-muted-foreground">(opcional)</span>
              </Label>
              <Textarea
                id="summary"
                placeholder="Ex: Elara e Kael aguardam a chegada dos mercadores em uma vila envolta em névoa misteriosa..."
                value={formData.summary}
                onChange={(e) => setFormData({ ...formData, summary: e.target.value })}
                maxLength={1000}
                disabled={isLoading}
                className="min-h-[120px] text-base"
              />
              <p className="text-xs text-muted-foreground">
                {formData.summary.length}/1000 caracteres
              </p>
            </div>

            {/* Mensagem de erro */}
            {validationError && (
              <div className="text-sm text-destructive bg-destructive/10 p-3 rounded-md">
                {validationError}
              </div>
            )}

            {/* Nota sobre ordem */}
            {mode === "create" && (
              <div className="text-sm text-muted-foreground bg-secondary/50 p-3 rounded-md">
                💡 O capítulo será criado automaticamente com a próxima ordem sequencial disponível.
              </div>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={isLoading}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? (
                <>
                  <Spinner className="mr-2 h-4 w-4" />
                  {loadingText}
                </>
              ) : (
                submitButtonText
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
