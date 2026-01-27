-- Migration: Create main tables for Autor LLM
-- Created: 2026-01-26
-- Description: Creates all core tables following the schema design

-- Enable uuid-ossp if not already enabled (should be from 00-initial-schema.sql)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Drop existing tables (cascade to handle foreign keys)
DROP TABLE IF EXISTS plot_points CASCADE;
DROP TABLE IF EXISTS chapters CASCADE;
DROP TABLE IF EXISTS plots CASCADE;
DROP TABLE IF EXISTS locations CASCADE;
DROP TABLE IF EXISTS characters CASCADE;
DROP TABLE IF EXISTS projects CASCADE;

-- Drop existing types
DROP TYPE IF EXISTS character_role CASCADE;
DROP TYPE IF EXISTS plot_type CASCADE;

-- =============================================================================
-- 1. Projects Table
-- =============================================================================
CREATE TABLE IF NOT EXISTS projects (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    title VARCHAR(200) NOT NULL,
    author VARCHAR(100) NOT NULL,
    synopsis TEXT,
    genre VARCHAR(50),
    target_word_count INTEGER NOT NULL DEFAULT 50000,
    current_word_count INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT projects_title_not_empty CHECK (LENGTH(TRIM(title)) > 0),
    CONSTRAINT projects_author_not_empty CHECK (LENGTH(TRIM(author)) > 0),
    CONSTRAINT projects_synopsis_length CHECK (synopsis IS NULL OR LENGTH(synopsis) <= 5000),
    CONSTRAINT projects_genre_length CHECK (genre IS NULL OR LENGTH(genre) <= 50),
    CONSTRAINT projects_target_word_count_positive CHECK (target_word_count >= 0),
    CONSTRAINT projects_current_word_count_nonnegative CHECK (current_word_count >= 0)
);

COMMENT ON TABLE projects IS 'Represents a book project with all its metadata';
COMMENT ON COLUMN projects.id IS 'Unique identifier (UUID)';
COMMENT ON COLUMN projects.title IS 'Book title (max 200 chars)';
COMMENT ON COLUMN projects.author IS 'Author name (max 100 chars)';
COMMENT ON COLUMN projects.synopsis IS 'Brief book synopsis (max 5000 chars)';
COMMENT ON COLUMN projects.genre IS 'Book genre (max 50 chars)';
COMMENT ON COLUMN projects.target_word_count IS 'Target word count for the book';
COMMENT ON COLUMN projects.current_word_count IS 'Current total word count';

-- =============================================================================
-- 2. Characters Table
-- =============================================================================
CREATE TYPE character_role AS ENUM ('Protagonist', 'Antagonist', 'Supporting', 'Minor');

CREATE TABLE IF NOT EXISTS characters (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    project_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    role character_role NOT NULL DEFAULT 'Supporting',
    backstory TEXT,
    appearance TEXT,
    personality TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT characters_name_not_empty CHECK (LENGTH(TRIM(name)) > 0),
    CONSTRAINT characters_description_length CHECK (LENGTH(description) <= 1000),
    CONSTRAINT characters_backstory_length CHECK (backstory IS NULL OR LENGTH(backstory) <= 5000),
    CONSTRAINT characters_appearance_length CHECK (appearance IS NULL OR LENGTH(appearance) <= 2000),
    CONSTRAINT characters_personality_length CHECK (personality IS NULL OR LENGTH(personality) <= 2000),
    CONSTRAINT fk_characters_project FOREIGN KEY (project_id) 
        REFERENCES projects(id) ON DELETE CASCADE
);

COMMENT ON TABLE characters IS 'Book characters with detailed attributes';
COMMENT ON COLUMN characters.description IS 'Brief character description (max 1000 chars)';
COMMENT ON COLUMN characters.backstory IS 'Detailed character backstory (max 5000 chars)';
COMMENT ON COLUMN characters.appearance IS 'Physical appearance description (max 2000 chars)';
COMMENT ON COLUMN characters.personality IS 'Personality traits and behaviors (max 2000 chars)';

-- =============================================================================
-- 3. Locations Table
-- =============================================================================
CREATE TABLE IF NOT EXISTS locations (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    project_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    geography TEXT,
    culture TEXT,
    significance TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT locations_name_not_empty CHECK (LENGTH(TRIM(name)) > 0),
    CONSTRAINT locations_description_length CHECK (LENGTH(description) <= 1000),
    CONSTRAINT locations_geography_length CHECK (geography IS NULL OR LENGTH(geography) <= 2000),
    CONSTRAINT locations_culture_length CHECK (culture IS NULL OR LENGTH(culture) <= 2000),
    CONSTRAINT locations_significance_length CHECK (significance IS NULL OR LENGTH(significance) <= 1000),
    CONSTRAINT fk_locations_project FOREIGN KEY (project_id) 
        REFERENCES projects(id) ON DELETE CASCADE
);

COMMENT ON TABLE locations IS 'Story locations and settings';
COMMENT ON COLUMN locations.description IS 'Brief location description (max 1000 chars)';
COMMENT ON COLUMN locations.geography IS 'Geographic details and physical characteristics (max 2000 chars)';
COMMENT ON COLUMN locations.culture IS 'Cultural aspects and social dynamics (max 2000 chars)';
COMMENT ON COLUMN locations.significance IS 'Narrative significance and role in story (max 1000 chars)';

-- =============================================================================
-- 4. Plots Table
-- =============================================================================
CREATE TYPE plot_type AS ENUM ('Main', 'Subplot', 'Character Arc', 'Romance', 'Mystery');

CREATE TABLE IF NOT EXISTS plots (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    project_id UUID NOT NULL,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    type plot_type NOT NULL DEFAULT 'Subplot',
    resolution TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT plots_title_not_empty CHECK (LENGTH(TRIM(title)) > 0),
    CONSTRAINT plots_description_length CHECK (LENGTH(description) <= 2000),
    CONSTRAINT plots_resolution_length CHECK (resolution IS NULL OR LENGTH(resolution) <= 2000),
    CONSTRAINT fk_plots_project FOREIGN KEY (project_id) 
        REFERENCES projects(id) ON DELETE CASCADE
);

COMMENT ON TABLE plots IS 'Plot lines and story arcs';
COMMENT ON COLUMN plots.title IS 'Plot title (max 200 chars)';
COMMENT ON COLUMN plots.type IS 'Type of plot (Main, Subplot, Character Arc, etc)';
COMMENT ON COLUMN plots.resolution IS 'Plot resolution/conclusion (max 2000 chars)';
COMMENT ON COLUMN plots.is_active IS 'Whether this plot is currently active in the story';

-- =============================================================================
-- 5. Chapters Table
-- =============================================================================
CREATE TABLE IF NOT EXISTS chapters (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    project_id UUID NOT NULL,
    title VARCHAR(200) NOT NULL,
    summary TEXT,
    content TEXT NOT NULL DEFAULT '',
    "order" INTEGER NOT NULL,
    word_count INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT chapters_title_not_empty CHECK (LENGTH(TRIM(title)) > 0),
    CONSTRAINT chapters_summary_length CHECK (summary IS NULL OR LENGTH(summary) <= 1000),
    CONSTRAINT chapters_order_positive CHECK ("order" > 0),
    CONSTRAINT chapters_word_count_nonnegative CHECK (word_count >= 0),
    CONSTRAINT fk_chapters_project FOREIGN KEY (project_id) 
        REFERENCES projects(id) ON DELETE CASCADE,
    CONSTRAINT chapters_unique_order_per_project UNIQUE (project_id, "order")
);

COMMENT ON TABLE chapters IS 'Book chapters with content';
COMMENT ON COLUMN chapters."order" IS 'Chapter sequence number (unique per project)';
COMMENT ON COLUMN chapters.word_count IS 'Automatically calculated word count';

-- =============================================================================
-- 6. PlotPoints Table
-- =============================================================================
CREATE TABLE IF NOT EXISTS plot_points (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    plot_id UUID NOT NULL,
    chapter_id UUID NOT NULL,
    intensity INTEGER NOT NULL DEFAULT 5,
    description VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT plot_points_intensity_range CHECK (intensity >= 0 AND intensity <= 10),
    CONSTRAINT fk_plot_points_plot FOREIGN KEY (plot_id) 
        REFERENCES plots(id) ON DELETE CASCADE,
    CONSTRAINT fk_plot_points_chapter FOREIGN KEY (chapter_id) 
        REFERENCES chapters(id) ON DELETE CASCADE,
    CONSTRAINT plot_points_unique_plot_chapter UNIQUE (plot_id, chapter_id)
);

COMMENT ON TABLE plot_points IS 'Plot intensity markers in chapters for narrative arc visualization';
COMMENT ON COLUMN plot_points.intensity IS 'Intensity from 0 (absent) to 10 (climax)';
COMMENT ON CONSTRAINT plot_points_unique_plot_chapter ON plot_points IS 'One plot can only have one point per chapter';

-- =============================================================================
-- 7. Embeddings Table (Vector Store for RAG)
-- =============================================================================
CREATE TYPE entity_type AS ENUM ('Character', 'Plot', 'Chapter');

CREATE TABLE IF NOT EXISTS embeddings (
    id UUID PRIMARY KEY DEFAULT extensions.uuid_generate_v4(),
    entity_type entity_type NOT NULL,
    entity_id UUID NOT NULL,
    content TEXT NOT NULL,
    vector extensions.vector(384) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT embeddings_content_not_empty CHECK (LENGTH(TRIM(content)) > 0)
);

COMMENT ON TABLE embeddings IS 'Vector embeddings for RAG (Retrieval-Augmented Generation)';
COMMENT ON COLUMN embeddings.vector IS 'pgvector embedding (384 dimensions for all-MiniLM-L6-v2)';
COMMENT ON COLUMN embeddings.entity_id IS 'Polymorphic FK to Character, Plot, or Chapter';

-- =============================================================================
-- Grant Permissions
-- =============================================================================
GRANT ALL ON TABLE projects TO postgres, authenticated, service_role;
GRANT ALL ON TABLE characters TO postgres, authenticated, service_role;
GRANT ALL ON TABLE locations TO postgres, authenticated, service_role;
GRANT ALL ON TABLE plots TO postgres, authenticated, service_role;
GRANT ALL ON TABLE chapters TO postgres, authenticated, service_role;
GRANT ALL ON TABLE plot_points TO postgres, authenticated, service_role;
GRANT ALL ON TABLE embeddings TO postgres, authenticated, service_role;

-- Allow anon role to read (for potential public access later)
GRANT SELECT ON TABLE projects TO anon;
GRANT SELECT ON TABLE characters TO anon;
GRANT SELECT ON TABLE locations TO anon;
GRANT SELECT ON TABLE plots TO anon;
GRANT SELECT ON TABLE chapters TO anon;
GRANT SELECT ON TABLE plot_points TO anon;
