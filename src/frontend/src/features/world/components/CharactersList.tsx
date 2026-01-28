import { Plus, Pencil, Trash } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
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

export function CharactersList() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
           <h2 className="text-lg font-semibold">Personagens</h2>
           <p className="text-sm text-muted-foreground">Gerencie os protagonistas e coadjuvantes da sua história.</p>
        </div>
        
        <Dialog>
            <DialogTrigger asChild>
                <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    Novo Personagem
                </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[600px]">
                <DialogHeader>
                    <DialogTitle>Criar Novo Personagem</DialogTitle>
                    <DialogDescription>
                        Defina os detalhes básicos para começar. Você pode expandir depois.
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
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2, 3].map((i) => (
             <Card key={i}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                   <CardTitle className="text-sm font-medium">
                      Elara Vance
                   </CardTitle>
                   <Badge variant="secondary">Protagonista</Badge>
                </CardHeader>
                <CardContent>
                   <p className="text-xs text-muted-foreground mt-2 line-clamp-3">
                      Uma jovem alquimista procurando a cura para a doença de cristal que assola sua vila. Determinada, mas impulsiva.
                   </p>
                   <div className="mt-4 flex items-center justify-end gap-2">
                      <Button variant="ghost" size="icon" className="h-8 w-8">
                         <Pencil className="h-4 w-4" />
                      </Button>
                      <Button variant="ghost" size="icon" className="h-8 w-8 text-destructive hover:text-destructive">
                         <Trash className="h-4 w-4" />
                      </Button>
                   </div>
                </CardContent>
             </Card>
          ))}
      </div>
    </div>
  )
}
