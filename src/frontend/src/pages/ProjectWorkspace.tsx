import { BookOpen, User } from "lucide-react";

export default function ProjectWorkspace() {
  return (
    <div className="w-full max-w-[850px] bg-background shadow-sm border my-8 min-h-[1100px] p-16 relative">
       <div className="mb-12 border-b pb-4">
          <h1 className="text-3xl font-bold font-sans tracking-tight text-foreground">Capítulo 1: A Vila Silenciosa</h1>
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

       <div className="font-serif text-lg leading-relaxed space-y-6 text-foreground/90 outline-none max-w-none" contentEditable suppressContentEditableWarning>
          <p>O vento uivava entre as frestas das janelas de madeira, um lamento constante que Elara aprendera a ignorar. A vila de Oakhaven estava envolta em uma névoa perpétua naquela manhã, o tipo de névoa que se agarra à pele e gela os ossos.</p>
  
          <p>"Eles não virão hoje," disse Kael, sua voz rouca quebrando o silêncio da pequena cabana.</p>

          <p>Elara se virou, a tensão evidente na linha rígida de seus ombros. "Eles têm que vir."</p>
       </div>
    </div>
  );
}
