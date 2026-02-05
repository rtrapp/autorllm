import { useState, useRef, useEffect } from 'react';
import { Bot, Send, Sparkles, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { useAgUIChat } from '@/hooks/use-ag-ui-chat';
import { AgMessageRenderer, StreamingMessageRenderer } from './AgMessageRenderer';
import { useNavigate } from 'react-router-dom';

interface BrainstormChatProps {
  onClose?: () => void;
}

export function BrainstormChat({ onClose }: BrainstormChatProps) {
  const [inputValue, setInputValue] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const messagesContainerRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();

  const {
    conversation,
    isStreaming,
    error,
    sendMessage,
  } = useAgUIChat();

  // Auto-scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [conversation.messages, conversation.streamingMessage]);

  // Handle scrollbar visibility on scroll
  useEffect(() => {
    const container = messagesContainerRef.current;
    if (!container) return;

    let scrollTimeout: number;

    const handleScroll = () => {
      container.classList.add('scrolling');
      
      clearTimeout(scrollTimeout);
      scrollTimeout = window.setTimeout(() => {
        container.classList.remove('scrolling');
      }, 1000);
    };

    container.addEventListener('scroll', handleScroll);
    return () => {
      container.removeEventListener('scroll', handleScroll);
      clearTimeout(scrollTimeout);
    };
  }, []);

  const handleSend = async () => {
    if (!inputValue.trim() || isStreaming) return;

    const messageToSend = inputValue.trim();
    setInputValue('');

    await sendMessage(messageToSend);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleAction = (actionType: string, payload?: Record<string, unknown>) => {
    console.log('Action triggered:', actionType, payload);
    
    switch (actionType) {
      case 'submit_answers':
        // User completed all questions, send answers back to LLM
        if (payload?.answers) {
          const answersText = Object.entries(payload.answers as Record<string, string>)
            .map(([, answer], index) => `Resposta ${index + 1}: ${answer}`)
            .join('\n\n');
          
          sendMessage(answersText);
        }
        break;
      case 'submit_choices':
        // User selected choice(s), send selection back to LLM
        if (payload?.choices) {
          const choicesText = typeof payload.choices === 'string' 
            ? payload.choices 
            : JSON.stringify(payload.choices);
          
          // Clear format to indicate user made a DECISION, not requesting more choices
          sendMessage(`✅ ESCOLHA CONFIRMADA: ${choicesText}`);
        }
        break;
      case 'save_project':
        // TODO: Implement save project logic
        console.log('Saving project with data:', payload);
        break;
      case 'update_outline':
        // TODO: Implement update outline logic
        console.log('Updating outline with data:', payload);
        break;
      case 'navigate':
        if (payload?.path) {
          navigate(payload.path as string);
        }
        break;
      default:
        console.warn('Unknown action type:', actionType);
    }
  };

  const hasMessages = conversation.messages.length > 0;

  return (
    <div className="flex flex-col h-full max-w-4xl mx-auto">
      {/* Header */}
      <div className="h-14 border-b flex items-center justify-between px-4 shrink-0 bg-background">
        <div className="font-medium flex items-center gap-2 text-primary">
          <Bot className="h-4.5 w-4.5" />
          Brainstorm com LLM
          <span className="text-xs text-muted-foreground font-normal">
            (AG-UI Protocol)
          </span>
        </div>
        {onClose && (
          <Button variant="ghost" size="sm" onClick={onClose}>
            Fechar
          </Button>
        )}
      </div>

      {/* Messages Area */}
      <div ref={messagesContainerRef} className="flex-1 overflow-y-auto p-6 space-y-6 scrollbar-hide-auto">
        {!hasMessages && !conversation.streamingMessage && (
          <div className="flex flex-col items-center justify-center h-full text-center p-6">
            <Sparkles className="h-16 w-16 text-primary mb-4 animate-pulse" />
            <h3 className="font-semibold text-2xl mb-3">Comece seu livro</h3>
            <p className="text-base text-muted-foreground max-w-md">
              Descreva sua ideia de livro abaixo. A LLM vai fazer perguntas para ajudar você a
              estruturar sua história usando o protocolo AG-UI.
            </p>
          </div>
        )}

        {conversation.messages.map((message) => (
          <AgMessageRenderer 
            key={message.id} 
            message={message} 
            onAction={handleAction}
          />
        ))}

        {/* Show streaming message in real-time */}
        {conversation.streamingMessage && (
          <StreamingMessageRenderer 
            content={conversation.streamingMessage.partialContent}
            onAction={handleAction}
          />
        )}

        {error && (
          <div className="bg-destructive/10 border border-destructive/20 text-destructive px-4 py-3 rounded-lg text-sm">
            <strong>Erro:</strong> {error}
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div className="p-6 border-t bg-background mt-auto">
        <div className="relative">
          <Textarea
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            onKeyDown={handleKeyDown}
            disabled={isStreaming}
            className="min-h-[100px] py-3 resize-none pr-12 font-sans focus-visible:ring-0"
            placeholder={
              !hasMessages
                ? 'Descreva sua ideia de livro...'
                : 'Responda às perguntas ou envie mais informações...'
            }
          />
          <Button
            size="icon"
            onClick={handleSend}
            disabled={!inputValue.trim() || isStreaming}
            className="absolute bottom-3 right-3 h-9 w-9 rounded-full"
          >
            {isStreaming ? (
              <Loader2 className="h-5 w-5 animate-spin" />
            ) : (
              <Send className="h-5 w-5" />
            )}
          </Button>
        </div>

        {/* Status */}
        <div className="text-xs text-muted-foreground mt-2 text-center">
          LLM Pronto (AG-UI Protocol via HTTP)
        </div>
      </div>
    </div>
  );
}
