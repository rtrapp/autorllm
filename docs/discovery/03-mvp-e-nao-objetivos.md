# MVP e Não-Objetivos: O Que Será (e Não Será) Feito

**Estado:** 🟢 Definido (Ciclo 1)  
**Última atualização:** 2026-01-26

---

## Filosofia do MVP

**"O menor sistema que já entrega prazer e produto concreto"**

Não é sobre features completas.  
É sobre **validar a experiência core**: escrever com LLM integrada e sair com algo real.

---

## O Que SERÁ Feito no MVP

### 1. **Estruturação Básica (Da Ideia ao Outline)**

✅ **Interface de brainstorming com LLM**
- Você descreve a ideia do livro
- LLM faz perguntas e ajuda a expandir
- Sistema gera outline inicial (capítulos)

✅ **Cadastro de entidades**
- Personagens (nome, descrição, papel)
- Locais (nome, descrição)
- Plot principal e sub-plots (resumo)

✅ **Estrutura de capítulos**
- Lista de capítulos com nome e resumo
- Ordem editável (drag & drop ou manual)

✅ **Visualização de arcos narrativos**
- Timeline visual mostrando progressão de arcos
- Visualização de plot principal e sub-plots ao longo dos capítulos
- Marcação de pontos-chave (início, clímax, resolução)

**Não precisa:**
- Templates de gêneros complexos
- Frameworks prescritivos (Jornada do Herói, Save the Cat)
- Análise automática de estrutura narrativa

---

### 2. **Escrita Assistida (Do Outline ao Texto)**

✅ **Editor de texto simples**
- Markdown ou WYSIWYG básico
- Um capítulo por vez
- Autosave

✅ **Comandos de LLM contextual**
- Selecionar texto + clicar "Reescrever" → LLM reescreve
- Selecionar texto + comando customizado → LLM executa
- Ex: "reescreva em tom mais sombrio", "adicione mais diálogo"

✅ **Contexto automático para LLM**
- LLM recebe: personagens, plot, capítulo atual, texto adjacente
- Usuário não precisa repetir contexto manualmente

**Não precisa:**
- Sugestões inline em tempo real (tipo Copilot)
- Validação de consistência automática
- Análise de tom ou estilo
- Múltiplas versões/branches de texto

---

### 3. **Geração do Produto Final (Do Texto ao PDF)**

✅ **Export para PDF**
- Formatação básica de livro (fonte legível, margens, quebras de capítulo)
- Capa simples com título e autor
- Sumário gerado automaticamente

✅ **Preview antes de exportar**
- Visualização de como ficará o PDF

**Não precisa:**
- Templates customizados de formatação
- Export para ePub, MOBI, HTML
- Editor WYSIWYG de layout
- Metadados avançados (ISBN, copyright, etc.)

---

### 4. **Persistência e Gestão**

✅ **Salvar projeto localmente**
- Banco de dados local (SQLite ou PostgreSQL local)
- Um projeto = um livro

✅ **CRUD básico**
- Criar, editar, deletar personagens/capítulos
- Reordenar capítulos

**Não precisa:**
- Versionamento Git-style
- Múltiplos projetos simultaneamente abertos
- Sincronização em cloud
- Colaboração multi-usuário

---

## O Que NÃO SERÁ Feito no MVP

### ❌ Features Complexas de Estruturação

- Templates de gêneros (ficção científica, fantasia, thriller)
- Frameworks narrativos prescritivos (Jornada do Herói, Save the Cat)
- Graph database complexo de tramas
- Análise automática de ritmo narrativo (beats por capítulo)
- Validação automática de estrutura narrativa

**Razão:** MVP terá visualização básica de arcos, mas não frameworks prescritivos ou análise automática.

---

### ❌ Validação e Linting Automático

- Detecção de erros de continuidade (personagem morto reaparece)
- Validação de estados (personagem em dois lugares ao mesmo tempo)
- Análise de tom consistente
- Detecção de sub-plots abandonadas

**Razão:** Requer NLP avançado e lógica complexa. Não é essencial para MVP.

---

### ❌ Colaboração e Compartilhamento

- Multi-usuário em tempo real
- Comentários e sugestões (tipo Google Docs)
- Compartilhar rascunho com beta readers
- Histórico de versões estilo Git

**Razão:** Produto pessoal, não colaborativo (por enquanto).

---

### ❌ Integração com Serviços Externos

- Publicação direta em Amazon KDP, Wattpad, etc.
- Integração com Grammarly ou ProWritingAid
- Backup em Google Drive/Dropbox
- API pública

**Razão:** Adiciona dependências externas. Foco em ferramenta standalone.

---

### ❌ Features Avançadas de LLM

- Múltiplas LLMs simultâneas (usar Claude E GPT)
- Fine-tuning de modelo específico para seu estilo
- Geração automática de capítulos inteiros sem input
- Chatbot de personagem (conversar com personagens)

**Razão:** Interessante, mas não core. MVP valida assistência básica primeiro.

---

### ❌ Interface Avançada

- Editor WYSIWYG complexo (tipo Word)
- Temas/skins customizáveis
- Atalhos de teclado customizados
- Mobile app

**Razão:** Foco em funcionalidade, não em polish de UI. Desktop web é suficiente.

---

## Critérios de Sucesso do MVP

### Como saber se o MVP funcionou?

✅ **Você conseguiu:**
1. Criar uma ideia de livro e gerar outline com ajuda da LLM
2. Cadastrar pelo menos 3 personagens e 1 plot
3. Escrever ao menos 1 capítulo completo (2-3k palavras) com assistência LLM
4. Usar comandos de reescrita pelo menos 5x
5. Exportar um PDF do rascunho

✅ **Você sentiu:**
1. Prazer no processo (não foi frustrante ou burocrático)
2. Que a LLM ajudou de verdade (não foi só "enfeite")
3. Vontade de continuar escrevendo

✅ **Você NÃO precisou:**
1. Copiar/colar contexto manualmente toda hora
2. Abrir ChatGPT separado para pedir ajuda
3. Usar 3 ferramentas diferentes para gerenciar o livro

---

## Roadmap Pós-MVP (Futuro)

Se o MVP validar o conceito, próximas evoluções:

### Fase 2: Estruturação Avançada
- Templates de gêneros
- Frameworks narrativos prescritivos (Jornada do Herói, Save the Cat)
- Graph database complexo de sub-plots
- Análise automática de estrutura

### Fase 3: Validação Inteligente
- Detecção de inconsistências
- Análise de tom e ritmo
- Sugestões proativas

### Fase 4: Colaboração
- Multi-usuário
- Versionamento Git-style
- Compartilhamento de rascunhos

### Fase 5: Publicação
- Export para ePub, MOBI
- Integração com plataformas (KDP, Wattpad)
- Templates profissionais de layout

---

## Escopo Visual (Diagrama Simplificado)

```
┌─────────────────────────────────────────────────┐
│                   MVP SCOPE                     │
├─────────────────────────────────────────────────┤
│                                                 │
│  1. IDEIA → OUTLINE                            │
│     [Chat LLM] → [Outline Generator]           │
│                                                 │
│  2. ENTIDADES + ARCOS                          │
│     [Personagens] [Locais] [Plots]             │
│     [Timeline Visual de Arcos] 📊              │
│                                                 │
│  3. ESCRITA                                     │
│     [Editor Markdown/WYSIWYG]                   │
│     [Comando: Reescrever] [Comando: Custom]    │
│                                                 │
│  4. EXPORT                                      │
│     [Gerar PDF] [Preview]                       │
│                                                 │
└─────────────────────────────────────────────────┘

        ❌ NÃO NO MVP:
        - Graph database complexo
        - Frameworks prescritivos
        - Validação automática
        - Colaboração
        - Multi-LLMs
        - Mobile
```

---

## Próximos Passos

- [ ] Detalhar arquitetura técnica (backend .NET + frontend React)
- [ ] Definir estrutura de dados (schemas de personagens, capítulos, etc.)
- [ ] Listar tecnologias específicas (editor de texto, PDF generator, vector DB)
- [ ] Estimar tempo de desenvolvimento (breakdown por feature)
