import { useState } from "react";
import { useParams } from "react-router-dom";
import { BookOpen, User, Settings } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Spinner } from "@/components/ui/spinner";
import { EditProjectDialog } from "@/features/projects/components";
import { useProject } from "@/features/projects/hooks/useProject";

export default function ProjectWorkspace() {
  const { id } = useParams<{ id: string }>();
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const { project, isLoading, error, refetch } = useProject(id);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Spinner className="h-8 w-8" />
      </div>
    );
  }

  if (error || !project) {
    return (
      <div className="container mx-auto p-8">
        <div className="text-center text-destructive">
          Erro ao carregar projeto
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Header do Projeto */}
      <div className="mb-8 flex items-start justify-between">
        <div>
          <h1 className="text-3xl font-bold font-serif tracking-tight">{project.title}</h1>
          {project.synopsis && (
            <p className="text-muted-foreground mt-2">{project.synopsis}</p>
          )}
          {project.genre && (
            <p className="text-sm text-muted-foreground mt-1">Gênero: {project.genre}</p>
          )}
        </div>
        <Button
          variant="outline"
          size="sm"
          onClick={() => setIsEditDialogOpen(true)}
          className="gap-2"
        >
          <Settings className="h-4 w-4" />
          Editar Projeto
        </Button>
      </div>

      {/* Conteúdo do Workspace (placeholder) */}
      <div className="w-full max-w-[850px] mx-auto bg-background shadow-sm border min-h-[1100px] p-16 relative">
        <div className="mb-12 border-b pb-4">
          <h2 className="text-3xl font-bold font-sans tracking-tight text-foreground">
            Capítulo 1: A Vila Silenciosa
          </h2>
          <div className="flex items-center gap-4 mt-4 text-sm text-muted-foreground font-sans">
            <div className="flex items-center gap-1">
              <User className="h-3.5 w-3.5" />
              Personagens: Elara, Kael
            </div>
            <div className="flex items-center gap-1">
              <BookOpen className="h-3.5 w-3.5" />
              Arco: Introdução do Conflito
            </div>
          </div>
        </div>

        <div
          className="font-serif text-lg leading-relaxed space-y-6 text-foreground/90 outline-none max-w-none"
          contentEditable
          suppressContentEditableWarning
        >
          <p>
            O vento uivava entre as frestas das janelas de madeira, um lamento constante que Elara
            aprendera a ignorar. A vila de Oakhaven estava envolta em uma névoa perpétua naquela
            manhã, o tipo de névoa que se agarra à pele e gela os ossos.
          </p>

          <p>"Eles não virão hoje," disse Kael, sua voz rouca quebrando o silêncio da pequena cabana.</p>

          <p>Elara se virou, a tensão evidente na linha rígida de seus ombros. "Eles têm que vir."</p>
        </div>
      </div>

      {/* Dialog de Edição */}
      <EditProjectDialog
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
        project={project}
        onSuccess={() => refetch()}
      />
    </div>
  );
}
