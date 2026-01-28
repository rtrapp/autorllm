import { 
  Bot, 
  MoreHorizontal, 
  Send, 
  Sparkles 
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

export function AssistantSidebar() {
  return (
    <aside className="w-96 bg-background border-l flex flex-col shrink-0 relative">
        <div className="h-14 border-b flex items-center justify-between px-4 shrink-0 bg-background">
           <div className="font-medium flex items-center gap-2 text-primary">
              <Bot className="h-4.5 w-4.5" />
              Assistente Neural
           </div>
           <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
              <MoreHorizontal className="h-4 w-4" />
           </Button>
        </div>

        <div className="flex-1 overflow-y-auto p-4 space-y-6 bg-secondary/20">
           {/* Analysis Card */}
           <div className="rounded-lg border bg-card p-4 shadow-sm">
              <h3 className="text-sm font-semibold flex items-center gap-2 mb-3">
                 <Sparkles className="h-4 w-4 text-primary" />
                 Análise da Cena Atual
              </h3>
              <div className="space-y-3 text-sm">
                 <div>
                    <span className="text-muted-foreground">Tom Detectado:</span>
                    <span className="font-medium ml-2 inline-flex items-center gap-1">
                       <span className="h-2 w-2 rounded-full bg-blue-400"></span> 
                       Melancólico, Tenso
                    </span>
                 </div>
                 <div>
                    <span className="text-muted-foreground">Progresso do Plot:</span>
                    <div className="w-full h-2 bg-secondary rounded-full mt-1 overflow-hidden">
                       <div className="h-full bg-emerald-500 w-1/4 rounded-full"></div>
                    </div>
                    <div className="flex justify-between text-xs text-muted-foreground mt-1">
                       <span>Incidente Incitante</span>
                       <span>25%</span>
                    </div>
                 </div>
              </div>
           </div>

           {/* Chat Example */}
           <div className="space-y-4">
              <div className="text-sm font-semibold">Chat com a Obra</div>
              
              <div className="flex justify-end">
                 <div className="bg-primary text-primary-foreground px-4 py-2 rounded-2xl rounded-tr-sm max-w-[85%] text-sm">
                    Gostaria de deixar o diálogo do Kael mais pessimista no final.
                 </div>
              </div>

              <div className="flex items-start gap-3">
                 <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
                    <Sparkles className="h-4 w-4 text-primary animate-pulse" />
                 </div>
                 <div className="bg-card border px-4 py-2 rounded-2xl rounded-tl-sm max-w-[90%] text-sm shadow-sm">
                    <p className="mb-2">Entendido. Aqui está uma sugestão para a fala final dele, focando na desesperança:</p>
                    <div className="bg-secondary/50 p-3 rounded-md border-l-2 border-primary font-serif italic text-foreground/90">
                       "O pacto..." Kael soltou uma risada curta e sem humor, o som morrendo rapidamente no ar frio. Ele embainhou a adaga com um clique definitivo. "Papel e tinta, Elara. Apenas papel e tinta contra o inverno que se aproxima. Eles já nos esqueceram."
                    </div>
                    <div className="mt-3 flex gap-2">
                       <Button variant="outline" size="sm" className="h-7 text-xs">Inserir no Texto</Button>
                       <Button variant="ghost" size="sm" className="h-7 text-xs text-muted-foreground">Tentar novamente</Button>
                    </div>
                 </div>
              </div>
           </div>
        </div>

        <div className="p-4 border-t bg-background mt-auto">
           <div className="relative">
              <Textarea 
                 className="min-h-[80px] py-2 resize-none pr-10 font-sans focus-visible:ring-0" 
                 placeholder="Peça sugestões, análise de personagens, ou reescrita..."
              />
              <Button size="icon" className="absolute bottom-2 right-2 h-8 w-8 rounded-full">
                 <Send className="h-4 w-4" />
              </Button>
           </div>
           <div className="text-xs text-muted-foreground mt-2 text-center flex items-center justify-center gap-1">
              <span className="h-2 w-2 rounded-full bg-emerald-500"></span>
              LLM Local (Mistral-7B) Pronto
           </div>
        </div>

     </aside>
  )
}
