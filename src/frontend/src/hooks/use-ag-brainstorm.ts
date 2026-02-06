import { useState, useCallback } from 'react';
import { useSignalR } from './use-signalr';
import type {
  AgMessage,
  ConversationState,
  BrainstormContext,
  OutlineData,
} from '@/types/ag-ui';
import { createTextMessage, parseAgContent, createBrainstormContext, createComponentMessage } from '@/types/ag-ui';
import { api } from '@/lib/api';

interface UseAgBrainstormReturn {
  conversation: ConversationState;
  context: BrainstormContext;
  isStreaming: boolean;
  isConnected: boolean;
  error: string | null;
  sendMessage: (text: string, category?: string) => Promise<void>;
  generateOutline: () => Promise<void>;
  updateContext: (updates: Partial<BrainstormContext>) => void;
  clearConversation: () => void;
}

/**
 * Hook para gerenciar sessão de brainstorm usando protocolo AG-UI
 */
export function useAgBrainstorm(initialBookIdea?: string): UseAgBrainstormReturn {
  const sessionId = useState(() => crypto.randomUUID())[0];
  
  const [conversation, setConversation] = useState<ConversationState>({
    messages: [],
    streamingMessage: null,
    sessionId,
  });

  // Estado para acumular contexto das respostas
  const [context, setContext] = useState<BrainstormContext>(() =>
    initialBookIdea ? createBrainstormContext(initialBookIdea) : { bookIdea: '', characters: [], locations: [], plots: [], chapters: [] }
  );

  const handleTokenReceived = useCallback((token: string) => {
    setConversation((prev) => {
      const currentStreaming = prev.streamingMessage;
      
      if (!currentStreaming) {
        // Start new streaming message
        return {
          ...prev,
          streamingMessage: {
            id: crypto.randomUUID(),
            role: 'assistant',
            partialContent: token,
            state: 'streaming',
          },
        };
      }

      // Append to existing streaming message
      return {
        ...prev,
        streamingMessage: {
          ...currentStreaming,
          partialContent: currentStreaming.partialContent + token,
        },
      };
    });
  }, []);

  const handleComplete = useCallback(() => {
    setConversation((prev) => {
      if (!prev.streamingMessage) return prev;

      // Convert streaming message to complete AG-UI message
      const completeMessage: AgMessage = {
        id: prev.streamingMessage.id,
        role: 'assistant',
        content: parseAgContent(prev.streamingMessage.partialContent),
        metadata: {
          timestamp: new Date(),
          sessionId: prev.sessionId,
        },
      };

      return {
        ...prev,
        messages: [...prev.messages, completeMessage],
        streamingMessage: null,
      };
    });
  }, []);

  const handleError = useCallback((errorMessage: string) => {
    console.error('Brainstorm error:', errorMessage);
    
    setConversation((prev) => ({
      ...prev,
      streamingMessage: prev.streamingMessage
        ? { ...prev.streamingMessage, state: 'error' }
        : null,
    }));
  }, []);

  const { isConnected, error, invoke } = useSignalR({
    onTokenReceived: handleTokenReceived,
    onComplete: handleComplete,
    onError: handleError,
  });

  const sendMessage = useCallback(
    async (text: string, category?: string) => {
      if (!isConnected) {
        console.log('Waiting for connection...');
        return;
      }

      // Create user message following AG-UI protocol
      const userMessage = createTextMessage('user', text, {
        timestamp: new Date(),
        sessionId: conversation.sessionId,
        category, // Track which question category this answers
      });

      // Add user message to conversation
      setConversation((prev) => ({
        ...prev,
        messages: [...prev.messages, userMessage],
      }));

      // Update context based on category and answer
      if (category) {
        updateContextFromAnswer(category, text);
      }

      try {
        // Determine if this is the first message (brainstorm) or continuation
        const isFirstMessage = conversation.messages.length === 0;
        
        if (isFirstMessage) {
          // Store initial book idea
          setContext(prev => ({ ...prev, bookIdea: text }));
          await invoke('StartBrainstorm', conversation.sessionId, text);
        } else {
          await invoke('ContinueBrainstorm', conversation.sessionId, text);
        }
      } catch (error) {
        console.error('Failed to send message:', error);
        handleError(error instanceof Error ? error.message : 'Unknown error');
      }
    },
    [isConnected, invoke, conversation.sessionId, conversation.messages.length, handleError]
  );

  /**
   * Update context based on question category and answer
   */
  const updateContextFromAnswer = useCallback((category: string, answer: string) => {
    console.log(`Updating context for category: ${category}`);
    console.log(`Answer: ${answer.substring(0, 100)}...`);

    setContext(prev => {
      const updated = { ...prev };

      // Map categories to context fields
      switch (category.toLowerCase()) {
        case 'título':
        case 'titulo':
          updated.title = answer.trim();
          break;

        case 'autor':
        case 'author':
          updated.author = answer.trim();
          break;

        case 'gênero':
        case 'genero':
        case 'genre':
          updated.genre = answer.trim();
          break;

        case 'sinopse':
        case 'synopsis':
          updated.synopsis = answer.trim();
          break;

        case 'tom':
        case 'tone':
          updated.tone = answer.trim();
          break;

        case 'público-alvo':
        case 'publico':
        case 'target audience':
          updated.targetAudience = answer.trim();
          break;

        case 'personagens':
        case 'characters':
        case 'protagonista':
        case 'protagonist':
          // Parse character names from answer (basic parsing)
          // TODO: Task 4 will create a proper parser
          updated.characters = parseCharactersFromAnswer(answer);
          break;

        case 'locais':
        case 'lugares':
        case 'locations':
        case 'cenários':
        case 'cenarios':
          updated.locations = parseLocationsFromAnswer(answer);
          break;

        case 'conflito':
        case 'conflict':
        case 'plot':
        case 'trama':
          updated.plots = parsePlotsFromAnswer(answer);
          break;

        case 'capítulos':
        case 'capitulos':
        case 'chapters':
        case 'estrutura':
          updated.chapters = parseChaptersFromAnswer(answer);
          break;

        default:
          console.log(`Unknown category: ${category}, storing as metadata`);
      }

      console.log('Updated context:', updated);
      return updated;
    });
  }, []);

  const updateContext = useCallback((updates: Partial<BrainstormContext>) => {
    setContext(prev => ({ ...prev, ...updates }));
  }, []);

  const generateOutline = useCallback(async () => {
    console.log('🚀 Generating outline from context:', context);
    
    try {
      // Criar mensagem de loading
      const loadingMessage = createTextMessage('assistant', '⏳ Gerando outline estruturado...', {
        timestamp: new Date(),
        sessionId: conversation.sessionId,
      });
      
      setConversation(prev => ({
        ...prev,
        messages: [...prev.messages, loadingMessage],
      }));

      // Chamar API
      const response = await api.post<{
        outline: OutlineData;
        validationErrors: string[];
        isValid: boolean;
      }>('/llm/brainstorm/generate-outline', {
        sessionId: conversation.sessionId,
        bookIdea: context.bookIdea,
        title: context.title,
        author: context.author,
        genre: context.genre,
        synopsis: context.synopsis,
        tone: context.tone,
        targetAudience: context.targetAudience,
        characters: context.characters,
        locations: context.locations,
        plots: context.plots,
        chapters: context.chapters,
      });

      console.log('✅ Outline generated:', response.data);

      // Remover mensagem de loading
      setConversation(prev => ({
        ...prev,
        messages: prev.messages.filter(m => m.id !== loadingMessage.id),
      }));

      // Criar mensagem com outline preview
      const outlineMessage = createComponentMessage('assistant', {
        type: 'outline-preview',
        outline: response.data.outline,
      }, {
        timestamp: new Date(),
        sessionId: conversation.sessionId,
      });

      // Se houver erros de validação, adicionar aviso
      if (!response.data.isValid && response.data.validationErrors.length > 0) {
        const errorText = `⚠️ O outline foi gerado mas tem alguns avisos:\n${response.data.validationErrors.join('\n')}`;
        const errorMessage = createTextMessage('assistant', errorText, {
          timestamp: new Date(),
          sessionId: conversation.sessionId,
        });
        
        setConversation(prev => ({
          ...prev,
          messages: [...prev.messages, errorMessage, outlineMessage],
        }));
      } else {
        setConversation(prev => ({
          ...prev,
          messages: [...prev.messages, outlineMessage],
        }));
      }
    } catch (error: any) {
      console.error('❌ Failed to generate outline:', error);
      
      const errorText = error.response?.data?.error || error.message || 'Erro ao gerar outline';
      const errorMessage = createTextMessage('assistant', `❌ ${errorText}`, {
        timestamp: new Date(),
        sessionId: conversation.sessionId,
      });
      
      setConversation(prev => ({
        ...prev,
        messages: prev.messages.map(m => 
          m.content[0]?.type === 'text' && m.content[0].text.includes('Gerando outline')
            ? errorMessage
            : m
        ),
      }));
    }
  }, [context, conversation.sessionId]);

  const clearConversation = useCallback(() => {
    setConversation({
      messages: [],
      streamingMessage: null,
      sessionId: crypto.randomUUID(),
    });
    setContext(createBrainstormContext(''));
  }, []);

  const isStreaming = conversation.streamingMessage !== null;

  return {
    conversation,
    context,
    isStreaming,
    isConnected,
    error,
    sendMessage,
    generateOutline,
    updateContext,
    clearConversation,
  };
}

// ============================================================================
// Helper Functions for Parsing Answers (Basic Implementation)
// TODO: Task 4 will create more sophisticated parsers
// ============================================================================

/**
 * Parse character names from answer text
 * Handles formats like: "Ana, João e Maria" or "1. Ana\n2. João"
 */
function parseCharactersFromAnswer(answer: string): BrainstormContext['characters'] {
  const chars: NonNullable<BrainstormContext['characters']> = [];
  
  // Try numbered list format: 1. Name - Description
  const numberedRegex = /^\d+\.\s*([^-\n]+)(?:\s*-\s*([^\n]+))?/gm;
  let match;
  
  while ((match = numberedRegex.exec(answer)) !== null) {
    const name = match[1]?.trim();
    const description = match[2]?.trim();
    
    if (name) {
      chars.push({
        name,
        description: description || '',
      });
    }
  }
  
  // If no numbered format, try comma-separated
  if (chars.length === 0) {
    const names = answer.split(/[,;]|(\se\s)|(\sand\s)/i);
    names.forEach(name => {
      const cleanName = name.trim();
      if (cleanName && cleanName.length > 1) {
        chars.push({ name: cleanName });
      }
    });
  }
  
  return chars;
}

/**
 * Parse locations from answer text
 */
function parseLocationsFromAnswer(answer: string): BrainstormContext['locations'] {
  const locs: NonNullable<BrainstormContext['locations']> = [];
  
  // Try numbered list format
  const numberedRegex = /^\d+\.\s*([^-\n]+)(?:\s*-\s*([^\n]+))?/gm;
  let match;
  
  while ((match = numberedRegex.exec(answer)) !== null) {
    const name = match[1]?.trim();
    const description = match[2]?.trim();
    
    if (name) {
      locs.push({
        name,
        description: description || '',
      });
    }
  }
  
  // If no numbered format, try comma-separated
  if (locs.length === 0) {
    const names = answer.split(/[,;]/);
    names.forEach(name => {
      const cleanName = name.trim();
      if (cleanName && cleanName.length > 1) {
        locs.push({ name: cleanName });
      }
    });
  }
  
  return locs;
}

/**
 * Parse plots from answer text
 */
function parsePlotsFromAnswer(answer: string): BrainstormContext['plots'] {
  const plots: NonNullable<BrainstormContext['plots']> = [];
  
  // Try numbered list format
  const numberedRegex = /^\d+\.\s*([^-\n]+)(?:\s*-\s*([^\n]+))?/gm;
  let match;
  
  while ((match = numberedRegex.exec(answer)) !== null) {
    const title = match[1]?.trim();
    const description = match[2]?.trim();
    
    if (title) {
      plots.push({
        title,
        description: description || '',
        type: plots.length === 0 ? 'Main' : 'Subplot', // First is Main, rest are Subplots
      });
    }
  }
  
  // If no numbered format, create a single main plot
  if (plots.length === 0 && answer.trim().length > 0) {
    plots.push({
      title: 'Conflito Principal',
      description: answer.trim(),
      type: 'Main',
    });
  }
  
  return plots;
}

/**
 * Parse chapters from answer text
 */
function parseChaptersFromAnswer(answer: string): BrainstormContext['chapters'] {
  const chapters: NonNullable<BrainstormContext['chapters']> = [];
  
  // Try numbered list format: 1. Title - Summary
  const numberedRegex = /^\d+\.\s*([^-\n]+)(?:\s*-\s*([^\n]+))?/gm;
  let match;
  let order = 1;
  
  while ((match = numberedRegex.exec(answer)) !== null) {
    const title = match[1]?.trim();
    const summary = match[2]?.trim();
    
    if (title) {
      chapters.push({
        title,
        summary: summary || '',
        order: order++,
      });
    }
  }
  
  return chapters;
}
