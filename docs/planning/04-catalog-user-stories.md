# Catálogo de User Stories

**Última atualização:** 2026-02-05  
**Status:** 🟢 Definido

---

## Lista de User Stories

| ID | História | Use Case | Status | Critérios de Aceitação |
|---|---|---|---|---|
| [US001](#us001) | Descrever ideia de livro para LLM | UC001 | Concluída | 3 critérios |
| [US002](#us002) | Receber perguntas da LLM para expandir ideia | UC001 | Concluída | 3 critérios |
| [US091](#us091) | Criar novo projeto manualmente | UC025 | Concluída | 5 critérios |
| [US092](#us092) | Visualizar lista de projetos | UC025 | Concluída | 5 critérios |
| [US093](#us093) | Editar informações do projeto | UC025 | Concluída | 5 critérios |
| [US094](#us094) | Deletar projeto com confirmação | UC025 | Concluída | 4 critérios |
| [US003](#us003) | Receber outline estruturado gerado pela LLM | UC001 | Planejado | 5 critérios |
| [US004](#us004) | Revisar e editar outline gerado | UC001 | Planejado | 3 critérios |
| [US005](#us005) | Salvar projeto com estrutura inicial | UC001 | Planejado | 4 critérios |
| [US006](#us006) | Criar novo personagem | UC002 | Concluída | 4 critérios |
| [US007](#us007) | Visualizar lista de personagens | UC002 | Concluída | 3 critérios |
| [US008](#us008) | Editar personagem existente | UC002 | Concluída | 3 critérios |
| [US009](#us009) | Deletar personagem com confirmação | UC002 | Concluída | 3 critérios |
| [US010](#us010) | Criar novo local | UC003 | Concluída | 3 critérios |
| [US011](#us011) | Visualizar lista de locais | UC003 | Concluída | 2 critérios |
| [US012](#us012) | Editar local existente | UC003 | Concluída | 3 critérios |
| [US013](#us013) | Deletar local | UC003 | Concluída | 2 critérios |
| [US014](#us014) | Criar novo plot | UC004 | Concluída | 4 critérios |
| [US015](#us015) | Visualizar lista de plots | UC004 | Concluída | 3 critérios |
| [US016](#us016) | Editar plot existente | UC004 | Concluída | 3 critérios |
| [US017](#us017) | Deletar plot com warning | UC004 | Concluída | 3 critérios |
| [US018](#us018) | Criar novo capítulo | UC005 | Concluída | 4 critérios |
| [US019](#us019) | Visualizar lista de capítulos | UC005 | Concluída | 3 critérios |
| [US020](#us020) | Reordenar capítulos | UC005 | Concluída | 3 critérios |
| [US021](#us021) | Editar título e resumo de capítulo | UC005 | Concluída | 3 critérios |
| [US022](#us022) | Deletar capítulo | UC005 | Concluída | 3 critérios |
| [US023](#us023) | Ver gráfico de arcos narrativos | UC006 | Planejado | 4 critérios |
| [US024](#us024) | Filtrar timeline por plot | UC006 | Planejado | 2 critérios |
| [US025](#us025) | Clicar em ponto e ir para capítulo | UC006 | Planejado | 2 critérios |
| [US026](#us026) | Marcar ponto de intensidade em capítulo | UC007 | Planejado | 4 critérios |
| [US027](#us027) | Editar intensidade de ponto | UC007 | Planejado | 3 critérios |
| [US028](#us028) | Remover ponto de plot | UC007 | Planejado | 2 critérios |
| [US029](#us029) | Escrever texto no editor | UC008 | Planejado | 3 critérios |
| [US030](#us030) | Formatar texto (negrito, itálico) | UC008 | Planejado | 3 critérios |
| [US031](#us031) | Ver contador de palavras em tempo real | UC008 | Planejado | 2 critérios |
| [US032](#us032) | Autosave automático | UC008 | Planejado | 3 critérios |
| [US033](#us033) | Navegar entre capítulos | UC009 | Planejado | 3 critérios |
| [US034](#us034) | Sistema salvar antes de trocar capítulo | UC009 | Planejado | 2 critérios |
| [US035](#us035) | Sistema salvar automaticamente | UC010 | Planejado | 3 critérios |
| [US036](#us036) | Ver indicador de status de salvamento | UC010 | Planejado | 3 critérios |
| [US037](#us037) | Selecionar texto e pedir reescrita | UC011 | Planejado | 4 critérios |
| [US038](#us038) | Ver resposta LLM em streaming | UC011 | Planejado | 3 critérios |
| [US039](#us039) | Aceitar sugestão da LLM | UC011 | Planejado | 2 critérios |
| [US040](#us040) | Rejeitar sugestão da LLM | UC011 | Planejado | 2 critérios |
| [US041](#us041) | Ajustar tom do texto selecionado | UC012 | Planejado | 3 critérios |
| [US042](#us042) | Digitar comando customizado | UC012 | Planejado | 3 critérios |
| [US043](#us043) | LLM manter coerência com contexto | UC012 | Planejado | 2 critérios |
| [US044](#us044) | Expandir trecho adicionando detalhes | UC013 | Planejado | 3 critérios |
| [US045](#us045) | Resumir trecho mantendo essência | UC013 | Planejado | 3 critérios |
| [US046](#us046) | Controlar nível de expansão/resumo | UC013 | Planejado | 2 critérios |
| [US047](#us047) | Sistema identificar personagens relevantes | UC014 | Planejado | 3 critérios |
| [US048](#us048) | Sistema buscar plots ativos | UC014 | Planejado | 3 critérios |
| [US049](#us049) | Sistema montar prompt contextualizado | UC014 | Planejado | 4 critérios |
| [US050](#us050) | Sistema gerar embeddings de entidades | UC015 | Planejado | 3 critérios |
| [US051](#us051) | Sistema buscar entidades por similaridade | UC015 | Planejado | 3 critérios |
| [US052](#us052) | Sistema atualizar embeddings ao editar | UC015 | Planejado | 2 critérios |
| [US053](#us053) | Exportar livro para PDF | UC016 | Planejado | 5 critérios |
| [US054](#us054) | Escolher local de salvamento PDF | UC016 | Planejado | 2 critérios |
| [US055](#us055) | PDF com formatação profissional | UC016 | Planejado | 4 critérios |
| [US056](#us056) | Sumário clicável no PDF | UC016 | Planejado | 2 critérios |
| [US057](#us057) | Visualizar preview do livro | UC017 | Planejado | 3 critérios |
| [US058](#us058) | Navegar páginas do preview | UC017 | Planejado | 2 critérios |
| [US059](#us059) | Subir Supabase local via Docker | UC018 | Concluída | 3 critérios |
| [US060](#us060) | Acessar Supabase Studio localmente | UC018 | Concluída | 2 critérios |
| [US061](#us061) | Confirmar pgvector habilitado | UC018 | Concluída | 2 critérios |
| [US062](#us062) | Criar migrations para tabelas | UC019 | Concluída | 4 critérios |
| [US063](#us063) | Executar migrations no banco | UC019 | Concluída | 2 critérios |
| [US064](#us064) | Verificar integridade dos schemas | UC019 | Concluída | 3 critérios |
| [US065](#us065) | Criar índices e constraints | UC019 | Concluída | 3 critérios |
| [US066](#us066) | Implementar handlers CQRS para Projects | UC020 | Concluída | 6 critérios |
| [US067](#us067) | Implementar handlers CQRS para Characters | UC020 | Concluída | 6 critérios |
| [US068](#us068) | Implementar handlers CQRS para Locations | UC020 | Concluída | 6 critérios |
| [US069](#us069) | Implementar handlers CQRS para Plots | UC020 | Concluída | 6 critérios |
| [US070](#us070) | Implementar handlers CQRS para Chapters | UC020 | Concluída | 6 critérios |
| [US071](#us071) | Implementar SignalR Hub streaming LLM | UC020 | Concluída | 4 critérios |
| [US072](#us072) | Conectar backend ao Ollama | UC021 | Concluída | 3 critérios |
| [US073](#us073) | Implementar streaming respostas LLM | UC021 | Concluída | 3 critérios |
| [US074](#us074) | Tratar erros de comunicação Ollama | UC021 | Concluída | 3 critérios |
| [US075](#us075) | Configurar modelo LLM via appsettings | UC021 | Concluída | 2 critérios |
| [US076](#us076) | Criar aplicação React com TypeScript | UC022 | Concluída | 4 critérios |
| [US077](#us077) | Configurar roteamento | UC022 | Concluída | 3 critérios |
| [US078](#us078) | Criar client HTTP para API | UC022 | Concluída | 3 critérios |
| [US079](#us079) | Implementar layout base | UC022 | Concluída | 4 critérios |
| [US080](#us080) | Configurar SignalR client | UC022 | Concluída | 3 critérios |
| [US081](#us081) | Implementar estrutura de Commands | UC023 | Concluída | 4 critérios |
| [US082](#us082) | Implementar estrutura de Queries | UC023 | Concluída | 4 critérios |
| [US083](#us083) | Integrar MediatR para CQRS | UC023 | Concluída | 3 critérios |
| [US084](#us084) | Criar validadores para Commands | UC023 | Concluída | 3 critérios |
| [US085](#us085) | Criar entidades de domínio ricas | UC024 | Concluída | 5 critérios |
| [US086](#us086) | Criar Value Objects | UC024 | Concluída | 4 critérios |
| [US087](#us087) | Implementar agregados com Aggregate Root | UC024 | Concluída | 4 critérios |
| [US088](#us088) | Criar Domain Services | UC024 | Concluída | 3 critérios |
| [US089](#us089) | Implementar Domain Events | UC024 | Concluída | 3 critérios |
| [US090](#us090) | Implementar handlers CQRS para PlotPoints | UC020 | Concluída | 6 critérios |

---

## Detalhamento das User Stories

### UC025: Gerenciar Projetos

#### US091
**Como** autor  
**Quero** criar novo projeto manualmente informando título e descrição básica  
**Para** começar a estruturar meu livro

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Tela inicial tem botão "Novo Projeto" visível (card "+" na grid + botão no header mobile)
2. ✅ Formulário tem campos: Título (obrigatório, max 200 chars), Descrição (opcional, textarea), Gênero (opcional, dropdown), Idioma (opcional)
3. ✅ Validação: título não pode ser vazio e não pode duplicar nome de projeto existente (validação assíncrona no backend)
4. ✅ Ao salvar, projeto é persistido no banco com `created_at` e `updated_at`
5. ✅ Após criação, usuário é redirecionado para tela do projeto (dashboard ou editor)

**Implementação:**
- Frontend: [ProjectsList.tsx](../../src/frontend/src/pages/ProjectsList.tsx) com layout baseado em protótipo
- Frontend: [NewProjectDialog.tsx](../../src/frontend/src/features/projects/components/NewProjectDialog.tsx)
- Backend: Validator atualizado com validação assíncrona de título duplicado
- Backend: Método `GetByTitleAsync` adicionado ao repository

---

#### US092
**Como** autor  
**Quero** visualizar lista de todos os meus projetos  
**Para** escolher em qual trabalhar

**Critérios de Aceitação:**
1. Tela inicial/dashboard mostra lista de projetos (cards ou tabela)
2. Cada projeto exibe: título, descrição resumida (primeiras 100 chars), data de última modificação
3. Lista é ordenada por última modificação (mais recente primeiro)
4. Clicar em projeto abre a tela de trabalho do projeto
5. Lista vazia mostra mensagem "Nenhum projeto ainda" com botão para criar

---

#### US093
**Como** autor  
**Quero** editar informações básicas de um projeto existente  
**Para** atualizar título ou descrição conforme evolui

**Critérios de Aceitação:**
1. Na tela do projeto, existe opção "Configurações" ou "Editar Projeto" (menu/ícone)
2. Abre mesmo formulário de criação, pré-preenchido
3. Validações são aplicadas (título obrigatório, sem duplicatas)
4. Ao salvar, `updated_at` é atualizado
5. Mudanças são persistidas imediatamente

---

#### US094
**Como** autor  
**Quero** deletar projeto com confirmação  
**Para** remover projetos abandonados ou testes

**Critérios de Aceitação:**
1. Na tela de configurações do projeto, existe botão "Deletar Projeto" (vermelho, destaque negativo)
2. Ao clicar, modal de confirmação aparece:
   - Se projeto tem conteúdo (capítulos, personagens): warning explícito "Todos os dados serão perdidos permanentemente"
   - Se projeto vazio: confirmação simples
3. Confirmação requer ação explícita (ex: digitar nome do projeto ou clicar "Confirmar Exclusão")
4. Ao confirmar, projeto e TODOS os dados relacionados (capítulos, personagens, plots, locations) são deletados em cascata
5. Usuário é redirecionado para lista de projetos com mensagem de sucesso

---

### UC001: Gerar Outline Inicial com Assistência LLM

#### US001
**Como** autor  
**Quero** descrever minha ideia de livro em texto livre para a LLM entender  
**Para** iniciar o processo de estruturação com assistência

**Status:** 🟢 Concluída (2026-01-28)

**Critérios de Aceitação:**
1. ✅ Interface tem campo de texto multi-linha para entrada da ideia (BrainstormChat.tsx)
2. ✅ Botão "Começar" inicia conversa com LLM (SignalR StartBrainstorm)
3. ✅ Ideia é enviada para LLM e resposta é exibida (streaming com tokens em tempo real)

**Implementação:**
- Backend: StartBrainstormCommand, StartBrainstormCommandHandler, LLMHub.StartBrainstorm
- Frontend: BrainstormChat component, useBrainstorm hook, BrainstormPage
- Rota: /brainstorm com link na página de projetos

---

#### US002
**Como** autor  
**Quero** que a LLM faça perguntas relevantes sobre minha ideia  
**Para** expandir e clarificar conceitos antes de gerar estrutura

**Critérios de Aceitação:**
1. ✅ LLM faz entre 3-5 perguntas focadas (gênero, protagonista, conflito, tom)
2. ✅ Interface exibe perguntas uma por vez ou em grupo
3. ✅ Autor pode responder e avançar

**Status:** Concluída (2026-02-05)

**Implementação Técnica:**
- Backend: Prompt atualizado em `BuildBrainstormPrompt()` para gerar EXATAMENTE 5 perguntas no formato `(Categoria) Pergunta`
- Tipos AG-UI: Novo tipo `QuestionListComponent` e interface `Question` em `ag-ui.ts`
- Parser: Função `parseQuestionsFromText()` detecta perguntas estruturadas automaticamente
- Componente: `QuestionList.tsx` renderiza perguntas com navegação interativa
- Renderer: `AgMessageRenderer` suporta renderização do novo componente `question-list`
- UX: Navegação entre perguntas, progress bar, atalho Ctrl+Enter, botões Anterior/Próxima/Concluir
- Fluxo: Ao concluir todas as perguntas, dispara action `submit_answers` que envia respostas via `ContinueBrainstorm`

---

#### US003
**Como** autor  
**Quero** que a LLM gere outline estruturado baseado na conversa  
**Para** ter ponto de partida sólido

**Critérios de Aceitação:**
1. Outline inclui: título sugerido, sinopse (200-500 palavras)
2. Lista de 5-12 capítulos com título e resumo
3. Pelo menos 3 personagens principais com descrição breve
4. Plot principal definido
5. Outline é exibido de forma clara e editável

---

#### US004
**Como** autor  
**Quero** revisar e editar o outline gerado antes de salvar  
**Para** ajustar conforme minha visão

**Critérios de Aceitação:**
1. Todos os campos do outline são editáveis inline
2. Posso adicionar/remover capítulos e personagens
3. Mudanças são validadas (ex: mínimo de 3 capítulos)

---

#### US005
**Como** autor  
**Quero** salvar o projeto com a estrutura inicial criada  
**Para** começar a desenvolver o livro

**Critérios de Aceitação:**
1. Botão "Salvar Projeto" persiste dados no banco
2. Projeto aparece na lista de projetos
3. Todos os elementos (personagens, capítulos, plots) são salvos corretamente
4. Redirecionamento para tela principal do projeto

---

### UC002: Gerenciar Personagens

#### US006
**Como** autor  
**Quero** criar novo personagem informando nome, papel e descrição  
**Para** definir elenco do livro

**Critérios de Aceitação:**
1. Formulário com campos: Nome (obrigatório), Papel (dropdown: Protagonista/Antagonista/Suporte/Menor), Descrição (textarea), Traits (JSON ou campos estruturados)
2. Validação: nome não pode estar vazio
3. Ao salvar, personagem é criado no banco
4. Personagem aparece na lista imediatamente

---

#### US007
**Como** autor  
**Quero** visualizar lista de todos os personagens do projeto  
**Para** ter visão geral do elenco

**Critérios de Aceitação:**
1. Lista mostra nome, papel e miniatura da descrição
2. Lista é ordenável (alfabética, por papel)
3. Clicar em personagem abre detalhes para edição

---

#### US008
**Como** autor  
**Quero** editar personagem existente  
**Para** atualizar informações conforme história evolui

**Critérios de Aceitação:**
1. Ao clicar em personagem, formulário de edição abre
2. Campos pré-preenchidos com dados atuais
3. Ao salvar, alterações são persistidas

---

#### US009
**Como** autor  
**Quero** deletar personagem com confirmação  
**Para** remover elementos não utilizados

**Critérios de Aceitação:**
1. Botão "Deletar" abre modal de confirmação
2. Se personagem está referenciado em capítulos, warning é exibido
3. Ao confirmar, personagem é removido do banco

---

### UC003: Gerenciar Locais

#### US010
**Como** autor  
**Quero** criar novo local informando nome e descrição  
**Para** definir cenários do livro

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Formulário com Nome (obrigatório) e Descrição (opcional)
2. ✅ Validação: nome não vazio
3. ✅ Local salvo aparece na lista

---

#### US011
**Como** autor  
**Quero** visualizar lista de locais  
**Para** ver todos os cenários definidos

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Lista mostra nome e miniatura da descrição
2. ✅ Lista ordenável alfabeticamente

---

#### US012
**Como** autor  
**Quero** editar local existente  
**Para** atualizar informações

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Ao clicar, formulário abre com dados atuais
2. ✅ Alterações são salvas ao submeter
3. ✅ Lista atualiza imediatamente

---

#### US013
**Como** autor  
**Quero** deletar local  
**Para** remover cenários não utilizados

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Botão deletar com confirmação
2. ✅ Local removido do banco

---

### UC004: Gerenciar Plots

#### US014
**Como** autor  
**Quero** criar novo plot definindo nome, tipo e descrição  
**Para** estruturar arcos narrativos

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Formulário: Título (obrigatório), Tipo (dropdown: Principal/Subplot), Descrição (textarea)
2. ✅ Plot salvo aparece na lista
3. ✅ Plot é vinculado ao projeto atual
4. ✅ Backend com PlotsController e handlers CQRS

---

#### US015
**Como** autor  
**Quero** visualizar lista de plots  
**Para** ver todos os arcos definidos

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Lista mostra título, tipo e miniatura da descrição
2. ✅ Plot principal destacado visualmente (negrito + label)
3. ✅ Lista ordenável (Main plots primeiro, depois alfabética)

---

#### US016
**Como** autor  
**Quero** editar plot existente  
**Para** ajustar arco conforme necessário

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Ao clicar, formulário abre com dados atuais
2. ✅ Alterações salvas ao submeter
3. ✅ Lista atualiza imediatamente

---

#### US017
**Como** autor  
**Quero** deletar plot com warning se houver pontos marcados  
**Para** evitar perda acidental de dados

**Status:** 🟢 Concluída

**Critérios de Aceitação:**
1. ✅ Botão deletar com confirmação
2. ✅ Plot removido do banco
3. ✅ Dialog de confirmação com mensagem clara

---

### UC005: Gerenciar Capítulos

#### US018
**Como** autor  
**Quero** criar novo capítulo com título e resumo  
**Para** estruturar o livro

**Critérios de Aceitação:**
1. Formulário: Título (obrigatório), Resumo (opcional)
2. Capítulo criado com ordem sequencial (próximo número disponível)
3. Conteúdo vazio inicialmente
4. Capítulo aparece na lista

---

#### US019
**Como** autor  
**Quero** visualizar lista de capítulos em ordem  
**Para** navegar pela estrutura

**Critérios de Aceitação:**
1. Lista mostra ordem, título, word count
2. Capítulos ordenados por campo Order
3. Indicador visual de capítulos com/sem conteúdo

---

#### US020
**Como** autor  
**Quero** reordenar capítulos  
**Para** reorganizar estrutura

**Critérios de Aceitação:**
1. Interface drag-and-drop ou botões up/down
2. Ao mover capítulo, campo Order é atualizado
3. Lista reflete mudança imediatamente

---

#### US021
**Como** autor  
**Quero** editar título e resumo de capítulo  
**Para** ajustar informações

**Critérios de Aceitação:**
1. Formulário de edição abre com dados atuais
2. Alterações salvas
3. Lista atualiza título

---

#### US022
**Como** autor  
**Quero** deletar capítulo  
**Para** remover capítulos não utilizados

**Critérios de Aceitação:**
1. Se capítulo tem conteúdo, warning é exibido
2. Confirmação obrigatória
3. Ao deletar, ordens dos capítulos seguintes são ajustadas

---

### UC006: Visualizar Timeline de Arcos

#### US023
**Como** autor  
**Quero** ver gráfico visual dos arcos narrativos ao longo dos capítulos  
**Para** entender progressão da história

**Critérios de Aceitação:**
1. Gráfico tipo linha com eixo X = capítulos, eixo Y = intensidade (0-10)
2. Cada plot é uma linha com cor distinta
3. Pontos marcados aparecem no gráfico
4. Legenda identifica cada plot

---

#### US024
**Como** autor  
**Quero** filtrar timeline por plot específico  
**Para** focar em arco individual

**Critérios de Aceitação:**
1. Checkbox ou dropdown para selecionar plots visíveis
2. Gráfico atualiza mostrando apenas plots selecionados

---

#### US025
**Como** autor  
**Quero** clicar em ponto do gráfico e ir para capítulo correspondente  
**Para** navegar rapidamente

**Critérios de Aceitação:**
1. Ao clicar em ponto, sistema abre capítulo no editor
2. Scroll automático para início do capítulo

---

### UC007: Marcar Pontos-Chave em Arcos

#### US026
**Como** autor  
**Quero** marcar ponto de intensidade de plot em capítulo específico  
**Para** registrar progressão do arco

**Critérios de Aceitação:**
1. Interface permite selecionar plot e capítulo
2. Slider ou input numérico para intensidade (0-10)
3. Campo opcional para descrição do ponto (ex: "Clímax do conflito X")
4. Ao salvar, ponto aparece na timeline

---

#### US027
**Como** autor  
**Quero** editar intensidade de ponto existente  
**Para** ajustar progressão

**Critérios de Aceitação:**
1. Ao clicar em ponto na timeline ou lista, formulário abre
2. Intensidade e descrição editáveis
3. Alterações salvas e gráfico atualiza

---

#### US028
**Como** autor  
**Quero** remover ponto de plot  
**Para** corrigir marcação incorreta

**Critérios de Aceitação:**
1. Botão deletar em ponto
2. Ponto removido do banco e gráfico atualiza

---

### UC008: Escrever Conteúdo de Capítulo

#### US029
**Como** autor  
**Quero** escrever texto no editor de capítulo  
**Para** criar conteúdo do livro

**Critérios de Aceitação:**
1. Editor Lexical carrega vazio ou com conteúdo existente
2. Digitação fluida, sem lag
3. Conteúdo persiste ao trocar de capítulo ou fechar aplicação

---

#### US030
**Como** autor  
**Quero** formatar texto (negrito, itálico, listas)  
**Para** dar estrutura ao conteúdo

**Critérios de Aceitação:**
1. Barra de ferramentas com botões: negrito, itálico, sublinhado, listas
2. Atalhos de teclado funcionais (Cmd+B, Cmd+I)
3. Formatação renderizada corretamente no editor e no PDF

---

#### US031
**Como** autor  
**Quero** ver contador de palavras em tempo real  
**Para** acompanhar progresso

**Critérios de Aceitação:**
1. Contador exibido em UI (ex: rodapé do editor)
2. Atualiza a cada 1 segundo

---

#### US032
**Como** autor  
**Quero** que conteúdo seja salvo automaticamente  
**Para** evitar perda de trabalho

**Critérios de Aceitação:**
1. Autosave a cada 5 segundos de inatividade
2. Indicador "Salvando..." aparece durante salvamento
3. Indicador "Salvo às HH:MM" confirma sucesso

---

### UC009: Navegar entre Capítulos

#### US033
**Como** autor  
**Quero** navegar entre capítulos clicando na lista  
**Para** trabalhar em diferentes partes do livro

**Critérios de Aceitação:**
1. Lista lateral com todos os capítulos
2. Ao clicar, capítulo carrega no editor
3. Capítulo anterior é salvo antes de trocar

---

#### US034
**Como** autor  
**Quero** que sistema salve antes de trocar de capítulo  
**Para** não perder alterações

**Critérios de Aceitação:**
1. Ao clicar em outro capítulo, sistema salva atual primeiro
2. Indicador de salvamento aparece

---

### UC010: Autosave de Conteúdo

#### US035
**Como** autor  
**Quero** que sistema salve automaticamente meu trabalho  
**Para** nunca perder progresso

**Critérios de Aceitação:**
1. Autosave dispara após 5s de inatividade
2. Também salva ao trocar de capítulo ou fechar app
3. Se houver erro de rede, retry automático 3x

---

#### US036
**Como** autor  
**Quero** ver indicador de status de salvamento  
**Para** ter confiança de que trabalho está seguro

**Critérios de Aceitação:**
1. Estados: "Salvando...", "Salvo às HH:MM", "Erro ao salvar"
2. Se erro, botão "Tentar novamente" aparece
3. Indicador sempre visível (rodapé ou header)

---

### UC011: Reescrever Trecho com LLM

#### US037
**Como** autor  
**Quero** selecionar texto e pedir reescrita  
**Para** melhorar passagem com ajuda da LLM

**Critérios de Aceitação:**
1. Ao selecionar texto, botão "Reescrever" aparece (tooltip ou menu contextual)
2. Ao clicar, texto + contexto são enviados para LLM
3. Resposta LLM é exibida em painel lateral ou modal
4. Botões "Aceitar" e "Rejeitar" disponíveis

---

#### US038
**Como** autor  
**Quero** ver resposta da LLM em streaming (palavra por palavra)  
**Para** acompanhar geração em tempo real

**Critérios de Aceitação:**
1. Texto LLM aparece progressivamente, não de uma vez
2. Indicador de "Gerando..." enquanto streaming ativo
3. Se LLM para de responder, timeout de 60s

---

#### US039
**Como** autor  
**Quero** aceitar sugestão da LLM  
**Para** substituir texto original

**Critérios de Aceitação:**
1. Botão "Aceitar" substitui texto selecionado pela sugestão
2. Editor reflete mudança imediatamente

---

#### US040
**Como** autor  
**Quero** rejeitar sugestão da LLM  
**Para** manter texto original

**Critérios de Aceitação:**
1. Botão "Rejeitar" fecha painel sem alterar texto
2. Texto original permanece selecionado

---

### UC012: Ajustar Tom/Estilo com LLM

#### US041
**Como** autor  
**Quero** ajustar tom do texto selecionado (ex: mais sombrio, mais formal)  
**Para** alinhar com atmosfera desejada

**Critérios de Aceitação:**
1. Menu de comandos com opções: "Tom sombrio", "Tom leve", "Mais formal", "Mais casual"
2. Ao selecionar, comando é enviado para LLM com contexto
3. Resposta exibida em streaming

---

#### US042
**Como** autor  
**Quero** digitar comando customizado  
**Para** ter flexibilidade em ajustes

**Critérios de Aceitação:**
1. Campo "Comando customizado" disponível
2. Aceita texto livre (ex: "reescreva focando no arco do personagem X")
3. LLM processa comando

---

#### US043
**Como** autor  
**Quero** que LLM mantenha coerência com contexto do livro  
**Para** evitar inconsistências

**Critérios de Aceitação:**
1. LLM recebe personagens, plots e capítulo adjacente no prompt
2. Sugestões não contradizem informações estabelecidas

---

### UC013: Expandir ou Resumir Texto com LLM

#### US044
**Como** autor  
**Quero** expandir trecho adicionando detalhes  
**Para** enriquecer passagem

**Critérios de Aceitação:**
1. Comando "Expandir" envia instrução para LLM
2. LLM retorna versão mais longa mantendo ideia central
3. Autor pode aceitar ou rejeitar

---

#### US045
**Como** autor  
**Quero** resumir trecho mantendo essência  
**Para** tornar texto mais conciso

**Critérios de Aceitação:**
1. Comando "Resumir" envia instrução
2. LLM retorna versão mais curta
3. Autor pode aceitar ou rejeitar

---

#### US046
**Como** autor  
**Quero** controlar nível de expansão/resumo  
**Para** ajustar resultado

**Critérios de Aceitação:**
1. Opções: "Expandir um pouco" vs "Expandir muito"
2. LLM ajusta nível de detalhe conforme solicitado

---

### UC014: Construir Contexto para Prompt LLM

#### US047
**Como** sistema  
**Quero** identificar personagens relevantes no capítulo atual  
**Para** incluir no contexto do prompt

**Critérios de Aceitação:**
1. Sistema analisa conteúdo do capítulo e identifica nomes de personagens
2. Busca personagens no banco
3. Top 3 personagens mais relevantes são incluídos no prompt

---

#### US048
**Como** sistema  
**Quero** buscar plots ativos no capítulo atual  
**Para** incluir arcos narrativos no contexto

**Critérios de Aceitação:**
1. Sistema busca PlotPoints vinculados ao capítulo
2. Plots correspondentes são incluídos no prompt
3. Se não houver pontos, plot principal é incluído por padrão

---

#### US049
**Como** sistema  
**Quero** montar prompt contextualizado automaticamente  
**Para** LLM ter informações relevantes sem intervenção do autor

**Critérios de Aceitação:**
1. Prompt segue estrutura: [Contexto Geral] + [Personagens] + [Plots] + [Capítulo Adjacente] + [Comando do Autor]
2. Limite de 4000 tokens respeitado
3. Se ultrapassar limite, prioriza: personagens > plots > adjacente
4. Prompt é logado para debug

---

### UC015: Busca Semântica de Entidades Relevantes

#### US050
**Como** sistema  
**Quero** gerar embeddings de entidades (personagens, locais, plots)  
**Para** habilitar busca semântica

**Critérios de Aceitação:**
1. Ao criar/editar personagem, sistema gera embedding da descrição
2. Embedding salvo na tabela Embeddings com EntityType e EntityId
3. Mesmo processo para locais e plots

---

#### US051
**Como** sistema  
**Quero** buscar entidades por similaridade semântica ao texto selecionado  
**Para** encontrar elementos relevantes automaticamente

**Critérios de Aceitação:**
1. Sistema gera embedding do texto selecionado
2. Busca vetorial (pgvector) retorna top 5 entidades mais similares
3. Entidades são incluídas no contexto do prompt

---

#### US052
**Como** sistema  
**Quero** atualizar embeddings ao editar entidades  
**Para** manter busca semântica precisa

**Critérios de Aceitação:**
1. Ao editar descrição de personagem/local/plot, embedding é regenerado
2. Embedding antigo é sobrescrito

---

### UC016: Exportar Livro para PDF

#### US053
**Como** autor  
**Quero** exportar livro completo para PDF  
**Para** ter produto final tangível

**Critérios de Aceitação:**
1. Botão "Exportar PDF" dispara geração
2. PDF inclui: capa (título + autor), sumário clicável, todos os capítulos em ordem
3. Geração completa em < 10 segundos
4. PDF salvo em local escolhido pelo autor
5. Notificação de sucesso exibida

---

#### US054
**Como** autor  
**Quero** escolher local de salvamento do PDF  
**Para** organizar arquivos conforme preferência

**Critérios de Aceitação:**
1. Dialog de sistema permite escolher pasta e nome do arquivo
2. Nome padrão: "{TítuloDoLivro}.pdf"

---

#### US055
**Como** autor  
**Quero** que PDF tenha formatação profissional  
**Para** resultado apresentável

**Critérios de Aceitação:**
1. Fonte legível (Merriweather, Georgia ou similar)
2. Margens: 2cm em todos os lados
3. Quebra de página antes de cada capítulo
4. Cabeçalho com título do livro (páginas pares) e número de capítulo (páginas ímpares)

---

#### US056
**Como** autor  
**Quero** sumário clicável no PDF  
**Para** navegação fácil

**Critérios de Aceitação:**
1. Sumário lista todos os capítulos com números de página
2. Clicar em entrada do sumário leva para página correspondente

---

### UC017: Visualizar Preview do Livro

#### US057
**Como** autor  
**Quero** visualizar preview do livro antes de exportar  
**Para** verificar formatação

**Critérios de Aceitação:**
1. Botão "Preview" abre visualização inline (PDF embed ou HTML simulando PDF)
2. Preview fiel ao PDF final
3. Carregamento < 3 segundos

---

#### US058
**Como** autor  
**Quero** navegar páginas do preview  
**Para** revisar todo o conteúdo

**Critérios de Aceitação:**
1. Controles de navegação (anterior, próxima, ir para página X)
2. Scroll funciona para navegar páginas

---

### UC018: Configurar Supabase Local

#### US059
**Como** desenvolvedor  
**Quero** subir Supabase local via Docker  
**Para** ter banco de dados funcional

**Critérios de Aceitação:**
1. Comando `docker-compose up -d` sobe todos os serviços Supabase
2. Supabase Studio acessível em `localhost:54323`
3. PostgreSQL acessível em `localhost:54322`

---

#### US060
**Como** desenvolvedor  
**Quero** acessar Supabase Studio localmente  
**Para** visualizar dados via UI

**Critérios de Aceitação:**
1. Studio carrega sem erros
2. Login funciona (credenciais default do Supabase local)

---

#### US061
**Como** desenvolvedor  
**Quero** confirmar que pgvector está habilitado  
**Para** garantir busca vetorial funcional

**Critérios de Aceitação:**
1. Comando SQL `CREATE EXTENSION IF NOT EXISTS vector;` executa sem erro
2. Query `SELECT * FROM pg_extension WHERE extname = 'vector';` retorna resultado

---

### UC019: Criar Schemas e Migrations

#### US062
**Como** desenvolvedor  
**Quero** criar migrations para todas as tabelas  
**Para** definir estrutura do banco

**Critérios de Aceitação:**
1. Arquivo de migration cria tabelas: Projects, Characters, Locations, Plots, Chapters, PlotPoints, Embeddings
2. PKs são UUIDs
3. FKs definidas corretamente
4. Timestamps (CreatedAt, UpdatedAt) em todas as tabelas

---

#### US063
**Como** desenvolvedor  
**Quero** executar migrations no banco local  
**Para** aplicar schemas

**Critérios de Aceitação:**
1. Comando `supabase db push` ou `dotnet ef database update` executa sem erro
2. Tabelas criadas aparecem no Supabase Studio

---

#### US064
**Como** desenvolvedor  
**Quero** verificar integridade dos schemas  
**Para** garantir consistência

**Critérios de Aceitação:**
1. Todas as FKs apontam para tabelas existentes
2. Tipos de dados corretos (UUID, TEXT, TIMESTAMP, INT, JSONB)
3. Constraints (NOT NULL, UNIQUE) aplicadas conforme design

---

#### US065
**Como** desenvolvedor  
**Quero** criar índices e constraints  
**Para** otimizar performance

**Critérios de Aceitação:**
1. Índice em `ProjectId` em todas as tabelas filhas
2. Índice em `Order` na tabela Chapters
3. Índice GIN em campo `Vector` da tabela Embeddings (para pgvector)

---

### UC020: Implementar API REST

#### US066
**Como** sistema  
**Quero** implementar Commands e Queries para operações de Projects  
**Para** gerenciar projetos através do padrão CQRS

**Critérios de Aceitação:**
1. CreateProjectCommand com handler que valida e persiste usando aggregate Project
2. GetProjectsQuery retorna lista de projetos com aggregates carregados
3. GetProjectByIdQuery retorna aggregate Project completo
4. UpdateProjectCommand valida e atualiza usando métodos do domain
5. DeleteProjectCommand verifica regras de negócio antes de remover
6. Controllers mapeiam HTTP → Commands/Queries via MediatR

---

#### US067
**Como** sistema  
**Quero** implementar Commands e Queries para operações de Characters  
**Para** gerenciar personagens através do padrão CQRS

**Critérios de Aceitação:**
1. CreateCharacterCommand valida e adiciona Character ao aggregate Project
2. GetCharactersByProjectQuery retorna lista de characters com value objects
3. GetCharacterByIdQuery retorna Character entity completo
4. UpdateCharacterCommand atualiza via métodos do domain (ex: Character.UpdateTraits)
5. DeleteCharacterCommand verifica referências antes de remover
6. Handlers trabalham com Character entity, não DTOs anêmicos

---

#### US068
**Como** sistema  
**Quero** implementar Commands e Queries para operações de Locations  
**Para** gerenciar locais através do padrão CQRS

**Critérios de Aceitação:**
1. CreateLocationCommand valida e adiciona Location ao aggregate Project
2. GetLocationsByProjectQuery retorna lista de locations
3. GetLocationByIdQuery retorna Location entity completo
4. UpdateLocationCommand atualiza via métodos do domain
5. DeleteLocationCommand verifica referências antes de remover
6. Handlers trabalham com Location entity rica com comportamento encapsulado

---

#### US069
**Como** sistema  
**Quero** implementar Commands e Queries para operações de Plots  
**Para** gerenciar arcos narrativos através do padrão CQRS

**Critérios de Aceitação:**
1. CreatePlotCommand valida regra de negócio (pelo menos 1 plot principal)
2. GetPlotsByProjectQuery retorna lista de plots com PlotPoints carregados
3. GetMainPlotQuery retorna Plot principal do projeto
4. UpdatePlotCommand atualiza via métodos do domain
5. DeletePlotCommand usa Domain Service para verificar PlotPoints e deletar em cascade
6. Handlers respeitam invariantes do aggregate Plot

---

#### US070
**Como** sistema  
**Quero** implementar Commands e Queries para operações de Chapters  
**Para** gerenciar capítulos através do padrão CQRS

**Critérios de Aceitação:**
1. CreateChapterCommand atribui Order sequencial via Domain Service
2. GetChaptersByProjectQuery retorna lista ordenada de chapters
3. GetChapterByIdQuery retorna Chapter aggregate com Content value object
4. UpdateChapterCommand atualiza via métodos do domain (ex: Chapter.UpdateContent)
5. DeleteChapterCommand ajusta Order dos chapters seguintes via Domain Service
6. ReorderChaptersCommand processa reordenação em batch, WordCount calculado via domain entity

---

#### US071
**Como** desenvolvedor  
**Quero** implementar SignalR Hub para streaming LLM  
**Para** enviar respostas LLM em tempo real

**Critérios de Aceitação:**
1. Hub `/llmhub` aceita conexões
2. Método `RequestRewrite(string chapterId, string selectedText, string command)` recebe requisição
3. Hub invoca LLM e faz streaming da resposta token por token
4. Cliente recebe eventos `OnTokenReceived(string token)` e `OnComplete()`

---

### UC021: Integrar Ollama com Backend

#### US072
**Como** desenvolvedor  
**Quero** conectar backend ao Ollama  
**Para** invocar LLM local

**Implementação Técnica Obrigatória:**
- ✅ Usar **Microsoft.Extensions.AI** (v10.0) para abstrações de LLM
- ✅ Usar **Semantic Kernel** (v1.x) para orquestração
- ✅ Usar **Microsoft Agents Framework** para gerenciamento de agentes
- ✅ Integrar com Ollama via HTTP client nativo do Semantic Kernel
- ✅ Substituir `ILLMService` por abstrações do Agents Framework

**Critérios de Aceitação:**
1. Serviço usa Semantic Kernel com connector Ollama (`http://localhost:11434/api/generate`)
2. Request inclui: model, prompt, stream: true
3. Conexão testada com health check
4. Abstrações do Microsoft.Extensions.AI implementadas

---

#### US073
**Como** desenvolvedor  
**Quero** implementar streaming de respostas LLM  
**Para** enviar tokens progressivamente ao frontend

**Implementação Técnica Obrigatória:**
- ✅ Usar **Semantic Kernel streaming APIs** para processar tokens
- ✅ Usar **Microsoft Agents Framework** para gerenciar estado do streaming
- ✅ Integrar com SignalR Hub existente (LLMHub)
- ✅ Implementar IAsyncEnumerable<string> para streaming eficiente

**Critérios de Aceitação:**
1. Response de Ollama processada via Semantic Kernel streaming (Server-Sent Events)
2. Cada token parseado e enviado via SignalR usando Agents Framework
3. Tratamento de erro se stream for interrompido
4. Streaming usa IAsyncEnumerable para performance

---

#### US074
**Como** desenvolvedor  
**Quero** tratar erros de comunicação com Ollama  
**Para** garantir robustez

**Implementação Técnica Obrigatória:**
- ✅ Usar **Microsoft Agents Framework error handling patterns**
- ✅ Implementar retry policies usando Semantic Kernel
- ✅ Circuit breaker pattern para falhas persistentes
- ✅ Logging estruturado integrado com Microsoft.Extensions.Logging

**Critérios de Aceitação:**
1. Se Ollama não responder, timeout de 60s configurado no Semantic Kernel
2. Erro retorna mensagem clara via Agents Framework: "LLM não disponível"
3. Log estruturado de erros usando Microsoft.Extensions.Logging
4. Retry policy configurado (3 tentativas com backoff exponencial)

---

#### US075
**Como** desenvolvedor  
**Quero** configurar modelo LLM via appsettings  
**Para** trocar modelo facilmente

**Implementação Técnica Obrigatória:**
- ✅ Usar **Semantic Kernel configuration patterns**
- ✅ Configurar modelo via **Microsoft.Extensions.AI abstrações**
- ✅ Registrar serviços do Agents Framework no DI container
- ✅ Suportar múltiplos modelos (gpt-oss-20b, llama3.1, qwen2.5)

**Critérios de Aceitação:**
1. `appsettings.json` tem seção: `"SemanticKernel": { "Model": "gpt-oss-20b", "Endpoint": "http://localhost:11434" }`
2. Serviço lê configuração via IOptions e configura Semantic Kernel
3. Agents Framework registrado corretamente no DI container
4. Documentação de modelos suportados (gpt-oss-20b, llama3.1:70b, qwen2.5:32b)

---

### UC022: Implementar Frontend Base

#### US076
**Como** desenvolvedor  
**Quero** criar aplicação React com TypeScript  
**Para** ter base do frontend

**Critérios de Aceitação:**
1. Projeto criado com Vite: `npm create vite@latest autor-llm-frontend -- --template react-ts`
2. Build e dev server funcionais
3. ESLint e Prettier configurados
4. TypeScript strict mode habilitado

---

#### US077
**Como** desenvolvedor  
**Quero** configurar roteamento  
**Para** navegar entre páginas

**Critérios de Aceitação:**
1. React Router instalado e configurado
2. Rotas: `/`, `/projects`, `/projects/:id`
3. Navegação funcional entre rotas

---

#### US078
**Como** desenvolvedor  
**Quero** criar client HTTP para API  
**Para** consumir backend

**Critérios de Aceitação:**
1. Axios instalado e configurado
2. Base URL: `http://localhost:5011/api`
3. Interceptors para tratamento de erros globais

---

#### US079
**Como** desenvolvedor  
**Quero** implementar layout base  
**Para** ter estrutura visual

**Critérios de Aceitação:**
1. Layout com sidebar (navegação) e área principal (conteúdo)
2. Header com título do projeto
3. Footer com informações de salvamento
4. Layout responsivo (desktop-first)

---

#### US080
**Como** desenvolvedor  
**Quero** configurar SignalR client  
**Para** receber streaming LLM

**Critérios de Aceitação:**
1. `@microsoft/signalr` instalado
2. Conexão estabelecida com `/llmhub`
3. Eventos `OnTokenReceived` e `OnComplete` ouvidos e tratados

---

### UC023: Implementar CQRS Pattern

#### US081
**Como** desenvolvedor  
**Quero** implementar estrutura de Commands  
**Para** separar operações de escrita

**Critérios de Aceitação:**
1. Pasta `Commands/` criada com subpastas por entidade (Projects, Characters, Chapters, etc)
2. Commands implementam IRequest<TResponse> (MediatR)
3. Exemplo: `CreateProjectCommand`, `UpdateChapterCommand`, `DeleteCharacterCommand`
4. Cada Command tem propriedades imutáveis (records ou readonly properties)

---

#### US082
**Como** desenvolvedor  
**Quero** implementar estrutura de Queries  
**Para** separar operações de leitura

**Critérios de Aceitação:**
1. Pasta `Queries/` criada com subpastas por entidade
2. Queries implementam IRequest<TResponse> (MediatR)
3. Exemplo: `GetProjectQuery`, `ListCharactersQuery`, `GetChapterContentQuery`
4. Queries retornam DTOs (ReadModels), nunca entidades de domínio

---

#### US083
**Como** desenvolvedor  
**Quero** integrar MediatR para CQRS  
**Para** dispatch automático de commands e queries

**Critérios de Aceitação:**
1. MediatR instalado via NuGet
2. Handlers registrados automaticamente no DI
3. Controllers usam `IMediator.Send()` em vez de chamar services diretamente

---

#### US084
**Como** desenvolvedor  
**Quero** criar validadores para Commands  
**Para** garantir dados válidos antes de processar

**Critérios de Aceitação:**
1. FluentValidation instalado
2. Validators criados para cada Command (ex: `CreateProjectCommandValidator`)
3. Pipeline behavior do MediatR valida Commands antes de executar Handler

---

### UC024: Implementar Domain Entities (DDD)

#### US085
**Como** desenvolvedor  
**Quero** criar entidades de domínio ricas  
**Para** encapsular comportamento e regras de negócio

**Critérios de Aceitação:**
1. Entidades criadas: `Project`, `Character`, `Chapter`, `Plot`, `Location`
2. Propriedades privadas com setters privados
3. Métodos públicos para mutação (ex: `Project.AddChapter()`, `Chapter.UpdateContent()`)
4. Validação no construtor e métodos (lançar exceções de domínio)
5. Entidades sempre em estado válido

---

#### US086
**Como** desenvolvedor  
**Quero** criar Value Objects  
**Para** representar conceitos sem identidade

**Critérios de Aceitação:**
1. Value Objects criados: `CharacterRole`, `PlotType`, `ChapterOrder`
2. Implementam equality by value (override Equals e GetHashCode)
3. Imutáveis (readonly properties)
4. Validação no construtor

---

#### US087
**Como** desenvolvedor  
**Quero** implementar agregados com Aggregate Root  
**Para** controlar consistência transacional

**Critérios de Aceitação:**
1. `Project` definido como Aggregate Root
2. Acesso a `Chapters`, `Characters`, `Plots` só via `Project`
3. Métodos no Aggregate Root para operações (ex: `Project.AddChapter()`, `Project.ReorderChapters()`)
4. Invariantes do agregado sempre garantidas

---

#### US088
**Como** desenvolvedor  
**Quero** criar Domain Services  
**Para** lógica que não pertence a uma entidade

**Critérios de Aceitação:**
1. Domain Services criados: `PlotProgressionService`, `CharacterConsistencyService`
2. Interfaces definidas no Domain Layer
3. Lógica stateless (sem estado interno)

---

#### US089
**Como** desenvolvedor  
**Quero** implementar Domain Events  
**Para** comunicação entre agregados

**Critérios de Aceitação:**
1. Classe base `DomainEvent` criada
2. Events específicos: `ChapterContentUpdatedEvent`, `CharacterCreatedEvent`
3. Agregados publicam events ao realizar operações

---

#### US090
**Como** sistema  
**Quero** implementar Commands e Queries para operações de PlotPoints  
**Para** gerenciar marcações de intensidade de plots em capítulos através do padrão CQRS

**Critérios de Aceitação:**
1. CreatePlotPointCommand cria PlotPoint validando que Plot e Chapter existem
2. GetPlotPointsByPlotQuery retorna lista de PlotPoints ordenada por Chapter.Order
3. GetPlotPointsByChapterQuery retorna todos PlotPoints de um capítulo
4. UpdatePlotPointCommand atualiza intensity e description via métodos do domain
5. DeletePlotPointCommand remove PlotPoint através do aggregate apropriado
6. Intensity é validado no range 0-10, e um Plot pode ter apenas 1 point por Chapter

---

## Resumo

**Total de User Stories:** 90  
**Total de Use Cases:** 24  
**Total de Features:** 12  
**Total de Epics:** 4

**Distribuição por Fase:**
- Fase 0 (Fundacional): 32 stories (22 originais + 10 DDD/CQRS)
- Fase 1 (MVP): 58 stories

Todas as stories estão acionáveis, testáveis e vinculadas aos respectivos use cases.
