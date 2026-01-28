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
import type { Location, CreateLocationInput, UpdateLocationInput } from "../types/location";

interface LocationFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  projectId: string;
  location?: Location;
  onSuccess?: () => void;
  onSubmit: (data: CreateLocationInput | UpdateLocationInput) => Promise<void>;
  isLoading: boolean;
}

export function LocationFormDialog({
  open,
  onOpenChange,
  mode,
  projectId,
  location,
  onSuccess,
  onSubmit,
  isLoading,
}: LocationFormDialogProps) {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    geography: "",
    culture: "",
    significance: "",
  });

  const [validationError, setValidationError] = useState("");

  // Reset form data quando o dialog abre ou location muda
  useEffect(() => {
    if (open) {
      if (mode === "edit" && location) {
        setFormData({
          name: location.name,
          description: location.description,
          geography: location.geography || "",
          culture: location.culture || "",
          significance: location.significance || "",
        });
      } else {
        setFormData({
          name: "",
          description: "",
          geography: "",
          culture: "",
          significance: "",
        });
      }
    }
  }, [open, mode, location]);

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
        await onSubmit({
          projectId,
          name: formData.name.trim(),
          description: formData.description.trim(),
          geography: formData.geography?.trim() || undefined,
          culture: formData.culture?.trim() || undefined,
          significance: formData.significance?.trim() || undefined,
        });

        // Resetar formulário
        setFormData({
          name: "",
          description: "",
          geography: "",
          culture: "",
          significance: "",
        });
      } else {
        // Modo edit
        if (!location) return;

        await onSubmit({
          projectId,
          locationId: location.id,
          name: formData.name.trim(),
          description: formData.description.trim(),
          geography: formData.geography?.trim() || undefined,
          culture: formData.culture?.trim() || undefined,
          significance: formData.significance?.trim() || undefined,
        });
      }

      // Fechar dialog
      onOpenChange(false);

      // Callback de sucesso
      if (onSuccess) {
        onSuccess();
      }
    } catch (err) {
      console.error(`Erro ao ${mode === "create" ? "criar" : "atualizar"} local:`, err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      setValidationError("");
      if (mode === "create") {
        setFormData({
          name: "",
          description: "",
          geography: "",
          culture: "",
          significance: "",
        });
      }
      onOpenChange(false);
    }
  };

  const dialogTitle = mode === "create" ? "Criar Novo Local" : "Editar Local";
  const dialogDescription =
    mode === "create"
      ? "Descreva um novo ambiente, cidade ou região da sua história."
      : "Atualize as informações do local.";
  const submitButtonText = mode === "create" ? "Criar Local" : "Salvar Alterações";
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
                placeholder="Ex: Cidadela de Ferro"
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

            {/* Descrição */}
            <div className="space-y-2">
              <Label htmlFor="description" className="text-base">
                Descrição
              </Label>
              <Textarea
                id="description"
                placeholder="Atmosfera, cheiros, sons, sensações gerais do local..."
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows={3}
                disabled={isLoading}
                className="resize-none"
              />
            </div>

            {/* Geografia */}
            <div className="space-y-2">
              <Label htmlFor="geography" className="text-base">
                Geografia
              </Label>
              <Textarea
                id="geography"
                placeholder="Clima, terreno, localização, características físicas..."
                value={formData.geography}
                onChange={(e) => setFormData({ ...formData, geography: e.target.value })}
                rows={3}
                disabled={isLoading}
                className="resize-none"
              />
            </div>

            {/* Cultura */}
            <div className="space-y-2">
              <Label htmlFor="culture" className="text-base">
                Cultura
              </Label>
              <Textarea
                id="culture"
                placeholder="Costumes, tradições, sociedade do local..."
                value={formData.culture}
                onChange={(e) => setFormData({ ...formData, culture: e.target.value })}
                rows={3}
                disabled={isLoading}
                className="resize-none"
              />
            </div>

            {/* Significância */}
            <div className="space-y-2">
              <Label htmlFor="significance" className="text-base">
                Significância na História
              </Label>
              <Textarea
                id="significance"
                placeholder="Importância narrativa, eventos importantes que ocorrem aqui..."
                value={formData.significance}
                onChange={(e) => setFormData({ ...formData, significance: e.target.value })}
                rows={2}
                disabled={isLoading}
                className="resize-none"
              />
            </div>
          </div>

          {/* Validation Error */}
          {validationError && (
            <div className="text-sm text-destructive mb-4">{validationError}</div>
          )}

          <DialogFooter className="gap-2">
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
