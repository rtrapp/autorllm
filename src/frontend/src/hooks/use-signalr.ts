import { useEffect, useCallback, useState } from 'react';
import { signalRService } from '@/lib/signalr';
import * as signalR from '@microsoft/signalr';

interface UseSignalROptions {
  onTokenReceived?: (token: string) => void;
  onComplete?: () => void;
  onError?: (error: string) => void;
}

interface UseSignalRReturn {
  connectionState: signalR.HubConnectionState | null;
  isConnected: boolean;
  isConnecting: boolean;
  error: string | null;
  connect: () => Promise<void>;
  disconnect: () => Promise<void>;
  invoke: (methodName: string, ...args: unknown[]) => Promise<unknown>;
}

/**
 * Custom React hook for SignalR connection management
 * @param options Configuration options for the hook
 * @returns SignalR connection utilities and state
 */
export function useSignalR(options: UseSignalROptions = {}): UseSignalRReturn {
  const { 
    onTokenReceived,
    onComplete,
    onError 
  } = options;

  const [connectionState, setConnectionState] = useState<signalR.HubConnectionState | null>(null);
  const [error, setError] = useState<string | null>(null);

  const updateConnectionState = useCallback(() => {
    setConnectionState(signalRService.getState());
  }, []);

  const connect = useCallback(async () => {
    try {
      setError(null);
      await signalRService.start();
      updateConnectionState();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to connect to SignalR';
      setError(errorMessage);
      console.error('SignalR connection failed:', err);
    }
  }, [updateConnectionState]);

  const disconnect = useCallback(async () => {
    try {
      await signalRService.stop();
      updateConnectionState();
    } catch (err) {
      console.error('SignalR disconnect error:', err);
    }
  }, [updateConnectionState]);

  const invoke = useCallback(async (methodName: string, ...args: unknown[]) => {
    try {
      return await signalRService.invoke(methodName, ...args);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to invoke SignalR method';
      setError(errorMessage);
      throw err;
    }
  }, []);

  // Setup event handlers
  useEffect(() => {
    const setupHandlers = async () => {
      // Ensure connection is established before registering handlers
      if (signalRService.getState() !== signalR.HubConnectionState.Connected) {
        try {
          await signalRService.start();
          updateConnectionState();
        } catch (error) {
          console.error('Failed to establish SignalR connection:', error);
          return;
        }
      }

      if (onTokenReceived) {
        signalRService.onTokenReceived(onTokenReceived);
      }

      if (onComplete) {
        signalRService.onComplete(onComplete);
      }

      if (onError) {
        signalRService.onError(onError);
      }
    };

    setupHandlers();

    // Cleanup handlers on unmount
    return () => {
      signalRService.off('OnTokenReceived');
      signalRService.off('OnBrainstormToken');
      signalRService.off('OnComplete');
      signalRService.off('OnBrainstormComplete');
      signalRService.off('OnError');
    };
  }, [onTokenReceived, onComplete, onError, updateConnectionState]);

  const isConnected = connectionState === signalR.HubConnectionState.Connected;
  const isConnecting = connectionState === signalR.HubConnectionState.Connecting || 
                       connectionState === signalR.HubConnectionState.Reconnecting;

  return {
    connectionState,
    isConnected,
    isConnecting,
    error,
    connect,
    disconnect,
    invoke,
  };
}
