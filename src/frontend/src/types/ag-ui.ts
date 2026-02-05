/**
 * AG-UI Protocol Types
 * Based on: https://github.com/ag-ui-protocol/ag-ui
 */

export type MessageRole = 'user' | 'assistant' | 'system';

export type ContentType = 'text' | 'component' | 'action' | 'error';

// Base Content Interface
export interface BaseContent {
  type: ContentType;
}

// Text Content
export interface TextContent extends BaseContent {
  type: 'text';
  text: string;
}

// Component Types
export type ComponentType = 
  | 'button' 
  | 'card' 
  | 'form' 
  | 'input'
  | 'select'
  | 'list'
  | 'outline-preview';

export interface ButtonComponent {
  type: 'button';
  label: string;
  action: Action;
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost';
}

export interface CardComponent {
  type: 'card';
  title: string;
  description?: string;
  content?: Content[];
  actions?: ButtonComponent[];
}

export interface OutlinePreviewComponent {
  type: 'outline-preview';
  title: string;
  synopsis: string;
  chapters: Array<{
    number: number;
    title: string;
    summary: string;
  }>;
  characters: Array<{
    name: string;
    role: string;
    description: string;
  }>;
  plotType: string;
}

export interface ComponentContent extends BaseContent {
  type: 'component';
  component: ButtonComponent | CardComponent | OutlinePreviewComponent;
}

// Action Types
export type ActionType = 
  | 'navigate'
  | 'submit'
  | 'api_call'
  | 'save_project'
  | 'update_outline';

export interface Action {
  type: ActionType;
  payload?: Record<string, unknown>;
  label?: string;
}

export interface ActionContent extends BaseContent {
  type: 'action';
  action: Action;
}

// Error Content
export interface ErrorContent extends BaseContent {
  type: 'error';
  error: string;
  code?: string;
  recoverable?: boolean;
}

// Union type for all content
export type Content = TextContent | ComponentContent | ActionContent | ErrorContent;

// Message Structure
export interface AgMessage {
  id: string;
  role: MessageRole;
  content: Content[];
  metadata?: {
    timestamp: Date;
    model?: string;
    sessionId?: string;
    tokens?: number;
    [key: string]: unknown;
  };
}

// Streaming States
export type StreamingState = 'idle' | 'streaming' | 'complete' | 'error';

export interface StreamingMessage {
  id: string;
  role: MessageRole;
  partialContent: string;
  state: StreamingState;
}

// Conversation State
export interface ConversationState {
  messages: AgMessage[];
  streamingMessage: StreamingMessage | null;
  sessionId: string;
  metadata?: Record<string, unknown>;
}

// Helper functions
export function createTextMessage(
  role: MessageRole,
  text: string,
  metadata?: AgMessage['metadata']
): AgMessage {
  return {
    id: crypto.randomUUID(),
    role,
    content: [{ type: 'text', text }],
    metadata: {
      timestamp: new Date(),
      ...metadata,
    },
  };
}

export function createComponentMessage(
  role: MessageRole,
  component: ComponentContent['component'],
  metadata?: AgMessage['metadata']
): AgMessage {
  return {
    id: crypto.randomUUID(),
    role,
    content: [{ type: 'component', component }],
    metadata: {
      timestamp: new Date(),
      ...metadata,
    },
  };
}

export function createErrorMessage(
  error: string,
  code?: string,
  recoverable = true
): AgMessage {
  return {
    id: crypto.randomUUID(),
    role: 'system',
    content: [{ type: 'error', error, code, recoverable }],
    metadata: {
      timestamp: new Date(),
    },
  };
}

// Parse content from string (for backend compatibility)
export function parseAgContent(text: string): Content[] {
  try {
    // Try to parse as JSON first (for structured content)
    const parsed = JSON.parse(text);
    if (Array.isArray(parsed)) {
      return parsed as Content[];
    }
    // If single object, wrap in array
    return [parsed as Content];
  } catch {
    // If not JSON, treat as plain text
    return [{ type: 'text', text }];
  }
}
