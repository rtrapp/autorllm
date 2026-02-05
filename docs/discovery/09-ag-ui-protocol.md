# Implementação do Protocolo AG-UI

**Data de Implementação:** 2026-01-29  
**Versão:** 1.0  
**Baseado em:** https://github.com/ag-ui-protocol/ag-ui

---

## Visão Geral

Este projeto implementa o **AG-UI Protocol** (Agentic UI Protocol) para comunicação estruturada entre a interface do usuário e a LLM. O protocolo define um formato padronizado para mensagens, componentes e ações, permitindo que a LLM controle aspectos da interface de forma declarativa.

---

## Estrutura de Mensagens (AgMessage)

Todas as mensagens seguem o formato:

```typescript
interface AgMessage {
  id: string;                    // UUID único
  role: 'user' | 'assistant' | 'system';
  content: Content[];            // Array de conteúdos
  metadata?: {
    timestamp: Date;
    model?: string;
    sessionId?: string;
    tokens?: number;
  };
}
```

---

## Tipos de Conteúdo

### 1. Text Content (Texto Simples)

```typescript
{
  type: 'text',
  text: 'Olá! Vou te ajudar a estruturar seu livro.'
}
```

### 2. Component Content (Componentes Interativos)

#### Button Component
```typescript
{
  type: 'component',
  component: {
    type: 'button',
    label: 'Salvar Projeto',
    action: { type: 'save_project', payload: { data } },
    variant: 'primary' | 'secondary' | 'outline' | 'ghost'
  }
}
```

#### Card Component
```typescript
{
  type: 'component',
  component: {
    type: 'card',
    title: 'Sugestão de Personagem',
    description: 'Protagonista da história',
    content: [...],
    actions: [ButtonComponent, ...]
  }
}
```

#### Outline Preview Component
```typescript
{
  type: 'component',
  component: {
    type: 'outline-preview',
    title: 'O Mago das Dimensões',
    synopsis: '...',
    chapters: [
      { number: 1, title: 'O Despertar', summary: '...' }
    ],
    characters: [
      { name: 'Kael', role: 'Protagonist', description: '...' }
    ],
    plotType: 'Fantasy'
  }
}
```

### 3. Error Content

```typescript
{
  type: 'error',
  error: 'Conexão com LLM falhou',
  code: 'LLM_UNAVAILABLE',
  recoverable: true
}
```

---

## Ações Disponíveis

| Action Type | Descrição | Payload |
|-------------|-----------|---------|
| `save_project` | Salva projeto com outline gerado | `{ outline: OutlinePreviewComponent }` |
| `update_outline` | Abre editor de outline | `{ outline: OutlinePreviewComponent }` |
| `navigate` | Navega para rota específica | `{ path: string }` |
| `submit` | Submete formulário | `{ formData: Record<string, unknown> }` |
| `api_call` | Chama endpoint da API | `{ endpoint: string, method: string, data?: unknown }` |

---

## Implementação Frontend

### Hook Principal: `useAgBrainstorm`

```typescript
const {
  conversation,      // Estado completo da conversa
  isStreaming,       // Se está recebendo tokens
  isConnected,       // Status conexão SignalR
  error,             // Erro atual (se houver)
  sendMessage,       // Envia mensagem do usuário
  clearConversation  // Limpa histórico
} = useAgBrainstorm();
```

### Componentes

1. **AgMessageRenderer** - Renderiza mensagens AG-UI
   - Suporta todos os tipos de conteúdo
   - Dispara callbacks de ações
   - Responsivo e acessível

2. **StreamingMessageRenderer** - Exibe streaming em tempo real
   - Animação de pulso durante streaming
   - Auto-scroll para última mensagem

### Fluxo de Dados

```
[User Input] 
    → sendMessage()
    → SignalR Hub (StartBrainstorm/ContinueBrainstorm)
    → LLM gera resposta
    → Streaming tokens via SignalR
    → handleTokenReceived() acumula tokens
    → handleComplete() converte para AgMessage
    → AgMessageRenderer renderiza componentes
```

---

## Implementação Backend (Futuro)

Para que a LLM envie mensagens AG-UI estruturadas, o backend deve:

### 1. Modificar Prompt da LLM

```csharp
var prompt = $"""
    Você é um assistente de brainstorming para escritores.
    
    FORMATO DE RESPOSTA: Você DEVE responder usando JSON no formato AG-UI Protocol.
    
    Exemplos:
    
    1. Texto simples:
    {{
      "type": "text",
      "text": "Entendi sua ideia! É sobre..."
    }}
    
    2. Card com perguntas:
    {{
      "type": "component",
      "component": {{
        "type": "card",
        "title": "Vamos explorar mais",
        "content": [
          {{ "type": "text", "text": "1. Qual é o gênero?" }}
        ]
      }}
    }}
    
    3. Outline completo:
    {{
      "type": "component",
      "component": {{
        "type": "outline-preview",
        "title": "Título do Livro",
        "synopsis": "...",
        "chapters": [...],
        "characters": [...],
        "plotType": "Fantasy"
      }}
    }}
    
    Ideia do usuário: {bookIdea}
    """;
```

### 2. Parser no Backend

```csharp
private AgUiContent ParseLLMResponse(string llmOutput)
{
    try 
    {
        return JsonSerializer.Deserialize<AgUiContent>(llmOutput);
    }
    catch 
    {
        // Fallback: trata como texto simples
        return new AgUiContent 
        { 
            Type = "text", 
            Text = llmOutput 
        };
    }
}
```

### 3. Enviar via SignalR

```csharp
await foreach (var token in _agentService.StreamCompletionAsync(prompt, ct))
{
    await Clients.Caller.SendAsync("OnBrainstormToken", token, ct);
}
```

---

## Exemplo de Uso Completo

### Usuário envia mensagem:
```typescript
await sendMessage("Quero escrever sobre um mago que viaja entre dimensões");
```

### LLM responde com AG-UI:
```json
{
  "type": "component",
  "component": {
    "type": "card",
    "title": "Ótima ideia! 🎭",
    "description": "Vamos estruturar sua história de fantasia multidimensional",
    "content": [
      {
        "type": "text",
        "text": "Para criar um outline completo, preciso saber:\n\n1. **Gênero**: É mais fantasia, ficção científica ou um mix?\n2. **Tom**: Sombrio, aventuresco, humorístico?\n3. **Protagonista**: Quem é esse mago? Qual sua idade e motivação?"
      }
    ],
    "actions": [
      {
        "type": "button",
        "label": "Já sei essas respostas",
        "action": { "type": "submit", "payload": { "skip_to": "outline" } }
      }
    ]
  }
}
```

### Frontend renderiza automaticamente:
- ✅ Card com título e descrição
- ✅ Texto formatado com markdown
- ✅ Botão clicável com ação configurada

---

## Benefícios do AG-UI

✅ **Estruturação Clara** - Mensagens bem definidas e tipadas  
✅ **Componentes Reutilizáveis** - LLM pode usar UI components existentes  
✅ **Ações Declarativas** - LLM controla fluxo sem código custom  
✅ **Extensibilidade** - Fácil adicionar novos componentes/ações  
✅ **Type Safety** - TypeScript garante contratos  
✅ **Streaming Otimizado** - Suporta partial updates  
✅ **Testabilidade** - Mensagens estruturadas facilitam testes  

---

## Próximos Passos

### Fase 2 (US002-US005):
1. ✅ Implementar backend para emitir AG-UI JSON
2. ✅ Criar componentes para forms dinâmicos
3. ✅ Implementar lógica de save_project
4. ✅ Adicionar histórico de conversação persistente
5. ✅ Testes E2E do fluxo completo

### Melhorias Futuras:
- 📊 Component: Chart/Graph para visualização
- 📝 Component: Rich Text Editor inline
- 🔄 Action: Undo/Redo de ações
- 💾 Serialização completa de conversas
- 🧪 Testes unitários para cada component type

---

## Referências

- [AG-UI Protocol Spec](https://github.com/ag-ui-protocol/ag-ui)
- [shadcn/ui Components](https://ui.shadcn.com/)
- [Microsoft SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
