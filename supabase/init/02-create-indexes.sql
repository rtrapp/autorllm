-- Migration: Create indexes and constraints for performance optimization
-- Created: 2026-01-26
-- Description: Adds indexes for common queries and optimizes vector search

-- Drop existing indexes
DROP INDEX IF EXISTS idx_projects_created_at CASCADE;
DROP INDEX IF EXISTS idx_projects_updated_at CASCADE;
DROP INDEX IF EXISTS idx_characters_project_id CASCADE;
DROP INDEX IF EXISTS idx_characters_role CASCADE;
DROP INDEX IF EXISTS idx_characters_project_role CASCADE;
DROP INDEX IF EXISTS idx_locations_project_id CASCADE;
DROP INDEX IF EXISTS idx_plots_project_id CASCADE;
DROP INDEX IF EXISTS idx_plots_type CASCADE;
DROP INDEX IF EXISTS idx_plots_project_type CASCADE;
DROP INDEX IF EXISTS idx_chapters_project_id CASCADE;
DROP INDEX IF EXISTS idx_chapters_order CASCADE;
DROP INDEX IF EXISTS idx_chapters_project_order CASCADE;
DROP INDEX IF EXISTS idx_plot_points_plot_id CASCADE;
DROP INDEX IF EXISTS idx_plot_points_chapter_id CASCADE;
DROP INDEX IF EXISTS idx_plot_points_plot_chapter CASCADE;
DROP INDEX IF EXISTS idx_chapters_embedding CASCADE;

-- =============================================================================
-- Indexes for Projects
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_projects_created_at ON projects(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_projects_updated_at ON projects(updated_at DESC);

COMMENT ON INDEX idx_projects_created_at IS 'For sorting projects by creation date';
COMMENT ON INDEX idx_projects_updated_at IS 'For sorting projects by last update';

-- =============================================================================
-- Indexes for Characters
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_characters_project_id ON characters(project_id);
CREATE INDEX IF NOT EXISTS idx_characters_role ON characters(role);
CREATE INDEX IF NOT EXISTS idx_characters_project_role ON characters(project_id, role);

COMMENT ON INDEX idx_characters_project_id IS 'Most common query: get all characters for a project';
COMMENT ON INDEX idx_characters_role IS 'For filtering by character role';
COMMENT ON INDEX idx_characters_project_role IS 'Composite index for role filtering within a project';

-- =============================================================================
-- Indexes for Locations
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_locations_project_id ON locations(project_id);

COMMENT ON INDEX idx_locations_project_id IS 'Get all locations for a project';

-- =============================================================================
-- Indexes for Plots
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_plots_project_id ON plots(project_id);
CREATE INDEX IF NOT EXISTS idx_plots_type ON plots(type);
CREATE INDEX IF NOT EXISTS idx_plots_project_type ON plots(project_id, type);

COMMENT ON INDEX idx_plots_project_id IS 'Get all plots for a project';
COMMENT ON INDEX idx_plots_type IS 'Filter by plot type (Main/SubPlot)';
COMMENT ON INDEX idx_plots_project_type IS 'Composite index for type filtering within a project';

-- =============================================================================
-- Indexes for Chapters
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_chapters_project_id ON chapters(project_id);
CREATE INDEX IF NOT EXISTS idx_chapters_project_order ON chapters(project_id, "order");
CREATE INDEX IF NOT EXISTS idx_chapters_updated_at ON chapters(updated_at DESC);

COMMENT ON INDEX idx_chapters_project_id IS 'Get all chapters for a project';
COMMENT ON INDEX idx_chapters_project_order IS 'CRITICAL: Get chapters in correct order (most frequent query)';
COMMENT ON INDEX idx_chapters_updated_at IS 'For finding recently edited chapters';

-- =============================================================================
-- Indexes for PlotPoints
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_plot_points_plot_id ON plot_points(plot_id);
CREATE INDEX IF NOT EXISTS idx_plot_points_chapter_id ON plot_points(chapter_id);
CREATE INDEX IF NOT EXISTS idx_plot_points_plot_chapter ON plot_points(plot_id, chapter_id);

COMMENT ON INDEX idx_plot_points_plot_id IS 'Get all points for a specific plot';
COMMENT ON INDEX idx_plot_points_chapter_id IS 'Get all plots mentioned in a chapter';
COMMENT ON INDEX idx_plot_points_plot_chapter IS 'Composite index for the unique constraint query';

-- =============================================================================
-- Indexes for Embeddings (Vector Store)
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_embeddings_entity_type ON embeddings(entity_type);
CREATE INDEX IF NOT EXISTS idx_embeddings_entity_id ON embeddings(entity_id);
CREATE INDEX IF NOT EXISTS idx_embeddings_entity_type_id ON embeddings(entity_type, entity_id);

-- Vector similarity search index using IVFFlat algorithm
-- Note: This requires the table to have data. If empty, index creation might be deferred.
-- cosine distance operator (<=>)
CREATE INDEX IF NOT EXISTS idx_embeddings_vector_cosine 
    ON embeddings USING ivfflat (vector extensions.vector_cosine_ops)
    WITH (lists = 100);

COMMENT ON INDEX idx_embeddings_entity_type IS 'Filter embeddings by type (Character/Plot/Chapter)';
COMMENT ON INDEX idx_embeddings_entity_id IS 'Find embedding for a specific entity';
COMMENT ON INDEX idx_embeddings_entity_type_id IS 'Composite index for entity lookup';
COMMENT ON INDEX idx_embeddings_vector_cosine IS 'CRITICAL: Vector similarity search using cosine distance (pgvector IVFFlat)';

-- =============================================================================
-- Functions for Automatic Triggers
-- =============================================================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION update_updated_at_column IS 'Automatically updates updated_at on row modification';

-- =============================================================================
-- Triggers for auto-updating updated_at
-- =============================================================================

CREATE TRIGGER trigger_projects_updated_at
    BEFORE UPDATE ON projects
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_characters_updated_at
    BEFORE UPDATE ON characters
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_locations_updated_at
    BEFORE UPDATE ON locations
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_plots_updated_at
    BEFORE UPDATE ON plots
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_chapters_updated_at
    BEFORE UPDATE ON chapters
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

COMMENT ON TRIGGER trigger_projects_updated_at ON projects IS 'Auto-update updated_at on modification';
COMMENT ON TRIGGER trigger_characters_updated_at ON characters IS 'Auto-update updated_at on modification';
COMMENT ON TRIGGER trigger_locations_updated_at ON locations IS 'Auto-update updated_at on modification';
COMMENT ON TRIGGER trigger_plots_updated_at ON plots IS 'Auto-update updated_at on modification';
COMMENT ON TRIGGER trigger_chapters_updated_at ON chapters IS 'Auto-update updated_at on modification';

-- =============================================================================
-- Function for Word Count Auto-calculation
-- =============================================================================

CREATE OR REPLACE FUNCTION calculate_word_count()
RETURNS TRIGGER AS $$
BEGIN
    -- Simple word count: split by whitespace
    NEW.word_count = array_length(regexp_split_to_array(TRIM(NEW.content), E'\\s+'), 1);
    
    -- Handle empty content
    IF NEW.content IS NULL OR LENGTH(TRIM(NEW.content)) = 0 THEN
        NEW.word_count = 0;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION calculate_word_count IS 'Automatically calculates word_count for chapter content';

-- Trigger for word count on chapters
CREATE TRIGGER trigger_chapters_word_count
    BEFORE INSERT OR UPDATE OF content ON chapters
    FOR EACH ROW
    EXECUTE FUNCTION calculate_word_count();

COMMENT ON TRIGGER trigger_chapters_word_count ON chapters IS 'Auto-calculate word_count when content changes';

-- =============================================================================
-- Function for Semantic Search (RAG)
-- =============================================================================

CREATE OR REPLACE FUNCTION search_embeddings(
    query_vector extensions.vector(384),
    entity_type_filter text DEFAULT NULL,
    similarity_threshold float DEFAULT 0.5,
    max_results int DEFAULT 10
)
RETURNS TABLE (
    id UUID,
    entity_type entity_type,
    entity_id UUID,
    content TEXT,
    similarity float
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        e.id,
        e.entity_type,
        e.entity_id,
        e.content,
        1 - (e.vector <=> query_vector) AS similarity
    FROM embeddings e
    WHERE 
        (entity_type_filter IS NULL OR e.entity_type::text = entity_type_filter)
        AND (1 - (e.vector <=> query_vector)) >= similarity_threshold
    ORDER BY e.vector <=> query_vector
    LIMIT max_results;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION search_embeddings IS 'Semantic search function for RAG. Returns entities similar to query_vector with cosine similarity';

-- =============================================================================
-- Statistics and Analysis Views (Optional but useful)
-- =============================================================================

-- View: Project summary statistics
CREATE OR REPLACE VIEW project_stats AS
SELECT 
    p.id,
    p.title,
    p.author,
    COUNT(DISTINCT c.id) AS character_count,
    COUNT(DISTINCT l.id) AS location_count,
    COUNT(DISTINCT pl.id) AS plot_count,
    COUNT(DISTINCT ch.id) AS chapter_count,
    COALESCE(SUM(ch.word_count), 0) AS total_word_count,
    p.created_at,
    p.updated_at
FROM projects p
LEFT JOIN characters c ON c.project_id = p.id
LEFT JOIN locations l ON l.project_id = p.id
LEFT JOIN plots pl ON pl.project_id = p.id
LEFT JOIN chapters ch ON ch.project_id = p.id
GROUP BY p.id, p.title, p.author, p.created_at, p.updated_at;

COMMENT ON VIEW project_stats IS 'Aggregated statistics for each project';

-- Grant permissions on the view
GRANT SELECT ON project_stats TO postgres, authenticated, service_role, anon;
