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

/**
 * BrainstormContext - Armazena respostas acumuladas do brainstorm
 * Campos baseados nas entidades de domínio reais do backend
 */
export interface BrainstormContext {
  // Dados do projeto
  bookIdea: string;                    // Ideia inicial do usuário
  title?: string;                       // Project.Title (max 200)
  author?: string;                      // Project.Author (max 100)
  synopsis?: string;                    // Project.Synopsis (max 5000)
  genre?: string;                       // Project.Genre (max 50)
  targetWordCount?: number;             // Project.TargetWordCount

  // Listas de entidades (dados brutos das respostas)
  characters?: Array<{
    name: string;                       // Character.Name (max 100)
    description?: string;               // Character.Description (max 1000)
    role?: 'Protagonist' | 'Antagonist' | 'Supporting' | 'Minor';  // CharacterRole
    backstory?: string;                 // Character.Backstory (max 5000)
    appearance?: string;                // Character.Appearance (max 2000)
    personality?: string;               // Character.Personality (max 2000)
  }>;

  locations?: Array<{
    name: string;                       // Location.Name (max 100)
    description?: string;               // Location.Description (max 1000)
    geography?: string;                 // Location.Geography (max 2000)
    culture?: string;                   // Location.Culture (max 2000)
    significance?: string;              // Location.Significance (max 1000)
  }>;

  plots?: Array<{
    title: string;                      // Plot.Title (max 200)
    description?: string;               // Plot.Description (max 2000)
    type?: 'Main' | 'Subplot' | 'Character Arc' | 'Romance' | 'Mystery';  // PlotType
    resolution?: string;                // Plot.Resolution (max 2000)
  }>;

  chapters?: Array<{
    title: string;                      // Chapter.Title (max 200)
    summary?: string;                   // Chapter.Summary (max 2000)
    order: number;                      // Chapter.Order
  }>;

  // Metadados
  tone?: string;
  themes?: string[];
  targetAudience?: string;
}

/**
 * OutlineData - Estrutura final do outline gerado pela LLM
 * Mapeia EXATAMENTE para as entidades de domínio
 */
export interface OutlineData {
  // Project fields
  title: string;                        // Project.Title
  author: string;                       // Project.Author
  synopsis: string;                     // Project.Synopsis (200-500 palavras recomendado)
  genre?: string;                       // Project.Genre
  targetWordCount?: number;             // Project.TargetWordCount (default: 50000)

  // Child entities
  characters: Array<{
    name: string;                       // Character.Name (obrigatório)
    description: string;                // Character.Description (obrigatório)
    role: 'Protagonist' | 'Antagonist' | 'Supporting' | 'Minor';  // CharacterRole (obrigatório)
    backstory?: string;                 // Character.Backstory (opcional)
    appearance?: string;                // Character.Appearance (opcional)
    personality?: string;               // Character.Personality (opcional)
  }>;

  locations?: Array<{
    name: string;                       // Location.Name (obrigatório)
    description: string;                // Location.Description (obrigatório)
    geography?: string;                 // Location.Geography (opcional)
    culture?: string;                   // Location.Culture (opcional)
    significance?: string;              // Location.Significance (opcional)
  }>;

  plots: Array<{
    title: string;                      // Plot.Title (obrigatório)
    description: string;                // Plot.Description (obrigatório)
    type: 'Main' | 'Subplot' | 'Character Arc' | 'Romance' | 'Mystery';  // PlotType (obrigatório)
    resolution?: string;                // Plot.Resolution (opcional)
  }>;

  chapters: Array<{
    title: string;                      // Chapter.Title (obrigatório)
    summary: string;                    // Chapter.Summary (obrigatório)
    order: number;                      // Chapter.Order (1, 2, 3...)
  }>;
}

export interface OutlinePreviewComponent {
  type: 'outline-preview';
  outline: OutlineData;
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
    
    // Check if it's an outline JSON (has required outline fields)
    if (isOutlineData(parsed)) {
      console.log('✅ DETECTED OUTLINE JSON!');
      return [{
        type: 'component',
        component: {
          type: 'outline-preview',
          outline: parsed as OutlineData
        } as OutlinePreviewComponent
      }];
    }
    
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
    
    // Check if text contains actions (use sanitized text)
    const actions = parseActionsFromText(sanitizedText);
    console.log('Actions found:', actions.length);
    
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
    
    if (actions.length > 0) {
      console.log('✅ DETECTED ACTIONS!');
      actions.forEach((a, i) => {
        console.log(`  Action ${i + 1}: ${a.type} - ${a.label?.substring(0, 50)}...`);
      });
      
      return buildActionContent(sanitizedText, actions);
    }
    
    console.log('No structured content found, treating as plain text');
    // If not JSON and no structured content, treat as plain text (sanitized)
    return [{ type: 'text', text: sanitizedText }];
  }
}

/**
 * Type guard to check if parsed JSON is an OutlineData
 */
function isOutlineData(obj: any): obj is OutlineData {
  return (
    obj &&
    typeof obj === 'object' &&
    typeof obj.title === 'string' &&
    typeof obj.author === 'string' &&
    typeof obj.synopsis === 'string' &&
    Array.isArray(obj.characters) &&
    Array.isArray(obj.locations) &&
    Array.isArray(obj.plots) &&
    Array.isArray(obj.chapters)
  );
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

/**
 * Parse actions from text in format: [ACTION] (action_type) Description
 * Example: [ACTION] (generate_outline) Você tem informação suficiente! Posso gerar o outline.
 */
export function parseActionsFromText(text: string): Array<{ type: string; label: string }> {
  console.log('=== parseActionsFromText called ===');
  const lines = text.split('\n').filter(line => line.trim());
  console.log('Total lines to check:', lines.length);
  
  const actions: Array<{ type: string; label: string }> = [];
  
  // Regex to match: [ACTION] (action_type) Description
  const actionRegex = /^\[ACTION\]\s*\(([^)]+)\)\s+(.+)$/;
  
  for (const line of lines) {
    const trimmedLine = line.trim();
    const match = trimmedLine.match(actionRegex);
    
    if (match) {
      const [, actionType, description] = match;
      console.log(`✅ MATCHED ACTION: (${actionType}) ${description.substring(0, 50)}`);
      actions.push({
        type: actionType.trim(),
        label: description.trim(),
      });
    } else {
      // Log lines that contain [ACTION] but don't match
      if (trimmedLine.includes('[ACTION]')) {
        console.log(`❌ NO MATCH (contains [ACTION]): ${trimmedLine.substring(0, 80)}`);
      }
    }
  }
  
  console.log('Total actions parsed:', actions.length);
  return actions;
}

/**
 * Build content array with context text + action buttons
 */
function buildActionContent(text: string, actions: Array<{ type: string; label: string }>): Content[] {
  const lines = text.split('\n').filter(line => line.trim());
  const firstActionIndex = lines.findIndex(line => /^\[ACTION\]/.test(line.trim()));
  
  const content: Content[] = [];
  
  // Add context text if exists
  if (firstActionIndex > 0) {
    const contextText = lines.slice(0, firstActionIndex).join('\n\n');
    console.log('Adding context text before actions:', contextText.substring(0, 100));
    content.push({ type: 'text', text: contextText });
  }
  
  // Add button components for each action
  console.log('Creating button components for', actions.length, 'actions');
  actions.forEach(action => {
    content.push({
      type: 'component',
      component: {
        type: 'button',
        label: action.label,
        action: { type: action.type },
        variant: action.type === 'generate_outline' ? 'primary' : 'secondary',
      } as ButtonComponent,
    });
  });
  
  return content;
}

/**
 * Create initial brainstorm context from book idea
 */
export function createBrainstormContext(bookIdea: string): BrainstormContext {
  return {
    bookIdea,
    characters: [],
    locations: [],
    plots: [],
    chapters: [],
  };
}

/**
 * Validate outline data against domain constraints
 */
export function validateOutline(outline: OutlineData): string[] {
  const errors: string[] = [];

  // Project validations
  if (!outline.title || outline.title.trim().length === 0) {
    errors.push('Title is required');
  }
  if (outline.title && outline.title.length > 200) {
    errors.push('Title cannot exceed 200 characters');
  }

  if (!outline.author || outline.author.trim().length === 0) {
    errors.push('Author is required');
  }
  if (outline.author && outline.author.length > 100) {
    errors.push('Author cannot exceed 100 characters');
  }

  if (!outline.synopsis || outline.synopsis.trim().length === 0) {
    errors.push('Synopsis is required');
  }
  if (outline.synopsis && outline.synopsis.length > 5000) {
    errors.push('Synopsis cannot exceed 5000 characters');
  }

  if (outline.genre && outline.genre.length > 50) {
    errors.push('Genre cannot exceed 50 characters');
  }

  // Characters validations (minimum 1 required)
  if (!outline.characters || outline.characters.length === 0) {
    errors.push('At least 1 character is required');
  }

  outline.characters?.forEach((char, index) => {
    if (!char.name || char.name.trim().length === 0) {
      errors.push(`Character ${index + 1}: Name is required`);
    }
    if (char.name && char.name.length > 100) {
      errors.push(`Character ${index + 1}: Name cannot exceed 100 characters`);
    }
    if (!char.description || char.description.trim().length === 0) {
      errors.push(`Character ${index + 1}: Description is required`);
    }
    if (char.description && char.description.length > 1000) {
      errors.push(`Character ${index + 1}: Description cannot exceed 1000 characters`);
    }
  });

  // Plots validations (minimum 1 Main plot required)
  if (!outline.plots || outline.plots.length === 0) {
    errors.push('At least 1 plot is required');
  }

  const hasMainPlot = outline.plots?.some(p => p.type === 'Main');
  if (!hasMainPlot) {
    errors.push('At least 1 Main plot is required');
  }

  outline.plots?.forEach((plot, index) => {
    if (!plot.title || plot.title.trim().length === 0) {
      errors.push(`Plot ${index + 1}: Title is required`);
    }
    if (plot.title && plot.title.length > 200) {
      errors.push(`Plot ${index + 1}: Title cannot exceed 200 characters`);
    }
    if (!plot.description || plot.description.trim().length === 0) {
      errors.push(`Plot ${index + 1}: Description is required`);
    }
    if (plot.description && plot.description.length > 2000) {
      errors.push(`Plot ${index + 1}: Description cannot exceed 2000 characters`);
    }
  });

  // Chapters validations (minimum 3 required)
  if (!outline.chapters || outline.chapters.length < 3) {
    errors.push('At least 3 chapters are required');
  }
  if (outline.chapters && outline.chapters.length > 12) {
    errors.push('Maximum 12 chapters allowed for initial outline');
  }

  outline.chapters?.forEach((chapter, index) => {
    if (!chapter.title || chapter.title.trim().length === 0) {
      errors.push(`Chapter ${index + 1}: Title is required`);
    }
    if (chapter.title && chapter.title.length > 200) {
      errors.push(`Chapter ${index + 1}: Title cannot exceed 200 characters`);
    }
    if (!chapter.summary || chapter.summary.trim().length === 0) {
      errors.push(`Chapter ${index + 1}: Summary is required`);
    }
    if (chapter.summary && chapter.summary.length > 2000) {
      errors.push(`Chapter ${index + 1}: Summary cannot exceed 2000 characters`);
    }
  });

  return errors;
}
