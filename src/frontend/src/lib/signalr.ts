import * as signalR from '@microsoft/signalr';

/**
 * SignalR Connection Manager for LLM Hub
 * Handles real-time communication with the backend LLM streaming service
 */
class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private hubUrl = 'http://localhost:5011/llmhub';

  /**
   * Initializes and starts the SignalR connection
   */
  async start(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR already connected');
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    try {
      await this.connection.start();
      console.log('SignalR connected successfully');
    } catch (error) {
      console.error('SignalR connection error:', error);
      throw error;
    }
  }

  /**
   * Stops the SignalR connection
   */
  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      console.log('SignalR disconnected');
    }
  }

  /**
   * Registers a handler for token received events from LLM streaming
   * @param callback Function to call when a token is received
   */
  onTokenReceived(callback: (token: string) => void): void {
    if (!this.connection) {
      throw new Error('SignalR connection not initialized. Call start() first.');
    }

    this.connection.on('OnTokenReceived', callback);
    this.connection.on('OnBrainstormToken', callback); // Support brainstorm events
  }

  /**
   * Registers a handler for completion events from LLM streaming
   * @param callback Function to call when streaming is complete
   */
  onComplete(callback: () => void): void {
    if (!this.connection) {
      throw new Error('SignalR connection not initialized. Call start() first.');
    }

    this.connection.on('OnComplete', callback);
    this.connection.on('OnBrainstormComplete', callback); // Support brainstorm events
  }

  /**
   * Registers a handler for error events from LLM streaming
   * @param callback Function to call when an error occurs
   */
  onError(callback: (error: string) => void): void {
    if (!this.connection) {
      throw new Error('SignalR connection not initialized. Call start() first.');
    }

    this.connection.on('OnError', callback);
  }

  /**
   * Removes a specific event handler
   * @param eventName Name of the event to remove handler from
   */
  off(eventName: string): void {
    if (this.connection) {
      this.connection.off(eventName);
    }
  }

  /**
   * Invokes a server method
   * @param methodName Name of the server method to invoke
   * @param args Arguments to pass to the method
   */
  async invoke(methodName: string, ...args: unknown[]): Promise<unknown> {
    if (!this.connection) {
      throw new Error('SignalR connection not initialized. Call start() first.');
    }

    if (this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('SignalR connection is not in Connected state');
    }

    return await this.connection.invoke(methodName, ...args);
  }

  /**
   * Gets the current connection state
   */
  getState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }
}

// Export singleton instance
export const signalRService = new SignalRService();
