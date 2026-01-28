import { 
  Download, 
  PenTool, 
  Save,
  Menu,
  Settings,
  Moon,
  LogOut,
  FolderOpen,
  Pencil
} from "lucide-react";
import { Link, useParams } from "react-router-dom";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useProject } from "@/features/projects/hooks/useProject";
import { useState } from "react";
import { EditProjectDialog } from "@/features/projects/components";

export function AppHeader() {
  const { id } = useParams<{ id: string }>();
  const { project, refetch } = useProject(id);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

  return (
    <header className="h-14 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 flex items-center justify-between px-4 shrink-0 z-10">
      <div className="flex items-center gap-4">
        <div 
          className="font-semibold text-lg flex items-center gap-2 group cursor-pointer"
          onClick={() => setIsEditDialogOpen(true)}
        >
          <PenTool className="h-5 w-5 text-primary" />
          <span>Projeto: {project?.title || 'Carregando...'}</span>
          <Pencil className="h-4 w-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
        </div>
        {project?.genre && (
          <Badge variant="secondary" className="font-medium">{project.genre}</Badge>
        )}
      </div>

      <div className="flex items-center gap-2">
         <Button variant="ghost" size="sm" className="text-muted-foreground gap-2">
           <Download className="h-4 w-4" />
           Exportar PDF
         </Button>
         <Separator orientation="vertical" className="h-6" />
         <Button variant="outline" size="sm" className="gap-2">
           <Save className="h-4 w-4" />
           Salvar
         </Button>
         
         <Separator orientation="vertical" className="h-6 mx-1" />

         <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="h-8 w-8">
                    <Menu className="h-4 w-4" />
                </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
                <DropdownMenuLabel>Opções</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem asChild>
                    <Link to="/projects" className="cursor-pointer">
                        <FolderOpen className="mr-2 h-4 w-4" />
                        <span>Meus Projetos</span>
                    </Link>
                </DropdownMenuItem>
                <DropdownMenuItem>
                    <Settings className="mr-2 h-4 w-4" />
                    <span>Configurações</span>
                </DropdownMenuItem>
                <DropdownMenuItem>
                    <Moon className="mr-2 h-4 w-4" />
                    <span>Tema</span>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="text-destructive">
                    <LogOut className="mr-2 h-4 w-4" />
                    <span>Sair</span>
                </DropdownMenuItem>
            </DropdownMenuContent>
         </DropdownMenu>
      </div>
      
      {project && (
        <EditProjectDialog
          open={isEditDialogOpen}
          onOpenChange={setIsEditDialogOpen}
          project={project}
          onSuccess={() => refetch()}
        />
      )}
    </header>
  )
}
