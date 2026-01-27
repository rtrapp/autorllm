# Stack Tecnológica: Decisões e Trade-offs

**Estado:** 🟢 Definido (Ciclo 2)  
**Última atualização:** 2026-01-26

---

## Decisões Fundamentais

### 1. Backend: .NET 10 + Microsoft Agents Framework

**Por quê .NET 10?**
- ✅ Performance superior para workloads assíncronos (comunicação com LLM)
- ✅ Microsoft Agents Framework nativo (integração com LLMs)
- ✅ Ecossistema maduro (Entity Framework, SignalR, etc.)
- ✅ Cross-platform (Linux, Windows, macOS)

**Trade-offs:**
- ❌ Curva de aprendizado se você vem de Node.js
- ❌ Menos bibliotecas de LLM comparado a Python
- ✅ Mas Agents Framework compensa isso

**Alternativas descartadas:**
- Node.js/TypeScript: menos performance para workloads CPU-intensive, Agents Framework não disponível
- Python: melhor para ML/LLM, mas pior para aplicação full-stack com frontend

---

### 2. LLM: GPT-OSS-20B Local via Ollama

**Por quê GPT-OSS-20B local?**
- ✅ Modelo open-source de 20B parâmetros (qualidade superior a Llama 8B)
- ✅ Custo zero (sem API paga)
- ✅ Privacidade total (dados não saem da máquina)
- ✅ Sem limites de rate limit ou token quota
- ✅ Funciona offline
- ✅ Hardware suficiente disponível

**Trade-offs:**
- ❌ Requer hardware potente (mas você tem)
- ✅ Qualidade superior a modelos 7B-8B
- ✅ Melhor contexto e raciocínio

**Modelo:**
- **GPT-OSS-20B** via Ollama
- Alternativas: Llama 3.1 70B (se hardware permitir) ou Qwen 2.5 32B

**Runtime:**
- **Ollama** (simples, cross-platform, API REST)
- Download: `ollama pull gpt-oss-20b`
- API endpoint: `http://localhost:11434`

---

### 3. Frontend: React + TypeScript

**Por quê React?**
- ✅ Ecossistema maduro para editores de texto ricos
- ✅ Componentização facilita módulos (editor, timeline, fichas)
- ✅ TypeScript para type-safety

**Trade-offs:**
- ❌ Bundle size maior que alternativas (Svelte, Vue)
- ✅ Mas não é problema para aplicação desktop/self-hosted

**Alternativas descartadas:**
- Vue: menos bibliotecas de editores de texto
- Svelte: ecossistema menor para componentes complexos
- Blazor: mantém tudo .NET, mas menos maduro para editores ricos

---

### 4. Editor de Texto: Lexical (Meta)

**Por quê Lexical?**
- ✅ Moderno, performático, extensível
- ✅ Suporta Markdown + WYSIWYG
- ✅ API poderosa para integração com LLM (comandos customizados)
- ✅ TypeScript nativo

**Trade-offs:**
- ❌ Relativamente novo (2022), menos maduro que Quill/Draft.js
- ✅ Mas Meta investe pesado, usado no Facebook/Instagram

**Alternativas:**
- TipTap (Prosemirror-based): mais maduro, mas menos performático
- Quill: simples, mas menos extensível
- Monaco (VS Code editor): overkill para narrativa, foco em código

---

### 5. Database: Supabase Local (Docker)

**Por quê Supabase local?**
- ✅ PostgreSQL + **pgvector extension** já configurado
- ✅ **Supabase Studio** (dashboard visual local)
- ✅ **REST API auto-gerada** (mesma experiência que cloud)
- ✅ **Realtime subscriptions** (WebSocket)
- ✅ **Storage** local para PDFs/backups
- ✅ **Custo zero** (tudo local)
- ✅ **Privacidade total** (dados não saem da máquina)
- ✅ **Funciona offline**
- ✅ Fácil migração para cloud se necessário (mesma API)

**Trade-offs:**
- ❌ Requer Docker instalado
- ❌ Configuração inicial (mas simplificada com CLI)
- ✅ Mas elimina vendor lock-in e custos

**Setup:**
```bash
# Instalar Supabase CLI
npm install -g supabase

# Inicializar projeto
supabase init

# Subir containers locais (PostgreSQL + APIs + Studio)
supabase start

# Acesso:
# - Database: postgresql://postgres:postgres@localhost:54322/postgres
# - API: http://localhost:54321
# - Studio: http://localhost:54323
```

**Containers:**
- PostgreSQL 15 com pgvector
- PostgREST (REST API)
- Realtime Server (WebSocket)
- Storage Server
- Kong (API Gateway)
- Supabase Studio (Dashboard)

**Alternativas descartadas:**
- Supabase Cloud: custo mensal, vendor lock-in
- PostgreSQL standalone: sem REST API/Realtime/Storage prontos
- PlanetScale/Neon: não rodam localmente

---

### 6. Geração de PDF: QuestPDF (.NET)

**Por quê QuestPDF?**
- ✅ Biblioteca .NET nativa, fluent API
- ✅ Layout flexível, similar a LaTeX mas mais simples
- ✅ Suporta fontes customizadas, imagens, formatação avançada
- ✅ Open-source, performance excelente

**Trade-offs:**
- ❌ Requer aprender API específica
- ✅ Mas documentação é excelente

**Alternativas descartadas:**
- Pandoc (Markdown → PDF): menos controle de layout
- iText/PDFSharp: mais verbosos, menos modernos
- HTML → PDF (wkhtmltopdf): qualidade inferior

---

### 7. Visualização de Arcos: Recharts (React)

**Por quê Recharts?**
- ✅ Biblioteca React de gráficos, fácil de integrar
- ✅ Suporta line charts, timelines, customizável
- ✅ Responsivo, animações suaves

**Trade-offs:**
- ❌ Bundle size ~200kb
- ✅ Aceitável para aplicação desktop

**Alternativas:**
- D3.js: mais poderoso, mas muito mais complexo
- Chart.js: não é React-native, requer wrappers
- Victory: similar, mas menos popular

---

### 8. Communication Backend ↔ Frontend: REST API + SignalR

**Por quê REST + SignalR?**
- ✅ **REST** para operações CRUD (personagens, capítulos)
- ✅ **SignalR** para streaming de respostas LLM (real-time)
- ✅ SignalR é nativo .NET, WebSocket com fallback

**Trade-offs:**
- ❌ Dois padrões de comunicação
- ✅ Mas cada um otimizado para seu caso de uso

**Alternativas descartadas:**
- GraphQL: overkill para aplicação simples
- gRPC: complexidade adicional, REST é suficiente

---

### 9. Context Management: RAG com Semantic Kernel

**Por quê Semantic Kernel?**
- ✅ Framework da Microsoft para orquestração de LLM
- ✅ Integração nativa com Agents Framework
- ✅ Suporta RAG, embeddings, prompt templating
- ✅ Plugins para memória persistente

**Como funciona:**
1. Todo conteúdo (personagens, plots, capítulos) é convertido em embeddings
2. Armazenado no pgvector
3. Quando LLM precisa de contexto, busca semântica retorna top-K relevantes
4. Contexto é injetado no prompt automaticamente

**Trade-offs:**
- ❌ Adiciona camada de complexidade
- ✅ Mas resolve problema crítico de context window limitado

---

### 10. Hosting: Aplicação Desktop (Electron/Tauri)

**Decisão inicial: Tauri**

**Por quê Tauri?**
- ✅ Bundle pequeno (~10-20MB vs 100MB+ Electron)
- ✅ Usa WebView nativo do OS (sem Chromium embarcado)
- ✅ Suporta .NET backend via sidecar
- ✅ Cross-platform (Windows, macOS, Linux)

**Trade-offs:**
- ❌ Menos maduro que Electron
- ❌ WebView pode ter inconsistências entre OSs
- ✅ Mas performance e tamanho compensam

**Alternativa:**
- Web self-hosted (Kestrel + Nginx): possível, mas requer configuração manual
- Electron: funciona, mas bundle gigante

---

## Stack Completa Visualizada

```
┌─────────────────────────────────────────────────────┐
│                   FRONTEND (React)                   │
├─────────────────────────────────────────────────────┤
│  - TypeScript                                        │
│  - Lexical (Editor de texto)                         │
│  - Recharts (Visualização de arcos)                  │
│  - TailwindCSS (Styling)                             │
│  - Zustand (State management)                        │
└─────────────────────────────────────────────────────┘
                       ↕ (REST + SignalR)
┌─────────────────────────────────────────────────────┐
│                BACKEND (.NET 10 + C#)                │
├─────────────────────────────────────────────────────┤
│  - ASP.NET Core (Web API + SignalR)                 │
│  - Microsoft Agents Framework (Orquestração LLM)    │
│  - Semantic Kernel (RAG + Context Management)       │
│  - Entity Framework Core (ORM)                      │
│  - QuestPDF (Geração de PDF)                        │
└─────────────────────────────────────────────────────┘
                       ↕
┌─────────────────────────────────────────────────────┐
│                   PERSISTENCE                        │
├─────────────────────────────────────────────────────┤
│  - Supabase (PostgreSQL gerenciado)                 │
│  - pgvector (Embeddings para RAG)                   │
│  - Supabase Storage (PDFs, backups)                 │
└─────────────────────────────────────────────────────┘
                       ↕
┌─────────────────────────────────────────────────────┐
│                 LLM (Local via Ollama)              │
├─────────────────────────────────────────────────────┤
│  - Ollama (Runtime)                                  │
│  - GPT-OSS-20B (Modelo local)                       │
│  - API REST (localhost:11434)                       │
└─────────────────────────────────────────────────────┘
                       ↕
┌─────────────────────────────────────────────────────┐
│                    PACKAGING                         │
├─────────────────────────────────────────────────────┤
│  - Tauri (Desktop app - Windows/macOS/Linux)        │
└─────────────────────────────────────────────────────┘
```

---

## Complexidade vs Valor

### Baixa Complexidade:
- CRUD de entidades (Entity Framework)
- REST API básico
- Editor de texto integrado (Lexical)

### Média Complexidade:
- SignalR para streaming LLM
- RAG com Semantic Kernel + pgvector
- Visualização de arcos (Recharts)
- Export PDF (QuestPDF)

### Alta Complexidade (mas necessária):
- Integração Microsoft Agents Framework
- Context management inteligente
- Orquestração de prompts

---

## Riscos Técnicos

### 1. Performance da LLM Local (GPT-OSS-20B)
**Risco:** Modelo 20B é pesado, pode ter latência em gerações longas

**Mitigação:**
- Hardware suficiente disponível (conforme confirmado)
- Streaming de resposta (usuário vê texto sendo gerado)
- Keep-alive do Ollama (evita cold start)
- Quantização 4-bit se necessário (Q4_K_M)

### 2. Context Window Limitado
**Risco:** Livro tem 50k+ palavras, LLM tem window de 8k-32k tokens

**Mitigação:**
- RAG: buscar apenas contexto relevante
- Summarização: resumir capítulos anteriores
- Estratégia de janelas deslizantes

### 3. Qualidade da LLM
**Risco:** Llama 8B pode não ter qualidade de GPT-4 para reescrita criativa

**Mitigação:**
- Prompts muito bem estruturados
- Few-shot examples embutidos
- Permitir ajuste fino de prompts pelo usuário
- Futuro: suporte a API externa (Claude/GPT) como fallback

### 4. Integração Tauri + .NET
**Risco:** Tauri com backend .NET via sidecar pode ter bugs

**Mitigação:**
- PoC inicial: validar comunicação Tauri ↔ .NET
- Alternativa: começar com web self-hosted, Tauri depois

---

## Tecnologias Específicas - Versões

```yaml
Backend:
  - .NET: 10.0
  - Entity Framework Core: 10.0
  - Microsoft.Extensions.AI: 10.0
  - Semantic Kernel: 1.x
  - QuestPDF: 2024.x
  - SignalR: incluído no ASP.NET Core

Frontend:
  - React: 18.x
  - TypeScript: 5.x
  - Lexical: ^0.19.0
  - Recharts: ^2.x
  - TailwindCSS: ^3.x
  - Zustand: ^4.x

Database:
  - Supabase CLI: Latest
  - Docker: 20.x+
  - Docker Compose: 2.x+
  - PostgreSQL: 15.x (via Supabase Docker)
  - pgvector: 0.7.x (incluído no Supabase)
  - Supabase JS/C# Client: ^2.x

LLM:
  - Ollama: 0.4.x+
  - Modelo: gpt-oss-20b (ou gpt-oss-20b:Q4_K_M para quantizado)
  - Alternativas: llama3.1:70b, qwen2.5:32b

Packaging:
  - Tauri: 2.x
```

---

## Próximos Passos

- [ ] Detalhar arquitetura de alto nível (componentes, fluxos de dados)
- [ ] Definir schemas de banco de dados
- [ ] Desenhar estratégia de contexto LLM (RAG)
- [ ] Criar PoC de integração Agents Framework + Ollama
