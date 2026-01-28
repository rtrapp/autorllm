import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useCreateProject } from "../hooks/useCreateProject";
import { useUpdateProject } from "../hooks/useUpdateProject";
import { Spinner } from "@/components/ui/spinner";
import type { Project } from "../hooks/useProjects";

interface ProjectFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  project?: Project;
  onSuccess?: () => void;
}

const GENRES = [
  "Fantasia",
  "Ficção Científica",
  "Romance",
  "Mistério",
  "Thriller",
  "Horror",
  "Aventura",
  "Drama",
  "Histórico",
  "Biografia",
  "Não Ficção",
  "Poesia",
  "Outro",
];

export function ProjectFormDialog({ 
  open, 
  onOpenChange, 
  mode, 
  project,
  onSuccess 
}: ProjectFormDialogProps) {
  const navigate = useNavigate();
  const { createProject, isLoading: isCreating, error: createError } = useCreateProject();
  const { updateProject, isLoading: isUpdating, error: updateError } = useUpdateProject();

  const isLoading = isCreating || isUpdating;
  const error = createError || updateError;

  const [formData, setFormData] = useState({
    title: project?.title || "",
    synopsis: project?.synopsis || "",
    genre: project?.genre || "",
  });

  const [validationError, setValidationError] = useState("");

  // Atualizar formData quando o projeto mudar (modo edit)
  useEffect(() => {
    if (mode === "edit" && project) {
      setFormData({
        title: project.title,
        synopsis: project.synopsis,
        genre: project.genre || "",
      });
    }
  }, [mode, project]);

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

    setValidationError("");

    try {
      if (mode === "create") {
        const result = await createProject({
          title: formData.title.trim(),
          author: "Usuário", // TODO: pegar do contexto de autenticação
          synopsis: formData.synopsis.trim(),
          genre: formData.genre || undefined,
        });

        // Resetar formulário
        setFormData({ title: "", synopsis: "", genre: "" });
        
        // Fechar dialog
        onOpenChange(false);

        // Redirecionar para o projeto criado
        navigate(`/projects/${result.projectId}`);
      } else {
        // Modo edit
        if (!project) return;

        await updateProject(project.id, {
          title: formData.title.trim(),
          author: project.author,
          synopsis: formData.synopsis.trim(),
          genre: formData.genre || undefined,
        });

        // Fechar dialog
        onOpenChange(false);

        // Callback de sucesso
        if (onSuccess) {
          onSuccess();
        }
      }
    } catch (err) {
      console.error(`Erro ao ${mode === "create" ? "criar" : "atualizar"} projeto:`, err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      setValidationError("");
      if (mode === "create") {
        setFormData({ title: "", synopsis: "", genre: "" });
      }
      onOpenChange(false);
    }
  };

  const dialogTitle = mode === "create" ? "Criar Novo Livro" : "Editar Projeto";
  const dialogDescription = mode === "create" 
    ? "Preencha as informações básicas do seu projeto. Você poderá editar tudo depois."
    : "Atualize as informações do seu projeto.";
  const submitButtonText = mode === "create" ? "Criar Projeto" : "Salvar Alterações";
  const loadingText = mode === "create" ? "Criando..." : "Salvando...";

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
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
                placeholder="Ex: As Crônicas de Aethel"
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

            {/* Sinopse/Descrição */}
            <div className="space-y-2">
              <Label htmlFor="synopsis" className="text-base">
                Sinopse ou Descrição
              </Label>
              <Textarea
                id="synopsis"
                placeholder="Descreva brevemente sobre o que é seu livro..."
                value={formData.synopsis}
                onChange={(e) => setFormData({ ...formData, synopsis: e.target.value })}
                rows={4}
                disabled={isLoading}
                className="resize-none"
              />
              <p className="text-xs text-muted-foreground">Opcional</p>
            </div>

            {/* Gênero */}
            <div className="space-y-2">
              <Label htmlFor="genre" className="text-base">
                Gênero
              </Label>
              <Select
                value={formData.genre}
                onValueChange={(value) => setFormData({ ...formData, genre: value })}
                disabled={isLoading}
              >
                <SelectTrigger id="genre">
                  <SelectValue placeholder="Selecione um gênero" />
                </SelectTrigger>
                <SelectContent>
                  {GENRES.map((genre) => (
                    <SelectItem key={genre} value={genre}>
                      {genre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <p className="text-xs text-muted-foreground">Opcional</p>
            </div>

            {/* Erro de validação */}
            {(validationError || error) && (
              <div className="bg-destructive/10 border border-destructive/50 text-destructive px-4 py-3 rounded-md text-sm">
                {validationError || error?.message}
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
