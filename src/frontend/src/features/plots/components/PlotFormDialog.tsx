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
import type { Plot, CreatePlotInput, UpdatePlotInput, PlotType } from "../types/plot";

interface PlotFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  projectId: string;
  plot?: Plot;
  onSuccess?: () => void;
  onSubmit: (data: CreatePlotInput | UpdatePlotInput) => Promise<void>;
  isLoading: boolean;
}

export function PlotFormDialog({
  open,
  onOpenChange,
  mode,
  projectId,
  plot,
  onSuccess,
  onSubmit,
  isLoading,
}: PlotFormDialogProps) {
  const [formData, setFormData] = useState({
    title: "",
    description: "",
    type: "Main" as PlotType,
    resolution: "",
  });

  const [validationError, setValidationError] = useState("");

  // Reset form data quando o dialog abre ou plot muda
  useEffect(() => {
    if (open) {
      if (mode === "edit" && plot) {
        setFormData({
          title: plot.title,
          description: plot.description,
          type: plot.type,
          resolution: plot.resolution || "",
        });
      } else {
        setFormData({
          title: "",
          description: "",
          type: "Main",
          resolution: "",
        });
      }
    }
  }, [open, mode, plot]);

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
        await onSubmit({
          projectId,
          title: formData.title.trim(),
          description: formData.description.trim(),
          type: formData.type,
          resolution: formData.resolution.trim() || undefined,
        });

        // Resetar formulário
        setFormData({
          title: "",
          description: "",
          type: "Main",
          resolution: "",
        });
      } else {
        // Modo edit
        if (!plot) return;

        await onSubmit({
          projectId,
          plotId: plot.id,
          title: formData.title.trim(),
          description: formData.description.trim(),
          type: formData.type,
          resolution: formData.resolution.trim() || undefined,
        });
      }

      // Fechar dialog
      onOpenChange(false);

      // Callback de sucesso
      if (onSuccess) {
        onSuccess();
      }
    } catch (err) {
      console.error(`Erro ao ${mode === "create" ? "criar" : "atualizar"} plot:`, err);
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen && !isLoading) {
      setValidationError("");
      if (mode === "create") {
        setFormData({
          title: "",
          description: "",
          type: "Main",
          resolution: "",
        });
      }
      onOpenChange(false);
    }
  };

  const dialogTitle = mode === "create" ? "Criar Novo Plot" : "Editar Plot";
  const dialogDescription =
    mode === "create"
      ? "Defina o arco narrativo do seu projeto."
      : "Atualize as informações do arco narrativo.";

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>{dialogTitle}</DialogTitle>
          <DialogDescription>{dialogDescription}</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit}>
          <div className="grid gap-4 py-4">
            {/* Título */}
            <div className="grid gap-2">
              <Label htmlFor="title">
                Título <span className="text-destructive">*</span>
              </Label>
              <Input
                id="title"
                name="title"
                placeholder="Ex: A Jornada do Herói"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                disabled={isLoading}
                required
                maxLength={200}
                autoComplete="off"
              />
            </div>

            {/* Tipo */}
            <div className="grid gap-2">
              <Label htmlFor="type">
                Tipo <span className="text-destructive">*</span>
              </Label>
              <Select
                value={formData.type}
                onValueChange={(value: PlotType) =>
                  setFormData({ ...formData, type: value })
                }
                disabled={isLoading}
              >
                <SelectTrigger id="type">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Main">Plot Principal</SelectItem>
                  <SelectItem value="Subplot">Subplot</SelectItem>
                  <SelectItem value="Character Arc">Arco de Personagem</SelectItem>
                  <SelectItem value="Romance">Romance</SelectItem>
                  <SelectItem value="Mystery">Mistério</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Descrição */}
            <div className="grid gap-2">
              <Label htmlFor="description">Descrição</Label>
              <Textarea
                id="description"
                name="description"
                placeholder="Descreva o arco narrativo..."
                rows={4}
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                disabled={isLoading}
                autoComplete="off"
              />
            </div>

            {/* Resolução (opcional) */}
            <div className="grid gap-2">
              <Label htmlFor="resolution">
                Resolução / Desfecho
              </Label>
              <Textarea
                id="resolution"
                name="resolution"
                placeholder="Como você planeja que este arco se resolva ou conclua..."
                rows={3}
                value={formData.resolution}
                onChange={(e) => setFormData({ ...formData, resolution: e.target.value })}
                disabled={isLoading}
                autoComplete="off"
              />
              <p className="text-xs text-muted-foreground">
                Opcional: esboce como pretende encerrar este arco narrativo
              </p>
            </div>

            {validationError && (
              <p className="text-sm text-destructive">{validationError}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => handleOpenChange(false)} disabled={isLoading}>
              Cancelar
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading && <Spinner className="mr-2 size-4" />}
              {mode === "create" ? "Criar" : "Salvar"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
