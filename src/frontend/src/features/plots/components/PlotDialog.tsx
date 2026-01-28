import { Plus } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import type { ReactNode } from "react"

interface PlotDialogProps {
    trigger?: ReactNode;
    mode?: 'create' | 'edit';
}

export function PlotDialog({ trigger, mode = 'create' }: PlotDialogProps) {
  return (
    <Dialog>
        <DialogTrigger asChild>
             {trigger || <Button size="sm" variant="ghost" className="h-6 w-6 p-0"><Plus className="h-4 w-4" /></Button>}
        </DialogTrigger>
        <DialogContent className="sm:max-w-[600px]">
            <DialogHeader>
                <DialogTitle>{mode === 'create' ? 'Criar Novo Plot' : 'Editar Plot'}</DialogTitle>
                <DialogDescription>
                    Defina o arco narrativo.
                </DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
                <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="title" className="text-right">
                        Título
                    </Label>
                    <Input id="title" placeholder="Ex: O Mistério do Medalhão" className="col-span-3" />
                </div>
                    <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="type" className="text-right">
                        Tipo
                    </Label>
                    <Select>
                        <SelectTrigger className="col-span-3">
                            <SelectValue placeholder="Selecione o tipo" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="main">Main Plot</SelectItem>
                            <SelectItem value="subplot">Subplot</SelectItem>
                            <SelectItem value="character">Character Arc</SelectItem>
                            <SelectItem value="romance">Romance</SelectItem>
                            <SelectItem value="mystery">Mystery</SelectItem>
                        </SelectContent>
                    </Select>
                </div>
                
                <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="description" className="text-right pt-2">
                        Descrição
                    </Label>
                    <Textarea id="description" placeholder="Resumo do conflito..." className="col-span-3 min-h-[100px]" />
                </div>
                    <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="resolution" className="text-right pt-2">
                        Resolução
                    </Label>
                    <Textarea id="resolution" placeholder="Como termina? (Opcional)" className="col-span-3" />
                </div>
            </div>
            <DialogFooter>
                <Button type="submit">Salvar Plot</Button>
            </DialogFooter>
        </DialogContent>
    </Dialog>
  )
}
