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
import type { ReactNode } from "react"

interface LocationDialogProps {
    trigger?: ReactNode;
    mode?: 'create' | 'edit';
}

export function LocationDialog({ trigger, mode = 'create' }: LocationDialogProps) {
  return (
    <Dialog>
        <DialogTrigger asChild>
            {trigger || <Button size="sm" variant="ghost" className="h-6 w-6 p-0"><Plus className="h-4 w-4" /></Button>}
        </DialogTrigger>
        <DialogContent className="sm:max-w-[600px]">
            <DialogHeader>
                <DialogTitle>{mode === 'create' ? 'Criar Novo Local' : 'Editar Local'}</DialogTitle>
                <DialogDescription>
                    Descreva o ambiente.
                </DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
                <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="name" className="text-right">
                        Nome
                    </Label>
                    <Input id="name" placeholder="Ex: Cidadela de Ferro" className="col-span-3" />
                </div>
                
                <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="description" className="text-right pt-2">
                        Descrição
                    </Label>
                    <Textarea id="description" placeholder="Atmosfera, cheiros, sons..." className="col-span-3 min-h-[100px]" />
                </div>
                    <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="geography" className="text-right pt-2">
                        Geografia
                    </Label>
                    <Textarea id="geography" placeholder="Clima, terreno, localização..." className="col-span-3" />
                </div>
            </div>
            <DialogFooter>
                <Button type="submit">Salvar Local</Button>
            </DialogFooter>
        </DialogContent>
    </Dialog>
  )
}
