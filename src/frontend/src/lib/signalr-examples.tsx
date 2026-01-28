/**
 * SignalR Integration - Usage Examples
 * 
 * This file demonstrates how to use the SignalR service and hook
 * for real-time LLM streaming communication.
 */

// Example 1: Using the SignalR service directly
import { signalRService } from '@/lib/signalr';

async function directServiceExample() {
  try {
    // Start connection
    await signalRService.start();

    // Register event handlers
    signalRService.onTokenReceived((token: string) => {
      console.log('Token received:', token);
    });

    signalRService.onComplete(() => {
      console.log('Streaming complete');
    });

    signalRService.onError((error: string) => {
      console.error('Streaming error:', error);
    });

    // Invoke server method (if needed)
    await signalRService.invoke('SomeMethod', 'arg1', 'arg2');

    // Stop connection when done
    await signalRService.stop();
  } catch (error) {
    console.error('SignalR error:', error);
  }
}

// Example 2: Using the React hook in a component
import { useSignalR } from '@/hooks/use-signalr';
import { useState, useEffect } from 'react';

function LLMStreamingComponent() {
  const [streamingText, setStreamingText] = useState('');
  const [isStreaming, setIsStreaming] = useState(false);

  const { isConnected, connect, disconnect, error } = useSignalR({
    onTokenReceived: (token: string) => {
      setStreamingText((prev) => prev + token);
    },
    onComplete: () => {
      setIsStreaming(false);
      console.log('Streaming completed');
    },
    onError: (error: string) => {
      console.error('Streaming error:', error);
      setIsStreaming(false);
    },
  });

  // Connect when component mounts
  useEffect(() => {
    connect();
    
    return () => {
      disconnect();
    };
  }, [connect, disconnect]);

  const handleStartStreaming = async () => {
    if (!isConnected) {
      console.error('Not connected to SignalR');
      return;
    }

    setStreamingText('');
    setIsStreaming(true);
    
    // Trigger streaming from backend
    // This would be implemented based on backend API
  };

  return (
    <div>
      <div>Status: {isConnected ? 'Connected' : 'Disconnected'}</div>
      {error && <div>Error: {error}</div>}
      
      <button onClick={handleStartStreaming} disabled={!isConnected || isStreaming}>
        Start Streaming
      </button>

      <div>
        <h3>Streamed Text:</h3>
        <pre>{streamingText}</pre>
      </div>
    </div>
  );
}

export { directServiceExample, LLMStreamingComponent };
