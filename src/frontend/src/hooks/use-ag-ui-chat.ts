import { useState, useCallback } from 'react';
import type { AgMessage, ConversationState } from '@/types/ag-ui';
import { createTextMessage, parseAgContent } from '@/types/ag-ui';

interface UseAgUIChatReturn {
  conversation: ConversationState;
  isStreaming: boolean;
  error: string | null;
  sendMessage: (text: string) => Promise<void>;
  clearConversation: () => void;
}

interface AgUIChatMessage {
  role: 'user' | 'assistant' | 'system';
  content: string;
}

interface AgUIChatRequest {
  messages: AgUIChatMessage[];
}

/**
 * Hook para comunicação com backend usando AG-UI Protocol via HTTP (padrão oficial).
 * Baseado no exemplo: https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/AGUIWebChat
 */
export function useAgUIChat(): UseAgUIChatReturn {
  const sessionId = useState(() => crypto.randomUUID())[0];
  
  const [conversation, setConversation] = useState<ConversationState>({
    messages: [],
    streamingMessage: null,
    sessionId,
  });

  const [error, setError] = useState<string | null>(null);

  const sendMessage = useCallback(
    async (text: string) => {
      setError(null);

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
        // Build request with conversation history
        const requestMessages: AgUIChatMessage[] = [
          ...conversation.messages.map((msg) => ({
            role: msg.role,
            content: msg.content.map((c) => c.type === 'text' ? c.text : '').join('\n'),
          })),
          {
            role: 'user',
            content: text,
          },
        ];

        const request: AgUIChatRequest = {
          messages: requestMessages,
        };

        // Start streaming message
        setConversation((prev) => ({
          ...prev,
          streamingMessage: {
            id: crypto.randomUUID(),
            role: 'assistant',
            partialContent: '',
            state: 'streaming',
          },
        }));

        // Make request to AG-UI endpoint with streaming support
        const response = await fetch('http://localhost:5011/ag-ui/brainstorm', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(request),
        });

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        if (!response.body) {
          throw new Error('Response body is null');
        }

        // Read streaming response
        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = '';
        let accumulatedText = '';
        let hasFinalized = false; // Flag to prevent duplicate finalization

        while (true) {
          const { done, value } = await reader.read();
          
          if (done) break;

          buffer += decoder.decode(value, { stream: true });
          
          // Parse SSE format (Server-Sent Events)
          const lines = buffer.split('\n');
          buffer = lines.pop() || ''; // Keep incomplete line in buffer

          for (const line of lines) {
            if (line.startsWith('data: ')) {
              const jsonStr = line.slice(6).trim(); // Remove "data: " prefix
              
              if (!jsonStr) continue;

              try {
                const event = JSON.parse(jsonStr);
                
                // Handle TEXT_MESSAGE_CONTENT events
                if (event.type === 'TEXT_MESSAGE_CONTENT' && event.delta) {
                  accumulatedText += event.delta;
                  
                  // Update streaming message with accumulated text
                  setConversation((prev) => {
                    const streamingMsg = prev.streamingMessage || {
                      id: event.messageId || crypto.randomUUID(),
                      role: 'assistant' as const,
                      partialContent: '',
                      state: 'streaming' as const,
                    };

                    return {
                      ...prev,
                      streamingMessage: {
                        ...streamingMsg,
                        partialContent: accumulatedText,
                      },
                    };
                  });
                }
                
                // Handle TEXT_MESSAGE_END / RUN_FINISHED
                if ((event.type === 'TEXT_MESSAGE_END' || event.type === 'RUN_FINISHED') && !hasFinalized) {
                  // Only finalize if we have accumulated text
                  if (accumulatedText.trim()) {
                    console.log('Finalizing message with text:', accumulatedText.substring(0, 100) + '...');
                    hasFinalized = true; // Set flag to prevent duplicate
                    
                    setConversation((prev) => {
                      const messageId = prev.streamingMessage?.id || crypto.randomUUID();
                      
                      const completeMessage: AgMessage = {
                        id: messageId,
                        role: 'assistant',
                        content: parseAgContent(accumulatedText),
                        metadata: {
                          timestamp: new Date(),
                          sessionId: prev.sessionId,
                        },
                      };

                      console.log('Adding complete message:', completeMessage);

                      return {
                        ...prev,
                        messages: [...prev.messages, completeMessage],
                        streamingMessage: null,
                      };
                    });
                    
                    // Don't reset yet - wait for next message to start
                  } else {
                    console.log('Ignoring end event - no accumulated text');
                    // Just clear streaming state
                    setConversation((prev) => ({
                      ...prev,
                      streamingMessage: null,
                    }));
                  }
                }
              } catch (e) {
                console.warn('Failed to parse SSE JSON:', jsonStr, e);
              }
            }
          }
        }
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Unknown error';
        console.error('Failed to send message:', errorMessage);
        setError(errorMessage);

        // Mark streaming message as error
        setConversation((prev) => ({
          ...prev,
          streamingMessage: prev.streamingMessage
            ? { ...prev.streamingMessage, state: 'error' }
            : null,
        }));
      }
    },
    [conversation.sessionId, conversation.messages]
  );

  const clearConversation = useCallback(() => {
    setConversation({
      messages: [],
      streamingMessage: null,
      sessionId: crypto.randomUUID(),
    });
    setError(null);
  }, []);

  const isStreaming = conversation.streamingMessage !== null;

  return {
    conversation,
    isStreaming,
    error,
    sendMessage,
    clearConversation,
  };
}
