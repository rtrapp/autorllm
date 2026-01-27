# Migration Files Documentation

## Overview

Created SQL migration files for AutorLLM database schema following Supabase initialization pattern.

## Files Created

### 1. `01-create-tables.sql`
Creates all core database tables with proper constraints and relationships.

**Tables Created:**
- ✅ `projects` - Main project/book entity
- ✅ `characters` - Book characters with traits (JSONB)
- ✅ `locations` - Story locations
- ✅ `plots` - Main plots and subplots
- ✅ `chapters` - Book chapters with content
- ✅ `plot_points` - Plot intensity markers for narrative arcs
- ✅ `embeddings` - Vector store for RAG (pgvector)

**Key Features:**
- All PKs use UUID with `uuid_generate_v4()`
- All FKs properly defined with `ON DELETE CASCADE`
- All tables have `created_at` and `updated_at` timestamps
- Proper constraints (NOT NULL, CHECK, UNIQUE)
- Enums for type safety: `character_role`, `plot_type`, `entity_type`
- Comments on tables and columns for documentation
- Proper permissions granted (authenticated, service_role, anon)

### 2. `02-create-indexes.sql`
Creates performance indexes, triggers, and helper functions.

**Indexes Created:**
- Project indexes: created_at, updated_at
- Character indexes: project_id, role, composite
- Location indexes: project_id
- Plot indexes: project_id, type, composite
- Chapter indexes: project_id, order (critical), updated_at
- PlotPoint indexes: plot_id, chapter_id, composite
- Embedding indexes: entity_type, entity_id, vector (IVFFlat)

**Triggers:**
- Auto-update `updated_at` on all tables
- Auto-calculate `word_count` on chapters when content changes

**Functions:**
- `update_updated_at_column()` - Timestamp trigger function
- `calculate_word_count()` - Word count calculation
- `search_embeddings()` - Semantic search for RAG

**Views:**
- `project_stats` - Aggregated project statistics

## Validation Against Acceptance Criteria

### US062 Criteria:

1. ✅ **Arquivo de migration cria tabelas: Projects, Characters, Locations, Plots, Chapters, PlotPoints, Embeddings**
   - All 7 tables created in `01-create-tables.sql`

2. ✅ **PKs são UUIDs**
   - All tables use `id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4()`

3. ✅ **FKs definidas corretamente**
   - characters.project_id → projects.id
   - locations.project_id → projects.id
   - plots.project_id → projects.id
   - chapters.project_id → projects.id
   - plot_points.plot_id → plots.id
   - plot_points.chapter_id → chapters.id
   - All with `ON DELETE CASCADE`

4. ✅ **Timestamps (CreatedAt, UpdatedAt) em todas as tabelas**
   - `created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()`
   - `updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()`
   - Auto-update triggers for `updated_at`

## Schema Highlights

### Data Integrity
- String fields validated (not empty, length limits)
- Numeric fields validated (positive, range checks)
- JSON fields (traits) default to empty object
- Unique constraints (chapter order per project, plot point per chapter)

### Performance
- Indexes on all foreign keys
- Composite indexes for common query patterns
- Vector index (IVFFlat) for semantic search
- Materialized view for project statistics

### Extensibility
- JSONB field for character traits (flexible schema)
- pgvector extension for embeddings (AI-ready)
- Entity type enum for polymorphic relationships
- Comments on all objects for maintainability

## Next Steps

Per the backlog:
- **US063**: Execute migrations on local database
- **US064**: Verify schema integrity
- **US065**: Ensure all indexes and constraints are active

## Notes

- Migrations are in `supabase/init/` directory
- Docker Compose will auto-execute files in `docker-entrypoint-initdb.d`
- Files must be numbered (00, 01, 02) for correct execution order
- Vector index (IVFFlat) requires tuning parameter `lists` based on data size
