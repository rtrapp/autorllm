import { 
  Bot, 
  ChevronDown, 
  ChevronRight, 
  Download, 
  FileText, 
  MoreHorizontal, 
  PenTool, 
  Plus, 
  Save, 
  Send, 
  Sparkles, 
  User, 
  BookOpen
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Textarea } from "@/components/ui/textarea";

export default function ProjectWorkspace() {
  return (
    <div className="h-screen flex flex-col overflow-hidden bg-secondary/30 font-sans">
      {/* Header */}
      <header className="h-14 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 flex items-center justify-between px-4 shrink-0 z-10">
        <div className="flex items-center gap-4">
          <div className="font-semibold text-lg flex items-center gap-2">
            <PenTool className="h-5 w-5 text-primary" />
            <span>Projeto: Crônicas de Aethel</span>
          </div>
          <Badge variant="secondary" className="font-medium">Rascunho 1.2</Badge>
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
        </div>
      </header>
      
      {/* Main Layout */}
      <main className="flex-1 flex overflow-hidden relative">
         
         {/* Left Sidebar */}
         <aside className="w-72 bg-background border-r flex flex-col shrink-0">
            <div className="flex items-center border-b p-1 bg-secondary/40">
                <Button variant="secondary" size="sm" className="flex-1 shadow-sm bg-background text-foreground h-8 rounded-sm">Manuscrito</Button>
                <Button variant="ghost" size="sm" className="flex-1 text-muted-foreground h-8 rounded-sm">Mundo</Button>
            </div>

            <div className="flex-1 overflow-y-auto p-4">
               <div className="text-sm font-medium text-muted-foreground mb-3">Estrutura do Livro</div>
               
               {/* Act 1 */}
               <div className="group">
                  <div className="flex items-center gap-2 py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm font-medium">
                     <ChevronDown className="h-4 w-4 text-muted-foreground" />
                     Ato 1: O Despertar
                  </div>
                  <div className="ml-4 pl-2 border-l border-border space-y-1 mt-1">
                     <div className="flex items-center gap-2 py-1.5 px-2 bg-secondary text-primary rounded cursor-pointer text-sm">
                        <FileText className="h-3.5 w-3.5" />
                        Cap 1: A Vila Silenciosa
                     </div>
                     <div className="flex justify-between items-center gap-2 py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm group/item">
                        <div className="flex items-center gap-2">
                           <FileText className="h-3.5 w-3.5 text-muted-foreground" />
                           Cap 2: O Chamado
                        </div>
                        <span className="h-2 w-2 rounded-full bg-emerald-500" title="Plot Principal Ativo"></span>
                     </div>
                  </div>
               </div>

               {/* Act 2 */}
               <div className="group mt-2">
                  <div className="flex items-center gap-2 py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm font-medium text-muted-foreground">
                     <ChevronRight className="h-4 w-4 text-muted-foreground/70" />
                     Ato 2: A Jornada
                  </div>
               </div>

            </div>

             <div className="p-4 border-t mt-auto">
                <Button variant="outline" className="w-full justify-start gap-2 h-9">
                   <Plus className="h-4 w-4" />
                   Novo Capítulo
                </Button>
             </div>
         </aside>

         {/* Center Editor */}
         <section className="flex-1 overflow-y-auto relative bg-secondary/30 flex justify-center">
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
                  <p>O vento uivava entre as frestas das janelas de madeira, um lamento constante que Elara aprendera a ignorar. A vila de Oakhaven estava envolta em uma névoa perpétua naquela manhã, o tipo de névoa que se agarra à pele e gela os ossos. Ela ajustou o xale sobre os ombros, seus dedos roçando o tecido áspero enquanto observava a rua deserta lá fora.</p>
          
                  <p>"Eles não virão hoje," disse Kael, sua voz rouca quebrando o silêncio da pequena cabana. Ele estava sentado à mesa, polindo uma adaga com um pedaço de couro desgastado. O movimento era rítmico, hipnótico.</p>

                  <p>Elara se virou, a tensão evidente na linha rígida de seus ombros. "Eles têm que vir. O pacto foi selado na última lua cheia. Se os mercadores de ferro não trouxerem os suprimentos, não sobreviveremos ao inverno."</p>

                  <p>Kael parou de polir. O metal brilhou à luz fraca da lareira. Ele levantou os olhos, e Elara viu o medo que ele tentava esconder atrás da máscara de indiferença. <span className="bg-primary/10 text-primary/80 border-b-2 border-primary/30 px-1 rounded">"Talvez o pacto não valha tanto quanto pensávamos," ele murmurou, embainhando a adaga com um clique seco que ecoou como um tiro na sala pequena.</span></p>
                  
                  <p>Ela odiava quando ele estava certo. O silêncio que se seguiu foi pesado, carregado com as palavras não ditas sobre a escassez de comida e as histórias crescentes sobre as sombras que se moviam nas florestas ao redor. O inverno estava chegando, e com ele, algo mais antigo que o frio.</p>
               </div>
            </div>
         </section>

         {/* Right Sidebar */}
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

      </main>
    </div>
  );
}
