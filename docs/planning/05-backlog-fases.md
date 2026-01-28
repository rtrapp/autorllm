# Backlog por Fases - MVP First

**Última atualização:** 2026-01-28  
**Status:** 🟢 Definido

---

## Visão Geral do Roadmap

Este documento organiza todo o backlog em fases incrementais, priorizando o MVP. Cada fase é funcional por si só e entrega valor concreto.

---

## Ordem de Implementação das User Stories

**Total:** 94 User Stories organizadas por sprint

| # | Story ID | Descrição | Sprint | Status |
|---|---|---|---|---|
| 1 | US059 | Subir Supabase local via Docker | Sprint 0 | 🟢 Concluída |
| 2 | US060 | Acessar Supabase Studio localmente | Sprint 0 | 🟢 Concluída |
| 3 | US061 | Confirmar pgvector habilitado | Sprint 0 | 🟢 Concluída |
| 4 | US062 | Criar migrations para tabelas | Sprint 0 | 🟢 Concluída |
| 5 | US063 | Executar migrations no banco | Sprint 0 | 🟢 Concluída |
| 6 | US064 | Verificar integridade dos schemas | Sprint 0 | 🟢 Concluída |
| 7 | US065 | Criar índices e constraints | Sprint 0 | 🟢 Concluída |
| 8 | US085 | Criar entidades de domínio ricas (DDD) | Sprint 0 | � Concluída |
| 9 | US086 | Criar Value Objects (DDD) | Sprint 0 | 🟢 Concluída |
| 10 | US087 | Implementar agregados com Aggregate Root (DDD) | Sprint 0 | � Concluída |
| 11 | US088 | Criar Domain Services (DDD) | Sprint 0 | 🟢 Concluída |
| 12 | US089 | Implementar Domain Events (DDD) | Sprint 0 | � Concluída |
| 13 | US081 | Implementar estrutura de Commands (CQRS) | Sprint 0 | � Concluída |
| 14 | US082 | Implementar estrutura de Queries (CQRS) | Sprint 0 | 🟢 Concluída |
| 15 | US083 | Integrar MediatR para CQRS | Sprint 0 | � Concluída |
| 16 | US084 | Criar validadores para Commands | Sprint 0 | � Concluída |
| 17 | US066 | Implementar handlers CQRS para Projects | Sprint 0 | � Concluída |
| 18 | US067 | Implementar handlers CQRS para Characters | Sprint 0 | � Concluída |
| 19 | US068 | Implementar handlers CQRS para Locations | Sprint 0 | 🟢 Concluída |
| 20 | US069 | Implementar handlers CQRS para Plots | Sprint 0 | 🟢 Concluída |
| 21 | US070 | Implementar handlers CQRS para Chapters | Sprint 0 | � Concluída |
| 22 | US090 | Implementar handlers CQRS para PlotPoints | Sprint 0 | 🟢 Concluída |
| 23 | US071 | Implementar SignalR Hub streaming LLM | Sprint 0 | 🟢 Concluída |
| 24 | US072 | Conectar backend ao Ollama | Sprint 0 | � Concluída |
| 25 | US073 | Implementar streaming respostas LLM | Sprint 0 | � Concluída |
| 26 | US074 | Tratar erros de comunicação Ollama | Sprint 0 | � Concluída |
| 27 | US075 | Configurar modelo LLM via appsettings | Sprint 0 | � Concluída |
| 28 | US076 | Criar aplicação React com TypeScript | Sprint 0 | � Concluída |
| 29 | US077 | Configurar roteamento | Sprint 0 | � Concluída |
| 30 | US078 | Criar client HTTP para API | Sprint 0 | 🟢 Concluída |
| 31 | US079 | Implementar layout base | Sprint 0 | � Concluída |
| 32 | US080 | Configurar SignalR client | Sprint 0 | 🟢 Concluída |
| 33 | US091 | Criar novo projeto manualmente | Sprint 1 | � Concluída |
| 34 | US092 | Visualizar lista de projetos | Sprint 1 | 🟢 Concluída |
| 35 | US093 | Editar informações do projeto | Sprint 1 | � Concluída |
| 36 | US094 | Deletar projeto com confirmação | Sprint 1 | � Concluída |
| 37 | US006 | Criar novo personagem | Sprint 1 | 🟢 Concluída |
| 38 | US007 | Visualizar lista de personagens | Sprint 1 | 🟢 Concluída |
| 39 | US008 | Editar personagem existente | Sprint 1 | 🟢 Concluída |
| 40 | US009 | Deletar personagem com confirmação | Sprint 1 | 🟢 Concluída |
| 41 | US010 | Criar novo local | Sprint 1 | � Concluída |
| 42 | US011 | Visualizar lista de locais | Sprint 1 | 🟢 Concluída |
| 43 | US012 | Editar local existente | Sprint 1 | 🟢 Concluída |
| 44 | US013 | Deletar local | Sprint 1 | 🟢 Concluída |
| 45 | US014 | Criar novo plot | Sprint 1 | � Concluída |
| 46 | US015 | Visualizar lista de plots | Sprint 1 | 🟢 Concluída |
| 47 | US016 | Editar plot existente | Sprint 1 | 🟢 Concluída |
| 48 | US017 | Deletar plot com warning | Sprint 1 | 🟢 Concluída |
| 49 | US018 | Criar novo capítulo | Sprint 1 | � Concluída |
| 50 | US019 | Visualizar lista de capítulos | Sprint 1 | 🟢 Concluída |
| 51 | US020 | Reordenar capítulos | Sprint 1 | 🔴 Pendente |
| 52 | US021 | Editar título e resumo de capítulo | Sprint 1 | 🔴 Pendente |
| 53 | US022 | Deletar capítulo | Sprint 1 | 🔴 Pendente |
| 54 | US001 | Descrever ideia de livro para LLM | Sprint 1 | 🔴 Pendente |
| 55 | US002 | Receber perguntas da LLM para expandir ideia | Sprint 1 | 🔴 Pendente |
| 56 | US003 | Receber outline estruturado gerado pela LLM | Sprint 1 | 🔴 Pendente |
| 57 | US004 | Revisar e editar outline gerado | Sprint 1 | 🔴 Pendente |
| 58 | US005 | Salvar projeto com estrutura inicial | Sprint 1 | 🔴 Pendente |
| 59 | US023 | Ver gráfico de arcos narrativos | Sprint 2 | 🔴 Pendente |
| 60 | US024 | Filtrar timeline por plot | Sprint 2 | 🔴 Pendente |
| 61 | US025 | Clicar em ponto e ir para capítulo | Sprint 2 | 🔴 Pendente |
| 62 | US026 | Marcar ponto de intensidade em capítulo | Sprint 2 | 🔴 Pendente |
| 63 | US027 | Editar intensidade de ponto | Sprint 2 | 🔴 Pendente |
| 64 | US028 | Remover ponto de plot | Sprint 2 | 🔴 Pendente |
| 65 | US029 | Escrever texto no editor | Sprint 2 | 🔴 Pendente |
| 66 | US030 | Formatar texto (negrito, itálico) | Sprint 2 | 🔴 Pendente |
| 67 | US031 | Ver contador de palavras em tempo real | Sprint 2 | 🔴 Pendente |
| 68 | US032 | Autosave automático | Sprint 2 | 🔴 Pendente |
| 69 | US033 | Navegar entre capítulos | Sprint 2 | 🔴 Pendente |
| 70 | US034 | Sistema salvar antes de trocar capítulo | Sprint 2 | 🔴 Pendente |
| 71 | US035 | Sistema salvar automaticamente | Sprint 2 | 🔴 Pendente |
| 72 | US036 | Ver indicador de status de salvamento | Sprint 2 | 🔴 Pendente |
| 73 | US047 | Sistema identificar personagens relevantes | Sprint 3 | 🔴 Pendente |
| 74 | US048 | Sistema buscar plots ativos | Sprint 3 | 🔴 Pendente |
| 75 | US049 | Sistema montar prompt contextualizado | Sprint 3 | 🔴 Pendente |
| 76 | US050 | Sistema gerar embeddings de entidades | Sprint 3 | 🔴 Pendente |
| 77 | US051 | Sistema buscar entidades por similaridade | Sprint 3 | 🔴 Pendente |
| 78 | US052 | Sistema atualizar embeddings ao editar | Sprint 3 | 🔴 Pendente |
| 79 | US037 | Selecionar texto e pedir reescrita | Sprint 3 | 🔴 Pendente |
| 80 | US038 | Ver resposta LLM em streaming | Sprint 3 | 🔴 Pendente |
| 81 | US039 | Aceitar sugestão da LLM | Sprint 3 | 🔴 Pendente |
| 82 | US040 | Rejeitar sugestão da LLM | Sprint 3 | 🔴 Pendente |
| 83 | US041 | Ajustar tom do texto selecionado | Sprint 3 | 🔴 Pendente |
| 84 | US042 | Digitar comando customizado | Sprint 3 | 🔴 Pendente |
| 85 | US043 | LLM manter coerência com contexto | Sprint 3 | 🔴 Pendente |
| 86 | US044 | Expandir trecho adicionando detalhes | Sprint 3 | 🔴 Pendente |
| 87 | US045 | Resumir trecho mantendo essência | Sprint 3 | 🔴 Pendente |
| 88 | US046 | Controlar nível de expansão/resumo | Sprint 3 | 🔴 Pendente |
| 89 | US053 | Exportar livro para PDF | Sprint 4 | 🔴 Pendente |
| 90 | US054 | Escolher local de salvamento PDF | Sprint 4 | 🔴 Pendente |
| 91 | US055 | PDF com formatação profissional | Sprint 4 | 🔴 Pendente |
| 92 | US056 | Sumário clicável no PDF | Sprint 4 | 🔴 Pendente |
| 93 | US057 | Visualizar preview do livro | Sprint 4 | 🔴 Pendente |
| 94 | US058 | Navegar páginas do preview | Sprint 4 | 🔴 Pendente |

**Legenda de Status:**
- 🔴 Pendente - Aguardando implementação
- 🟡 Em Progresso - Sendo desenvolvida
- 🟢 Concluída - Implementada e testada

**Próxima Story:** US018 - Criar novo capítulo

---

## Fase 0: Fundacional (Pré-requisito para MVP)

**Objetivo:** Estabelecer infraestrutura técnica mínima para suportar o MVP.

**Duração estimada:** Sprint inicial de setup

**Critério de Conclusão:** Backend, frontend, banco e LLM funcionais e integrados.

---

### Epic E004: Infraestrutura e Persistência

#### Feature F009: Setup de Banco de Dados
- ✅ UC018: Configurar Supabase Local
  - US059: Subir Supabase local via Docker
  - US060: Acessar Supabase Studio localmente
  - US061: Confirmar pgvector habilitado

- ✅ UC019: Criar Schemas e Migrations
  - US062: Criar migrations para tabelas
  - US063: Executar migrations no banco
  - US064: Verificar integridade dos schemas
  - US065: Criar índices e constraints

#### Feature F010: API Backend (.NET)
- ✅ UC024: Implementar Domain Entities (DDD)
  - US085: Criar entidades de domínio ricas
  - US086: Criar Value Objects  
  - US087: Implementar agregados com Aggregate Root
  - US088: Criar Domain Services
  - US089: Implementar Domain Events

- ✅ UC023: Implementar CQRS Pattern
  - US081: Implementar estrutura de Commands
  - US082: Implementar estrutura de Queries
  - US083: Integrar MediatR para CQRS
  - US084: Criar validadores para Commands

- ✅ UC020: Implementar Application Layer com CQRS
  - US066: Implementar handlers CQRS para Projects
  - US067: Implementar handlers CQRS para Characters
  - US068: Implementar handlers CQRS para Locations
  - US069: Implementar handlers CQRS para Plots
  - US070: Implementar handlers CQRS para Chapters
  - US090: Implementar handlers CQRS para PlotPoints
  - ✅ US071: Implementar SignalR Hub streaming LLM

#### Feature F011: Integração LLM Local (Ollama)

**⚠️ STACK TÉCNICA OBRIGATÓRIA:**
- **Microsoft.Extensions.AI** (v10.0)
- **Semantic Kernel** (v1.x) 
- **Microsoft Agents Framework**
- **NÃO usar HttpClient direto** - usar abstrações do Semantic Kernel

- ✅ UC021: Integrar Ollama com Backend
  - ✅ US072: Conectar backend ao Ollama
  - ✅ US073: Implementar streaming respostas LLM
  - ✅ US074: Tratar erros de comunicação Ollama
  - ✅ US075: Configurar modelo LLM via appsettings

#### Feature F012: Frontend Base (React)
- ✅ UC022: Implementar Frontend Base
  - US076: Criar aplicação React com TypeScript
  - US077: Configurar roteamento
  - US078: Criar client HTTP para API
  - US079: Implementar layout base
  - US080: Configurar SignalR client

**Total Fase 0:**
- 4 Features
- 8 Use Cases (6 originais + 2 DDD/CQRS)
- 32 User Stories (22 originais + 10 DDD/CQRS)

---

## Fase 1: MVP (Funcionalidade Core)

**Objetivo:** Entregar experiência completa: estruturar livro → escrever com LLM → exportar PDF.

**Duração estimada:** 3-4 sprints

**Critério de Conclusão:** Autor consegue criar livro do zero, escrever com assistência LLM e exportar PDF profissional.

---

### Epic E001: Estruturação Narrativa Base

#### Feature F013: Gestão de Projetos
**Prioridade:** CRÍTICA (pré-requisito para todas outras features)

- ✅ UC025: Gerenciar Projetos
  - US091: Criar novo projeto manualmente
  - US092: Visualizar lista de projetos
  - US093: Editar informações do projeto
  - US094: Deletar projeto com confirmação

**Valor:** Sem projeto criado, nenhuma outra funcionalidade pode ser usada. É o ponto de partida obrigatório.

**Nota:** Este UC representa criação manual de projeto. UC001 (brainstorming com LLM) é uma forma alternativa/avançada de criar projeto.

---

#### Feature F001: Brainstorming Inicial com LLM
**Prioridade:** ALTA (diferencial competitivo)

- ✅ UC001: Gerar Outline Inicial com Assistência LLM
  - US001: Descrever ideia de livro para LLM
  - US002: Receber perguntas da LLM para expandir ideia
  - US003: Receber outline estruturado gerado pela LLM
  - US004: Revisar e editar outline gerado
  - US005: Salvar projeto com estrutura inicial

**Valor:** Acelera processo de estruturação, torna experiência única.

---

#### Feature F002: Gestão de Entidades Narrativas
**Prioridade:** ALTA (essencial para contexto LLM)

- ✅ UC002: Gerenciar Personagens
  - US006: Criar novo personagem
  - US007: Visualizar lista de personagens
  - US008: Editar personagem existente
  - US009: Deletar personagem com confirmação

- ✅ UC003: Gerenciar Locais
  - US010: Criar novo local
  - US011: Visualizar lista de locais
  - US012: Editar local existente
  - US013: Deletar local

- ✅ UC004: Gerenciar Plots
  - US014: Criar novo plot
  - US015: Visualizar lista de plots
  - US016: Editar plot existente
  - US017: Deletar plot com warning

- ✅ UC005: Gerenciar Capítulos
  - US018: Criar novo capítulo
  - US019: Visualizar lista de capítulos
  - US020: Reordenar capítulos
  - US021: Editar título e resumo de capítulo
  - US022: Deletar capítulo

**Valor:** Base de dados para assistência contextualizada.

---

#### Feature F003: Visualização de Arcos Narrativos
**Prioridade:** MÉDIA (nice-to-have no MVP, mas entrega valor visual)

- ✅ UC006: Visualizar Timeline de Arcos
  - US023: Ver gráfico de arcos narrativos
  - US024: Filtrar timeline por plot
  - US025: Clicar em ponto e ir para capítulo

- ✅ UC007: Marcar Pontos-Chave em Arcos
  - US026: Marcar ponto de intensidade em capítulo
  - US027: Editar intensidade de ponto
  - US028: Remover ponto de plot

**Valor:** Visão macro da estrutura, identifica desequilíbrios narrativos.

---

### Epic E002: Editor de Texto Assistido

#### Feature F004: Editor de Capítulos
**Prioridade:** ALTA (core da experiência)

- ✅ UC008: Escrever Conteúdo de Capítulo
  - US029: Escrever texto no editor
  - US030: Formatar texto (negrito, itálico)
  - US031: Ver contador de palavras em tempo real
  - US032: Autosave automático

- ✅ UC009: Navegar entre Capítulos
  - US033: Navegar entre capítulos
  - US034: Sistema salvar antes de trocar capítulo

- ✅ UC010: Autosave de Conteúdo
  - US035: Sistema salvar automaticamente
  - US036: Ver indicador de status de salvamento

**Valor:** Experiência de escrita fluida e segura.

---

#### Feature F006: Gerenciamento de Contexto Automático
**Prioridade:** ALTA (pré-requisito para F005)

- ✅ UC014: Construir Contexto para Prompt LLM
  - US047: Sistema identificar personagens relevantes
  - US048: Sistema buscar plots ativos
  - US049: Sistema montar prompt contextualizado

- ✅ UC015: Busca Semântica de Entidades Relevantes
  - US050: Sistema gerar embeddings de entidades
  - US051: Sistema buscar entidades por similaridade
  - US052: Sistema atualizar embeddings ao editar

**Valor:** LLM "lembra" de tudo automaticamente.

**Nota:** Implementar antes de F005 para garantir contexto disponível.

---

#### Feature F005: Comandos LLM Contextuais
**Prioridade:** ALTA (diferencial competitivo central)

- ✅ UC011: Reescrever Trecho com LLM
  - US037: Selecionar texto e pedir reescrita
  - US038: Ver resposta LLM em streaming
  - US039: Aceitar sugestão da LLM
  - US040: Rejeitar sugestão da LLM

- ✅ UC012: Ajustar Tom/Estilo com LLM
  - US041: Ajustar tom do texto selecionado
  - US042: Digitar comando customizado
  - US043: LLM manter coerência com contexto

- ✅ UC013: Expandir ou Resumir Texto com LLM
  - US044: Expandir trecho adicionando detalhes
  - US045: Resumir trecho mantendo essência
  - US046: Controlar nível de expansão/resumo

**Valor:** Co-criação inteligente, assistência sob demanda.

---

### Epic E003: Geração de Produto Final

#### Feature F007: Geração de PDF
**Prioridade:** ALTA (produto tangível final)

- ✅ UC016: Exportar Livro para PDF
  - US053: Exportar livro para PDF
  - US054: Escolher local de salvamento PDF
  - US055: PDF com formatação profissional
  - US056: Sumário clicável no PDF

**Valor:** Transforma trabalho em artefato real e compartilhável.

---

#### Feature F008: Preview do Livro
**Prioridade:** MÉDIA (validação antes de exportar)

- ✅ UC017: Visualizar Preview do Livro
  - US057: Visualizar preview do livro
  - US058: Navegar páginas do preview

**Valor:** Confiança na formatação antes de gerar PDF.

---

**Total Fase 1 (MVP):**
- 9 Features (adicionada F013: Gestão de Projetos)
- 17 Use Cases (adicionado UC025: Gerenciar Projetos)
- 62 User Stories (adicionadas US091-094)

---

## Ordem de Implementação Sugerida

### Sprint 0: Fundacional
1. F009: Setup de Banco de Dados
2. F010: API Backend (.NET)
3. F011: Integração LLM Local (Ollama)
4. F012: Frontend Base (React)

**Checkpoint:** Stack completa funcional

---

### Sprint 1: Estruturação Base
1. **F013: Gestão de Projetos** (CRUD básico - OBRIGATÓRIO PRIMEIRO)
2. F002: Gestão de Entidades Narrativas (CRUD completo)
3. F001: Brainstorming Inicial com LLM (alternativa avançada)

**Checkpoint:** Autor consegue criar projeto (manual ou via LLM) e estruturar entidades

---

### Sprint 2: Visualização + Editor Base
1. F003: Visualização de Arcos Narrativos
2. F004: Editor de Capítulos

**Checkpoint:** Autor consegue ver estrutura e escrever texto

---

### Sprint 3: Assistência LLM
1. F006: Gerenciamento de Contexto Automático
2. F005: Comandos LLM Contextuais

**Checkpoint:** Autor consegue escrever com assistência inteligente

---

### Sprint 4: Produto Final
1. F007: Geração de PDF
2. F008: Preview do Livro

**Checkpoint:** MVP COMPLETO - autor consegue gerar livro em PDF

---

## Métricas de Sucesso do MVP

**Funcionalidade:**
- ✅ Criar projeto com outline via brainstorming LLM
- ✅ Cadastrar personagens, locais, plots, capítulos
- ✅ Visualizar timeline de arcos narrativos
- ✅ Escrever conteúdo de capítulos com editor
- ✅ Usar comandos LLM contextuais (reescrever, ajustar tom, expandir/resumir)
- ✅ Exportar livro completo em PDF profissional

**Performance:**
- Resposta LLM < 5 segundos para comandos simples
- Geração de PDF < 10 segundos
- Autosave sem lag perceptível

**Qualidade:**
- LLM mantém coerência com contexto do livro
- PDF com formatação legível e profissional
- Zero perda de dados (autosave + persistência confiável)

---

## Próximas Fases (Pós-MVP)

### Fase 2: Expansão Inicial
- Colaboração multi-dispositivo (sync entre desktop/mobile)
- Histórico de versões (undo/redo avançado, branches)
- Temas e preferências de formatação customizados

### Fase 3: Consolidação
- Análise automática de consistência (personagens, timelines)
- Sugestões proativas da LLM (não apenas reativas)
- Export para ePub, MOBI

### Fase 4: Evoluções Avançadas
- Integração com APIs de publicação (Amazon KDP, Lulu)
- Comunidade/sharing de projetos
- Templates de gêneros (ficção científica, fantasia, thriller)

---

## Validação de Consistência

### ✅ Rastreabilidade Completa
- 4 Epics → 13 Features → 25 Use Cases → 94 User Stories
- Todas as stories vinculadas a use cases
- Todos os use cases vinculados a features
- Todas as features vinculadas a epics

### ✅ Arquitetura DDD + CQRS Implementada
- Domain Layer: 5 stories (Entities, Value Objects, Aggregates, Domain Services, Events)
- Application Layer (CQRS): 4 stories (Commands, Queries, MediatR, Validators)
- Separação clara entre escrita e leitura
- Entidades de domínio ricas com comportamento encapsulado

### ✅ MVP Validado
- Fase 0 habilita infraestrutura (incluindo DDD + CQRS)
- Fase 1 entrega valor completo: estruturação + escrita assistida + PDF
- Nenhuma dependência futura quebrada

### ✅ Incrementalidade
- Cada feature é independente dentro de sua epic
- Dependências entre features explicitadas (ex: F006 antes de F005)
- DDD/CQRS implementados antes de features funcionais
- Cada sprint entrega checkpoint funcional

### ✅ Clareza
- Nenhuma story ambígua ou genérica
- Critérios de aceitação objetivos e testáveis
- Backlog pronto para execução com arquitetura robusta

---

## Conclusão

O planejamento está completo, consistente e acionável. O MVP está claramente definido e priorizado, com arquitetura sólida baseada em DDD + CQRS.

**Arquitetura:**
- Clean Architecture com Domain-Driven Design
- CQRS para separação de responsabilidades
- Entidades de domínio ricas
- Command/Query Handlers via MediatR
- Domain Events para comunicação entre agregados

**Próximo passo:** Iniciar Sprint 0 (Fundacional) com foco em DDD + CQRS.
