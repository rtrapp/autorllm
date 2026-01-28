# Catálogo de Features

**Última atualização:** 2026-01-27  
**Status:** 🟢 Definido

---

## Lista de Features

| ID | Nome | Epic | Fase | Status | Use Cases |
|---|---|---|---|---|---|
| [F001](#f001-brainstorming-inicial-com-llm) | Brainstorming Inicial com LLM | E001 | MVP | Planejado | 1 |
| [F002](#f002-gestão-de-entidades-narrativas) | Gestão de Entidades Narrativas | E001 | MVP | Parcial | 4 |
| [F013](#f013-gestão-de-projetos) | Gestão de Projetos | E001 | MVP | Completo | 1 |
| [F003](#f003-visualização-de-arcos-narrativos) | Visualização de Arcos Narrativos | E001 | MVP | Planejado | 2 |
| [F004](#f004-editor-de-capítulos) | Editor de Capítulos | E002 | MVP | Planejado | 3 |
| [F005](#f005-comandos-llm-contextuais) | Comandos LLM Contextuais | E002 | MVP | Planejado | 3 |
| [F006](#f006-gerenciamento-de-contexto-automático) | Gerenciamento de Contexto Automático | E002 | MVP | Planejado | 2 |
| [F007](#f007-geração-de-pdf) | Geração de PDF | E003 | MVP | Planejado | 1 |
| [F008](#f008-preview-do-livro) | Preview do Livro | E003 | MVP | Planejado | 1 |
| [F009](#f009-setup-de-banco-de-dados) | Setup de Banco de Dados | E004 | Fase 0 | Completo | 2 |
| [F010](#f010-api-backend-net) | API Backend (.NET) | E004 | Fase 0 | Completo | 3 |
| [F011](#f011-integração-llm-local-ollama) | Integração LLM Local (Ollama) | E004 | Fase 0 | Completo | 1 |
| [F012](#f012-frontend-base-react) | Frontend Base (React) | E004 | Fase 0 | Completo | 1 |

---

## Epic E001: Estruturação Narrativa Base

### F013: Gestão de Projetos
**Epic:** E001  
**Fase:** MVP (Fase 1)  
**Status:** Completo

**Descrição:**  
CRUD básico para projetos de livros. Permite criar projeto manualmente (informando título, descrição, gênero), listar todos os projetos, editar informações e deletar projetos.

**Objetivo:**  
Fornecer ponto de entrada obrigatório - sem projeto criado, nenhuma outra funcionalidade (personagens, capítulos, etc) pode ser usada.

**Use Cases:**
- UC025: Gerenciar Projetos

**Dependências:**  
- F009 (Setup de Banco de Dados)
- F010 (API Backend)
- F012 (Frontend Base)

**Nota:** Esta é a feature CRÍTICA que deve ser implementada primeiro no Sprint 1, antes de F002 (entidades narrativas).

---

### F001: Brainstorming Inicial com LLM
**Epic:** E001  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Interface conversacional onde o autor descreve sua ideia e a LLM faz perguntas para expandir, sugerindo plots, personagens, estrutura de capítulos.

**Objetivo:**  
Transformar ideia bruta em outline estruturado rapidamente. Forma alternativa/avançada de criar projeto (comparado com F013).

**Use Cases:**
- UC001: Gerar Outline Inicial com Assistência LLM

**Dependências:**  
- F013 (Gestão de Projetos - ou integra criação manual)
- F011 (Integração LLM Local)
- F010 (API Backend)
- F012 (Frontend Base)

---

### F002: Gestão de Entidades Narrativas
**Epic:** E001  
**Fase:** MVP (Fase 1)  
**Status:** Parcial

**Descrição:**  
CRUD completo para personagens, locais, plots e capítulos. Interface simples com formulários.

**Objetivo:**  
Permitir que autor crie, edite, visualize e delete elementos narrativos essenciais.

**Use Cases:**
- UC002: Gerenciar Personagens ✅
- UC003: Gerenciar Locais ✅
- UC004: Gerenciar Plots ✅
- UC005: Gerenciar Capítulos 🔄

**Dependências:**  
- F009 (Setup de Banco de Dados)
- F010 (API Backend)
- F012 (Frontend Base)

---

### F003: Visualização de Arcos Narrativos
**Epic:** E001  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Gráfico interativo (timeline) mostrando progressão de plots ao longo dos capítulos, com intensidade e pontos-chave marcados.

**Objetivo:**  
Dar ao autor visão macro da estrutura narrativa, identificando desequilíbrios (ex: subplot desaparece).

**Use Cases:**
- UC006: Visualizar Timeline de Arcos
- UC007: Marcar Pontos-Chave em Arcos

**Dependências:**  
- F002 (Gestão de Entidades)
- F012 (Frontend Base)

---

## Epic E002: Editor de Texto Assistido

### F004: Editor de Capítulos
**Epic:** E002  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Editor de texto rico (Lexical) onde autor escreve conteúdo dos capítulos. Suporta Markdown, autosave, navegação entre capítulos.

**Objetivo:**  
Oferecer experiência de escrita fluida e confortável.

**Use Cases:**
- UC008: Escrever Conteúdo de Capítulo
- UC009: Navegar entre Capítulos
- UC010: Autosave de Conteúdo

**Dependências:**  
- F002 (Gestão de Entidades - capítulos existem)
- F010 (API Backend)
- F012 (Frontend Base)

---

### F005: Comandos LLM Contextuais
**Epic:** E002  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Comandos que autor pode invocar no editor: selecionar texto e pedir "reescrever", "ajustar tom", "expandir diálogo". LLM processa com contexto do livro.

**Objetivo:**  
Transformar LLM em assistente criativo on-demand durante escrita.

**Use Cases:**
- UC011: Reescrever Trecho com LLM
- UC012: Ajustar Tom/Estilo com LLM
- UC013: Expandir ou Resumir Texto com LLM

**Dependências:**  
- F004 (Editor de Capítulos)
- F006 (Gerenciamento de Contexto)
- F011 (Integração LLM)

---

### F006: Gerenciamento de Contexto Automático
**Epic:** E002  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Sistema que, ao invocar LLM, busca automaticamente personagens, plots, capítulos adjacentes relevantes e injeta no prompt sem intervenção do autor.

**Objetivo:**  
Fazer LLM "lembrar" de tudo sem que autor precise repetir contexto.

**Use Cases:**
- UC014: Construir Contexto para Prompt LLM
- UC015: Busca Semântica de Entidades Relevantes

**Dependências:**  
- F002 (Gestão de Entidades - dados existem)
- F009 (Setup de Banco com pgvector)
- F011 (Integração LLM)

---

## Epic E003: Geração de Produto Final

### F007: Geração de PDF
**Epic:** E003  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Funcionalidade que gera PDF formatado do livro completo: capa, sumário, capítulos em ordem, margens e tipografia profissionais.

**Objetivo:**  
Transformar texto em produto tangível e compartilhável.

**Use Cases:**
- UC016: Exportar Livro para PDF

**Dependências:**  
- F002 (Gestão de Entidades - capítulos e projeto existem)
- F010 (API Backend com QuestPDF)

---

### F008: Preview do Livro
**Epic:** E003  
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Descrição:**  
Visualização inline no frontend de como o livro ficará no PDF antes de exportar.

**Objetivo:**  
Dar confiança ao autor de que formatação está correta antes de gerar.

**Use Cases:**
- UC017: Visualizar Preview do Livro

**Dependências:**  
- F007 (Geração de PDF - mesma lógica)
- F012 (Frontend Base)

---

## Epic E004: Infraestrutura e Persistência

### F009: Setup de Banco de Dados
**Epic:** E004  
**Fase:** Fase 0 (Fundacional)  
**Status:** Parcial

**Descrição:**  
Configuração completa de Supabase local (Docker) com PostgreSQL + pgvector. Schemas de dados, migrations, seeds iniciais.

**Objetivo:**  
Estabelecer persistência de dados relacional e vetorial.

**Use Cases:**
- UC018: Configurar Supabase Local (Concluída)
- UC019: Criar Schemas e Migrations (Em Implementação)

**Dependências:**  
Nenhuma (é ponto de partida)

---

### F010: API Backend (.NET)
**Epic:** E004  
**Fase:** Fase 0 (Fundacional)  
**Status:** Em Implementação

**Descrição:**  
API REST em ASP.NET Core com endpoints CRUD para todas as entidades, SignalR hub para streaming LLM.

**Objetivo:**  
Camada de negócio e orquestração do sistema.

**Use Cases:**
- UC020: Implementar Application Layer com CQRS (Planejado)
- UC023: Implementar CQRS Pattern (Concluída)
- UC024: Implementar Domain Entities (DDD) (Concluída)

**Dependências:**  
- F009 (Setup de Banco - API consome dados)

---

### F011: Integração LLM Local (Ollama)
**Epic:** E004  
**Fase:** Fase 0 (Fundacional)  
**Status:** Planejado

**Descrição:**  
Integração com Ollama (LLM local) via API REST. Configuração de prompts, streaming de respostas, tratamento de erros.

**Objetivo:**  
Habilitar assistência inteligente com LLM local.

**Use Cases:**
- UC021: Integrar Ollama com Backend

**Dependências:**  
- F010 (API Backend - LLM é chamada via API)

---

### F012: Frontend Base (React)
**Epic:** E004  
**Fase:** Fase 0 (Fundacional)  
**Status:** Planejado

**Descrição:**  
Aplicação React com TypeScript, roteamento, estado global, integração com API backend. Layout base com navegação.

**Objetivo:**  
Interface web funcional pronta para receber features.

**Use Cases:**
- UC022: Implementar Frontend Base

**Dependências:**  
- F010 (API Backend - frontend consome API)

---

## Dependências entre Features

```
Fase 0:
F009 (Banco) → F010 (API) → F011 (LLM)
                      ↓
               F012 (Frontend)

Fase 1 (MVP):
F001 (Brainstorming) ← depende de F010, F011, F012
F002 (Entidades) ← depende de F009, F010, F012
F003 (Timeline) ← depende de F002, F012
F004 (Editor) ← depende de F002, F010, F012
F005 (Comandos LLM) ← depende de F004, F006, F011
F006 (Contexto) ← depende de F002, F009, F011
F007 (PDF) ← depende de F002, F010
F008 (Preview) ← depende de F007, F012
```

---

## Roadmap de Features por Fase

### Fase 0 (Fundacional)
1. F009: Setup de Banco de Dados
2. F010: API Backend (.NET)
3. F011: Integração LLM Local (Ollama)
4. F012: Frontend Base (React)

### Fase 1 (MVP)
**Estruturação:**
1. F001: Brainstorming Inicial com LLM
2. F002: Gestão de Entidades Narrativas
3. F003: Visualização de Arcos Narrativos

**Escrita:**
4. F004: Editor de Capítulos
5. F006: Gerenciamento de Contexto Automático
6. F005: Comandos LLM Contextuais

**Produto Final:**
7. F007: Geração de PDF
8. F008: Preview do Livro
