import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";
import { Plus, Sparkles, MoreVertical } from "lucide-react";
import { useState } from "react";
import { NewProjectDialog } from "@/features/projects/components/NewProjectDialog";
import { useProjects } from "@/features/projects/hooks/useProjects";
import { Spinner } from "@/components/ui/spinner";
import { formatDistanceToNow } from "date-fns";
import { ptBR } from "date-fns/locale";

export default function ProjectsList() {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const { projects, isLoading, error } = useProjects();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <Spinner className="h-8 w-8" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto p-8">
        <div className="text-center text-destructive">
          Erro ao carregar projetos: {error.message}
        </div>
      </div>
    );
  }

  return (
    <main className="container mx-auto px-4 pt-10 pb-12">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Meus Projetos</h1>
          <p className="text-muted-foreground mt-1">
            Gerencie suas obras e continue escrevendo de onde parou.
          </p>
        </div>
        <Button onClick={() => setIsDialogOpen(true)} className="md:hidden w-full gap-2">
          <Plus className="h-4 w-4" />
          Novo Livro
        </Button>
      </div>

      {projects.length === 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <button
            onClick={() => setIsDialogOpen(true)}
            className="group relative flex flex-col items-center justify-center gap-4 p-8 rounded-xl border-2 border-dashed border-muted-foreground/25 hover:border-primary/50 bg-secondary/10 hover:bg-secondary/30 transition-all cursor-pointer h-[280px]"
          >
            <div className="h-14 w-14 rounded-full bg-background border shadow-sm group-hover:shadow-md flex items-center justify-center text-primary transition-all group-hover:scale-110">
              <Plus className="h-7 w-7" />
            </div>
            <div className="text-center">
              <h3 className="text-lg font-semibold">Criar Novo Livro</h3>
              <p className="text-sm text-muted-foreground mt-1">
                Comece do zero ou use a IA para estruturar uma ideia.
              </p>
            </div>
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Novo Projeto Card */}
          <button
            onClick={() => setIsDialogOpen(true)}
            className="group relative flex flex-col items-center justify-center gap-4 p-8 rounded-xl border-2 border-dashed border-muted-foreground/25 hover:border-primary/50 bg-secondary/10 hover:bg-secondary/30 transition-all cursor-pointer h-[280px]"
          >
            <div className="h-14 w-14 rounded-full bg-background border shadow-sm group-hover:shadow-md flex items-center justify-center text-primary transition-all group-hover:scale-110">
              <Plus className="h-7 w-7" />
            </div>
            <div className="text-center">
              <h3 className="text-lg font-semibold">Criar Novo Livro</h3>
              <p className="text-sm text-muted-foreground mt-1">
                Comece do zero ou use a IA para estruturar uma ideia.
              </p>
            </div>
          </button>

          {/* Project Cards */}
          {projects.map((project) => {
            const genreColor = getGenreColor(project.genre);
            const genreBadge = getGenreBadgeStyle(project.genre);
            const aiUsage = getAIUsageLevel(project.currentWordCount);

            return (
              <div
                key={project.id}
                className="bg-card rounded-xl border shadow-sm flex flex-col h-[280px] transition-all duration-200 hover:shadow-md hover:-translate-y-1 border-border/60 relative overflow-hidden group"
              >
                <div className={`h-3 bg-gradient-to-r ${genreColor}`}></div>

                <div className="p-6 flex flex-col flex-1">
                  <div className="flex justify-between items-start mb-4">
                    <div className={`flex items-center gap-2 text-xs font-medium ${genreBadge.text} ${genreBadge.bg} px-2 py-1 rounded-full`}>
                      <Sparkles className="h-3 w-3" />
                      {genreBadge.label}
                    </div>
                    <button className="text-muted-foreground hover:text-foreground p-1 rounded -mr-2">
                      <MoreVertical className="h-5 w-5" />
                    </button>
                  </div>

                  <Link to={`/projects/${project.id}`} className="flex-1">
                    <h2 className="font-serif text-2xl font-bold mb-2 group-hover:text-primary transition-colors line-clamp-2">
                      {project.title}
                    </h2>
                    <p className="text-muted-foreground text-sm line-clamp-2 mb-6">
                      {project.synopsis || "Sem descrição"}
                    </p>
                  </Link>

                  <div className="mt-auto space-y-3">
                    <div className="flex items-center justify-between text-sm mb-1">
                      <span className="font-medium">
                        {getProgressLabel(project.currentWordCount, project.targetWordCount)}
                      </span>
                      <span className="text-muted-foreground">
                        {formatWordCount(project.currentWordCount)}
                      </span>
                    </div>
                    <div className="h-2 w-full bg-secondary rounded-full overflow-hidden">
                      <div
                        className="h-full bg-primary transition-all duration-500"
                        style={{
                          width: `${getProgress(project.currentWordCount, project.targetWordCount)}%`,
                        }}
                      ></div>
                    </div>

                    <div className="flex items-center justify-between text-xs text-muted-foreground pt-2">
                      <span>
                        {formatDistanceToNow(new Date(project.updatedAt), {
                          addSuffix: true,
                          locale: ptBR,
                        })}
                      </span>
                      <span className="flex items-center gap-1" title="Nível de assistência da IA">
                        <Sparkles className={`h-3.5 w-3.5 ${aiUsage.color}`} />
                        Neural: {aiUsage.label}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      <NewProjectDialog open={isDialogOpen} onOpenChange={setIsDialogOpen} />
    </main>
  );
}

// Helper functions
function normalizeString(str: string): string {
  return str
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, '');
}

function getGenreColor(genre?: string): string {
  const colors: Record<string, string> = {
    fantasia: "from-emerald-500/20 to-teal-500/20",
    scifi: "from-indigo-500/20 to-purple-500/20",
    romance: "from-pink-500/20 to-rose-500/20",
    misterio: "from-slate-500/20 to-gray-500/20",
    terror: "from-red-500/20 to-orange-500/20",
    ficcaocientifica: "from-indigo-500/20 to-purple-500/20",
  };

  const key = genre ? normalizeString(genre) : "";
  return colors[key] || "from-secondary to-secondary";
}

function getGenreBadgeStyle(genre?: string): { text: string; bg: string; label: string } {
  const styles: Record<string, { text: string; bg: string; label: string }> = {
    fantasia: { 
      text: "text-emerald-600", 
      bg: "bg-emerald-50", 
      label: "Fantasia Épica" 
    },
    scifi: { 
      text: "text-indigo-600", 
      bg: "bg-indigo-50", 
      label: "Sci-Fi Noir" 
    },
    ficcaocientifica: { 
      text: "text-indigo-600", 
      bg: "bg-indigo-50", 
      label: "Ficção Científica" 
    },
    romance: { 
      text: "text-pink-600", 
      bg: "bg-pink-50", 
      label: "Romance" 
    },
    misterio: { 
      text: "text-slate-600", 
      bg: "bg-slate-50", 
      label: "Mistério" 
    },
    terror: { 
      text: "text-red-600", 
      bg: "bg-red-50", 
      label: "Terror" 
    },
  };

  const key = genre ? normalizeString(genre) : "";
  return styles[key] || { 
    text: "text-muted-foreground", 
    bg: "bg-secondary", 
    label: genre || "Sem gênero definido" 
  };
}

function getProgress(current: number, target: number): number {
  if (target === 0) return 0;
  return Math.min(Math.round((current / target) * 100), 100);
}

function getProgressLabel(current: number, target: number): string {
  const progress = getProgress(current, target);
  if (progress === 0) return "Planejamento";
  if (progress < 25) return "Rascunho Inicial";
  if (progress < 75) return `${progress}% Concluído`;
  if (progress < 100) return `${progress}% Concluído`;
  return "Concluído";
}

function formatWordCount(count: number): string {
  if (count === 0) return "0 palavras";
  if (count < 1000) return `${count} palavras`;
  return `${(count / 1000).toFixed(1)}k palavras`;
}

function getAIUsageLevel(wordCount: number): { label: string; color: string } {
  if (wordCount === 0) return { label: "Nenhum", color: "text-muted-foreground opacity-50" };
  if (wordCount < 5000) return { label: "Alto", color: "text-primary" };
  if (wordCount < 20000) return { label: "Médio", color: "text-primary/70" };
  return { label: "Baixo", color: "text-muted-foreground" };
}
