import { 
  ChevronDown, 
  ChevronRight, 
  FileText, 
  Plus,
  Users,
  Map,
  BookOpen,
  Pencil
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { CharacterDialog } from "@/features/world/components/CharacterDialog";
import { LocationDialog } from "@/features/world/components/LocationDialog";
import { PlotDialog } from "@/features/world/components/PlotDialog";

export function AppSidebar() {
  const [activeTab, setActiveTab] = useState<'manuscript' | 'world'>('manuscript');

  return (
    <aside className="w-72 bg-background border-r flex flex-col shrink-0 transition-all duration-300">
        <div className="flex items-center border-b p-1 bg-secondary/40">
            <Button 
                variant={activeTab === 'manuscript' ? "secondary" : "ghost"} 
                size="sm" 
                className={`flex-1 shadow-sm h-8 rounded-sm ${activeTab === 'manuscript' ? "bg-background text-foreground" : "text-muted-foreground"}`}
                onClick={() => setActiveTab('manuscript')}
            >
                Manuscrito
            </Button>
            <Button 
                variant={activeTab === 'world' ? "secondary" : "ghost"} 
                size="sm" 
                className={`flex-1 shadow-sm h-8 rounded-sm ${activeTab === 'world' ? "bg-background text-foreground" : "text-muted-foreground"}`}
                onClick={() => setActiveTab('world')}
            >
                Mundo
            </Button>
        </div>

        <div className="flex-1 overflow-y-auto p-4">
           {activeTab === 'manuscript' ? (
             <>
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

                <div className="p-4 border-t mt-auto">
                    <Button variant="outline" className="w-full justify-start gap-2 h-9">
                    <Plus className="h-4 w-4" />
                    Novo Capítulo
                    </Button>
                </div>
             </>
           ) : (
             <div className="space-y-6">
                 {/* Characters Section */}
                 <div>
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            <Users className="h-4 w-4" /> Personagens
                        </span>
                        <CharacterDialog />
                    </div>
                    <div className="space-y-1">
                        {['Elara Vance', 'Kael', 'Master Thorne'].map(char => (
                            <CharacterDialog key={char} mode="edit" trigger={
                                <div className="flex items-center justify-between py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm group">
                                    <span>{char}</span>
                                    <Pencil className="h-3 w-3 opacity-0 group-hover:opacity-100 text-muted-foreground" />
                                </div>
                            } />
                        ))}
                    </div>
                 </div>

                 {/* Locations Section */}
                 <div>
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            <Map className="h-4 w-4" /> Locais
                        </span>
                        <LocationDialog />
                    </div>
                    <div className="space-y-1">
                        {['Vila Oakhaven', 'Floresta dos Sussurros'].map(loc => (
                            <LocationDialog key={loc} mode="edit" trigger={
                                <div className="flex items-center justify-between py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm group">
                                    <span>{loc}</span>
                                    <Pencil className="h-3 w-3 opacity-0 group-hover:opacity-100 text-muted-foreground" />
                                </div>
                            } />
                        ))}
                    </div>
                 </div>

                 {/* Plots Section */}
                 <div>
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            <BookOpen className="h-4 w-4" /> Plots
                        </span>
                        <PlotDialog />
                    </div>
                    <div className="space-y-1">
                        {['A Praga de Cristal', 'O Passado de Kael'].map(plot => (
                             <PlotDialog key={plot} mode="edit" trigger={
                                <div className="flex items-center justify-between py-1.5 px-2 hover:bg-secondary rounded cursor-pointer text-sm group">
                                    <span>{plot}</span>
                                    <span className="h-2 w-2 rounded-full bg-emerald-500" title="Ativo"></span>
                                </div>
                             } />
                        ))}
                    </div>
                 </div>
             </div>
           )}

        </div>
     </aside>
  )
}
