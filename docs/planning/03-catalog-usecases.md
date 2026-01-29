# Catálogo de Use Cases

**Última atualização:** 2026-01-27  
**Status:** 🟢 Definido

---

## Lista de Use Cases

| ID | Nome | Feature | Status | Stories |
|---|---|---|---|---|
| [UC001](#uc001-gerar-outline-inicial-com-assistência-llm) | Gerar Outline Inicial com Assistência LLM | F001 | Em Implementacao | 5 |
| [UC002](#uc002-gerenciar-personagens) | Gerenciar Personagens | F002 | Concluída | 4 |
| [UC025](#uc025-gerenciar-projetos) | Gerenciar Projetos | F013 | Concluída | 4 |
| [UC003](#uc003-gerenciar-locais) | Gerenciar Locais | F002 | Concluída | 4 |
| [UC004](#uc004-gerenciar-plots) | Gerenciar Plots | F002 | Concluída | 4 |
| [UC005](#uc005-gerenciar-capítulos) | Gerenciar Capítulos | F002 | Concluída | 5 |
| [UC006](#uc006-visualizar-timeline-de-arcos) | Visualizar Timeline de Arcos | F003 | Planejado | 3 |
| [UC007](#uc007-marcar-pontos-chave-em-arcos) | Marcar Pontos-Chave em Arcos | F003 | Planejado | 3 |
| [UC008](#uc008-escrever-conteúdo-de-capítulo) | Escrever Conteúdo de Capítulo | F004 | Planejado | 4 |
| [UC009](#uc009-navegar-entre-capítulos) | Navegar entre Capítulos | F004 | Planejado | 2 |
| [UC010](#uc010-autosave-de-conteúdo) | Autosave de Conteúdo | F004 | Planejado | 2 |
| [UC011](#uc011-reescrever-trecho-com-llm) | Reescrever Trecho com LLM | F005 | Planejado | 4 |
| [UC012](#uc012-ajustar-tomestilo-com-llm) | Ajustar Tom/Estilo com LLM | F005 | Planejado | 3 |
| [UC013](#uc013-expandir-ou-resumir-texto-com-llm) | Expandir ou Resumir Texto com LLM | F005 | Planejado | 3 |
| [UC014](#uc014-construir-contexto-para-prompt-llm) | Construir Contexto para Prompt LLM | F006 | Planejado | 3 |
| [UC015](#uc015-busca-semântica-de-entidades-relevantes) | Busca Semântica de Entidades Relevantes | F006 | Planejado | 3 |
| [UC016](#uc016-exportar-livro-para-pdf) | Exportar Livro para PDF | F007 | Planejado | 4 |
| [UC017](#uc017-visualizar-preview-do-livro) | Visualizar Preview do Livro | F008 | Planejado | 2 |
| [UC018](#uc018-configurar-supabase-local) | Configurar Supabase Local | F009 | Concluída | 3 |
| [UC019](#uc019-criar-schemas-e-migrations) | Criar Schemas e Migrations | F009 | Concluída | 4 |
| [UC020](#uc020-implementar-application-layer-com-cqrs) | Implementar Application Layer com CQRS | F010 | Concluída | 7 |
| [UC021](#uc021-integrar-ollama-com-backend) | Integrar Ollama com Backend | F011 | Concluída | 4 |
| [UC022](#uc022-implementar-frontend-base) | Implementar Frontend Base | F012 | Concluída | 5 |
| [UC023](#uc023-implementar-cqrs-pattern) | Implementar CQRS Pattern | F010 | Concluída | 4 |
| [UC024](#uc024-implementar-domain-entities-ddd) | Implementar Domain Entities (DDD) | F010 | Concluída | 5 |

---

## Feature F013: Gestão de Projetos

### UC025: Gerenciar Projetos

**Feature:** F013  
**Objetivo:** Criar, visualizar, editar e deletar projetos de livros (criação manual, sem LLM).

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa tela inicial (dashboard ou lista de projetos)
2. Clica em "Novo Projeto"
3. Preenche formulário: Título (obrigatório), Descrição (opcional), Gênero (opcional), Idioma (opcional)
4. Salva projeto
5. Projeto é criado no banco e aparece na lista
6. Autor pode selecionar projeto para trabalhar
7. Autor pode editar informações básicas do projeto
8. Autor pode deletar projeto (com confirmação e warning se houver conteúdo)

**Regras de Negócio:**
- Título é obrigatório e único por usuário
- Ao deletar projeto, todos os dados relacionados (personagens, capítulos, etc) são deletados em cascata
- Deve haver confirmação explícita antes de deletar
- Um projeto vazio (sem capítulos/personagens) pode ser deletado sem warning adicional

**User Stories:**
- US091: Como autor, quero criar novo projeto manualmente informando título e descrição
- US092: Como autor, quero visualizar lista de todos os meus projetos
- US093: Como autor, quero editar informações básicas de um projeto existente
- US094: Como autor, quero deletar projeto com confirmação

**Nota:** Este UC representa o ponto de partida OBRIGATÓRIO. Sem projeto criado, não é possível criar personagens, capítulos, etc. UC001 (brainstorming LLM) é uma forma alternativa/avançada de criar projeto.

---

## Feature F001: Brainstorming Inicial com LLM

### UC001: Gerar Outline Inicial com Assistência LLM

**Feature:** F001  
**Status:** 🟡 Em Implementacao (1/5 stories concluídas)  
**Objetivo:** Transformar ideia bruta do autor em estrutura de livro (outline) através de conversação com LLM.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa interface de novo projeto (/brainstorm)
2. Descreve ideia inicial em texto livre ✅ **Concluído**
3. LLM faz perguntas para expandir (gênero, conflito central, protagonista) ⏳ **Próximo**
4. Autor responde às perguntas
5. LLM gera outline: título sugerido, sinopse, lista de capítulos com resumos, personagens principais, plot principal
6. Autor revisa e ajusta outline
7. Sistema salva projeto com estrutura inicial

**Regras de Negócio:**
- Outline deve ter no mínimo 3 capítulos
- Deve ter pelo menos 1 personagem
- Plot principal é obrigatório

**User Stories:**
- ✅ US001: Como autor, quero descrever minha ideia de livro para a LLM entender
- 🔴 US002: Como autor, quero que a LLM faça perguntas para expandir minha ideia
- 🔴 US003: Como autor, quero que a LLM gere outline estruturado baseado na conversa
- 🔴 US004: Como autor, quero revisar e editar o outline gerado
- 🔴 US005: Como autor, quero salvar o projeto com a estrutura inicial criada

**Implementação Atual (US001):**
- ✅ Backend: StartBrainstormCommand + Handler + Validator
- ✅ SignalR Hub: StartBrainstorm e ContinueBrainstorm methods
- ✅ Frontend: BrainstormChat component com streaming em tempo real
- ✅ Hook: useBrainstorm para gerenciar estado do chat
- ✅ Rota: /brainstorm acessível pela página de projetos

---

## Feature F002: Gestão de Entidades Narrativas

### UC002: Gerenciar Personagens

**Feature:** F002  
**Objetivo:** Criar, visualizar, editar e deletar personagens do livro.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa lista de personagens do projeto
2. Clica em "Novo Personagem"
3. Preenche: nome, papel (protagonista/antagonista/suporte), descrição, traits (idade, personalidade, etc)
4. Salva personagem
5. Personagem aparece na lista
6. Autor pode editar ou deletar personagem

**Regras de Negócio:**
- Nome é obrigatório
- Traits são opcionais
- Ao deletar personagem, verificar se está referenciado em capítulos (warning)

**User Stories:**
- US006: Como autor, quero criar novo personagem informando nome, papel e descrição
- US007: Como autor, quero visualizar lista de todos os personagens
- US008: Como autor, quero editar personagem existente
- US009: Como autor, quero deletar personagem com confirmação

---

### UC003: Gerenciar Locais

**Feature:** F002  
**Objetivo:** Criar, visualizar, editar e deletar locais/cenários do livro.

**Status:** 🟢 Concluída

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa lista de locais
2. Cria novo local: nome e descrição
3. Salva local
4. Pode editar ou deletar

**Regras de Negócio:**
- Nome é obrigatório
- Descrição opcional

**User Stories:**
- US010: Como autor, quero criar novo local informando nome e descrição ✅
- US011: Como autor, quero visualizar lista de locais ✅
- US012: Como autor, quero editar local existente ✅
- US013: Como autor, quero deletar local ✅

---

### UC004: Gerenciar Plots

**Feature:** F002  
**Objetivo:** Criar, visualizar, editar e deletar plots (arcos narrativos).

**Status:** 🟢 Concluída

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa lista de plots
2. Cria novo plot: nome, tipo (principal/subplot), descrição
3. Salva plot
4. Pode editar ou deletar

**Regras de Negócio:**
- Deve existir pelo menos 1 plot principal
- Nome e tipo são obrigatórios

**User Stories:**
- US014: Como autor, quero criar novo plot definindo nome, tipo e descrição ✅
- US015: Como autor, quero visualizar lista de plots ✅
- US016: Como autor, quero editar plot existente ✅
- US017: Como autor, quero deletar plot (com warning se houver pontos marcados) ✅

---

### UC005: Gerenciar Capítulos

**Feature:** F002  
**Objetivo:** Criar, visualizar, reordenar, editar e deletar capítulos.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa lista de capítulos
2. Cria novo capítulo: título e resumo (conteúdo vem depois)
3. Reordena capítulos via drag-and-drop ou controles
4. Edita título/resumo
5. Deleta capítulo (com warning se tiver conteúdo)

**Regras de Negócio:**
- Título é obrigatório
- Ordem dos capítulos deve ser sequencial (1, 2, 3...)
- Ao deletar capítulo com conteúdo, confirmar ação

**User Stories:**
- US018: Como autor, quero criar novo capítulo com título e resumo
- US019: Como autor, quero visualizar lista de capítulos em ordem
- US020: Como autor, quero reordenar capítulos
- US021: Como autor, quero editar título e resumo de capítulo
- US022: Como autor, quero deletar capítulo

---

## Feature F003: Visualização de Arcos Narrativos

### UC006: Visualizar Timeline de Arcos

**Feature:** F003  
**Objetivo:** Ver progressão visual de plots ao longo dos capítulos.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor acessa visualização de timeline
2. Sistema mostra gráfico com eixo X = capítulos, eixo Y = intensidade
3. Cada plot é uma linha no gráfico
4. Autor vê claramente onde cada arco está ativo

**Regras de Negócio:**
- Apenas plots com pontos marcados aparecem
- Intensidade varia de 0 a 10

**User Stories:**
- US023: Como autor, quero ver gráfico visual dos arcos narrativos
- US024: Como autor, quero filtrar timeline por plot específico
- US025: Como autor, quero clicar em ponto do gráfico e ir para capítulo

---

### UC007: Marcar Pontos-Chave em Arcos

**Feature:** F003  
**Objetivo:** Marcar pontos de intensidade de um plot em capítulos específicos.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor seleciona plot
2. Seleciona capítulo
3. Define intensidade (0-10) e descrição opcional (ex: "Clímax do conflito X")
4. Salva ponto
5. Ponto aparece na timeline

**Regras de Negócio:**
- Um plot pode ter múltiplos pontos em capítulos diferentes
- Intensidade: 0 = ausente, 10 = máxima

**User Stories:**
- US026: Como autor, quero marcar ponto de intensidade de plot em capítulo
- US027: Como autor, quero editar intensidade de ponto existente
- US028: Como autor, quero remover ponto de plot

---

## Feature F004: Editor de Capítulos

### UC008: Escrever Conteúdo de Capítulo

**Feature:** F004  
**Objetivo:** Escrever texto do capítulo no editor.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor seleciona capítulo na lista
2. Editor carrega conteúdo (vazio ou existente)
3. Autor escreve texto (Markdown ou WYSIWYG)
4. Editor faz autosave periódico
5. Contador de palavras atualiza em tempo real

**Regras de Negócio:**
- Conteúdo é salvo automaticamente a cada 5 segundos ou ao mudar de capítulo
- Suporta formatação básica (negrito, itálico, listas)

**User Stories:**
- US029: Como autor, quero escrever texto no editor de capítulo
- US030: Como autor, quero formatar texto (negrito, itálico, etc)
- US031: Como autor, quero ver contador de palavras em tempo real
- US032: Como autor, quero que conteúdo seja salvo automaticamente

---

### UC009: Navegar entre Capítulos

**Feature:** F004  
**Objetivo:** Trocar de capítulo durante escrita sem perder trabalho.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor está escrevendo capítulo X
2. Clica em capítulo Y na lista lateral
3. Sistema salva conteúdo de X
4. Carrega conteúdo de Y no editor

**Regras de Negócio:**
- Sempre salvar antes de trocar
- Mostrar indicador de salvamento bem-sucedido

**User Stories:**
- US033: Como autor, quero navegar entre capítulos clicando na lista
- US034: Como autor, quero que sistema salve antes de trocar de capítulo

---

### UC010: Autosave de Conteúdo

**Feature:** F004  
**Objetivo:** Garantir que trabalho não seja perdido.

**Atores:** Sistema

**Fluxo Resumido:**
1. Autor escreve no editor
2. A cada 5 segundos de inatividade, sistema salva
3. Mostra indicador "Salvando..." e depois "Salvo às HH:MM"
4. Se houver erro, mostra warning

**Regras de Negócio:**
- Autosave a cada 5 segundos de inatividade
- Salvar também ao fechar aplicação

**User Stories:**
- US035: Como autor, quero que sistema salve automaticamente meu trabalho
- US036: Como autor, quero ver indicador de status de salvamento

---

## Feature F005: Comandos LLM Contextuais

### UC011: Reescrever Trecho com LLM

**Feature:** F005  
**Objetivo:** Selecionar texto e pedir reescrita.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor seleciona trecho de texto
2. Clica em "Reescrever" (ou atalho)
3. Sistema envia texto + contexto para LLM
4. LLM retorna versão reescrita (streaming)
5. Autor pode aceitar, rejeitar ou pedir nova versão

**Regras de Negócio:**
- Contexto inclui: personagens, plot atual, capítulo adjacente
- Streaming de resposta (palavra por palavra)

**User Stories:**
- US037: Como autor, quero selecionar texto e pedir reescrita
- US038: Como autor, quero ver resposta da LLM em streaming
- US039: Como autor, quero aceitar sugestão da LLM
- US040: Como autor, quero rejeitar sugestão e manter original

---

### UC012: Ajustar Tom/Estilo com LLM

**Feature:** F005  
**Objetivo:** Alterar tom do texto (ex: mais sombrio, mais leve).

**Atores:** Autor

**Fluxo Resumido:**
1. Autor seleciona trecho
2. Escolhe comando customizado: "tom mais sombrio", "mais formal", etc
3. LLM processa com contexto
4. Retorna versão ajustada

**Regras de Negócio:**
- Lista de comandos pré-definidos + campo livre
- Contexto sempre incluso

**User Stories:**
- US041: Como autor, quero ajustar tom do texto selecionado
- US042: Como autor, quero digitar comando customizado
- US043: Como autor, quero que LLM mantenha coerência com contexto

---

### UC013: Expandir ou Resumir Texto com LLM

**Feature:** F005  
**Objetivo:** Aumentar ou reduzir tamanho de trecho.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor seleciona trecho
2. Escolhe "Expandir" ou "Resumir"
3. LLM retorna versão ajustada

**Regras de Negócio:**
- Expandir: manter ideia central, adicionar detalhes
- Resumir: manter ideia central, remover detalhes

**User Stories:**
- US044: Como autor, quero expandir trecho adicionando detalhes
- US045: Como autor, quero resumir trecho mantendo essência
- US046: Como autor, quero controlar nível de expansão/resumo

---

## Feature F006: Gerenciamento de Contexto Automático

### UC014: Construir Contexto para Prompt LLM

**Feature:** F006  
**Objetivo:** Ao invocar LLM, sistema monta prompt com contexto relevante automaticamente.

**Atores:** Sistema

**Fluxo Resumido:**
1. Autor invoca comando LLM (ex: reescrever)
2. Sistema identifica capítulo atual
3. Busca personagens mencionados no capítulo
4. Busca plots ativos (pontos no capítulo)
5. Busca capítulos adjacentes (anterior e próximo)
6. Monta prompt: contexto + comando do autor
7. Envia para LLM

**Regras de Negócio:**
- Limite de tokens no contexto (ex: 4000 tokens)
- Priorizar: personagens do capítulo > plots ativos > capítulos adjacentes

**User Stories:**
- US047: Como sistema, quero identificar personagens relevantes no capítulo
- US048: Como sistema, quero buscar plots ativos no capítulo
- US049: Como sistema, quero montar prompt contextualizado automaticamente

---

### UC015: Busca Semântica de Entidades Relevantes

**Feature:** F006  
**Objetivo:** Usar embeddings (pgvector) para encontrar personagens/locais/plots relevantes.

**Atores:** Sistema

**Fluxo Resumido:**
1. Sistema gera embedding do texto selecionado
2. Faz busca vetorial em personagens, locais, plots
3. Retorna top 5 mais similares
4. Adiciona ao contexto do prompt

**Regras de Negócio:**
- Embeddings gerados ao salvar entidades
- Busca usa similaridade de cosseno

**User Stories:**
- US050: Como sistema, quero gerar embeddings de entidades
- US051: Como sistema, quero buscar entidades por similaridade semântica
- US052: Como sistema, quero atualizar embeddings ao editar entidades

---

## Feature F007: Geração de PDF

### UC016: Exportar Livro para PDF

**Feature:** F007  
**Objetivo:** Gerar PDF do livro completo.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor clica em "Exportar PDF"
2. Sistema carrega: título, autor, capítulos em ordem
3. Gera PDF: capa, sumário, conteúdo dos capítulos
4. Salva PDF em local escolhido pelo autor
5. Mostra notificação de sucesso

**Regras de Negócio:**
- Formatação: fonte legível (Merriweather ou similar), margens 2cm, quebra de capítulo
- Sumário clicável (links internos)
- Capa simples: título e autor centralizados

**User Stories:**
- US053: Como autor, quero exportar livro para PDF
- US054: Como autor, quero escolher local de salvamento do PDF
- US055: Como autor, quero que PDF tenha formatação profissional
- US056: Como autor, quero sumário clicável no PDF

---

## Feature F008: Preview do Livro

### UC017: Visualizar Preview do Livro

**Feature:** F008  
**Objetivo:** Ver como livro ficará no PDF antes de exportar.

**Atores:** Autor

**Fluxo Resumido:**
1. Autor clica em "Preview"
2. Sistema renderiza visualização inline (PDF ou HTML simulando PDF)
3. Autor navega páginas
4. Pode fechar preview e voltar para edição

**Regras de Negócio:**
- Preview deve ser fiel ao PDF final
- Carregamento < 3 segundos

**User Stories:**
- US057: Como autor, quero visualizar preview do livro antes de exportar
- US058: Como autor, quero navegar páginas do preview

---

## Feature F009: Setup de Banco de Dados

### UC018: Configurar Supabase Local

**Feature:** F009  
**Objetivo:** Subir Supabase local via Docker com PostgreSQL + pgvector.

**Atores:** Desenvolvedor

**Fluxo Resumido:**
1. Clonar repositório
2. Executar `docker-compose up` na pasta do projeto
3. Supabase Studio acessível em `localhost:54323`
4. PostgreSQL acessível em `localhost:54322`
5. Verificar health checks

**Regras de Negócio:**
- Usar Supabase CLI ou docker-compose oficial
- Incluir pgvector extension

**User Stories:**
- US059: Como desenvolvedor, quero subir Supabase local via Docker
- US060: Como desenvolvedor, quero acessar Supabase Studio localmente
- US061: Como desenvolvedor, quero confirmar que pgvector está habilitado

---

### UC019: Criar Schemas e Migrations

**Feature:** F009  
**Objetivo:** Definir schemas SQL de todas as tabelas e executar migrations.

**Atores:** Desenvolvedor

**Fluxo Resumido:**
1. Criar arquivos de migration SQL ou usar EF Core Migrations
2. Definir tabelas: Projects, Characters, Locations, Plots, Chapters, PlotPoints, Embeddings
3. Executar migrations no banco local
4. Verificar tabelas criadas

**Regras de Negócio:**
- Seguir schemas definidos no discovery
- Usar UUIDs como PKs
- Timestamps (CreatedAt, UpdatedAt) em todas as tabelas

**User Stories:**
- US062: Como desenvolvedor, quero criar migrations para todas as tabelas
- US063: Como desenvolvedor, quero executar migrations no banco local
- US064: Como desenvolvedor, quero verificar integridade dos schemas
- US065: Como desenvolvedor, quero criar índices e constraints

---

## Feature F010: API Backend (.NET)

### UC020: Implementar Application Layer com CQRS

**Feature:** F010  
**Objetivo:** Criar Application Layer que orquestra operações de negócio através de Commands e Queries, expondo API REST como interface.

**Atores:** Sistema

**Fluxo Resumido:**
1. API Controllers recebem requisições HTTP
2. Controllers criam Command/Query apropriado
3. MediatR dispatch para Handler correspondente
4. Handler executa lógica via Domain Entities e Repositories
5. Response é serializado e retornado via Controller
6. SignalR Hub para streaming LLM em tempo real

**Regras de Negócio:**
- Commands modificam estado (POST/PUT/DELETE)
- Queries apenas leem (GET)
- Validação via FluentValidation antes de executar Command
- Handlers trabalham com Domain Entities ricas, não DTOs anêmicos
- Repositories retornam aggregates completos

**User Stories:**
- US066: Como sistema, quero handlers CQRS para operações de Projects
- US067: Como sistema, quero handlers CQRS para operações de Characters
- US068: Como sistema, quero handlers CQRS para operações de Locations
- US069: Como sistema, quero handlers CQRS para operações de Plots
- US070: Como sistema, quero handlers CQRS para operações de Chapters
- US090: Como sistema, quero handlers CQRS para operações de PlotPoints
- US071: Como sistema, quero SignalR Hub para streaming respostas LLM

---

## Feature F011: Integração LLM Local (Ollama)

### UC021: Integrar Ollama com Backend

**Feature:** F011  
**Objetivo:** Conectar backend ao Ollama para invocar LLM local.

**Atores:** Desenvolvedor

**⚠️ IMPORTANTE - Stack Técnica Obrigatória:**
- **Microsoft.Extensions.AI** (v10.0) - Abstrações de LLM
- **Semantic Kernel** (v1.x) - Orquestração e RAG
- **Microsoft Agents Framework** - Gerenciamento de agentes
- **Ollama Connector** para Semantic Kernel

**Fluxo Resumido:**
1. Instalar Ollama localmente
2. Baixar modelo (ex: `ollama pull gpt-oss-20b`)
3. Configurar Semantic Kernel com Ollama connector
4. Implementar streaming via Agents Framework
5. Testar integração

**Regras de Negócio:**
- Endpoint Ollama: `http://localhost:11434/api/generate`
- Streaming via Semantic Kernel streaming APIs
- Timeout configurável (ex: 60s)
- Usar abstrações do Microsoft.Extensions.AI (não criar HttpClient direto)

**User Stories:**
- US072: Como desenvolvedor, quero conectar backend ao Ollama usando Semantic Kernel
- US073: Como desenvolvedor, quero implementar streaming via Agents Framework
- US074: Como desenvolvedor, quero tratar erros usando Semantic Kernel patterns
- US075: Como desenvolvedor, quero configurar modelo via Semantic Kernel configuration

---

## Feature F012: Frontend Base (React)

### UC022: Implementar Frontend Base

**Feature:** F012  
**Objetivo:** Criar aplicação React funcional com roteamento e integração com backend.

**Atores:** Desenvolvedor

**Fluxo Resumido:**
1. Criar projeto React com TypeScript (Vite)
2. Configurar roteamento (React Router)
3. Configurar estado global (Zustand ou Context API)
4. Criar client HTTP para API backend (Axios)
5. Criar layout base com navegação

**Regras de Negócio:**
- TypeScript estrito
- Axios para HTTP, SignalR client para WebSocket
- Layout responsivo (desktop-first)

**User Stories:**
- US076: Como desenvolvedor, quero criar aplicação React com TypeScript
- US077: Como desenvolvedor, quero configurar roteamento
- US078: Como desenvolvedor, quero criar client HTTP para API
- US079: Como desenvolvedor, quero implementar layout base
- US080: Como desenvolvedor, quero configurar SignalR client

---

## Feature F010: API Backend (.NET) - Continuação

### UC023: Implementar CQRS Pattern

**Feature:** F010  
**Objetivo:** Separar operações de escrita (Commands) e leitura (Queries) seguindo pattern CQRS.

**Atores:** Desenvolvedor

**Fluxo Resumido:**
1. Criar estrutura de Commands e Command Handlers
2. Criar estrutura de Queries e Query Handlers
3. Implementar MediatR para dispatch de commands/queries
4. Integrar CQRS nos controllers
5. Testar separação write/read

**Regras de Negócio:**
- Commands retornam void ou ID da entidade criada
- Queries retornam DTOs (nunca entidades de domínio)
- Handlers devem ser idempotentes quando possível
- Validação via FluentValidation em Commands

**User Stories:**
- US081: Como desenvolvedor, quero implementar estrutura de Commands
- US082: Como desenvolvedor, quero implementar estrutura de Queries
- US083: Como desenvolvedor, quero integrar MediatR para CQRS
- US084: Como desenvolvedor, quero criar validadores para Commands

---

### UC024: Implementar Domain Entities (DDD)

**Feature:** F010  
**Objetivo:** Criar entidades de domínio ricas com comportamento encapsulado seguindo DDD.

**Atores:** Desenvolvedor

**Fluxo Resumido:**
1. Criar entidades de domínio (Project, Character, Chapter, Plot)
2. Criar Value Objects (CharacterRole, PlotType)
3. Implementar agregados (Project como Aggregate Root)
4. Criar Domain Services para lógica complexa
5. Definir Domain Events para comunicação entre agregados

**Regras de Negócio:**
- Entidades sempre válidas (validação no construtor)
- Aggregate Root controla acesso a entidades filhas
- Value Objects são imutáveis
- Domain Services para lógica que não pertence a uma entidade

**User Stories:**
- US085: Como desenvolvedor, quero criar entidades de domínio ricas
- US086: Como desenvolvedor, quero criar Value Objects
- US087: Como desenvolvedor, quero implementar agregados com Aggregate Root
- US088: Como desenvolvedor, quero criar Domain Services
- US089: Como desenvolvedor, quero implementar Domain Events

---

## Mapeamento Completo: Use Cases → Stories

Total de Use Cases: 24  
Total estimado de Stories: ~89

Todos os use cases estão mapeados para features, e todas as stories estão listadas nos use cases detalhados acima.
