# Ideia Principal: Editor de Livros com Assistência LLM

**Estado:** 🟢 Definida (Ciclo 0)  
**Última atualização:** 2026-01-26  
**Usuário:** Pessoal (não comercial)

---

## A Ideia Clarificada

Criar um **editor de texto para autores** que queiram escrever livros com ajuda de **LLM local**, onde:
- O sistema auxilia na identificação de plots, sub-plots, arcos narrativos, personagens, diálogos, tons
- O autor mantém controle criativo, mas tem a LLM como assistente sob demanda
- O produto final é um **livro em PDF**

**Não é apenas um processador de texto. É um ambiente de autoria estruturada com IA integrada.**

---

## O Problema que Tentamos Resolver

**O problema é meu, e é real:**

Eu quero escrever ficção, mas:
- Sozinho, perco momentum e estrutura
- Com LLMs atuais (ChatGPT, Claude), a experiência é desconexa e efêmera
- Não existe ferramenta que trate a LLM como **parceiro criativo persistente**
- As conversas se perdem, o contexto se fragmenta, o trabalho não vira "produto"

**Lacuna real:**
- Preciso de uma ferramenta que mantenha contexto de longo prazo
- Que sintetize minhas ideias sem apagar minha voz
- Que transforme sessões de brainstorm em artefatos concretos
- Que me dê prazer no processo, não só no resultado

---

## Para Quem Isso Serve?

**Usuário primário:** EU.

**Secundário (se um dia alguém mais usar):**
- Pessoas que gostam de pensar em voz alta com LLMs
- Quem quer escrever ficção mas precisa de um parceiro de brainstorming
- Quem entende de lógica e quer organizar criatividade de forma estruturada

**Característica chave:**
- Não é sobre vender
- Não é sobre best-sellers
- É sobre **prazer de criar e ter algo real no final**

---

## O Fluxo de Trabalho Idealizado

### 1. **Da Ideia ao Esboço**
**Entrada:** "Gostaria de criar um livro sobre a descoberta de uma nova tecnologia capaz de..."

**O sistema ajuda:**
- Definir plots e sub-plots
- Criar personagens principais e secundários
- Estabelecer arcos narrativos
- Estruturar a história em capítulos
- Gerar base/outline de cada capítulo

### 2. **Da Estrutura à Escrita**
**Com a estrutura definida:**
- Autor escreve capítulo a capítulo
- LLM oferece sugestões contextuais
- Autor pode pedir ajustes: "reescreva esse parágrafo ajustando o arco X"
- Autor pode selecionar trecho e pedir refinamento

### 3. **Do Texto ao Produto**
**Ao final:**
- Sistema gera PDF do livro
- Formatação profissional
- Controle de versões e revisões

---

## Modelo de Controle: Meio-Termo

**Humano lidera, LLM assiste:**
- Autor escreve um parágrafo → pede para LLM ajustar tom
- Autor seleciona trecho → pede reescrita focada em arco narrativo X
- Autor define personagem → LLM sugere diálogos consistentes
- Autor decide sempre, LLM executa sob demanda

---

## Analogia Central

**"Scrivener + Grammarly + ChatGPT, mas integrados nativamente"**

Ou ainda:

**"Cursor.AI para livros, não para código"**

Onde:
- Scrivener = organização estrutural (capítulos, personagens, plots)
- Grammarly = assistência contextual no texto
- ChatGPT = capacidade generativa sob demanda
- Cursor.AI = modelo de interação (seleciona, pede ajuste, aprova)

---

## Os 3 Pilares do Sistema

### 1. Estruturação Narrativa - "Da Ideia ao Outline"
Sistema ajuda a transformar ideia bruta em estrutura concreta.

**Features:**
- Gerador de plots e sub-plots
- Criador de fichas de personagens
- Estruturador de arcos narrativos
- Organizador de capítulos

**Questões abertas:**
- Como balancear criatividade com estrutura?
- Templates de gêneros (ficção científica, fantasia, thriller)?

### 2. Escrita Assistida - "Do Outline ao Texto"
Editor de texto com LLM integrada que responde a comandos.

**Features:**
- Editor WYSIWYG ou Markdown
- Seleção de texto + comando LLM ("reescreva em tom noir")
- Sugestões contextuais baseadas em arcos definidos
- Validação de consistência (personagem/tom/timeline)

**Questões abertas:**
- Como manter contexto de todo o livro durante escrita?
- Como evitar que LLM "invente" coisas fora da estrutura?

### 3. Geração de Produto Final - "Do Texto ao PDF"
Sistema exporta livro formatado profissionalmente.

**Features:**
- Export para PDF com formatação de livro
- Controle de estilos (fonte, margens, capítulos)
- Versionamento e revisões
- Metadados (autor, título, sinopse)

**Questões abertas:**
- Usar biblioteca existente (Pandoc, LaTeX) ou custom?
- Permitir templates customizados?

---

## Stack Tecnológica Definida

### Backend: .NET 10
- **Framework:** ASP.NET Core
- **LLM Integration:** Microsoft Agents Framework
- **LLM:** Local (Ollama, LM Studio, ou similar)
- **Database:** PostgreSQL (estrutura) + Vector DB (embeddings)
- **File Storage:** Sistema de arquivos local

### Frontend: React/TypeScript
- **UI:** React com editor de texto (Lexical, TipTap ou Quill)
- **State:** Zustand ou Redux Toolkit
- **Styling:** TailwindCSS ou Chakra UI

### Infraestrutura:
- **LLM local:** Para privacidade e custo zero
- **Hosting:** Aplicação desktop (Electron/Tauri) ou self-hosted web

---

## Complexidade Técnica Real

**Moderada:**
- Backend .NET com Agents Framework
- Integração com LLM local
- Editor de texto rico no frontend

**Baixa:**
- CRUD de estruturas (personagens, capítulos)
- Export para PDF (usar bibliotecas prontas)

**Risco técnico:**
- Performance da LLM local (velocidade de geração)
- Context window (manter todo o livro em contexto)
- Qualidade das sugestões da LLM

---

## Próximos Passos

- [x] Definir qual LLM usar: **LLM local**
- [x] Definir stack backend: **.NET 10 + Microsoft Agents Framework**
- [x] Definir produto final: **PDF do livro**
- [x] Definir modelo de controle: **Meio-termo (autor lidera, LLM assiste)**
- [ ] Detalhar MVP e escopo
- [ ] Desenhar arquitetura técnica
- [ ] Definir padrões de código e estrutura
