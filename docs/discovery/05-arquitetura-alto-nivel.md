# Arquitetura de Alto Nível: Componentes e Fluxos

**Estado:** 🟢 Definido (Ciclo 2)  
**Última atualização:** 2026-01-26

---

## Visão Geral

Sistema de **3 camadas** com **DDD (Domain-Driven Design) + CQRS (Command Query Responsibility Segregation)**:
- Frontend React (SPA)
- Backend .NET (API + Orquestração LLM) com CQRS
- Persistência Supabase + LLM Local

**Estrutura do Projeto:**
- **Monorepo** em `src/` com separação `backend/` e `frontend/`
- Backend: 4 projetos .NET (Api, Application, Domain, Infrastructure)
- Frontend: React + TypeScript com Vite

**Modelo de comunicação:**
- REST para operações síncronas (CRUD via CQRS)
- SignalR/WebSocket para streaming assíncrono (LLM responses)
- Agents Framework para orquestração inteligente

**Padrão Arquitetural:**
- **DDD**: Domínio isolado, linguagem ubíqua, entidades ricas
- **CQRS**: Separação Commands (write) vs Queries (read)
- **Use Cases**: Lógica de aplicação em handlers dedicados

---

## Diagrama de Arquitetura

```
┌──────────────────────────────────────────────────────────────┐
│                      DESKTOP APP (Tauri)                      │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              FRONTEND (React + TypeScript)              │  │
│  │                                                         │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │  │
│  │  │   Editor     │  │  Timeline    │  │   Project   │ │  │
│  │  │  (Lexical)   │  │  Visualizer  │  │  Manager    │ │  │
│  │  └──────────────┘  └──────────────┘  └─────────────┘ │  │
│  │                                                         │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │  │
│  │  │  Character   │  │  LLM Chat    │  │  PDF Export │ │  │
│  │  │  Manager     │  │  Interface   │  │  Preview    │ │  │
│  │  └──────────────┘  └──────────────┘  └─────────────┘ │  │
│  │                                                         │  │
│  │         ↕ (REST)            ↕ (SignalR/WS)            │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
                              ↕
┌──────────────────────────────────────────────────────────────┐
│                  BACKEND (ASP.NET Core + C#)                  │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                    API LAYER                            │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │  │
│  │  │   Projects   │  │  Characters  │  │   Chapters  │ │  │
│  │  │  Controller  │  │  Controller  │  │  Controller │ │  │
│  │  └──────────────┘  └──────────────┘  └─────────────┘ │  │
│  │                                                         │  │
│  │  ┌──────────────┐  ┌──────────────┐                   │  │
│  │  │  LLM Hub     │  │  Export      │                   │  │
│  │  │  (SignalR)   │  │  Controller  │                   │  │
│  │  └──────────────┘  └──────────────┘                   │  │
│  └────────────────────────────────────────────────────────┘  │
│                              ↕                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              APPLICATION LAYER (CQRS)                     │  │
│  │                                                           │  │
│  │  ┌───────────────────────────────────────────────┐  │  │
│  │  │         COMMANDS (Write Operations)            │  │  │
│  │  │  - CreateProjectCommandHandler              │  │  │
│  │  │  - UpdateChapterCommandHandler              │  │  │
│  │  │  - DeleteCharacterCommandHandler            │  │  │
│  │  │  - RewriteTextCommandHandler (LLM)          │  │  │
│  │  └───────────────────────────────────────────────┘  │  │
│  │                                                           │  │
│  │  ┌───────────────────────────────────────────────┐  │  │
│  │  │          QUERIES (Read Operations)             │  │  │
│  │  │  - GetProjectQueryHandler                   │  │  │
│  │  │  - ListCharactersQueryHandler               │  │  │
│  │  │  - GetChapterContentQueryHandler            │  │  │
│  │  │  - GetPlotArcsQueryHandler                  │  │  │
│  │  └───────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────────────┘  │
│                              ↕                                │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │                   DOMAIN LAYER (DDD)                       │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │  │
│  │  │   Entities   │  │  Value Objects│  │  Aggregates │ │  │
│  │  │   (Project,  │  │  (CharacterRole,│  │  (Project  │ │  │
│  │  │   Character, │  │   PlotType)  │  │   Root)     │ │  │
│  │  │   Chapter)   │  │              │  │            │ │  │
│  │  └──────────────┘  └──────────────┘  └─────────────┘ │  │
│  │                                                           │  │
│  │  ┌───────────────────────────────────────────────┐  │  │
│  │  │            Domain Services                   │  │  │
│  │  │  - PlotProgressionService                   │  │  │
│  │  │  - CharacterConsistencyService              │  │  │
│  │  └───────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────────────┘  │
│  │                                                         │  │
│  │  ┌────────────────────────────────────────────────┐   │  │
│  │  │       LLM ORCHESTRATION SERVICE                │   │  │
│  │  │  ┌───────────────┐  ┌──────────────────────┐ │   │  │
│  │  │  │ Agents        │  │  Semantic Kernel     │ │   │  │
│  │  │  │ Framework     │→ │  (RAG + Context Mgmt)│ │   │  │
│  │  │  └───────────────┘  └──────────────────────┘ │   │  │
│  │  │           ↕                                    │   │  │
│  │  │  ┌───────────────────────────────────────┐   │   │  │
│  │  │  │    Context Builder Service            │   │   │  │
│  │  │  │  - Busca personagens relevantes       │   │   │  │
│  │  │  │  - Busca plots/sub-plots              │   │   │  │
│  │  │  │  - Busca capítulos adjacentes         │   │   │  │
│  │  │  │  - Monta prompt contextualizado       │   │   │  │
│  │  │  └───────────────────────────────────────┘   │   │  │
│  │  └────────────────────────────────────────────────┘   │  │
│  │                                                         │  │
│  │  ┌──────────────┐                                      │  │
│  │  │  PDF Export  │  (QuestPDF)                          │  │
│  │  │  Service     │                                      │  │
│  │  └──────────────┘                                      │  │
│  └────────────────────────────────────────────────────────┘  │
│                              ↕                                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                  DATA ACCESS LAYER                      │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │  │
│  │  │  Repositories │  │  DbContext   │  │  Migrations │ │  │
│  │  │  (EF Core)   │  │              │  │             │ │  │
│  │  └──────────────┘  └──────────────┘  └─────────────┘ │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
                              ↕
┌──────────────────────────────────────────────────────────────┐
│                   PERSISTENCE & LLM                           │
│  ┌────────────────────────────────┐  ┌──────────────────┐   │
│  │   PostgreSQL + pgvector        │  │   Ollama (LLM)   │   │
│  │  ┌──────────────────────────┐ │  │  ┌────────────┐  │   │
│  │  │  Relational Tables:      │ │  │  │ Llama 3.1  │  │   │
│  │  │  - Projects              │ │  │  │ 8B Instruct│  │   │
│  │  │  - Characters            │ │  │  └────────────┘  │   │
│  │  │  - Locations             │ │  │                  │   │
│  │  │  - Plots                 │ │  │  API: REST       │   │
│  │  │  - Chapters              │ │  │  Port: 11434     │   │
│  │  │  - ChapterContent        │ │  │                  │   │
│  │  └──────────────────────────┘ │  └──────────────────┘   │
│  │                                │                          │
│  │  ┌──────────────────────────┐ │                          │
│  │  │  Vector Store:           │ │                          │
│  │  │  - Embeddings            │ │                          │
│  │  │    (Characters, Plots,   │ │                          │
│  │  │     Chapters)            │ │                          │
│  │  └──────────────────────────┘ │                          │
│  └────────────────────────────────┘                          │
└──────────────────────────────────────────────────────────────┘
```

---

## Componentes Principais

### 1. Frontend Components

#### Editor Component (Lexical)
**Responsabilidade:**
- Renderizar editor de texto para capítulos
- Suportar comandos de LLM (selecionar texto + ação)
- Autosave periódico
- Mostrar loading state durante processamento LLM

**Integrações:**
- SignalR client para receber streaming de texto da LLM
- REST API para salvar/carregar conteúdo

#### Timeline Visualizer (Recharts)
**Responsabilidade:**
- Renderizar gráfico de arcos narrativos
- Mostrar progressão de plots ao longo dos capítulos
- Permitir marcação de pontos-chave (início, clímax, resolução)
- Interativo: clicar em ponto vai para capítulo

**Dados:**
- Array de arcos (plot principal + sub-plots)
- Cada arco tem pontos: `{ chapterId, intensity: 0-10 }`

#### Character Manager
**Responsabilidade:**
- CRUD de personagens
- Visualizar fichas estruturadas
- Buscar personagens por nome/papel

#### LLM Chat Interface
**Responsabilidade:**
- Interface de brainstorming inicial
- Conversa com LLM para gerar outline
- Histórico de conversas
- Streaming de respostas em tempo real

#### PDF Export Preview
**Responsabilidade:**
- Pré-visualização do PDF antes de exportar
- Configuração de formatação (fonte, margens)
- Botão de download

---

### 2. Backend Services

#### Project Service
**Responsabilidade:**
- Criar, listar, atualizar, deletar projetos
- Um projeto = um livro
- Metadados: título, autor, sinopse, data criação

**Dependências:**
- ProjectRepository (EF Core)

#### Character Service
**Responsabilidade:**
- CRUD de personagens
- Validação de dados (nome obrigatório)
- Gerar embeddings para busca semântica

**Dependências:**
- CharacterRepository
- EmbeddingService (Semantic Kernel)

#### Chapter Service
**Responsabilidade:**
- CRUD de capítulos
- Reordenação (atualizar `Order` field)
- Salvar conteúdo (Markdown/HTML)

**Dependências:**
- ChapterRepository

#### LLM Orchestration Service ⭐ (Core)
**Responsabilidade:**
- Receber comandos do frontend:
  - "Reescrever esse parágrafo"
  - "Gerar outline baseado nessa ideia"
  - "Sugerir diálogo para personagem X"
- Montar contexto relevante (RAG)
- Chamar Agents Framework
- Fazer streaming de resposta via SignalR

**Fluxo interno:**
```
1. Recebe comando + seleção de texto
2. Busca contexto relevante:
   - Personagens mencionados no texto
   - Plot do capítulo atual
   - Capítulos adjacentes (anterior/posterior)
3. Monta prompt estruturado:
   [System] Você é assistente de escrita criativa...
   [Context] Personagens: {...}, Plot: {...}
   [User] Reescreva esse parágrafo em tom noir
   [Selection] {texto selecionado}
4. Chama Ollama via Agents Framework
5. Faz streaming da resposta para frontend via SignalR
```

**Dependências:**
- Microsoft Agents Framework
- Semantic Kernel (RAG)
- Context Builder Service
- Ollama API client (localhost:11434)

#### Context Builder Service
**Responsabilidade:**
- Buscar personagens relevantes (busca semântica via embeddings)
- Buscar plots/sub-plots do projeto
- Buscar capítulos adjacentes
- Montar texto de contexto estruturado

**Exemplo de contexto montado:**
```
=== PERSONAGENS RELEVANTES ===
- Ana Silva: Protagonista, detetive particular, 35 anos
- João Costa: Antagonista, empresário corrupto

=== PLOT PRINCIPAL ===
Ana investiga desaparecimento de jovem, descobre conspiração

=== CAPÍTULO ATUAL (Cap 5) ===
Resumo: Ana confronta João pela primeira vez

=== CAPÍTULO ANTERIOR (Cap 4) ===
Resumo: Ana encontra pista crucial no escritório
```

#### PDF Export Service
**Responsabilidade:**
- Receber projeto completo (metadados + capítulos)
- Gerar PDF usando QuestPDF
- Formatação: capa, sumário, capítulos, fontes

**Fluxo:**
```
1. Carregar projeto + todos capítulos
2. Ordenar capítulos por `Order` field
3. Gerar PDF:
   - Capa com título/autor
   - Sumário com links
   - Capítulos formatados
4. Retornar stream de bytes para download
```

---

### 3. Data Access Layer

#### Opção A: Supabase Client Direto (Simplificado)
**Usar Supabase JS Client diretamente nos services**

Vantagens:
- Menos código (sem repositories)
- REST API auto-gerada
- Realtime subscriptions built-in

Desvantagens:
- Menos controle/abstração
- Acoplamento com Supabase

#### Opção B: Repository Pattern (Recomendado)
**Criar repositories que usam Supabase por baixo**

Vantagens:
- Desacoplamento (mais fácil trocar DB no futuro)
- Testes unitários (mock repositories)
- Lógica de negócio isolada

Desvantagens:
- Mais código boilerplate

**Interfaces:**
```csharp
IProjectRepository
ICharacterRepository
ILocationRepository
IPlotRepository
IChapterRepository
IEmbeddingRepository
```

**Implementação:**
- Usar Supabase .NET Client
- Métodos: GetAsync, CreateAsync, UpdateAsync, DeleteAsync, QueryAsync

---

## Fluxos de Dados Principais

### Fluxo 1: Criar Projeto e Gerar Outline

```
USER → [Frontend] "Quero escrever sobre descoberta de tecnologia..."
  ↓
[Frontend] → POST /api/projects/brainstorm (via SignalR)
  ↓
[Backend - LLM Orchestration Service]
  1. Conecta SignalR hub
  2. Chama Agents Framework com prompt:
     "Ajude o autor a desenvolver essa ideia em um outline de livro"
  3. Ollama gera outline (streaming)
  4. Cada chunk é enviado via SignalR para frontend
  5. Frontend renderiza texto em tempo real
  ↓
USER revisa outline, aprova
  ↓
[Frontend] → POST /api/projects
  Body: { title, author, outline }
  ↓
[Backend - Project Service]
  1. Cria projeto no DB
  2. Parseia outline e cria capítulos automaticamente
  3. Retorna projectId
  ↓
[Frontend] navega para tela de edição do projeto
```

---

### Fluxo 2: Escrever Capítulo com Assistência LLM

```
USER escreve texto no Editor (Lexical)
  ↓ (autosave a cada 5s)
[Frontend] → PATCH /api/chapters/{id}
  Body: { content: "..." }
  ↓
[Backend - Chapter Service]
  Salva conteúdo no DB
---
USER seleciona parágrafo + clica "Reescrever em tom noir"
  ↓
[Frontend] → POST /api/llm/rewrite (via SignalR)
  Body: { 
    chapterId, 
    selectedText, 
    command: "rewrite_noir" 
  }
  ↓
[Backend - LLM Orchestration Service]
  1. Busca contexto:
     - Context Builder Service busca:
       * Personagens (embedding search via Supabase)
       * Plot do capítulo (query Supabase)
       * Capítulos adjacentes (query Supabase)
  2. Monta prompt:
     [Context] {...}
     [Instruction] Reescreva em tom noir
     [Text] {selectedText}
  3. Chama OpenAI API (GPT-OSS-20B) com streaming
  4. Streaming de resposta via SignalR
  ↓
[Frontend - Editor]
  1. Renderiza texto gerado em tempo real
  2. Mostra botões: [Aceitar] [Rejeitar] [Tentar novamente]
  ↓
USER clica [Aceitar]
  ↓
[Frontend] substitui texto selecionado pelo gerado
  ↓
Autosave salva nova versão
```

---

### Fluxo 3: Visualizar Arcos Narrativos

```
USER navega para aba "Arcos"
  ↓
[Frontend] → GET /api/projects/{id}/arcs
  ↓
[Backend - Plot Service]
  1. Busca plots e sub-plots do projeto
  2. Para cada plot, busca pontos de intensidade por capítulo
  3. Retorna JSON:
     [
       {
         name: "Plot Principal",
         points: [
           { chapterId: 1, order: 1, intensity: 2 },
           { chapterId: 2, order: 2, intensity: 5 },
           ...
         ]
       }
     ]
  ↓
[Frontend - Timeline Visualizer]
  Renderiza line chart com Recharts
---
USER clica em ponto do gráfico (ex: Cap 3)
  ↓
[Frontend] navega para editor do Cap 3
```

---

### Fluxo 4: Exportar PDF

```
USER clica "Exportar PDF"
  ↓
[Frontend] → POST /api/export/pdf/{projectId}
  ↓
[Backend - PDF Export Service]
  1. Busca projeto completo:
     - Metadados (título, autor)
     - Todos capítulos (ordenados por `Order`)
  2. Gera PDF com QuestPDF:
     - Capa
     - Sumário
     - Capítulos formatados
  3. Retorna stream de bytes
  ↓
[Frontend] dispara download do arquivo
```

---

## Estratégia de RAG (Retrieval-Augmented Generation)

### Como funciona:

1. **Indexação (Offline):**
   - Quando personagem/plot/capítulo é criado/atualizado:
   - Gera embedding (vetor de 384-1024 dimensões)
   - Salva no pgvector

2. **Busca (Online):**
   - Quando LLM precisa de contexto:
   - Converte query em embedding
   - Busca top-K mais similares no pgvector (cosine similarity)
   - Retorna textos correspondentes

3. **Injeção no Prompt:**
   - Contexto recuperado é injetado no prompt antes do comando

### Exemplo:

**Query:** "Reescreva esse parágrafo"  
**Texto selecionado:** "Ana olhou para João com desconfiança..."

**Busca semântica:**
- Top 2 personagens: Ana, João (match perfeito)
- Top 1 plot: "Ana investiga João"

**Prompt montado:**
```
[System] Você é assistente de escrita criativa...

[Context]
Personagens:
- Ana Silva: Detetive particular, 35 anos, cínica mas empática
- João Costa: Empresário, 45 anos, carismático mas manipulador

Plot: Ana investiga desaparecimento, suspeita de João

[Instruction] Reescreva esse parágrafo em tom noir

[Text]
Ana olhou para João com desconfiança...
```

---

## Segurança e Performance

### Segurança:
- **Aplicação local:** sem autenticação complexa (single-user)
- **Sanitização:** evitar SQL injection (EF Core parametrizado)
- **Validação:** input validation em DTOs

### Performance:
- **Embeddings:** cache em memória (evitar recálculo constante)
- **DB queries:** indexes em `ProjectId`, `Order`, `CreatedAt`
- **SignalR:** connection pooling, compression
- **Ollama:** keep-alive para evitar cold start

---

## Próximos Passos

- [ ] Definir schemas de banco de dados (DDL)
- [ ] Detalhar DTOs e contratos de API
- [ ] Desenhar estrutura de pastas (.NET + React)
- [ ] Criar PoC: Agents Framework + Ollama + SignalR streaming
