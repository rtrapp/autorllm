import { BookOpen, User } from "lucide-react";
import { useWorkspace } from "@/contexts/WorkspaceContext";

export default function ProjectWorkspace() {
  const { selectedChapter } = useWorkspace();

  if (!selectedChapter) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center text-muted-foreground">
          <BookOpen className="h-12 w-12 mx-auto mb-4 opacity-50" />
          <p>Selecione um capítulo para editar</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      {/* Conteúdo do Workspace - Editor de Capítulo */}
      <div className="w-full max-w-[850px] mx-auto bg-background shadow-sm border min-h-[1100px] p-16 relative">
        <div className="mb-12 border-b pb-4">
          <h2 className="text-2xl font-bold font-sans tracking-tight text-foreground mb-4">
            Capítulo {selectedChapter.order}: {selectedChapter.title}
          </h2>
          {selectedChapter.summary && (
            <p className="text-sm text-muted-foreground mb-4 italic">
              {selectedChapter.summary}
            </p>
          )}
          <div className="flex items-center gap-4 text-sm text-muted-foreground font-sans">
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
          {selectedChapter.content ? (
            <div dangerouslySetInnerHTML={{ __html: selectedChapter.content }} />
          ) : (
            <p className="text-muted-foreground italic">
              Comece a escrever o conteúdo do capítulo...
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
