# Análise de Ajustes ao Padrão DDD + CQRS

**Data:** 2026-01-26  
**Status:** ✅ Completo

---

## Objetivo

Avaliar se algum use case ou story anteriormente criado deveria ser ajustado ao padrão DDD + CQRS adotado na arquitetura.

---

## Análise Realizada

### ✅ Use Cases que NÃO precisaram de ajuste

**UC001 a UC017: Features Funcionais**
- **Justificativa:** Focam em regras de negócio e fluxos de usuário do ponto de vista funcional
- **Exemplos:** Gerenciar Personagens, Escrever Capítulo, Reescrever com LLM, Exportar PDF
- **Conclusão:** Mantidos como estão - representam corretamente as necessidades do usuário

**UC018-UC019: Infraestrutura de Banco**
- **Justificativa:** Tratam de configuração e schemas de banco de dados
- **Exemplos:** Configurar Supabase Local, Criar Migrations
- **Conclusão:** Mantidos como estão - são independentes do padrão arquitetural

**UC021-UC022: Integração LLM e Frontend**
- **Justificativa:** Focam em integração com serviços externos e interface
- **Exemplos:** Integrar Ollama, Implementar Frontend Base
- **Conclusão:** Mantidos como estão - são camadas de infraestrutura/apresentação

---

## ⚠️ Use Cases que FORAM AJUSTADOS

### UC020: ~~Implementar API REST~~ → **Implementar Application Layer com CQRS**

**Problema identificado:**
- Use case original focava em "endpoints CRUD" de forma técnica e anêmica
- Não refletia o padrão CQRS (separação Command/Query)
- Stories mencionavam apenas DTOs e endpoints HTTP
- Tratava a API como camada anêmica, não como Application Layer

**Ajustes realizados:**

#### 1. Reescrita do Use Case UC020

**Antes:**
- **Objetivo:** Criar endpoints REST para CRUD de todas as entidades
- **Atores:** Desenvolvedor
- **Fluxo:** Controllers → Services → Repositories → EF Core

**Depois:**
- **Objetivo:** Criar Application Layer que orquestra operações de negócio através de Commands e Queries
- **Atores:** Sistema
- **Fluxo:** Controllers → Commands/Queries → MediatR → Handlers → Domain Entities → Repositories

**Regras de Negócio adicionadas:**
- Commands modificam estado (POST/PUT/DELETE)
- Queries apenas leem (GET)
- Validação via FluentValidation antes de executar Command
- Handlers trabalham com Domain Entities ricas, não DTOs anêmicos
- Repositories retornam aggregates completos

---

#### 2. Reescrita das User Stories US066-US070

##### US066: ~~Implementar endpoints CRUD para Projects~~ → **Implementar handlers CQRS para Projects**

**Critérios de Aceitação - Antes:**
1. `GET /api/projects` retorna lista
2. `POST /api/projects` cria novo
3. `PUT /api/projects/{id}` atualiza
4. `DELETE /api/projects/{id}` deleta
5. (5 critérios - foco em HTTP)

**Critérios de Aceitação - Depois:**
1. CreateProjectCommand com handler que valida e persiste usando aggregate Project
2. GetProjectsQuery retorna lista de projetos com aggregates carregados
3. GetProjectByIdQuery retorna aggregate Project completo
4. UpdateProjectCommand valida e atualiza usando métodos do domain
5. DeleteProjectCommand verifica regras de negócio antes de remover
6. Controllers mapeiam HTTP → Commands/Queries via MediatR
(6 critérios - foco em domain + CQRS)

---

##### US067: ~~Implementar endpoints CRUD para Characters~~ → **Implementar handlers CQRS para Characters**

**Critérios de Aceitação - Antes:**
1. Endpoints HTTP para CRUD
2. DTOs validados
3. (5 critérios técnicos)

**Critérios de Aceitação - Depois:**
1. CreateCharacterCommand valida e adiciona Character ao aggregate Project
2. GetCharactersByProjectQuery retorna lista de characters com value objects
3. GetCharacterByIdQuery retorna Character entity completo
4. UpdateCharacterCommand atualiza via métodos do domain (ex: Character.UpdateTraits)
5. DeleteCharacterCommand verifica referências antes de remover
6. Handlers trabalham com Character entity, não DTOs anêmicos
(6 critérios - foco em domain entities + value objects)

---

##### US068: ~~Implementar endpoints CRUD para Locations~~ → **Implementar handlers CQRS para Locations**

**Ajustes similares a US067:**
- Commands/Queries específicos
- Trabalha com Location entity rica
- Handlers com comportamento encapsulado
- 6 critérios focados em domain

---

##### US069: ~~Implementar endpoints CRUD para Plots~~ → **Implementar handlers CQRS para Plots**

**Critérios de Aceitação - Depois (destaques):**
1. CreatePlotCommand valida regra de negócio (pelo menos 1 plot principal)
2. GetMainPlotQuery retorna Plot principal do projeto
3. DeletePlotCommand usa **Domain Service** para verificar PlotPoints e deletar em cascade
4. Handlers respeitam **invariantes do aggregate Plot**

**Diferencial:** Enfatiza uso de Domain Services para lógica complexa

---

##### US070: ~~Implementar endpoints CRUD para Chapters~~ → **Implementar handlers CQRS para Chapters**

**Critérios de Aceitação - Depois (destaques):**
1. CreateChapterCommand atribui Order sequencial via **Domain Service**
2. GetChapterByIdQuery retorna Chapter aggregate com **Content value object**
3. UpdateChapterCommand atualiza via métodos do domain (ex: Chapter.UpdateContent)
4. DeleteChapterCommand ajusta Order dos chapters seguintes via **Domain Service**
5. ReorderChaptersCommand processa reordenação em batch
6. WordCount calculado via **domain entity**

**Diferencial:** Reordenação como Command separado, cálculos no domain

---

## Impacto nos Documentos

### Documentos atualizados:

1. **[03-catalog-usecases.md](03-catalog-usecases.md)**
   - UC020 reescrito completamente
   - Título atualizado na tabela de índice

2. **[04-catalog-user-stories.md](04-catalog-user-stories.md)**
   - US066-US070 reescritas com foco em CQRS
   - Critérios de aceitação expandidos (5→6)
   - Tabela de índice atualizada

3. **[05-backlog-fases.md](05-backlog-fases.md)**
   - Título do UC020 atualizado na Fase 0
   - Descrições de US066-US070 atualizadas na tabela ordenada
   - Manteve sequência e sprints (nenhuma story removida ou adicionada)

---

## Resumo dos Ajustes

| Item | Antes | Depois | Razão |
|------|-------|--------|-------|
| UC020 | Implementar API REST | Implementar Application Layer com CQRS | Refletir arquitetura DDD+CQRS |
| US066 | Endpoints CRUD Projects | Handlers CQRS Projects | Foco em Commands/Queries + Domain |
| US067 | Endpoints CRUD Characters | Handlers CQRS Characters | Trabalhar com entities e value objects |
| US068 | Endpoints CRUD Locations | Handlers CQRS Locations | Comportamento encapsulado no domain |
| US069 | Endpoints CRUD Plots | Handlers CQRS Plots | Domain Services + Invariantes |
| US070 | Endpoints CRUD Chapters | Handlers CQRS Chapters | Domain entities ricas + Commands batch |

**Total de stories ajustadas:** 6 (US066-US071 - sendo US071 mantida sem alteração)  
**Total de stories do projeto:** 89 (mantido)  
**Total de use cases ajustados:** 1 (UC020)  
**Total de use cases do projeto:** 24 (mantido)

---

## Consistência Arquitetural

### ✅ Antes dos ajustes:
- UC023-UC024 implementavam DDD + CQRS
- UC020 implementava CRUD anêmico
- **Inconsistência:** 2 padrões arquiteturais conflitantes

### ✅ Depois dos ajustes:
- UC024: Domain Entities (DDD) → Base rica
- UC023: CQRS Pattern → Separação Commands/Queries
- UC020: Application Layer com CQRS → Orquestração usando os padrões
- **Consistência:** Stack completo e coerente

---

## Conclusão

**Status:** ✅ Todos os use cases e stories agora estão alinhados com o padrão DDD + CQRS

**Próxima ação:** Iniciar implementação com Sprint 0, começando por US059 (Supabase Docker)

**Arquitetura garantida:**
- Domain Layer rica com entidades, value objects, aggregates, domain services e domain events
- Application Layer com CQRS (Commands/Queries/Handlers)
- Infrastructure Layer com Repositories e EF Core
- Presentation Layer (API Controllers) delegando para Application

---

## Validação Final

### Checklist de Consistência:
- ✅ Todos os use cases refletem arquitetura DDD + CQRS
- ✅ Nenhuma story fala em "CRUD anêmico" ou "DTOs sem comportamento"
- ✅ Handlers trabalham com domain entities, não modelos anêmicos
- ✅ Validações via FluentValidation (Application Layer)
- ✅ Regras de negócio no Domain (entities, value objects, domain services)
- ✅ Separação clara: Command (escrita) vs Query (leitura)
- ✅ MediatR como mediador entre Controllers e Handlers

**Conclusão:** Planejamento está pronto para implementação DDD + CQRS consistente! 🚀
