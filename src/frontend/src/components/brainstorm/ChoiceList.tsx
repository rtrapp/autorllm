import { useState } from 'react';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Check } from 'lucide-react';
import type { ChoiceListComponent } from '@/types/ag-ui';

interface ChoiceListProps {
  component: ChoiceListComponent;
  onSelect: (selectedChoices: string[]) => void;
}

export function ChoiceList({ component, onSelect }: ChoiceListProps) {
  const { choices, allowMultiple = false, contextText } = component;
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());

  const handleToggleChoice = (choiceId: string) => {
    if (allowMultiple) {
      const newSelected = new Set(selectedIds);
      if (newSelected.has(choiceId)) {
        newSelected.delete(choiceId);
      } else {
        newSelected.add(choiceId);
      }
      setSelectedIds(newSelected);
    } else {
      // Single selection - replace
      setSelectedIds(new Set([choiceId]));
    }
  };

  const handleConfirm = () => {
    const selected = choices
      .filter(choice => selectedIds.has(choice.id))
      .map(choice => `${choice.option}: ${choice.description}`);
    
    onSelect(selected);
  };

  const isSelected = (choiceId: string) => selectedIds.has(choiceId);

  return (
    <Card className="w-full max-w-3xl mx-auto">
      <CardHeader>
        <CardTitle className="text-lg">
          {contextText || 'Escolha uma opção:'}
        </CardTitle>
        {allowMultiple && (
          <p className="text-sm text-muted-foreground">
            Você pode selecionar múltiplas opções
          </p>
        )}
      </CardHeader>
      <CardContent className="space-y-3">
        {choices.map((choice) => (
          <button
            key={choice.id}
            onClick={() => handleToggleChoice(choice.id)}
            className={`w-full text-left p-4 rounded-lg border-2 transition-all hover:shadow-md ${
              isSelected(choice.id)
                ? 'border-blue-500 bg-blue-50 dark:bg-blue-950'
                : 'border-gray-200 dark:border-gray-700 hover:border-gray-300'
            }`}
          >
            <div className="flex items-start gap-3">
              <div
                className={`flex-shrink-0 w-6 h-6 rounded-full border-2 flex items-center justify-center transition-all ${
                  isSelected(choice.id)
                    ? 'border-blue-500 bg-blue-500'
                    : 'border-gray-300 dark:border-gray-600'
                }`}
              >
                {isSelected(choice.id) && (
                  <Check className="w-4 h-4 text-white" />
                )}
              </div>
              <div className="flex-1">
                <h4 className="font-semibold text-base mb-1">
                  {choice.option}
                </h4>
                <p className="text-sm text-gray-600 dark:text-gray-400">
                  {choice.description}
                </p>
              </div>
            </div>
          </button>
        ))}
      </CardContent>
      <CardFooter className="flex justify-between items-center">
        <p className="text-sm text-muted-foreground">
          {selectedIds.size} {allowMultiple ? 'selecionada(s)' : 'selecionada'}
        </p>
        <Button
          onClick={handleConfirm}
          disabled={selectedIds.size === 0}
        >
          Confirmar
        </Button>
      </CardFooter>
    </Card>
  );
}
