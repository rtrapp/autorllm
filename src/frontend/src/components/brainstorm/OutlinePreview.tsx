import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Button } from '@/components/ui/button';
import { BookOpen, Users, MapPin, GitBranch, FileText, Check } from 'lucide-react';
import type { OutlineData } from '@/types/ag-ui';

interface OutlinePreviewProps {
  outline: OutlineData;
  onAccept?: () => void;
  onRegenerate?: () => void;
}

const getRoleBorderColor = (role: string): string => {
  const colors: Record<string, string> = {
    'Protagonist': 'border-blue-500',
    'Antagonist': 'border-red-500',
    'Supporting': 'border-green-500',
    'Minor': 'border-gray-400',
  };
  return colors[role] || 'border-gray-400';
};

const getPlotTypeBorderColor = (type: string): string => {
  const colors: Record<string, string> = {
    'Main': 'border-purple-500',
    'Subplot': 'border-indigo-400',
    'Character Arc': 'border-cyan-500',
    'Romance': 'border-pink-500',
    'Mystery': 'border-orange-500',
  };
  return colors[type] || 'border-gray-400';
};

export function OutlinePreview({ outline, onAccept, onRegenerate }: OutlinePreviewProps) {
  return (
    <Card className="w-full max-w-4xl mx-auto">
      <CardHeader>
        <div className="flex items-start justify-between">
          <div className="space-y-2">
            <CardTitle className="text-2xl flex items-center gap-2">
              <BookOpen className="h-6 w-6" />
              {outline.title}
            </CardTitle>
            <CardDescription>por {outline.author}</CardDescription>
            {outline.genre && (
              <Badge variant="secondary" className="mt-2">
                {outline.genre}
              </Badge>
            )}
          </div>
          <div className="flex gap-2">
            {onRegenerate && (
              <Button variant="outline" size="sm" onClick={onRegenerate}>
                Regenerar
              </Button>
            )}
            {onAccept && (
              <Button size="sm" onClick={onAccept} className="gap-2">
                <Check className="h-4 w-4" />
                Aceitar Outline
              </Button>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* Synopsis */}
        <div className="space-y-2">
          <h3 className="font-semibold text-lg flex items-center gap-2">
            <FileText className="h-5 w-5" />
            Sinopse
          </h3>
          <p className="text-sm text-muted-foreground leading-relaxed">
            {outline.synopsis}
          </p>
          {outline.targetWordCount && (
            <p className="text-xs text-muted-foreground mt-2">
              Contagem alvo: {outline.targetWordCount.toLocaleString('pt-BR')} palavras
            </p>
          )}
        </div>

        <Separator />

        {/* Characters */}
        <div className="space-y-3">
          <h3 className="font-semibold text-lg flex items-center gap-2">
            <Users className="h-5 w-5" />
            Personagens ({outline.characters.length})
          </h3>
          <ScrollArea className="h-64">
            <div className="space-y-3 pr-4">
              {outline.characters.map((char, idx) => (
                <div
                  key={idx}
                  className={`border-l-4 ${getRoleBorderColor(char.role)} pl-3 py-2 space-y-1`}
                >
                  <div className="flex items-center gap-2">
                    <span className="font-medium">{char.name}</span>
                    <Badge variant="outline" className="text-xs">
                      {char.role}
                    </Badge>
                  </div>
                  <p className="text-sm text-muted-foreground">{char.description}</p>
                  {char.backstory && (
                    <p className="text-xs text-muted-foreground mt-1">
                      <strong>Passado:</strong> {char.backstory}
                    </p>
                  )}
                </div>
              ))}
            </div>
          </ScrollArea>
        </div>

        <Separator />

        {/* Locations */}
        {outline.locations && outline.locations.length > 0 && (
          <>
            <div className="space-y-3">
              <h3 className="font-semibold text-lg flex items-center gap-2">
                <MapPin className="h-5 w-5" />
                Locais ({outline.locations.length})
              </h3>
              <div className="grid gap-3">
                {outline.locations.map((loc, idx) => (
                  <div
                    key={idx}
                    className="border-l-4 border-amber-500 pl-3 py-2 space-y-1"
                  >
                    <span className="font-medium">{loc.name}</span>
                    <p className="text-sm text-muted-foreground">{loc.description}</p>
                    {loc.significance && (
                      <p className="text-xs text-muted-foreground mt-1">
                        <strong>Importância:</strong> {loc.significance}
                      </p>
                    )}
                  </div>
                ))}
              </div>
            </div>
            <Separator />
          </>
        )}

        {/* Plots */}
        <div className="space-y-3">
          <h3 className="font-semibold text-lg flex items-center gap-2">
            <GitBranch className="h-5 w-5" />
            Tramas ({outline.plots.length})
          </h3>
          <div className="space-y-3">
            {outline.plots.map((plot, idx) => (
              <div
                key={idx}
                className={`border-l-4 ${getPlotTypeBorderColor(plot.type)} pl-3 py-2 space-y-1`}
              >
                <div className="flex items-center gap-2">
                  <span className="font-medium">{plot.title}</span>
                  <Badge variant="outline" className="text-xs">
                    {plot.type}
                  </Badge>
                </div>
                <p className="text-sm text-muted-foreground">{plot.description}</p>
                {plot.resolution && (
                  <p className="text-xs text-muted-foreground mt-1">
                    <strong>Resolução:</strong> {plot.resolution}
                  </p>
                )}
              </div>
            ))}
          </div>
        </div>

        <Separator />

        {/* Chapters */}
        <div className="space-y-3">
          <h3 className="font-semibold text-lg flex items-center gap-2">
            <BookOpen className="h-5 w-5" />
            Capítulos ({outline.chapters.length})
          </h3>
          <ScrollArea className="h-96">
            <div className="space-y-3 pr-4">
              {outline.chapters
                .sort((a, b) => a.order - b.order)
                .map((chapter) => (
                  <div
                    key={chapter.order}
                    className="border-l-4 border-slate-500 pl-3 py-2 space-y-1"
                  >
                    <div className="flex items-center gap-2">
                      <Badge variant="secondary" className="text-xs">
                        #{chapter.order}
                      </Badge>
                      <span className="font-medium">{chapter.title}</span>
                    </div>
                    <p className="text-sm text-muted-foreground">{chapter.summary}</p>
                  </div>
                ))}
            </div>
          </ScrollArea>
        </div>
      </CardContent>
    </Card>
  );
}
