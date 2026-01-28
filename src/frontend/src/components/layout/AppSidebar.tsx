import { 
  ChevronDown, 
  ChevronRight, 
  FileText, 
  Plus,
  Users,
  Map,
  BookOpen,
  Pencil,
  Trash2,
  GripVertical
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Spinner } from "@/components/ui/spinner";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { useCharacters } from "@/features/characters/hooks/useCharacters";
import { CharacterFormDialog } from "@/features/characters/components/CharacterFormDialog";
import { DeleteCharacterDialog } from "@/features/characters/components/DeleteCharacterDialog";
import { CHARACTER_ROLES, type Character } from "@/features/characters/types";
import { useLocations } from "@/features/locations/hooks/useLocations";
import { LocationFormDialog } from "@/features/locations/components/LocationFormDialog";
import { DeleteLocationDialog } from "@/features/locations/components/DeleteLocationDialog";
import type { Location } from "@/features/locations/types/location";
import { usePlots } from "@/features/plots/hooks/usePlots";
import { PlotFormDialog } from "@/features/plots/components/PlotFormDialog";
import { DeletePlotDialog } from "@/features/plots/components/DeletePlotDialog";
import type { Plot } from "@/features/plots/types/plot";
import { useChapters } from "@/features/chapters/hooks";
import { ChapterFormDialog, DeleteChapterDialog } from "@/features/chapters/components";
import type { Chapter } from "@/features/chapters/types/chapter";
import { useWorkspace } from "@/contexts/WorkspaceContext";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

export function AppSidebar() {
  const [activeTab, setActiveTab] = useState<'manuscript' | 'world'>('manuscript');
  const { id: projectId } = useParams<{ id: string }>();
  const { selectedChapter, setSelectedChapter } = useWorkspace();
  
  const { characters, isLoading: isLoadingCharacters, refetch: refetchCharacters } = useCharacters(projectId);
  const [isCreateCharacterDialogOpen, setIsCreateCharacterDialogOpen] = useState(false);
  const [editingCharacter, setEditingCharacter] = useState<Character | null>(null);
  const [deletingCharacter, setDeletingCharacter] = useState<Character | null>(null);
  
  const { locations, isLoading: isLoadingLocations, createLocation, updateLocation, deleteLocation } = useLocations(projectId);
  const [isCreateLocationDialogOpen, setIsCreateLocationDialogOpen] = useState(false);
  const [editingLocation, setEditingLocation] = useState<Location | null>(null);
  const [deletingLocation, setDeletingLocation] = useState<Location | null>(null);
  const [isSavingLocation, setIsSavingLocation] = useState(false);
  
  const { plots, isLoading: isLoadingPlots, createPlot, updatePlot, deletePlot } = usePlots(projectId);
  const [isCreatePlotDialogOpen, setIsCreatePlotDialogOpen] = useState(false);
  const [editingPlot, setEditingPlot] = useState<Plot | null>(null);
  const [deletingPlot, setDeletingPlot] = useState<Plot | null>(null);
  const [isSavingPlot, setIsSavingPlot] = useState(false);
  
  const { chapters, isLoading: isLoadingChapters, createChapter, updateChapter, deleteChapter, reorderChapters } = useChapters(projectId);
  const [isCreateChapterDialogOpen, setIsCreateChapterDialogOpen] = useState(false);
  const [editingChapter, setEditingChapter] = useState<Chapter | null>(null);
  const [deletingChapter, setDeletingChapter] = useState<Chapter | null>(null);
  const [isSavingChapter, setIsSavingChapter] = useState(false);
  
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

  // Handlers para Locations
  const handleCreateLocation = async (data: any) => {
    setIsSavingLocation(true);
    try {
      await createLocation(data);
      setIsCreateLocationDialogOpen(false);
    } finally {
      setIsSavingLocation(false);
    }
  };

  const handleUpdateLocation = async (data: any) => {
    setIsSavingLocation(true);
    try {
      await updateLocation(data);
      setEditingLocation(null);
    } finally {
      setIsSavingLocation(false);
    }
  };

  const handleDeleteLocation = async () => {
    if (!deletingLocation) return;
    setIsSavingLocation(true);
    try {
      await deleteLocation(deletingLocation.id);
      setDeletingLocation(null);
    } finally {
      setIsSavingLocation(false);
    }
  };

  // Handlers para Chapters
  const handleCreateChapter = async (data: any) => {
    setIsSavingChapter(true);
    try {
      await createChapter(data);
      setIsCreateChapterDialogOpen(false);
    } finally {
      setIsSavingChapter(false);
    }
  };

  const handleUpdateChapter = async (data: any) => {
    setIsSavingChapter(true);
    try {
      await updateChapter(data);
      setEditingChapter(null);
    } finally {
      setIsSavingChapter(false);
    }
  };

  const handleDeleteChapter = async (chapterId: string) => {
    setIsSavingChapter(true);
    try {
      await deleteChapter(chapterId);
      setDeletingChapter(null);
    } finally {
      setIsSavingChapter(false);
    }
  };

  // Handlers para Plots
  const handleCreatePlot = async (data: any) => {
    setIsSavingPlot(true);
    try {
      await createPlot(data);
      setIsCreatePlotDialogOpen(false);
    } finally {
      setIsSavingPlot(false);
    }
  };

  const handleUpdatePlot = async (data: any) => {
    setIsSavingPlot(true);
    try {
      await updatePlot(data);
      setEditingPlot(null);
    } finally {
      setIsSavingPlot(false);
    }
  };

  const handleDeletePlot = async () => {
    if (!deletingPlot) return;
    setIsSavingPlot(true);
    try {
      await deletePlot(deletingPlot.id);
      setDeletingPlot(null);
    } finally {
      setIsSavingPlot(false);
    }
  };

  // Drag-and-drop sensors
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // Handler para reordenação de capítulos
  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (!over || active.id === over.id) {
      return;
    }

    const oldIndex = chapters.findIndex((c) => c.id === active.id);
    const newIndex = chapters.findIndex((c) => c.id === over.id);

    if (oldIndex === -1 || newIndex === -1) {
      return;
    }

    // Reordenar localmente (otimista)
    const newChapters = arrayMove(chapters, oldIndex, newIndex);
    
    // Enviar nova ordem ao backend
    try {
      await reorderChapters(newChapters.map((c) => c.id));
    } catch (error) {
      console.error("Error reordering chapters:", error);
      // Aqui poderíamos reverter a mudança otimista se necessário
    }
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

  // Componente SortableChapter para drag-and-drop
  function SortableChapter({ chapter }: { chapter: Chapter }) {
    const {
      attributes,
      listeners,
      setNodeRef,
      transform,
      transition,
      isDragging,
    } = useSortable({ id: chapter.id });

    const style = {
      transform: CSS.Transform.toString(transform),
      transition,
      opacity: isDragging ? 0.5 : 1,
    };

    return (
      <div
        ref={setNodeRef}
        style={style}
        className={`group flex items-center justify-between py-1.5 pl-2 pr-2 hover:bg-secondary rounded text-sm ${
          selectedChapter?.id === chapter.id ? 'bg-secondary border-l-2 border-primary' : ''
        }`}
      >
        <div className="flex items-center gap-2 flex-1 min-w-0">
          <button
            {...attributes}
            {...listeners}
            className="cursor-grab active:cursor-grabbing p-1 hover:bg-primary/10 rounded shrink-0"
            title="Arrastar para reordenar"
          >
            <GripVertical className="h-3.5 w-3.5 text-muted-foreground" />
          </button>
          <div 
            className="flex items-center gap-2 flex-1 min-w-0 cursor-pointer"
            onClick={() => setSelectedChapter(chapter)}
          >
            <FileText className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
            <span className="truncate">
              Cap {chapter.order}: {chapter.title}
            </span>
            {chapter.wordCount > 0 && (
              <span className="h-2 w-2 rounded-full bg-emerald-500 shrink-0" title={`${chapter.wordCount} palavras`}></span>
            )}
          </div>
        </div>
        <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100">
          <button
            onClick={(e) => {
              e.stopPropagation();
              setEditingChapter(chapter);
            }}
            className="p-1 hover:bg-primary/10 rounded"
            title="Editar capítulo"
          >
            <Pencil className="h-3 w-3 text-muted-foreground" />
          </button>
          <button
            onClick={(e) => {
              e.stopPropagation();
              setDeletingChapter(chapter);
            }}
            className="p-1 hover:bg-destructive/10 rounded"
            title="Deletar capítulo"
          >
            <Trash2 className="h-3 w-3 text-muted-foreground hover:text-destructive" />
          </button>
        </div>
      </div>
    );
  }

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
               <div className="text-sm font-medium text-muted-foreground mb-3">Capítulos</div>
               
               {isLoadingChapters ? (
                 <div className="flex justify-center py-4">
                   <Spinner className="h-5 w-5" />
                 </div>
               ) : chapters.length === 0 ? (
                 <div className="text-xs text-muted-foreground text-center py-4">
                   Nenhum capítulo criado ainda
                 </div>
               ) : (
                 <DndContext
                   sensors={sensors}
                   collisionDetection={closestCenter}
                   onDragEnd={handleDragEnd}
                 >
                   <SortableContext
                     items={chapters.map((c) => c.id)}
                     strategy={verticalListSortingStrategy}
                   >
                     <div className="space-y-0.5">
                       {chapters.map((chapter) => (
                         <SortableChapter key={chapter.id} chapter={chapter} />
                       ))}
                     </div>
                   </SortableContext>
                 </DndContext>
               )}
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
                                        className={`flex items-center justify-between py-1.5 pl-3 pr-2 hover:bg-secondary rounded text-sm group border-l-4 ${group.color}`}
                                      >
                                        <div 
                                          className="flex-1 min-w-0 truncate cursor-pointer"
                                          onClick={() => setEditingCharacter(char)}
                                        >
                                          {char.name}
                                        </div>
                                        <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100">
                                          <button
                                            onClick={(e) => {
                                              e.stopPropagation();
                                              setEditingCharacter(char);
                                            }}
                                            className="p-1 hover:bg-primary/10 rounded"
                                            title="Editar personagem"
                                          >
                                            <Pencil className="h-3 w-3 text-muted-foreground" />
                                          </button>
                                          <button
                                            onClick={(e) => {
                                              e.stopPropagation();
                                              setDeletingCharacter(char);
                                            }}
                                            className="p-1 hover:bg-destructive/10 rounded"
                                            title="Deletar personagem"
                                          >
                                            <Trash2 className="h-3 w-3 text-muted-foreground hover:text-destructive" />
                                          </button>
                                        </div>
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
                        <Button 
                          variant="ghost" 
                          size="icon" 
                          className="h-6 w-6"
                          onClick={(e) => {
                            e.stopPropagation();
                            setIsCreateLocationDialogOpen(true);
                          }}
                        >
                          <Plus className="h-4 w-4" />
                        </Button>
                    </div>
                    
                    {expandedSections.locations && (
                      <>
                        {isLoadingLocations ? (
                          <div className="flex justify-center py-4">
                            <Spinner className="h-5 w-5" />
                          </div>
                        ) : locations.length === 0 ? (
                          <div className="text-xs text-muted-foreground text-center py-2">
                            Nenhum local criado
                          </div>
                        ) : (
                          <div className="space-y-0.5">
                            {locations.sort((a, b) => a.name.localeCompare(b.name, 'pt-BR')).map(location => (
                              <div 
                                key={location.id}
                                className="flex items-center justify-between py-1.5 pl-3 pr-2 hover:bg-secondary rounded text-sm group border-l-4 border-amber-500"
                              >
                                <div 
                                  className="flex-1 min-w-0 truncate cursor-pointer"
                                  onClick={() => setEditingLocation(location)}
                                >
                                  {location.name}
                                </div>
                                <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100">
                                  <button
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      setEditingLocation(location);
                                    }}
                                    className="p-1 hover:bg-primary/10 rounded"
                                    title="Editar local"
                                  >
                                    <Pencil className="h-3 w-3 text-muted-foreground" />
                                  </button>
                                  <button
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      setDeletingLocation(location);
                                    }}
                                    className="p-1 hover:bg-destructive/10 rounded"
                                    title="Deletar local"
                                  >
                                    <Trash2 className="h-3 w-3 text-muted-foreground hover:text-destructive" />
                                  </button>
                                </div>
                              </div>
                            ))}
                          </div>
                        )}
                      </>
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
                        <Button
                          size="sm"
                          variant="ghost"
                          className="h-6 w-6 p-0"
                          onClick={(e) => {
                            e.stopPropagation();
                            setIsCreatePlotDialogOpen(true);
                          }}
                        >
                          <Plus className="h-4 w-4" />
                        </Button>
                    </div>
                    {expandedSections.plots && (
                      <>
                        {isLoadingPlots ? (
                          <div className="flex items-center justify-center py-4">
                            <Spinner className="size-4" />
                          </div>
                        ) : plots && plots.length === 0 ? (
                          <p className="text-xs text-muted-foreground px-2 py-1">Nenhum plot ainda</p>
                        ) : (
                          <div className="space-y-1">
                            {plots?.sort((a: Plot, b: Plot) => {
                              // Main plots primeiro
                              if (a.type === 'Main' && b.type !== 'Main') return -1;
                              if (a.type !== 'Main' && b.type === 'Main') return 1;
                              // Depois alfabeticamente
                              return a.title.localeCompare(b.title, 'pt-BR');
                            }).map((plot: Plot) => (
                              <div
                                key={plot.id}
                                className={`flex items-center justify-between py-1.5 pl-3 pr-2 hover:bg-secondary rounded text-sm group border-l-4 ${getPlotTypeBorderColor(plot.type)}`}
                              >
                                <div 
                                  className="flex-1 min-w-0 truncate cursor-pointer"
                                  onClick={() => setEditingPlot(plot)}
                                >
                                  <span className={plot.type === 'Main' ? 'font-semibold' : ''}>
                                    {plot.title}
                                  </span>
                                  <span className="ml-2 text-xs text-muted-foreground">
                                    ({getPlotTypeLabel(plot.type)})
                                  </span>
                                </div>
                                <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100">
                                  <button
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      setEditingPlot(plot);
                                    }}
                                    className="p-1 hover:bg-primary/10 rounded"
                                    title="Editar plot"
                                  >
                                    <Pencil className="h-3 w-3 text-muted-foreground" />
                                  </button>
                                  <button
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      setDeletingPlot(plot);
                                    }}
                                    className="p-1 hover:bg-destructive/10 rounded"
                                    title="Deletar plot"
                                  >
                                    <Trash2 className="h-3 w-3 text-muted-foreground hover:text-destructive" />
                                  </button>
                                </div>
                              </div>
                            ))}
                          </div>
                        )}
                      </>
                    )}
                 </div>
             </div>
           )}

        </div>

        {/* Botão Novo Capítulo - Fixo na parte inferior quando tab Manuscrito */}
        {activeTab === 'manuscript' && (
          <div className="p-4 border-t mt-auto bg-background">
            <Button 
              variant="outline" 
              className="w-full justify-start gap-2 h-9"
              onClick={() => setIsCreateChapterDialogOpen(true)}
            >
              <Plus className="h-4 w-4" />
              Novo Capítulo
            </Button>
          </div>
        )}

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
              key={editingCharacter?.id || 'edit-character'}
              open={!!editingCharacter}
              onOpenChange={(open) => !open && setEditingCharacter(null)}
              mode="edit"
              projectId={projectId}
              character={editingCharacter || undefined}
              onSuccess={() => {
                refetchCharacters();
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

            <LocationFormDialog
              open={isCreateLocationDialogOpen}
              onOpenChange={setIsCreateLocationDialogOpen}
              mode="create"
              projectId={projectId}
              onSuccess={() => setIsCreateLocationDialogOpen(false)}
              onSubmit={handleCreateLocation}
              isLoading={isSavingLocation}
            />

            <LocationFormDialog
              key={editingLocation?.id || 'edit-location'}
              open={!!editingLocation}
              onOpenChange={(open) => !open && setEditingLocation(null)}
              mode="edit"
              projectId={projectId}
              location={editingLocation || undefined}
              onSuccess={() => setEditingLocation(null)}
              onSubmit={handleUpdateLocation}
              isLoading={isSavingLocation}
            />

            <DeleteLocationDialog
              open={!!deletingLocation}
              onOpenChange={(open) => !open && setDeletingLocation(null)}
              location={deletingLocation}
              onDelete={handleDeleteLocation}
              isLoading={isSavingLocation}
            />

            <PlotFormDialog
              open={isCreatePlotDialogOpen}
              onOpenChange={setIsCreatePlotDialogOpen}
              mode="create"
              projectId={projectId}
              onSuccess={() => setIsCreatePlotDialogOpen(false)}
              onSubmit={handleCreatePlot}
              isLoading={isSavingPlot}
            />

            <PlotFormDialog
              key={editingPlot?.id || 'edit-plot'}
              open={!!editingPlot}
              onOpenChange={(open) => !open && setEditingPlot(null)}
              mode="edit"
              projectId={projectId}
              plot={editingPlot || undefined}
              onSuccess={() => setEditingPlot(null)}
              onSubmit={handleUpdatePlot}
              isLoading={isSavingPlot}
            />

            <DeletePlotDialog
              open={!!deletingPlot}
              onOpenChange={(open) => !open && setDeletingPlot(null)}
              plot={deletingPlot}
              onConfirm={handleDeletePlot}
              isLoading={isSavingPlot}
            />

            <ChapterFormDialog
              open={isCreateChapterDialogOpen}
              onOpenChange={setIsCreateChapterDialogOpen}
              mode="create"
              onSubmit={handleCreateChapter}
              isLoading={isSavingChapter}
            />

            <ChapterFormDialog
              key={editingChapter?.id || 'edit-chapter'}
              open={!!editingChapter}
              onOpenChange={(open) => !open && setEditingChapter(null)}
              mode="edit"
              chapter={editingChapter || undefined}
              onSubmit={handleUpdateChapter}
              isLoading={isSavingChapter}
            />

            <DeleteChapterDialog
              open={!!deletingChapter}
              onOpenChange={(open) => !open && setDeletingChapter(null)}
              chapter={deletingChapter}
              onDelete={handleDeleteChapter}
              isLoading={isSavingChapter}
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

// Função auxiliar para obter cor do plot type (borda lateral)
function getPlotTypeBorderColor(type: string): string {
  const colors: Record<string, string> = {
    'Main': 'border-purple-500',
    'Subplot': 'border-indigo-400',
    'Character Arc': 'border-cyan-500',
    'Romance': 'border-pink-500',
    'Mystery': 'border-orange-500',
  };
  return colors[type] || 'border-gray-400';
}

// Função auxiliar para obter label do plot type
function getPlotTypeLabel(type: string): string {
  const labels: Record<string, string> = {
    'Main': 'Principal',
    'Subplot': 'Subplot',
    'Character Arc': 'Arco de Personagem',
    'Romance': 'Romance',
    'Mystery': 'Mistério',
  };
  return labels[type] || type;
}
