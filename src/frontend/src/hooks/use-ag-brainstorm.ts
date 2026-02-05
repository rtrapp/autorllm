import { useState, useCallback } from 'react';
import { useSignalR } from './use-signalr';
import type {
  AgMessage,
  ConversationState,
} from '@/types/ag-ui';
import { createTextMessage, parseAgContent } from '@/types/ag-ui';

interface UseAgBrainstormReturn {
  conversation: ConversationState;
  isStreaming: boolean;
  isConnected: boolean;
  error: string | null;
  sendMessage: (text: string) => Promise<void>;
  clearConversation: () => void;
}

/**
 * Hook para gerenciar sessão de brainstorm usando protocolo AG-UI
 */
export function useAgBrainstorm(): UseAgBrainstormReturn {
  const sessionId = useState(() => crypto.randomUUID())[0];
  
  const [conversation, setConversation] = useState<ConversationState>({
    messages: [],
    streamingMessage: null,
    sessionId,
  });

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
    async (text: string) => {
      if (!isConnected) {
        console.log('Waiting for connection...');
        return;
      }

      // Create user message following AG-UI protocol
      const userMessage = createTextMessage('user', text, {
        timestamp: new Date(),
        sessionId: conversation.sessionId,
      });

      // Add user message to conversation
      setConversation((prev) => ({
        ...prev,
        messages: [...prev.messages, userMessage],
      }));

      try {
        // Determine if this is the first message (brainstorm) or continuation
        const isFirstMessage = conversation.messages.length === 0;
        
        if (isFirstMessage) {
          await invoke('StartBrainstorm', text);
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

  const clearConversation = useCallback(() => {
    setConversation({
      messages: [],
      streamingMessage: null,
      sessionId: crypto.randomUUID(),
    });
  }, []);

  const isStreaming = conversation.streamingMessage !== null;

  return {
    conversation,
    isStreaming,
    isConnected,
    error,
    sendMessage,
    clearConversation,
  };
}
