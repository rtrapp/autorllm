# Catálogo de Epics

**Última atualização:** 2026-01-27  
**Status:** 🟢 Definido

---

## Lista de Epics

| ID | Nome | Fase | Status | Features |
|---|---|---|---|---|
| [E001](#e001-estruturação-narrativa-base) | Estruturação Narrativa Base | MVP (Fase 1) | Planejado | 3 features |
| [E002](#e002-editor-de-texto-assistido) | Editor de Texto Assistido | MVP (Fase 1) | Planejado | 3 features |
| [E003](#e003-geração-de-produto-final) | Geração de Produto Final | MVP (Fase 1) | Planejado | 2 features |
| [E004](#e004-infraestrutura-e-persistência) | Infraestrutura e Persistência | Fase 0 (Fundacional) | Parcial | 4 features |

---

## Detalhamento das Epics

### E001: Estruturação Narrativa Base
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Objetivo:**  
Permitir ao autor estruturar seu livro definindo elementos narrativos fundamentais (personagens, locais, plots, capítulos) e visualizar a progressão dos arcos narrativos.

**Valor de Negócio:**  
Esta é a fundação da experiência do produto. Sem estruturação, o livro é apenas texto solto. Com estruturação, a LLM consegue oferecer assistência contextualizada e o autor tem visão clara da obra.

**Features Relacionadas:**
- F001: Brainstorming Inicial com LLM
- F002: Gestão de Entidades Narrativas
- F003: Visualização de Arcos Narrativos

**Critérios de Sucesso:**
- Autor consegue criar estrutura completa de um livro (personagens, plots, capítulos) em até 30 minutos
- LLM auxilia na expansão da ideia inicial
- Visualização clara dos arcos narrativos ao longo dos capítulos

---

### E002: Editor de Texto Assistido
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Objetivo:**  
Oferecer ao autor um editor de texto onde ele escreve os capítulos com assistência contextualizada da LLM, que conhece toda a estrutura do livro.

**Valor de Negócio:**  
Este é o diferencial competitivo central: assistência LLM que LEMBRA de tudo. Transforma escrita solitária em co-criação inteligente.

**Features Relacionadas:**
- F004: Editor de Capítulos
- F005: Comandos LLM Contextuais
- F006: Gerenciamento de Contexto Automático

**Critérios de Sucesso:**
- Autor consegue escrever texto com assistência LLM sem repetir contexto manualmente
- Tempo de resposta da LLM < 5 segundos para comandos simples
- Editor mantém histórico e autosave funcional

---

### E003: Geração de Produto Final
**Fase:** MVP (Fase 1)  
**Status:** Planejado

**Objetivo:**  
Transformar o trabalho do autor em um produto concreto: um livro em PDF com formatação profissional.

**Valor de Negócio:**  
Entrega tangível. Sem isso, o sistema seria apenas "mais uma ferramenta de rascunhos". Com PDF, o autor tem algo real, compartilhável, publicável.

**Features Relacionadas:**
- F007: Geração de PDF
- F008: Preview do Livro

**Critérios de Sucesso:**
- Geração de PDF completo do livro em < 10 segundos
- Formatação legível e profissional (fonte, margens, quebras de capítulo)
- Preview fiel ao PDF final

---

### E004: Infraestrutura e Persistência
**Fase:** Fase 0 (Fundacional)  
**Status:** Parcial

**Objetivo:**  
Estabelecer a base técnica que suporta todo o sistema: banco de dados, API, integração com LLM local, e interface web.

**Valor de Negócio:**  
Sem infraestrutura, nada funciona. Esta epic não entrega valor direto ao usuário final, mas é pré-requisito para todas as outras.

**Features Relacionadas:**
- F009: Setup de Banco de Dados (Concluída - UC018 Concluída, UC019 Concluída)
- F010: API Backend (.NET) (Concluída - UC023 Concluída, UC024 Concluída, UC020 Concluída)
- F011: Integração LLM Local (Ollama) (Concluída - UC021 Concluída)
- F012: Frontend Base (React) (Parcial)

**Critérios de Sucesso:**
- Banco de dados operacional com schemas completos
- API REST funcional com endpoints CRUD
- LLM local respondendo a requisições
- Frontend servindo e conectando ao backend

---

## Dependências entre Epics

```
E004 (Infraestrutura) → Base para todas as outras
  ↓
E001 (Estruturação) → Necessária antes de E002
  ↓
E002 (Editor Assistido) → Usa estrutura de E001
  ↓
E003 (Geração PDF) → Usa conteúdo criado em E002
```

---

## Roadmap Visual

```
Fase 0 (Fundacional)
├─ E004: Infraestrutura e Persistência

Fase 1 (MVP)
├─ E001: Estruturação Narrativa Base
├─ E002: Editor de Texto Assistido
└─ E003: Geração de Produto Final
```

**Estimativa MVP:** Fase 0 + Fase 1 = núcleo funcional completo
