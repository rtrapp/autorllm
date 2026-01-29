import { useState, useCallback } from 'react';
import { useSignalR } from './use-signalr';

export interface BrainstormMessage {
  id: string;
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
}

interface UseBrainstormReturn {
  messages: BrainstormMessage[];
  isStreaming: boolean;
  streamingContent: string;
  isConnected: boolean;
  error: string | null;
  startBrainstorm: (bookIdea: string) => Promise<void>;
  sendMessage: (message: string) => Promise<void>;
  clearMessages: () => void;
}

/**
 * Hook para gerenciar sessão de brainstorm com LLM via SignalR
 */
export function useBrainstorm(): UseBrainstormReturn {
  const [messages, setMessages] = useState<BrainstormMessage[]>([]);
  const [isStreaming, setIsStreaming] = useState(false);
  const [streamingContent, setStreamingContent] = useState('');
  const [currentMessageId, setCurrentMessageId] = useState<string | null>(null);

  const handleTokenReceived = useCallback((token: string) => {
    setStreamingContent((prev) => prev + token);
  }, []);

  const handleComplete = useCallback(() => {
    if (currentMessageId && streamingContent) {
      setMessages((prev) => [
        ...prev,
        {
          id: currentMessageId,
          role: 'assistant',
          content: streamingContent,
          timestamp: new Date(),
        },
      ]);
    }

    setIsStreaming(false);
    setStreamingContent('');
    setCurrentMessageId(null);
  }, [currentMessageId, streamingContent]);

  const handleError = useCallback((error: string) => {
    console.error('Brainstorm error:', error);
    setIsStreaming(false);
    setStreamingContent('');
    setCurrentMessageId(null);
  }, []);

  const {
    isConnected,
    error,
    invoke,
  } = useSignalR({
    onTokenReceived: handleTokenReceived,
    onComplete: handleComplete,
    onError: handleError,
  });

  const startBrainstorm = useCallback(
    async (bookIdea: string) => {
      if (!isConnected) {
        console.log('Waiting for connection...');
        return;
      }

      // Add user message
      const userMessageId = crypto.randomUUID();
      setMessages((prev) => [
        ...prev,
        {
          id: userMessageId,
          role: 'user',
          content: bookIdea,
          timestamp: new Date(),
        },
      ]);

      // Start streaming assistant response
      setIsStreaming(true);
      const assistantMessageId = crypto.randomUUID();
      setCurrentMessageId(assistantMessageId);
      setStreamingContent('');

      try {
        await invoke('StartBrainstorm', bookIdea);
      } catch (error) {
        console.error('Failed to start brainstorm:', error);
        setIsStreaming(false);
        setCurrentMessageId(null);
      }
    },
    [isConnected, invoke]
  );

  const sendMessage = useCallback(
    async (message: string) => {
      if (!isConnected) {
        console.log('Waiting for connection...');
        return;
      }

      // Add user message
      const userMessageId = crypto.randomUUID();
      setMessages((prev) => [
        ...prev,
        {
          id: userMessageId,
          role: 'user',
          content: message,
          timestamp: new Date(),
        },
      ]);

      // Start streaming assistant response
      setIsStreaming(true);
      const assistantMessageId = crypto.randomUUID();
      setCurrentMessageId(assistantMessageId);
      setStreamingContent('');

      try {
        // For now, use sessionId as empty string (could be enhanced later)
        await invoke('ContinueBrainstorm', '', message);
      } catch (error) {
        console.error('Failed to send message:', error);
        setIsStreaming(false);
        setCurrentMessageId(null);
      }
    },
    [isConnected, invoke]
  );

  const clearMessages = useCallback(() => {
    setMessages([]);
    setStreamingContent('');
    setCurrentMessageId(null);
    setIsStreaming(false);
  }, []);

  return {
    messages,
    isStreaming,
    streamingContent,
    isConnected,
    error,
    startBrainstorm,
    sendMessage,
    clearMessages,
  };
}
