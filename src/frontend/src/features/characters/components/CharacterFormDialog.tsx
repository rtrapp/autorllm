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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Spinner } from "@/components/ui/spinner";
import { useCreateCharacter } from "../hooks/useCreateCharacter";
import { useUpdateCharacter } from "../hooks/useUpdateCharacter";
import { CHARACTER_ROLES, type Character, type CharacterRole } from "../types";

interface CharacterFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  projectId: string;
  character?: Character;
  onSuccess?: () => void;
}

export function CharacterFormDialog({
  open,
  onOpenChange,
  mode,
  projectId,
  character,
  onSuccess,
}: CharacterFormDialogProps) {
  const { createCharacter, isLoading: isCreating, error: createError } = useCreateCharacter();
  const { updateCharacter, isLoading: isUpdating, error: updateError } = useUpdateCharacter();

  const isLoading = isCreating || isUpdating;
  const error = createError || updateError;

  const [formData, setFormData] = useState({
    name: character?.name || "",
    role: character?.role || "Supporting" as CharacterRole,
    description: character?.description || "",
    backstory: character?.backstory || "",
    appearance: character?.appearance || "",
    personality: character?.personality || "",
  });

  const [validationError, setValidationError] = useState("");

  // Atualizar formData quando o personagem mudar (modo edit)
  useEffect(() => {
    if (mode === "edit" && character) {
      setFormData({
        name: character.name,
        role: character.role,
        description: character.description,
        backstory: character.backstory || "",
        appearance: character.appearance || "",
        personality: character.personality || "",
      });
    }
  }, [mode, character]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validação
    if (!formData.name.trim()) {
      setValidationError("O nome é obrigatório");
      return;
    }

    if (formData.name.length > 100) {
      setValidationError("O nome deve ter no máximo 100 caracteres");
      return;
    }

    setValidationError("");

    try {
      if (mode === "create") {
        await createCharacter({
          projectId,
          name: formData.name.trim(),
          role: formData.role,
          description: formData.description.trim(),
          backstory: formData.backstory?.trim() || null,
          appearance: formData.appearance?.trim() || null,
          personality: formData.personality?.trim() || null,
        });

        // Resetar formulário
        setFormData({
          name: "",
          role: "Supporting",
          description: "",
          backstory: "",
          appearance: "",
          personality: "",
        });
      } else {
        // Modo edit
        if (!character) return;

        await updateCharacter(projectId, character.id, {
          name: formData.name.trim(),
          role: formData.role,
          description: formData.description.trim(),
          backstory: formData.backstory?.trim() || null,
          appearance: formData.appearance?.trim() || null,
          personality: formData.personality?.trim() || null,
        });
      }

      // Fechar dialog
      onOpenChange(false);

      // Callback de sucesso
      if (onSuccess) {
        onSuccess();
      }
    } catch (err) {
      console.error(`Erro ao ${mode === "create" ? "criar" : "atualizar"} personagem:`, err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      setValidationError("");
      if (mode === "create") {
        setFormData({
          name: "",
          role: "Supporting",
          description: "",
          backstory: "",
          appearance: "",
          personality: "",
        });
      }
      onOpenChange(false);
    }
  };

  const dialogTitle = mode === "create" ? "Criar Novo Personagem" : "Editar Personagem";
  const dialogDescription =
    mode === "create"
      ? "Defina as características principais do personagem."
      : "Atualize as informações do personagem.";
  const submitButtonText = mode === "create" ? "Criar Personagem" : "Salvar Alterações";
  const loadingText = mode === "create" ? "Criando..." : "Salvando...";

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[650px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-serif">{dialogTitle}</DialogTitle>
          <DialogDescription>{dialogDescription}</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit}>
          <div className="space-y-6 py-4">
            {/* Nome */}
            <div className="space-y-2">
              <Label htmlFor="name" className="text-base">
                Nome <span className="text-destructive">*</span>
              </Label>
              <Input
                id="name"
                placeholder="Ex: Elara Moonwhisper"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                maxLength={100}
                disabled={isLoading}
                className="text-base"
                autoFocus
              />
              <p className="text-xs text-muted-foreground">
                {formData.name.length}/100 caracteres
              </p>
            </div>

            {/* Papel */}
            <div className="space-y-2">
              <Label htmlFor="role" className="text-base">
                Papel <span className="text-destructive">*</span>
              </Label>
              <Select
                value={formData.role}
                onValueChange={(value) => setFormData({ ...formData, role: value as CharacterRole })}
                disabled={isLoading}
              >
                <SelectTrigger id="role">
                  <SelectValue placeholder="Selecione um papel" />
                </SelectTrigger>
                <SelectContent>
                  {CHARACTER_ROLES.map((role) => (
                    <SelectItem key={role.value} value={role.value}>
                      {role.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Descrição */}
            <div className="space-y-2">
              <Label htmlFor="description" className="text-base">
                Descrição
              </Label>
              <Textarea
                id="description"
                placeholder="Breve resumo do personagem..."
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows={3}
                disabled={isLoading}
                className="resize-none"
              />
            </div>

            {/* Backstory */}
            <div className="space-y-2">
              <Label htmlFor="backstory" className="text-base">
                História (Backstory)
              </Label>
              <Textarea
                id="backstory"
                placeholder="Passado do personagem, origens, eventos importantes..."
                value={formData.backstory}
                onChange={(e) => setFormData({ ...formData, backstory: e.target.value })}
                rows={3}
                disabled={isLoading}
                className="resize-none"
              />
              <p className="text-xs text-muted-foreground">Opcional</p>
            </div>

            {/* Aparência */}
            <div className="space-y-2">
              <Label htmlFor="appearance" className="text-base">
                Aparência
              </Label>
              <Textarea
                id="appearance"
                placeholder="Características físicas, estilo de vestimenta..."
                value={formData.appearance}
                onChange={(e) => setFormData({ ...formData, appearance: e.target.value })}
                rows={2}
                disabled={isLoading}
                className="resize-none"
              />
              <p className="text-xs text-muted-foreground">Opcional</p>
            </div>

            {/* Personalidade */}
            <div className="space-y-2">
              <Label htmlFor="personality" className="text-base">
                Personalidade
              </Label>
              <Textarea
                id="personality"
                placeholder="Traços de personalidade, maneirismos, motivações..."
                value={formData.personality}
                onChange={(e) => setFormData({ ...formData, personality: e.target.value })}
                rows={2}
                disabled={isLoading}
                className="resize-none"
              />
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
            <div className="flex gap-2 ml-auto">
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
            </div>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
