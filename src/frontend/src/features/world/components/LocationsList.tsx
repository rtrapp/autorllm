import { Plus, Pencil, Trash } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
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

export function LocationsList() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
           <h2 className="text-lg font-semibold">Locais</h2>
           <p className="text-sm text-muted-foreground">Defina o mundo onde sua história acontece.</p>
        </div>
        
        <Dialog>
            <DialogTrigger asChild>
                <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    Novo Local
                </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[600px]">
                <DialogHeader>
                    <DialogTitle>Criar Novo Local</DialogTitle>
                    <DialogDescription>
                        Descreva um novo ambiente, cidade, ou região.
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
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2].map((i) => (
             <Card key={i}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                   <CardTitle className="text-sm font-medium">
                      Floresta dos Sussurros
                   </CardTitle>
                </CardHeader>
                <CardContent>
                   <p className="text-xs text-muted-foreground mt-2 line-clamp-3">
                      Uma floresta antiga onde as árvores parecem conversar entre si. A neblina nunca se dissipa completamente, e viajantes frequentemente se perdem.
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
