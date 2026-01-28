import { useState } from "react";
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
import { Spinner } from "@/components/ui/spinner";

interface NewProjectDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
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

const LANGUAGES = [
  { value: "pt-BR", label: "Português (Brasil)" },
  { value: "pt-PT", label: "Português (Portugal)" },
  { value: "en-US", label: "Inglês (EUA)" },
  { value: "en-GB", label: "Inglês (UK)" },
  { value: "es-ES", label: "Espanhol" },
  { value: "fr-FR", label: "Francês" },
];

export function NewProjectDialog({ open, onOpenChange }: NewProjectDialogProps) {
  const navigate = useNavigate();
  const { createProject, isLoading, error } = useCreateProject();

  const [formData, setFormData] = useState({
    title: "",
    synopsis: "",
    genre: "",
    language: "pt-BR",
  });

  const [validationError, setValidationError] = useState("");

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
      const result = await createProject({
        title: formData.title.trim(),
        author: "Usuário", // TODO: pegar do contexto de autenticação
        synopsis: formData.synopsis.trim(),
        genre: formData.genre || undefined,
      });

      // Resetar formulário
      setFormData({
        title: "",
        synopsis: "",
        genre: "",
        language: "pt-BR",
      });

      // Fechar dialog
      onOpenChange(false);

      // Redirecionar para o projeto criado
      navigate(`/projects/${result.projectId}`);
    } catch (err) {
      // Erro já está sendo tratado pelo hook
      console.error("Erro ao criar projeto:", err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      // Resetar validação ao fechar
      setValidationError("");
      onOpenChange(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle className="text-2xl font-serif">Criar Novo Livro</DialogTitle>
          <DialogDescription>
            Preencha as informações básicas do seu projeto. Você poderá editar tudo depois.
          </DialogDescription>
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

            {/* Gênero e Idioma lado a lado */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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

              {/* Idioma */}
              <div className="space-y-2">
                <Label htmlFor="language" className="text-base">
                  Idioma
                </Label>
                <Select
                  value={formData.language}
                  onValueChange={(value) => setFormData({ ...formData, language: value })}
                  disabled={isLoading}
                >
                  <SelectTrigger id="language">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {LANGUAGES.map((lang) => (
                      <SelectItem key={lang.value} value={lang.value}>
                        {lang.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <p className="text-xs text-muted-foreground">Opcional</p>
              </div>
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
                  Criando...
                </>
              ) : (
                "Criar Projeto"
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
