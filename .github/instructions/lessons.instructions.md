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
---

### 11. PostgreSQL ENUMs requerem cast explícito em INSERTs
**Data:** 2026-01-28  
**Contexto:** US014-US017 - Implementação de Plots CRUD  
**Lição:** Ao inserir valores em colunas PostgreSQL do tipo ENUM, é **OBRIGATÓRIO** fazer cast explícito usando `::enum_type`, caso contrário o PostgreSQL retornará erro 42804.  
**Erro Comum:**
```
Npgsql.PostgresException: 42804: column "type" is of type plot_type but expression is of type text
```

**Solução:**
```csharp
// ❌ INCORRETO - Sem cast
const string sql = @"
    INSERT INTO plots (id, project_id, title, type, ...)
    VALUES (@Id, @ProjectId, @Title, @Type, ...)";

// ✅ CORRETO - Com cast explícito
const string sql = @"
    INSERT INTO plots (id, project_id, title, type, ...)
    VALUES (@Id, @ProjectId, @Title, @Type::plot_type, ...)";
```

**Padrão a seguir:**
- `@Role::character_role` para roles de personagens
- `@Type::plot_type` para tipos de plot
- `@Genre::genre_type` para gêneros literários
- etc.

**Motivo:** PostgreSQL não faz conversão automática de TEXT para ENUM por segurança de tipos.

---

### 12. Campos opcionais devem estar presentes em TODA a stack
**Data:** 2026-01-28  
**Contexto:** US014-US017 - Campo `resolution` faltando no CreatePlotCommand  
**Lição:** Quando um campo existe na entidade de domínio e na tabela SQL, ele **DEVE** estar presente em TODAS as camadas, mesmo que seja opcional. Não assumir que campos opcionais podem ser omitidos.

**Checklist para campos opcionais:**
1. ✅ **SQL:** Coluna existe com constraint NULL permitido
2. ✅ **Entidade de Domínio:** Propriedade com tipo nullable (`string?`)
3. ✅ **CreateCommand:** Propriedade opcional presente (`string? Resolution { get; init; }`)
4. ✅ **UpdateCommand:** Propriedade opcional presente
5. ✅ **Validator:** Regra de validação (mesmo que só tamanho máximo)
6. ✅ **Factory Method:** Parâmetro opcional com default (`string? resolution = null`)
7. ✅ **Aggregate Method:** Passa o parâmetro ao factory
8. ✅ **DTO:** Propriedade opcional presente
9. ✅ **Frontend Type:** Campo opcional no interface (`resolution?: string`)
10. ✅ **Frontend Form:** Campo presente no formulário (mesmo que não obrigatório)

**Exemplo Completo:**
```csharp
// Entity
public string? Resolution { get; private set; }

// Factory
public static Plot Create(..., string? resolution = null)

// Command
public string? Resolution { get; init; }

// Validator
RuleFor(x => x.Resolution)
    .MaximumLength(2000).WithMessage("Resolution must be under 2000 characters");
```

```typescript
// Frontend Type
export interface CreatePlotInput {
  title: string;
  description: string;
  type: PlotType;
  resolution?: string;  // ✅ Presente mesmo sendo opcional
}

// Form State
const [formData, setFormData] = useState({
  title: "",
  description: "",
  type: "Main",
  resolution: "",  // ✅ Presente com valor default vazio
});
```

**Motivo:** Omitir campos opcionais quebra a simetria da stack e dificulta futuras adições de lógica de negócio relacionada ao campo.

---

### 13. Enums devem estar completos em todas as camadas
**Data:** 2026-01-28  
**Contexto:** US014-US017 - PlotType com apenas 2 de 5 valores possíveis no frontend  
**Lição:** Quando um ENUM é definido no backend ou banco de dados, **TODOS** os valores devem estar disponíveis no frontend, mesmo que inicialmente só alguns sejam usados.

**Problema Encontrado:**
```sql
-- SQL tinha 5 tipos
CREATE TYPE plot_type AS ENUM ('Main', 'Subplot', 'Character Arc', 'Romance', 'Mystery');
```

```csharp
// Backend tinha 5 tipos
public static readonly PlotType Main = new("Main");
public static readonly PlotType Subplot = new("Subplot");
public static readonly PlotType Character = new("Character Arc");
public static readonly PlotType Romance = new("Romance");
public static readonly PlotType Mystery = new("Mystery");
```

```typescript
// ❌ Frontend tinha apenas 2
export type PlotType = 'Main' | 'Subplot';
```

**Solução:**
```typescript
// ✅ Frontend completo
export type PlotType = 'Main' | 'Subplot' | 'Character Arc' | 'Romance' | 'Mystery';

// ✅ Dropdown completo
<SelectContent>
  <SelectItem value="Main">Plot Principal</SelectItem>
  <SelectItem value="Subplot">Subplot</SelectItem>
  <SelectItem value="Character Arc">Arco de Personagem</SelectItem>
  <SelectItem value="Romance">Romance</SelectItem>
  <SelectItem value="Mystery">Mistério</SelectItem>
</SelectContent>
```

**Checklist para ENUMs:**
1. ✅ Verificar SQL: quais valores existem no `CREATE TYPE ... AS ENUM`
2. ✅ Verificar backend: quais valores estão no ValueObject/classe
3. ✅ Sincronizar frontend: TypeScript union type deve ter TODOS os valores
4. ✅ Atualizar dropdowns/selects com TODAS as opções
5. ✅ Adicionar traduções/labels para novos valores

**Motivo:** Evita erros 500 quando backend retorna valores que frontend não conhece. Garante UX consistente.

---

### 14. Indicadores visuais devem ser consistentes por tipo de entidade
**Data:** 2026-01-28  
**Contexto:** US014-US017 - Adição de cores para tipos de plot  
**Lição:** Cada categoria/tipo de entidade deve ter um sistema de cores **consistente e único** para facilitar identificação visual rápida pelo usuário.

**Padrão Estabelecido:**

**Personagens (Character Roles):**
```tsx
const colors = {
  'Protagonist': 'border-blue-500',    // 🔵 Azul
  'Antagonist': 'border-red-500',      // 🔴 Vermelho
  'Supporting': 'border-green-500',    // 🟢 Verde
  'Minor': 'border-gray-400',          // ⚪ Cinza
};
```

**Locais (Locations):**
```tsx
const color = 'border-amber-500';  // 🟡 Âmbar/Amarelo (único para todos)
```

**Plots (Plot Types):**
```tsx
const colors = {
  'Main': 'border-purple-500',         // 🟣 Roxo
  'Subplot': 'border-indigo-400',      // 🔵 Índigo
  'Character Arc': 'border-cyan-500',  // 🔷 Ciano
  'Romance': 'border-pink-500',        // 💗 Rosa
  'Mystery': 'border-orange-500',      // 🟠 Laranja
};
```

**Regras de Design:**
1. ✅ **Cores distintas** entre diferentes categorias (nunca reutilizar esquema de cores)
2. ✅ **Bordas laterais** (`border-l-4`) são preferíveis a círculos/badges
3. ✅ **Função auxiliar** centralizada para mapear tipo → cor
4. ✅ **Documentar** as cores escolhidas para manter consistência

**Implementação:**
```tsx
function getPlotTypeBorderColor(type: string): string {
  const colors: Record<string, string> = {
    'Main': 'border-purple-500',
    // ... outros tipos
  };
  return colors[type] || 'border-gray-400';  // Fallback
}

// Uso
<div className={`border-l-4 ${getPlotTypeBorderColor(plot.type)}`}>
  {plot.title}
</div>
```

**Benefícios:**
- Identificação visual instantânea do tipo de entidade
- Reduz carga cognitiva ao navegar listas
- Interface mais profissional e polida
- Facilita localização de elementos específicos

---

### 15. React keys devem ser únicas POR TIPO de componente
**Data:** 2026-01-28  
**Contexto:** US014-US017 - Erro "Encountered two children with the same key 'edit'"  
**Lição:** Quando múltiplos dialogs de edição existem no mesmo componente pai, **NUNCA** usar a mesma string como fallback de key. Cada tipo de dialog deve ter seu próprio prefixo único.

**Problema:**
```tsx
// ❌ INCORRETO - Três dialogs diferentes com mesma key fallback
<CharacterFormDialog key={editingCharacter?.id || 'edit'} />
<LocationFormDialog key={editingLocation?.id || 'edit'} />  // ❌ Conflito!
<PlotFormDialog key={editingPlot?.id || 'edit'} />          // ❌ Conflito!
```

**Erro gerado:**
```
Warning: Encountered two children with the same key, `edit`. Keys should be unique
so that components maintain their identity across updates.
```

**Solução:**
```tsx
// ✅ CORRETO - Keys únicas por tipo de entidade
<CharacterFormDialog 
  key={editingCharacter?.id || 'edit-character'} 
  {...props} 
/>

<LocationFormDialog 
  key={editingLocation?.id || 'edit-location'} 
  {...props} 
/>

<PlotFormDialog 
  key={editingPlot?.id || 'edit-plot'} 
  {...props} 
/>
```

**Padrão de Nomenclatura:**
- `'create-{entity}'` para dialogs de criação
- `'edit-{entity}'` para dialogs de edição
- `'delete-{entity}'` para dialogs de confirmação de deleção

**Benefícios:**
- Elimina warnings do React
- Garante que cada dialog mantenha seu estado corretamente
- Facilita debugging (keys descritivas)
- Evita bugs de renderização quando múltiplos dialogs são usados

**Checklist:**
1. ✅ Identificar TODOS os componentes condicionalmente renderizados no mesmo pai
2. ✅ Garantir que NENHUMA key se repete entre componentes
3. ✅ Usar prefixos descritivos (`edit-character`, não apenas `edit`)
4. ✅ Testar no Chrome DevTools Console para confirmar zero warnings

---

### 16. SEMPRE verificar padrões existentes antes de implementar nova feature
**Data:** 2026-01-28  
**Contexto:** US014-US017 - Implementação inicial de Plots com react-query (errado)  
**Lição:** **NUNCA** assumir tecnologias ou padrões sem verificar o código existente primeiro. Implementar uma feature usando um padrão diferente das demais causa inconsistência e retrabalho total.

**Erro Cometido:**
```typescript
// ❌ ERRADO - Implementado com react-query
// src/features/plots/api/plotsApi.ts
export const plotsApi = {
  getPlots: async (projectId: string) => { ... },
};

// ❌ ERRADO - Hook usando react-query
export function usePlots(projectId?: string) {
  return useQuery({
    queryKey: ['plots', projectId],
    queryFn: () => plotsApi.getPlots(projectId!),
  });
}
```

**Padrão Correto do Projeto:**
```typescript
// ✅ CORRETO - Padrão usado em Characters e Locations
export function usePlots(projectId?: string) {
  const [plots, setPlots] = useState<Plot[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const fetchPlots = useCallback(async () => {
    if (!projectId) return;
    const response = await api.get<Plot[]>(`/projects/${projectId}/plots`);
    setPlots(response.data);
  }, [projectId]);

  useEffect(() => { fetchPlots(); }, [fetchPlots]);

  return { plots, isLoading, createPlot, updatePlot, deletePlot };
}
```

**Checklist OBRIGATÓRIO antes de implementar:**
1. ✅ **Pesquisar features similares** - grep -r "use{Entity}" src/features/
2. ✅ **Analisar o padrão existente** - Ler COMPLETAMENTE um hook similar
3. ✅ **Verificar dependências** - Checar package.json
4. ✅ **Confirmar estrutura de pastas** - Seguir a mesma organização
5. ✅ **Replicar naming conventions** - Mesmos nomes de métodos/propriedades
6. ✅ **Validar com usuário** se houver dúvida sobre o padrão

**Consequências do erro:**
- ❌ Arquivo plotsApi.ts criado desnecessariamente (deletado depois)
- ❌ Hook usePlots.ts completo reescrito do zero
- ❌ Tempo perdido em implementação descartada
- ❌ Inconsistência arquitetural temporária
- ❌ Revisão e correção necessárias

**Regra de Ouro:** Quando em dúvida, copie o padrão existente. Projetos estabelecidos TÊM padrões definidos - introduzir novos padrões sem consenso quebra consistência.