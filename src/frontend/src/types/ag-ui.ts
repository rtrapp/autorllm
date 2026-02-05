/**
 * AG-UI Protocol Types
 * Based on: https://github.com/ag-ui-protocol/ag-ui
 */

/**
 * Sanitize text to remove Microsoft Agent Framework metadata leakage
 * Removes patterns like: <|channel|>commentary to=assistant <|constrain|>1<|message|>
 */
function sanitizeAgentMetadata(text: string): string {
  // Remove Microsoft Agent Framework internal markers
  return text
    .replace(/<\|channel\|>[^<]*<\|constrain\|>[^<]*<\|message\|>/g, '')
    .replace(/<\|channel\|>[^<]*<\|message\|>/g, '')
    .replace(/<\|[^|]+\|>/g, '') // Remove any remaining <|tag|> patterns
    .trim();
}

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
  | 'question-list'
  | 'choice-list'
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

export interface Question {
  id: string;
  category: string;
  text: string;
  answered?: boolean;
  answer?: string;
}

export interface QuestionListComponent {
  type: 'question-list';
  questions: Question[];
  currentIndex?: number;
  showOneAtATime?: boolean;
}

export interface Choice {
  id: string;
  option: string;
  description: string;
  selected?: boolean;
}

export interface ChoiceListComponent {
  type: 'choice-list';
  choices: Choice[];
  allowMultiple?: boolean;
  contextText?: string;
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
  component: ButtonComponent | CardComponent | QuestionListComponent | ChoiceListComponent | OutlinePreviewComponent;
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
  console.log('=== parseAgContent called ===');
  console.log('Input text length:', text.length);
  console.log('First 500 chars:', text.substring(0, 500));
  
  // Sanitize metadata leakage FIRST
  const sanitizedText = sanitizeAgentMetadata(text);
  if (sanitizedText !== text) {
    console.log('⚠️ Removed agent metadata from text');
    console.log('Sanitized length:', sanitizedText.length);
  }
  
  try {
    // Try to parse as JSON first (for structured content)
    const parsed = JSON.parse(sanitizedText);
    console.log('Successfully parsed as JSON');
    if (Array.isArray(parsed)) {
      return parsed as Content[];
    }
    // If single object, wrap in array
    return [parsed as Content];
  } catch {
    console.log('Not JSON, checking for structured content...');
    
    // Check if text contains structured questions (use sanitized text)
    const questions = parseQuestionsFromText(sanitizedText);
    console.log('Questions found:', questions.length);
    
    // Check if text contains choices (use sanitized text)
    const choices = parseChoicesFromText(sanitizedText);
    console.log('Choices found:', choices.length);
    
    if (questions.length > 0) {
      console.log('✅ DETECTED STRUCTURED QUESTIONS!');
      questions.forEach((q, i) => {
        console.log(`  Question ${i + 1}: (${q.category}) ${q.text.substring(0, 50)}...`);
      });
      
      return buildQuestionContent(sanitizedText, questions);
    }
    
    if (choices.length > 0) {
      console.log('✅ DETECTED STRUCTURED CHOICES!');
      choices.forEach((c, i) => {
        console.log(`  Choice ${i + 1}: (${c.option}) ${c.description.substring(0, 50)}...`);
      });
      
      return buildChoiceContent(sanitizedText, choices);
    }
    
    console.log('No structured content found, treating as plain text');
    // If not JSON and no structured content, treat as plain text (sanitized)
    return [{ type: 'text', text: sanitizedText }];
  }
}

/**
 * Build content array with intro text + question list component
 */
function buildQuestionContent(text: string, questions: Question[]): Content[] {
  const lines = text.split('\n').filter(line => line.trim());
  const firstQuestionIndex = lines.findIndex(line => /^\([^)]+\)\s+[^[]/.test(line.trim()));
  
  const content: Content[] = [];
  
  // Add intro text if exists
  if (firstQuestionIndex > 0) {
    const introText = lines.slice(0, firstQuestionIndex).join('\n\n');
    console.log('Adding intro text:', introText.substring(0, 100));
    content.push({ type: 'text', text: introText });
  }
  
  // Add question list component
  console.log('Creating QuestionListComponent with', questions.length, 'questions');
  content.push({
    type: 'component',
    component: {
      type: 'question-list',
      questions,
      currentIndex: 0,
      showOneAtATime: true,
    } as QuestionListComponent,
  });
  
  return content;
}

/**
 * Build content array with context text + choice list component
 */
function buildChoiceContent(text: string, choices: Choice[]): Content[] {
  const lines = text.split('\n').filter(line => line.trim());
  const firstChoiceIndex = lines.findIndex(line => /^\[ESCOLHA\]/.test(line.trim()));
  
  const content: Content[] = [];
  
  // Add context text if exists
  let contextText = '';
  if (firstChoiceIndex > 0) {
    contextText = lines.slice(0, firstChoiceIndex).join('\n\n');
    console.log('Adding context text:', contextText.substring(0, 100));
  }
  
  // Add choice list component
  console.log('Creating ChoiceListComponent with', choices.length, 'choices');
  content.push({
    type: 'component',
    component: {
      type: 'choice-list',
      choices,
      allowMultiple: false,
      contextText,
    } as ChoiceListComponent,
  });
  
  return content;
}

/**
 * Parse questions from text in format: (Category) Question text
 * Example: (Gênero e Tom) Qual é o gênero da sua história?
 */
export function parseQuestionsFromText(text: string): Question[] {
  console.log('=== parseQuestionsFromText called ===');
  const lines = text.split('\n').filter(line => line.trim());
  console.log('Total lines to check:', lines.length);
  
  const questions: Question[] = [];
  
  // Regex to match: (Category) Question text (but NOT [ESCOLHA])
  const questionRegex = /^\(([^)]+)\)\s+([^[].+)$/;
  
  for (const line of lines) {
    const trimmedLine = line.trim();
    if (trimmedLine.startsWith('[ESCOLHA]')) continue; // Skip choices
    
    const match = trimmedLine.match(questionRegex);
    
    if (match) {
      const [, category, questionText] = match;
      console.log(`✅ MATCHED QUESTION: (${category}) ${questionText.substring(0, 50)}`);
      questions.push({
        id: crypto.randomUUID(),
        category: category.trim(),
        text: questionText.trim(),
      });
    } else {
      // Log lines that don't match
      if (trimmedLine.length > 0 && trimmedLine.includes('(') && !trimmedLine.includes('[ESCOLHA]')) {
        console.log(`❌ NO MATCH (contains parenthesis): ${trimmedLine.substring(0, 80)}`);
      }
    }
  }
  
  console.log('Total questions parsed:', questions.length);
  return questions;
}

/**
 * Parse choices from text in format: [ESCOLHA] (Option) Description
 * Example: [ESCOLHA] (Thriller de Conspiração) Uma organização secreta...
 */
export function parseChoicesFromText(text: string): Choice[] {
  console.log('=== parseChoicesFromText called ===');
  const lines = text.split('\n').filter(line => line.trim());
  console.log('Total lines to check:', lines.length);
  
  const choices: Choice[] = [];
  
  // Regex to match: [ESCOLHA] (Option) Description
  const choiceRegex = /^\[ESCOLHA\]\s*\(([^)]+)\)\s+(.+)$/;
  
  for (const line of lines) {
    const trimmedLine = line.trim();
    const match = trimmedLine.match(choiceRegex);
    
    if (match) {
      const [, option, description] = match;
      console.log(`✅ MATCHED CHOICE: (${option}) ${description.substring(0, 50)}`);
      choices.push({
        id: crypto.randomUUID(),
        option: option.trim(),
        description: description.trim(),
      });
    } else {
      // Log lines that contain [ESCOLHA] but don't match
      if (trimmedLine.includes('[ESCOLHA]')) {
        console.log(`❌ NO MATCH (contains [ESCOLHA]): ${trimmedLine.substring(0, 80)}`);
      }
    }
  }
  
  console.log('Total choices parsed:', choices.length);
  return choices;
}
