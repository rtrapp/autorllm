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

interface CharacterDialogProps {
    children?: ReactNode;
    trigger?: ReactNode;
    mode?: 'create' | 'edit';
}

export function CharacterDialog({ trigger, mode = 'create' }: CharacterDialogProps) {
  return (
    <Dialog>
        <DialogTrigger asChild>
            {trigger || <Button size="sm" variant="ghost" className="h-6 w-6 p-0"><Plus className="h-4 w-4" /></Button>}
        </DialogTrigger>
        <DialogContent className="sm:max-w-[600px]">
            <DialogHeader>
                <DialogTitle>{mode === 'create' ? 'Criar Novo Personagem' : 'Editar Personagem'}</DialogTitle>
                <DialogDescription>
                    Defina os detalhes do personagem.
                </DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
                <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="name" className="text-right">
                        Nome
                    </Label>
                    <Input id="name" placeholder="Ex: Elara Vance" className="col-span-3" />
                </div>
                <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="role" className="text-right">
                        Papel
                    </Label>
                    <Select>
                        <SelectTrigger className="col-span-3">
                            <SelectValue placeholder="Selecione um papel" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="protagonist">Protagonista</SelectItem>
                            <SelectItem value="antagonist">Antagonista</SelectItem>
                            <SelectItem value="supporting">Coadjuvante</SelectItem>
                            <SelectItem value="minor">Secundário</SelectItem>
                        </SelectContent>
                    </Select>
                </div>
                <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="description" className="text-right pt-2">
                        Descrição
                    </Label>
                    <Textarea id="description" placeholder="Breve descrição..." className="col-span-3 min-h-[100px]" />
                </div>
                <div className="grid grid-cols-4 items-start gap-4">
                    <Label htmlFor="personality" className="text-right pt-2">
                        Personalidade
                    </Label>
                        <Textarea id="personality" placeholder="Traços de personalidade..." className="col-span-3" />
                </div>
            </div>
            <DialogFooter>
                <Button type="submit">Salvar Personagem</Button>
            </DialogFooter>
        </DialogContent>
    </Dialog>
  )
}
