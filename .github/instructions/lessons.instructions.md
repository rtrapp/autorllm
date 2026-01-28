---
applyTo: '**'
---

## Lições Aprendidas

### 1. Testes Unitários são Obrigatórios
**Data:** 2026-01-27  
**Contexto:** US082 - Implementação de Queries (CQRS)  
**Lição:** SEMPRE criar testes unitários ao implementar novas funcionalidades (Commands, Queries, Handlers, etc). Não considerar a story completa sem testes.  
**Padrão:** Para cada Query/Command criado, criar pelo menos 3 testes básicos:
- Teste de interface (implementa IRequest<T>)
- Teste de propriedades
- Teste de imutabilidade (record)

### 2. MediatR RequestHandlerDelegate requer CancellationToken
**Data:** 2026-01-27  
**Contexto:** US083 - Integração MediatR  
**Lição:** No MediatR 14+, o `RequestHandlerDelegate<T>` mudou a assinatura e requer um `CancellationToken` como parâmetro.  
**Exemplo Correto:**
```csharp
// Correto (MediatR 14+)
await behavior.Handle(request, (ct) => Task.FromResult(response), CancellationToken.None);

// Incorreto
await behavior.Handle(request, () => Task.FromResult(response), CancellationToken.None);
```

### 3. Records internos não podem ser mockados com Moq
**Data:** 2026-01-27  
**Contexto:** US083 - Testes de ValidationBehavior  
**Lição:** Records usados em testes com Moq devem ser `public`, não `private` ou `internal`, porque FluentValidation é strong-named e Moq não consegue criar proxies para tipos não-públicos.  
**Solução:** Declarar test requests/responses como `public record`

### 4. NUNCA colocar DTOs dentro do Repository
**Data:** 2026-01-27  
**Contexto:** US066 - Implementação do ProjectRepository  
**Lição:** DTOs NÃO devem existir dentro de Repositories. A responsabilidade de reconstruir (hidratar) uma entidade de domínio a partir do banco pertence à própria entidade de domínio, não ao repositório.  
**Solução:** Criar método `internal static Hydrate()` na entidade de domínio para reconstitui-la do banco. Repository usa esse método, não DTOs internos.  
**Exemplo Correto:**
```csharp
// Na entidade de domínio
internal static Project Hydrate(Guid id, string title, ...) { ... }

// No repository
private static Project MapToEntity(dynamic row)
{
    return Project.Hydrate(row.id, row.title, ...);
}
```

### 5. Schema SQL DEVE estar sincronizado com as Entidades de Domínio
**Data:** 2026-01-27  
**Contexto:** US067 - Implementação de handlers CQRS para Characters  
**Lição:** SEMPRE validar que o schema SQL no banco de dados reflete EXATAMENTE as propriedades das entidades de domínio. Implementar handlers CQRS sem validar a persistência resulta em funcionalidade incompleta.  
**Checklist Obrigatório:**
1. ✅ Verificar se TODAS as propriedades da entidade têm colunas correspondentes no SQL
2. ✅ Validar tipos de dados compatíveis (string → TEXT/VARCHAR, int → INTEGER, etc)
3. ✅ Conferir constraints (NOT NULL, CHECK, tamanhos máximos)
4. ✅ Implementar Repository.LoadEntitiesAsync() para carregar child entities
5. ✅ Implementar Repository.SaveEntitiesAsync() para persistir child entities
6. ✅ Implementar Repository.SyncEntitiesAsync() para sincronizar mudanças (delete + re-insert)
7. ✅ Testar end-to-end com INSERT e SELECT no banco real

**Exemplo Incorreto:**
```sql
-- SQL com campos faltantes
CREATE TABLE characters (
    id UUID,
    name VARCHAR(100),
    traits JSONB  -- ❌ Não reflete Backstory, Appearance, Personality separados
);
```

**Exemplo Correto:**
```sql
-- SQL sincronizado com a entidade
CREATE TABLE characters (
    id UUID,
    project_id UUID,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    role character_role NOT NULL DEFAULT 'Supporting',
    backstory TEXT,           -- ✅ Campo separado
    appearance TEXT,          -- ✅ Campo separado
    personality TEXT,         -- ✅ Campo separado
    created_at TIMESTAMP,
    updated_at TIMESTAMP
);
```

**Repository deve:**
```csharp
// ✅ Carregar child entities ao buscar aggregate root
public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
{
    var project = await LoadProjectAsync(id, ct);
    if (project != null)
        await LoadCharactersAsync(project, ct);  // ← OBRIGATÓRIO
    return project;
}

// ✅ Sincronizar child entities ao atualizar
public async Task UpdateAsync(Project project, CancellationToken ct)
{
    await UpdateProjectAsync(project, ct);
    await SyncCharactersAsync(project, ct);  // ← OBRIGATÓRIO
}
```

---

### 6. Sempre adicionar método Hydrate ao corrigir schema SQL
**Data:** 2026-01-27  
**Contexto:** US068 - Implementação de handlers CQRS para Locations  
**Lição:** Quando o schema SQL é corrigido para adicionar colunas faltantes, **SEMPRE** adicionar o método `internal static Hydrate()` na entidade de domínio correspondente para reconstitui-la do banco.  
**Checklist obrigatório ao corrigir schema:**
1. ✅ Adicionar colunas no SQL com constraints corretos
2. ✅ Adicionar método `Hydrate()` na entidade
3. ✅ Adicionar método `HydrateXXX()` no aggregate root (se aplicável)
4. ✅ Atualizar repository para usar `Hydrate()` ao carregar do banco
5. ✅ Testar persistência e carregamento end-to-end

**Exemplo:**
```csharp
// Na entidade
internal static Location Hydrate(
    Guid id,
    Guid projectId,
    string name,
    string description,
    string? geography,
    string? culture,
    string? significance,
    DateTime createdAt,
    DateTime updatedAt)
{
    return new Location { /* ... */ };
}

// No aggregate root
internal void HydrateLocation(Location location)
{
    _locations.Add(location);
}
```

---

### 7. Sempre executar scripts SQL após modificar schema
**Data:** 2026-01-27  
**Contexto:** US068 - Após corrigir schema de locations  
**Lição:** Sempre que o schema SQL for modificado (adicionar/remover colunas, alterar constraints), **OBRIGATORIAMENTE** executar os scripts de criação no banco de dados.  
**Comandos obrigatórios:**
```bash
# 1. Executar script de criação de tabelas (com DROP CASCADE)
docker exec -i autor_llm_postgres psql -U postgres -d postgres < supabase/init/01-create-tables.sql

# 2. Executar script de criação de índices
docker exec -i autor_llm_postgres psql -U postgres -d postgres < supabase/init/02-create-indexes.sql

# 3. Verificar schema da tabela modificada
docker exec -i autor_llm_postgres psql -U postgres -d postgres -c "\d <nome_tabela>"
```
**Motivo:** Sem executar os scripts, o banco fica desatualizado e operações de persistência falham em runtime, mesmo com todos os testes unitários passando.

---

### 8. Organização de Layout: Manuscrito vs Mundo
**Data:** 2026-01-28  
**Contexto:** US006-009 - Implementação de Character Management Frontend  
**Lição:** A interface tem DOIS contextos distintos que NÃO devem ser misturados:
- **Tab "Manuscrito"** → Estrutura narrativa (Atos, Capítulos, Cenas)
- **Tab "Mundo"** → Entidades de worldbuilding (Personagens, Locais, Plots)

**Regras de Posicionamento:**
- ❌ **NUNCA** colocar Personagens, Locais ou Plots na tab "Manuscrito"
- ✅ **SEMPRE** colocar entidades de worldbuilding na tab "Mundo"
- ✅ Personagens aparecem no Manuscrito apenas como **referências** dentro de capítulos (ex: "Personagens: Elara, Kael")

**Exemplo Incorreto:**
```tsx
// ❌ Colocar personagens na tab Manuscrito
{activeTab === 'manuscript' && (
  <div>
    <CharacterList /> {/* ERRADO! */}
  </div>
)}
```

**Exemplo Correto:**
```tsx
// ✅ Personagens na tab Mundo
{activeTab === 'world' && (
  <div>
    <CharacterSection />
    <LocationSection />
    <PlotSection />
  </div>
)}
```

**Motivo:** Separação clara de preocupações - o Manuscrito é o produto final (estrutura narrativa), o Mundo são os building blocks (elementos que compõem a história).

---

### 9. SEMPRE testar funcionalidades com Chrome DevTools antes de marcar como concluída
**Data:** 2026-01-28  
**Contexto:** US006-009 - Character CRUD Implementation  
**Lição:** Testes unitários passando NÃO garantem que a funcionalidade está completa. **OBRIGATORIAMENTE** usar Chrome DevTools para validar integração end-to-end antes de considerar uma story concluída.

**Checklist de Validação com DevTools:**

**1. Network Tab (Requisições HTTP):**
```javascript
// ✅ Verificar payload do request
{
  "name": "Maria",
  "description": "Protagonista da história",
  "backstory": "...",
  "appearance": "...",
  "personality": "...",
  "role": "Protagonist"
}

// ✅ Verificar response (status 200/201)
// ✅ Confirmar que TODOS os campos foram salvos separadamente
```

**2. Console Tab (Erros de Runtime):**
- ✅ Verificar se há erros de JavaScript
- ✅ Validar warnings de React (keys, hooks, etc)
- ✅ Confirmar que não há erros de CORS

**3. Application Tab (State Management):**
- ✅ Verificar localStorage/sessionStorage se aplicável
- ✅ Validar tokens de autenticação

**4. Validações Obrigatórias:**
- ✅ **CREATE:** Request body contém todos os campos; response retorna entidade criada
- ✅ **READ:** GET retorna TODOS os campos populados (não null/undefined inesperados)
- ✅ **UPDATE:** PUT/PATCH contém IDs corretos (route params + body); todos os campos são atualizados
- ✅ **DELETE:** Retorna 204 No Content; entidade removida da lista

**Exemplo de Bug Descoberto com DevTools:**
```javascript
// Bug: Campo "role" causando erro 500
// Network → Request Payload:
{
  "role": "Supporting"  // ❌ Backend esperava tipo ENUM, recebia string
}

// Solução: Cast explícito no SQL
@Role::character_role  // ✅ Conversão para tipo ENUM do PostgreSQL
```

**Workflow Obrigatório:**
1. Implementar funcionalidade
2. Testes unitários passam
3. Build compila sem erros
4. **Abrir Chrome DevTools** (F12)
5. Testar CADA operação CRUD manualmente
6. Validar request/response no Network tab
7. Confirmar zero erros no Console
8. **SÓ ENTÃO** marcar story como concluída

**Motivo:** Validação de camada de apresentação (frontend), camada de API (backend) e persistência (database) juntas. Testes unitários isolados não capturam problemas de integração como:
- Serialização/deserialização JSON incorreta
- Conversões de tipo (string → enum, date formats)
- IDs faltantes em requests
- CORS issues
- Campos sendo concatenados em vez de salvos separadamente

---

### 10. UI/UX: Agrupamento e Hierarquia Visual
**Data:** 2026-01-28  
**Contexto:** Enhancement - Character Role Color Coding & Grouping  
**Lição:** Listas longas devem ser organizadas hierarquicamente com grupos colapsáveis e indicadores visuais para melhorar usabilidade.

**Padrões de Organização:**

**1. Grupos Colapsáveis:**
```tsx
// ✅ Seções principais com chevron indicator
<div onClick={() => toggleSection('characters')}>
  {expanded ? <ChevronDown /> : <ChevronRight />}
  Personagens
</div>

// ✅ Subgrupos por categoria
<div onClick={() => toggleGroup('Protagonist')}>
  {expanded ? <ChevronDown /> : <ChevronRight />}
  Protagonista (3)
</div>
```

**2. Indicadores Visuais:**
- ✅ **Borda lateral colorida** (`border-l-4`) melhor que ícones circulares
- ✅ Cores consistentes com sistema de design (ex: genre colors em projects)
- ❌ **EVITAR** círculos pequenos (dificulta visualização)

**Exemplo:**
```tsx
// ❌ Difícil de ver
<div className="h-2 w-2 rounded-full bg-blue-500" />

// ✅ Destaque visual claro
<div className="border-l-4 border-blue-500 pl-3">
  {character.name}
</div>
```

**3. Ordenação:**
- ✅ Alfabética dentro de cada grupo: `items.sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'))`
- ✅ Ordem fixa de grupos: `['Protagonist', 'Antagonist', 'Supporting', 'Minor']`

**4. Contadores:**
- ✅ Mostrar quantidade de itens: "Protagonista (3)"
- ✅ Ajuda o usuário a entender distribuição

**Benefícios:**
- Reduz scroll para listas longas
- Localização rápida de itens
- Visão geral da distribuição (quantos de cada tipo)
- Interface limpa e organizada
