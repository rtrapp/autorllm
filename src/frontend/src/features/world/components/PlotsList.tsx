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

export function PlotsList() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
           <h2 className="text-lg font-semibold">Plots</h2>
           <p className="text-sm text-muted-foreground">Estruture os arcos narrativos, conflitos e resoluções.</p>
        </div>
        
        <Dialog>
            <DialogTrigger asChild>
                <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    Novo Plot
                </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[600px]">
                <DialogHeader>
                    <DialogTitle>Criar Novo Plot</DialogTitle>
                    <DialogDescription>
                        Defina um novo arco narrativo para sua história.
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
      </div>

      <div className="grid grid-cols-1 gap-4">
          {[1].map((i) => (
             <Card key={i}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                   <div className="space-y-1">
                      <CardTitle className="text-base font-medium">
                         A Praga de Cristal
                      </CardTitle>
                      <Badge>Main Plot</Badge>
                   </div>
                   <div className="flex gap-2">
                        <Button variant="ghost" size="icon" className="h-8 w-8">
                            <Pencil className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="icon" className="h-8 w-8 text-destructive hover:text-destructive">
                            <Trash className="h-4 w-4" />
                        </Button>
                   </div>
                </CardHeader>
                <CardContent>
                   <p className="text-sm text-muted-foreground mt-2">
                      Uma doença misteriosa começa a transformar os habitantes da vila em estátuas de cristal vivo. Elara precisa encontrar a fonte antes que consuma a todos.
                   </p>
                   <div className="mt-4 flex items-center gap-4 text-xs text-muted-foreground">
                        <span className="flex items-center gap-1">
                            <span className="w-2 h-2 rounded-full bg-emerald-500"></span>
                            Ativo
                        </span>
                        <span>Resolução Pendente</span>
                   </div>
                </CardContent>
             </Card>
          ))}
      </div>
    </div>
  )
}
