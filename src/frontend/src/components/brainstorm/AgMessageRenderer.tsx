import { Sparkles, AlertCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import ReactMarkdown from 'react-markdown';
import rehypeRaw from 'rehype-raw';
import remarkGfm from 'remark-gfm';
import { QuestionList } from './QuestionList';
import { ChoiceList } from './ChoiceList';
import type {
  AgMessage,
  Content,
  TextContent,
  ComponentContent,
  ErrorContent,
  ButtonComponent,
  CardComponent,
  QuestionListComponent,
  ChoiceListComponent,
  OutlinePreviewComponent,
} from '@/types/ag-ui';

interface AgMessageRendererProps {
  message: AgMessage;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}

export function AgMessageRenderer({ message, onAction }: AgMessageRendererProps) {
  if (message.role === 'user') {
    return (
      <div className="flex justify-end">
        <div className="bg-primary text-primary-foreground px-4 py-2 rounded-2xl rounded-tr-sm max-w-[85%] text-sm">
          {renderContent(message.content)}
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-start gap-3">
      <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
        <Sparkles className="h-4 w-4 text-primary" />
      </div>
      <div className="flex-1 text-sm">
        {message.content.map((content, index) => (
          <div key={index}>
            {renderContentItem(content, onAction)}
          </div>
        ))}
      </div>
    </div>
  );
}

function renderContent(content: Content[]): React.ReactNode {
  return content.map((item, index) => (
    <span key={index}>{renderContentItem(item)}</span>
  ));
}

function renderContentItem(
  content: Content,
  onAction?: (action: string, payload?: Record<string, unknown>) => void
): React.ReactNode {
  switch (content.type) {
    case 'text':
      return <TextRenderer content={content} onAction={onAction} />;
    case 'component':
      return <ComponentRenderer content={content} onAction={onAction} />;
    case 'error':
      return <ErrorRenderer content={content} />;
    default:
      return null;
  }
}

// Parser para componentes AG-UI customizados (ex: <AGUIButton>, <AGICard>)
function parseAGUIComponents(text: string, onAction?: (action: string, payload?: Record<string, unknown>) => void): React.ReactNode[] {
  const elements: React.ReactNode[] = [];
  
  // Regex para AGUIButton (self-closing)
  const aguiButtonRegex = /<AGUIButton\s+label="([^"]+)"\s+action="([^"]+)"\s*\/>/g;
  
  // Regex para AGICard (com conteúdo interno)
  const agiCardRegex = /<AGICard\s+title="([^"]+)"\s*>([\s\S]*?)<\/AGICard>/g;
  
  // Criar array de matches com tipo e posição
  interface Match {
    type: 'button' | 'card';
    index: number;
    endIndex: number;
    label?: string;
    action?: string;
    title?: string;
    content?: string;
  }
  
  const matches: Match[] = [];
  
  let match;
  while ((match = aguiButtonRegex.exec(text)) !== null) {
    matches.push({
      type: 'button',
      index: match.index,
      endIndex: aguiButtonRegex.lastIndex,
      label: match[1],
      action: match[2],
    });
  }
  
  while ((match = agiCardRegex.exec(text)) !== null) {
    matches.push({
      type: 'card',
      index: match.index,
      endIndex: agiCardRegex.lastIndex,
      title: match[1],
      content: match[2],
    });
  }
  
  // Ordenar matches por índice
  matches.sort((a, b) => a.index - b.index);
  
  let lastIndex = 0;
  
  matches.forEach((match) => {
    // Adicionar texto antes do componente
    if (match.index > lastIndex) {
      const textBefore = text.substring(lastIndex, match.index);
      elements.push(
        <ReactMarkdown 
          key={`text-${lastIndex}`}
          remarkPlugins={[remarkGfm]}
          rehypePlugins={[rehypeRaw]}
          components={getMarkdownComponents()}
        >
          {textBefore}
        </ReactMarkdown>
      );
    }
    
    // Adicionar componente
    if (match.type === 'button') {
      elements.push(
        <Button
          key={`button-${match.index}`}
          onClick={() => onAction?.(match.action!)}
          className="mr-2 mb-2"
          variant="default"
          size="sm"
        >
          {match.label}
        </Button>
      );
    } else if (match.type === 'card') {
      elements.push(
        <Card key={`card-${match.index}`} className="my-3">
          <CardHeader>
            <CardTitle className="text-base">{match.title}</CardTitle>
          </CardHeader>
          <CardContent>
            <ReactMarkdown 
              remarkPlugins={[remarkGfm]}
              rehypePlugins={[rehypeRaw]}
              components={getMarkdownComponents()}
            >
              {match.content!}
            </ReactMarkdown>
          </CardContent>
        </Card>
      );
    }
    
    lastIndex = match.endIndex;
  });
  
  // Adicionar texto restante
  if (lastIndex < text.length) {
    const textAfter = text.substring(lastIndex);
    elements.push(
      <ReactMarkdown 
        key={`text-${lastIndex}`}
        remarkPlugins={[remarkGfm]}
        rehypePlugins={[rehypeRaw]}
        components={getMarkdownComponents()}
      >
        {textAfter}
      </ReactMarkdown>
    );
  }
  
  return elements.length > 0 ? elements : [
    <ReactMarkdown 
      key="default"
      remarkPlugins={[remarkGfm]}
      rehypePlugins={[rehypeRaw]}
      components={getMarkdownComponents()}
    >
      {text}
    </ReactMarkdown>
  ];
}

function getMarkdownComponents() {
  return {
    h2: ({node, ...props}: any) => <h2 className="font-semibold text-lg mt-4 mb-2" {...props} />,
    h3: ({node, ...props}: any) => <h3 className="font-semibold text-base mt-3 mb-2" {...props} />,
    table: ({node, ...props}: any) => (
      <div className="overflow-x-auto my-3">
        <table className="border-collapse border border-border w-full text-sm" {...props} />
      </div>
    ),
    th: ({node, ...props}: any) => <th className="border border-border px-3 py-2 bg-muted font-semibold text-left" {...props} />,
    td: ({node, ...props}: any) => <td className="border border-border px-3 py-2" {...props} />,
    a: ({node, ...props}: any) => <a className="text-primary underline hover:text-primary/80" target="_blank" rel="noopener noreferrer" {...props} />,
    strong: ({node, ...props}: any) => <strong className="font-semibold" {...props} />,
    hr: ({node, ...props}: any) => <hr className="my-3 border-border" {...props} />,
  };
}

function TextRenderer({ content, onAction }: { content: TextContent; onAction?: (action: string, payload?: Record<string, unknown>) => void }) {
  const hasAGUIComponents = /<AGUI|<AGI/.test(content.text);
  
  if (hasAGUIComponents) {
    return (
      <div className="prose prose-sm max-w-none dark:prose-invert">
        {parseAGUIComponents(content.text, onAction)}
      </div>
    );
  }
  
  return (
    <div className="prose prose-sm max-w-none dark:prose-invert">
      <ReactMarkdown 
        remarkPlugins={[remarkGfm]}
        rehypePlugins={[rehypeRaw]}
        components={getMarkdownComponents()}
      >
        {content.text}
      </ReactMarkdown>
    </div>
  );
}

function ComponentRenderer({
  content,
  onAction,
}: {
  content: ComponentContent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  const { component } = content;

  switch (component.type) {
    case 'button':
      return <ButtonRenderer component={component} onAction={onAction} />;
    case 'card':
      return <CardRenderer component={component} onAction={onAction} />;
    case 'question-list':
      return <QuestionListRenderer component={component} onAction={onAction} />;
    case 'choice-list':
      return <ChoiceListRenderer component={component} onAction={onAction} />;
    case 'outline-preview':
      return <OutlinePreviewRenderer component={component} onAction={onAction} />;
    default:
      return null;
  }
}

function ButtonRenderer({
  component,
  onAction,
}: {
  component: ButtonComponent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  const variantMap = {
    primary: 'default',
    secondary: 'secondary',
    outline: 'outline',
    ghost: 'ghost',
  };

  return (
    <Button
      variant={variantMap[component.variant || 'primary'] as any}
      size="sm"
      onClick={() => onAction?.(component.action.type, component.action.payload)}
      className="mt-2"
    >
      {component.label}
    </Button>
  );
}

function CardRenderer({
  component,
  onAction,
}: {
  component: CardComponent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  return (
    <div className="bg-secondary/20 border border-secondary rounded-lg p-4 mt-2">
      <h4 className="font-semibold mb-1">{component.title}</h4>
      {component.description && (
        <p className="text-xs text-muted-foreground mb-3">{component.description}</p>
      )}
      {component.content && (
        <div className="space-y-2">
          {component.content.map((content, index) => (
            <div key={index}>{renderContentItem(content, onAction)}</div>
          ))}
        </div>
      )}
      {component.actions && component.actions.length > 0 && (
        <div className="flex gap-2 mt-3">
          {component.actions.map((action, index) => (
            <ButtonRenderer key={index} component={action} onAction={onAction} />
          ))}
        </div>
      )}
    </div>
  );
}

function QuestionListRenderer({
  component,
  onAction,
}: {
  component: QuestionListComponent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  const handleAnswer = (questionId: string, answer: string) => {
    console.log('Question answered:', questionId, answer);
  };

  const handleComplete = (answers: Record<string, string>) => {
    console.log('All questions answered:', answers);
    // Trigger action to send answers back to LLM
    onAction?.('submit_answers', { answers });
  };

  return (
    <div className="mt-3">
      <QuestionList
        component={component}
        onAnswer={handleAnswer}
        onComplete={handleComplete}
      />
    </div>
  );
}

function ChoiceListRenderer({
  component,
  onAction,
}: {
  component: ChoiceListComponent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  const handleSelect = (selectedChoices: string[]) => {
    console.log('Choices selected:', selectedChoices);
    // Format as readable text and send back to LLM
    const choicesText = selectedChoices.join('\n\n');
    onAction?.('submit_choices', { choices: choicesText });
  };

  return (
    <div className="mt-3">
      <ChoiceList
        component={component}
        onSelect={handleSelect}
      />
    </div>
  );
}

function OutlinePreviewRenderer({
  component,
  onAction,
}: {
  component: OutlinePreviewComponent;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}) {
  return (
    <div className="bg-secondary/10 border rounded-lg p-4 mt-3 space-y-4">
      <div>
        <h3 className="font-bold text-base">{component.title}</h3>
        <p className="text-xs text-muted-foreground mt-1">
          {component.plotType} • {component.chapters.length} capítulos • {component.characters.length} personagens
        </p>
      </div>

      <div>
        <h4 className="font-semibold text-sm mb-1">Sinopse</h4>
        <p className="text-xs">{component.synopsis}</p>
      </div>

      <div>
        <h4 className="font-semibold text-sm mb-2">Capítulos</h4>
        <div className="space-y-2">
          {component.chapters.slice(0, 3).map((chapter) => (
            <div key={chapter.number} className="text-xs">
              <span className="font-medium">
                {chapter.number}. {chapter.title}
              </span>
              <p className="text-muted-foreground ml-4">{chapter.summary}</p>
            </div>
          ))}
          {component.chapters.length > 3 && (
            <p className="text-xs text-muted-foreground ml-4">
              ... e mais {component.chapters.length - 3} capítulos
            </p>
          )}
        </div>
      </div>

      <div>
        <h4 className="font-semibold text-sm mb-2">Personagens</h4>
        <div className="space-y-1">
          {component.characters.map((character) => (
            <div key={character.name} className="text-xs">
              <span className="font-medium">{character.name}</span>
              <span className="text-muted-foreground"> ({character.role})</span>
            </div>
          ))}
        </div>
      </div>

      <div className="flex gap-2 pt-2">
        <Button
          size="sm"
          onClick={() =>
            onAction?.('save_project', {
              outline: component,
            })
          }
        >
          Salvar Projeto
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() =>
            onAction?.('update_outline', {
              outline: component,
            })
          }
        >
          Editar Outline
        </Button>
      </div>
    </div>
  );
}

function ErrorRenderer({ content }: { content: ErrorContent }) {
  return (
    <div className="bg-destructive/10 border border-destructive/20 text-destructive px-3 py-2 rounded-md text-xs flex items-start gap-2 mt-2">
      <AlertCircle className="h-4 w-4 shrink-0 mt-0.5" />
      <div>
        <strong>Erro{content.code ? ` (${content.code})` : ''}:</strong> {content.error}
      </div>
    </div>
  );
}

interface StreamingMessageRendererProps {
  content: string;
  onAction?: (action: string, payload?: Record<string, unknown>) => void;
}

export function StreamingMessageRenderer({ content, onAction }: StreamingMessageRendererProps) {
  const hasAGUIComponents = /<AGUI|<AGI/.test(content);
  
  return (
    <div className="flex items-start gap-3">
      <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
        <Sparkles className="h-4 w-4 text-primary animate-pulse" />
      </div>
      <div className="flex-1 text-sm">
        {hasAGUIComponents ? (
          <div className="prose prose-sm max-w-none dark:prose-invert">
            {parseAGUIComponents(content, onAction)}
          </div>
        ) : (
          <div className="prose prose-sm max-w-none dark:prose-invert">
            <ReactMarkdown 
              remarkPlugins={[remarkGfm]}
              rehypePlugins={[rehypeRaw]}
              components={getMarkdownComponents()}
            >
              {content}
            </ReactMarkdown>
          </div>
        )}
      </div>
    </div>
  );
}
