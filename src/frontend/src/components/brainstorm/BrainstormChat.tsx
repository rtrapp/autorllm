import { useState, useRef, useEffect } from 'react';
import { Bot, Send, Sparkles, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { useBrainstorm, type BrainstormMessage } from '@/hooks/use-brainstorm';

interface BrainstormChatProps {
  onClose?: () => void;
}

export function BrainstormChat({ onClose }: BrainstormChatProps) {
  const [inputValue, setInputValue] = useState('');
  const [hasStarted, setHasStarted] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const {
    messages,
    isStreaming,
    streamingContent,
    isConnected,
    error,
    startBrainstorm,
    sendMessage,
  } = useBrainstorm();

  // Auto-scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = async () => {
    if (!inputValue.trim() || isStreaming) return;

    const messageToSend = inputValue.trim();
    setInputValue('');

    if (!hasStarted) {
      await startBrainstorm(messageToSend);
      setHasStarted(true);
    } else {
      await sendMessage(messageToSend);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <aside className="w-96 bg-background border-l flex flex-col shrink-0 relative">
      {/* Header */}
      <div className="h-14 border-b flex items-center justify-between px-4 shrink-0 bg-background">
        <div className="font-medium flex items-center gap-2 text-primary">
          <Bot className="h-4.5 w-4.5" />
          Brainstorm com LLM
        </div>
        {onClose && (
          <Button variant="ghost" size="sm" onClick={onClose}>
            Fechar
          </Button>
        )}
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-secondary/20">
        {messages.length === 0 && !hasStarted && (
          <div className="flex flex-col items-center justify-center h-full text-center p-6">
            <Sparkles className="h-12 w-12 text-primary mb-4 animate-pulse" />
            <h3 className="font-semibold text-lg mb-2">Comece seu livro</h3>
            <p className="text-sm text-muted-foreground">
              Descreva sua ideia de livro abaixo. A LLM vai fazer perguntas para ajudar você a
              estruturar sua história.
            </p>
          </div>
        )}

        {messages.map((message) => (
          <MessageBubble key={message.id} message={message} />
        ))}

        {/* Show streaming message in real-time */}
        {isStreaming && streamingContent && (
          <div className="flex items-start gap-3">
            <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
              <Sparkles className="h-4 w-4 text-primary animate-pulse" />
            </div>
            <div className="bg-card border px-4 py-2 rounded-2xl rounded-tl-sm max-w-[90%] text-sm shadow-sm">
              <div className="whitespace-pre-wrap">{streamingContent}</div>
            </div>
          </div>
        )}

        {error && (
          <div className="bg-destructive/10 border border-destructive/20 text-destructive px-4 py-3 rounded-lg text-sm">
            <strong>Erro:</strong> {error}
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div className="p-4 border-t bg-background mt-auto">
        <div className="relative">
          <Textarea
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            onKeyDown={handleKeyDown}
            disabled={isStreaming || !isConnected}
            className="min-h-[80px] py-2 resize-none pr-10 font-sans focus-visible:ring-0"
            placeholder={
              !hasStarted
                ? 'Descreva sua ideia de livro...'
                : 'Responda às perguntas ou envie mais informações...'
            }
          />
          <Button
            size="icon"
            onClick={handleSend}
            disabled={!inputValue.trim() || isStreaming || !isConnected}
            className="absolute bottom-2 right-2 h-8 w-8 rounded-full"
          >
            {isStreaming ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Send className="h-4 w-4" />
            )}
          </Button>
        </div>

        {/* Connection Status */}
        <div className="text-xs text-muted-foreground mt-2 text-center flex items-center justify-center gap-1">
          <span
            className={`h-2 w-2 rounded-full ${
              isConnected ? 'bg-emerald-500' : 'bg-gray-400'
            }`}
          />
          {isConnected ? 'LLM Pronto' : 'Conectando...'}
        </div>
      </div>
    </aside>
  );
}

interface MessageBubbleProps {
  message: BrainstormMessage;
}

function MessageBubble({ message }: MessageBubbleProps) {
  if (message.role === 'user') {
    return (
      <div className="flex justify-end">
        <div className="bg-primary text-primary-foreground px-4 py-2 rounded-2xl rounded-tr-sm max-w-[85%] text-sm">
          {message.content}
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-start gap-3">
      <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
        <Sparkles className="h-4 w-4 text-primary" />
      </div>
      <div className="bg-card border px-4 py-2 rounded-2xl rounded-tl-sm max-w-[90%] text-sm shadow-sm">
        <div className="whitespace-pre-wrap">{message.content}</div>
      </div>
    </div>
  );
}
