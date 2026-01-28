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
import { Spinner } from "@/components/ui/spinner";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { useCharacters } from "@/features/characters/hooks/useCharacters";
import { CharacterFormDialog } from "@/features/characters/components/CharacterFormDialog";
import { DeleteCharacterDialog } from "@/features/characters/components/DeleteCharacterDialog";
import { CHARACTER_ROLES, type Character } from "@/features/characters/types";
import { LocationDialog } from "@/features/world/components/LocationDialog";
import { PlotDialog } from "@/features/world/components/PlotDialog";

export function AppSidebar() {
  const [activeTab, setActiveTab] = useState<'manuscript' | 'world'>('manuscript');
  const { id: projectId } = useParams<{ id: string }>();
  
  const { characters, isLoading: isLoadingCharacters, refetch: refetchCharacters } = useCharacters(projectId);
  const [isCreateCharacterDialogOpen, setIsCreateCharacterDialogOpen] = useState(false);
  const [editingCharacter, setEditingCharacter] = useState<Character | null>(null);
  const [deletingCharacter, setDeletingCharacter] = useState<Character | null>(null);
  
  // Estados para controlar seções e grupos expandidos
  const [expandedSections, setExpandedSections] = useState({
    characters: true,
    locations: true,
    plots: true,
  });
  const [expandedCharacterGroups, setExpandedCharacterGroups] = useState<Record<string, boolean>>({
    'Protagonist': true,
    'Antagonist': true,
    'Supporting': true,
    'Minor': true,
  });

  const getRoleLabel = (role: string) => {
    return CHARACTER_ROLES.find((r) => r.value === role)?.label || role;
  };

  const toggleSection = (section: keyof typeof expandedSections) => {
    setExpandedSections(prev => ({ ...prev, [section]: !prev[section] }));
  };

  const toggleCharacterGroup = (role: string) => {
    setExpandedCharacterGroups(prev => ({ ...prev, [role]: !prev[role] }));
  };

  // Agrupar e ordenar personagens
  const groupedCharacters = () => {
    const roleOrder = ['Protagonist', 'Antagonist', 'Supporting', 'Minor'];
    const groups: Record<string, Character[]> = {};
    
    // Inicializar grupos
    roleOrder.forEach(role => {
      groups[role] = [];
    });
    
    // Agrupar personagens
    characters.forEach(char => {
      if (groups[char.role]) {
        groups[char.role].push(char);
      }
    });
    
    // Ordenar alfabeticamente dentro de cada grupo
    Object.keys(groups).forEach(role => {
      groups[role].sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'));
    });
    
    return roleOrder.map(role => ({
      role,
      label: getRoleLabel(role),
      characters: groups[role],
      color: getRoleBorderColor(role),
    })).filter(group => group.characters.length > 0);
  };

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
                    <div 
                      className="flex items-center justify-between mb-2 cursor-pointer"
                      onClick={() => toggleSection('characters')}
                    >
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            {expandedSections.characters ? (
                              <ChevronDown className="h-4 w-4" />
                            ) : (
                              <ChevronRight className="h-4 w-4" />
                            )}
                            <Users className="h-4 w-4" /> Personagens
                        </span>
                        <Button 
                          variant="ghost" 
                          size="icon" 
                          className="h-6 w-6"
                          onClick={(e) => {
                            e.stopPropagation();
                            setIsCreateCharacterDialogOpen(true);
                          }}
                        >
                          <Plus className="h-4 w-4" />
                        </Button>
                    </div>
                    
                    {expandedSections.characters && (
                      <>
                        {isLoadingCharacters ? (
                          <div className="flex justify-center py-4">
                            <Spinner className="h-5 w-5" />
                          </div>
                        ) : characters.length === 0 ? (
                          <div className="text-xs text-muted-foreground text-center py-2">
                            Nenhum personagem criado
                          </div>
                        ) : (
                          <div className="space-y-3">
                            {groupedCharacters().map(group => (
                              <div key={group.role}>
                                {/* Header do grupo */}
                                <div 
                                  className="flex items-center gap-2 py-1 px-2 hover:bg-secondary/50 rounded cursor-pointer text-xs font-medium text-muted-foreground"
                                  onClick={() => toggleCharacterGroup(group.role)}
                                >
                                  {expandedCharacterGroups[group.role] ? (
                                    <ChevronDown className="h-3.5 w-3.5" />
                                  ) : (
                                    <ChevronRight className="h-3.5 w-3.5" />
                                  )}
                                  <span>{group.label}</span>
                                  <span className="text-[10px] text-muted-foreground/70">
                                    ({group.characters.length})
                                  </span>
                                </div>
                                
                                {/* Personagens do grupo */}
                                {expandedCharacterGroups[group.role] && (
                                  <div className="mt-1 space-y-0.5">
                                    {group.characters.map(char => (
                                      <div 
                                        key={char.id}
                                        className={`flex items-center justify-between py-1.5 pl-3 pr-2 hover:bg-secondary rounded cursor-pointer text-sm group border-l-4 ${group.color}`}
                                        onClick={() => setEditingCharacter(char)}
                                      >
                                        <div className="flex-1 min-w-0 truncate">
                                          {char.name}
                                        </div>
                                        <Pencil className="h-3 w-3 opacity-0 group-hover:opacity-100 text-muted-foreground flex-shrink-0" />
                                      </div>
                                    ))}
                                  </div>
                                )}
                              </div>
                            ))}
                          </div>
                        )}
                      </>
                    )}
                 </div>

                 {/* Locations Section */}
                 <div>
                    <div 
                      className="flex items-center justify-between mb-2 cursor-pointer"
                      onClick={() => toggleSection('locations')}
                    >
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            {expandedSections.locations ? (
                              <ChevronDown className="h-4 w-4" />
                            ) : (
                              <ChevronRight className="h-4 w-4" />
                            )}
                            <Map className="h-4 w-4" /> Locais
                        </span>
                        <LocationDialog />
                    </div>
                    {expandedSections.locations && (
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
                    )}
                 </div>

                 {/* Plots Section */}
                 <div>
                    <div 
                      className="flex items-center justify-between mb-2 cursor-pointer"
                      onClick={() => toggleSection('plots')}
                    >
                        <span className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                            {expandedSections.plots ? (
                              <ChevronDown className="h-4 w-4" />
                            ) : (
                              <ChevronRight className="h-4 w-4" />
                            )}
                            <BookOpen className="h-4 w-4" /> Plots
                        </span>
                        <PlotDialog />
                    </div>
                    {expandedSections.plots && (
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
                    )}
                 </div>
             </div>
           )}

        </div>

        {/* Dialogs */}
        {projectId && (
          <>
            <CharacterFormDialog
              open={isCreateCharacterDialogOpen}
              onOpenChange={setIsCreateCharacterDialogOpen}
              mode="create"
              projectId={projectId}
              onSuccess={refetchCharacters}
            />

            <CharacterFormDialog
              open={!!editingCharacter}
              onOpenChange={(open) => !open && setEditingCharacter(null)}
              mode="edit"
              projectId={projectId}
              character={editingCharacter || undefined}
              onSuccess={() => {
                refetchCharacters();
                setEditingCharacter(null);
              }}
              onDelete={() => {
                setDeletingCharacter(editingCharacter);
                setEditingCharacter(null);
              }}
            />

            <DeleteCharacterDialog
              open={!!deletingCharacter}
              onOpenChange={(open) => !open && setDeletingCharacter(null)}
              character={deletingCharacter}
              projectId={projectId}
              onSuccess={() => {
                refetchCharacters();
                setDeletingCharacter(null);
              }}
            />
          </>
        )}
     </aside>
  )
}

// Função auxiliar para obter cor do role (borda lateral)
function getRoleBorderColor(role: string): string {
  const colors: Record<string, string> = {
    'Protagonist': 'border-blue-500',
    'Antagonist': 'border-red-500',
    'Supporting': 'border-green-500',
    'Minor': 'border-gray-400',
  };
  return colors[role] || 'border-gray-400';
}
