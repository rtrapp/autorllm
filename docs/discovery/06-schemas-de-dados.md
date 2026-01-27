# Schemas de Dados: Estrutura do Banco

**Estado:** 🟢 Definido (Ciclo 2)  
**Última atualização:** 2026-01-26

---

## Visão Geral

**Database:** Supabase Local (Docker) - PostgreSQL 15 + pgvector  
**Client:** Supabase .NET Client ou Supabase JS Client  
**Padrão:** SQL Migrations (Supabase CLI) ou EF Core Migrations

---

## Diagrama ER (Relacional)

```
┌─────────────────┐
│    Projects     │
├─────────────────┤
│ Id (PK)         │
│ Title           │
│ Author          │
│ Synopsis        │──┐
│ CreatedAt       │  │
│ UpdatedAt       │  │
└─────────────────┘  │
                     │ 1:N
    ┌────────────────┴──────────────────────┬──────────────┬─────────────┐
    │                                       │              │             │
    ▼                                       ▼              ▼             ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│   Characters    │  │   Locations     │  │     Plots       │  │    Chapters     │
├─────────────────┤  ├─────────────────┤  ├─────────────────┤  ├─────────────────┤
│ Id (PK)         │  │ Id (PK)         │  │ Id (PK)         │  │ Id (PK)         │
│ ProjectId (FK)  │  │ ProjectId (FK)  │  │ ProjectId (FK)  │  │ ProjectId (FK)  │
│ Name            │  │ Name            │  │ Name            │  │ Title           │
│ Description     │  │ Description     │  │ Type            │  │ Summary         │
│ Role            │  │ CreatedAt       │  │ Description     │  │ Content         │
│ Traits (JSON)   │  │ UpdatedAt       │  │ CreatedAt       │  │ Order           │
│ CreatedAt       │  └─────────────────┘  │ UpdatedAt       │  │ WordCount       │
│ UpdatedAt       │                       └─────────────────┘  │ CreatedAt       │
└─────────────────┘                                            │ UpdatedAt       │
                                                               └─────────────────┘
                                                                        │
                                                                        │ 1:N
                                                                        ▼
                                                               ┌─────────────────┐
                                                               │   PlotPoints    │
                                                               ├─────────────────┤
                                                               │ Id (PK)         │
                                                               │ PlotId (FK)     │
                                                               │ ChapterId (FK)  │
                                                               │ Intensity (0-10)│
                                                               │ Description     │
                                                               │ CreatedAt       │
                                                               └─────────────────┘

┌─────────────────────────────────────────────────────────────┐
│              Embeddings (Vector Store)                       │
├─────────────────────────────────────────────────────────────┤
│ Id (PK)                                                      │
│ EntityType (Character|Plot|Chapter)                         │
│ EntityId (FK polymorphic)                                    │
│ Content (text used for embedding)                           │
│ Vector (vector(384) or vector(1024)) ← pgvector            │
│ CreatedAt                                                    │
└─────────────────────────────────────────────────────────────┘
```

---

## Schemas Detalhados

### 1. Projects

**Descrição:** Representa um livro inteiro.

```csharp
public class Project
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<Plot> Plots { get; set; } = new List<Plot>();
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
```

**Validações:**
- Title: required, max 200 chars
- Author: required, max 100 chars
- Synopsis: optional, max 2000 chars

---

### 2. Characters

**Descrição:** Personagens do livro.

```csharp
public class Character
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CharacterRole Role { get; set; } // Enum: Protagonist, Antagonist, Supporting
    public string Traits { get; set; } = "{}"; // JSON: { "age": 35, "personality": "..." }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
}

public enum CharacterRole
{
    Protagonist,
    Antagonist,
    Supporting,
    Minor
}
```

**Validações:**
- Name: required, max 100 chars
- Description: optional, max 5000 chars
- Traits: JSON válido

**Exemplo de Traits:**
```json
{
  "age": 35,
  "occupation": "Detective",
  "personality": "Cynical but empathetic",
  "backstory": "Former police officer, now private investigator"
}
```

---

### 3. Locations

**Descrição:** Locais da história.

```csharp
public class Location
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
}
```

**Validações:**
- Name: required, max 100 chars
- Description: optional, max 5000 chars

---

### 4. Plots

**Descrição:** Plot principal e sub-plots.

```csharp
public class Plot
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlotType Type { get; set; } // Enum: Main, SubPlot
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<PlotPoint> PlotPoints { get; set; } = new List<PlotPoint>();
}

public enum PlotType
{
    Main,
    SubPlot
}
```

**Validações:**
- Name: required, max 200 chars
- Description: optional, max 5000 chars

---

### 5. Chapters

**Descrição:** Capítulos do livro.

```csharp
public class Chapter
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML or Markdown
    public int Order { get; set; } // 1, 2, 3...
    public int WordCount { get; set; } // Calculado automaticamente
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<PlotPoint> PlotPoints { get; set; } = new List<PlotPoint>();
}
```

**Validações:**
- Title: required, max 200 chars
- Summary: optional, max 1000 chars
- Content: unlimited (TEXT column)
- Order: unique per project

**Indexes:**
```sql
CREATE INDEX idx_chapters_project_order ON Chapters(ProjectId, Order);
```

---

### 6. PlotPoints

**Descrição:** Pontos de intensidade de um plot em um capítulo (para visualização de arcos).

```csharp
public class PlotPoint
{
    public Guid Id { get; set; }
    public Guid PlotId { get; set; }
    public Guid ChapterId { get; set; }
    public int Intensity { get; set; } // 0-10 (0 = ausente, 10 = clímax)
    public string Description { get; set; } = string.Empty; // Ex: "Confronto final"
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public Plot Plot { get; set; } = null!;
    public Chapter Chapter { get; set; } = null!;
}
```

**Validações:**
- Intensity: 0-10
- Description: optional, max 500 chars

**Constraint:**
```sql
UNIQUE(PlotId, ChapterId) -- Um plot só pode ter um ponto por capítulo
```

---

### 7. Embeddings (Vector Store)

**Descrição:** Armazena embeddings para RAG.

```csharp
public class Embedding
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; } // Enum: Character, Plot, Chapter
    public Guid EntityId { get; set; } // FK polimórfica (pode apontar para Character.Id, Plot.Id, etc.)
    public string Content { get; set; } = string.Empty; // Texto usado para gerar embedding
    public Vector Vector { get; set; } = null!; // pgvector type
    public DateTime CreatedAt { get; set; }
}

public enum EntityType
{
    Character,
    Plot,
    Chapter
}
```

**pgvector setup:**
```sql
CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE Embeddings (
    Id UUID PRIMARY KEY,
    EntityType VARCHAR(50) NOT NULL,
    EntityId UUID NOT NULL,
    Content TEXT NOT NULL,
    Vector VECTOR(384), -- ou 1024, dependendo do modelo de embedding
    CreatedAt TIMESTAMP NOT NULL
);

CREATE INDEX idx_embeddings_vector ON Embeddings USING ivfflat (Vector vector_cosine_ops);
```

**Busca semântica (exemplo SQL):**
```sql
SELECT EntityType, EntityId, Content, 
       1 - (Vector <=> :queryVector) AS similarity
FROM Embeddings
WHERE EntityType = 'Character'
ORDER BY Vector <=> :queryVector
LIMIT 5;
```

---

## Relacionamentos

```
Project 1:N Characters
Project 1:N Locations
Project 1:N Plots
Project 1:N Chapters

Plot 1:N PlotPoints
Chapter 1:N PlotPoints

PlotPoint N:1 Plot
PlotPoint N:1 Chapter
```

---

## Migrations Strategy

**Abordagem:** SQL Migrations (Supabase Dashboard ou CLI)

**Workflow:**
1. Criar tabelas via Supabase Dashboard (SQL Editor)
2. Ou usar Supabase CLI: `supabase migration new migration_name`
3. Escrever SQL no arquivo gerado
4. Aplicar: `supabase db push`

**Alternativa (.NET):**
- Usar Entity Framework com Supabase connection string
- Migrations tradicionais EF Core

**Seed data (opcional):**
- SQL scripts no Supabase
- Ou seed via backend na primeira execução

---

## Queries Comuns

### 1. Buscar todos personagens de um projeto
```csharp
var characters = await _dbContext.Characters
    .Where(c => c.ProjectId == projectId)
    .OrderBy(c => c.Name)
    .ToListAsync();
```

### 2. Buscar capítulos ordenados
```csharp
var chapters = await _dbContext.Chapters
    .Where(c => c.ProjectId == projectId)
    .OrderBy(c => c.Order)
    .Select(c => new { c.Id, c.Title, c.Order, c.WordCount })
    .ToListAsync();
```

### 3. Buscar plot points para visualização de arcos
```csharp
var plotArcs = await _dbContext.Plots
    .Where(p => p.ProjectId == projectId)
    .Include(p => p.PlotPoints)
        .ThenInclude(pp => pp.Chapter)
    .Select(p => new {
        PlotName = p.Name,
        Points = p.PlotPoints
            .OrderBy(pp => pp.Chapter.Order)
            .Select(pp => new {
                ChapterId = pp.ChapterId,
                ChapterOrder = pp.Chapter.Order,
                Intensity = pp.Intensity
            })
    })
    .ToListAsync();
```

### 4. Busca semântica (RAG) com Supabase
```typescript
// Usando Supabase JS Client (ou equivalente em C#)

// Gerar embedding da query
const queryVector = await generateEmbedding(query);

// Buscar top-K mais similares usando pgvector
const { data, error } = await supabase
  .rpc('search_embeddings', {
    query_vector: queryVector,
    entity_type: 'Character',
    top_k: 5
  });

// Função SQL no Supabase:
CREATE FUNCTION search_embeddings(
  query_vector vector(384),
  entity_type text,
  top_k int
)
RETURNS TABLE (
  id uuid,
  entity_id uuid,
  content text,
  similarity float
)
LANGUAGE sql
AS $$
  SELECT 
    id, 
    entity_id, 
    content,
    1 - (vector <=> query_vector) as similarity
  FROM embeddings
  WHERE entity_type = entity_type
  ORDER BY vector <=> query_vector
  LIMIT top_k;
$$;
```

---

## Performance Considerations

### Indexes
```sql
-- Busca por projeto (mais comum)
CREATE INDEX idx_characters_project ON Characters(ProjectId);
CREATE INDEX idx_locations_project ON Locations(ProjectId);
CREATE INDEX idx_plots_project ON Plots(ProjectId);
CREATE INDEX idx_chapters_project_order ON Chapters(ProjectId, Order);

-- Busca de plot points
CREATE INDEX idx_plotpoints_plot ON PlotPoints(PlotId);
CREATE INDEX idx_plotpoints_chapter ON PlotPoints(ChapterId);

-- Busca semântica
CREATE INDEX idx_embeddings_vector ON Embeddings USING ivfflat (Vector vector_cosine_ops);
CREATE INDEX idx_embeddings_entity ON Embeddings(EntityType, EntityId);
```

### Paginação
- Chapters: não precisa (projeto tem ~20-30 capítulos)
- Characters: pagination se > 100 (raro)

### Caching
- Embeddings: cache em memória (evitar recálculo)
- Project metadata: cache (muda raramente)

---

## Próximos Passos

- [ ] Criar projeto no Supabase
- [ ] Configurar pgvector extension
- [ ] Criar tabelas via SQL migrations
- [ ] Configurar Row Level Security (RLS) se necessário
- [ ] Integrar Supabase Client no backend .NET
- [ ] Testar queries de performance
