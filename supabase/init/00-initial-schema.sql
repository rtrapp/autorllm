-- Basic Roles
CREATE ROLE anon NOLOGIN NOINHERIT;
CREATE ROLE authenticated NOLOGIN NOINHERIT;
CREATE ROLE service_role NOLOGIN NOINHERIT BYPASSRLS;
CREATE ROLE authenticator NOINHERIT LOGIN PASSWORD 'postgres';
CREATE ROLE supabase_admin NOINHERIT LOGIN PASSWORD 'postgres' SUPERUSER;
CREATE ROLE dashboard_user NOINHERIT LOGIN PASSWORD 'postgres' SUPERUSER;

GRANT anon TO authenticator;
GRANT authenticated TO authenticator;
GRANT service_role TO authenticator;
GRANT supabase_admin TO authenticator;
GRANT dashboard_user TO authenticator; -- Allow dashboard user to masquerade

-- Schemas
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS storage;
CREATE SCHEMA IF NOT EXISTS realtime;
CREATE SCHEMA IF NOT EXISTS extensions;
CREATE SCHEMA IF NOT EXISTS graphql;
CREATE SCHEMA IF NOT EXISTS graphql_public;

-- Types
CREATE TYPE auth.aal_level AS ENUM ('aal1', 'aal2', 'aal3');
CREATE TYPE auth.code_challenge_method AS ENUM ('s256', 'plain');
CREATE TYPE auth.factor_status AS ENUM ('unverified', 'verified');
CREATE TYPE auth.factor_type AS ENUM ('totp', 'webauthn');

-- Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA extensions;
CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA extensions;
CREATE EXTENSION IF NOT EXISTS pgjwt WITH SCHEMA extensions;
CREATE EXTENSION IF NOT EXISTS vector WITH SCHEMA extensions;

-- Grant usage on schemas
GRANT USAGE ON SCHEMA public TO postgres, anon, authenticated, service_role;
GRANT USAGE ON SCHEMA extensions TO postgres, anon, authenticated, service_role;
GRANT USAGE ON SCHEMA auth TO postgres, anon, authenticated, service_role, supabase_admin;
GRANT USAGE ON SCHEMA storage TO postgres, anon, authenticated, service_role, supabase_admin;
GRANT USAGE ON SCHEMA realtime TO postgres, anon, authenticated, service_role, supabase_admin;

-- Set Default Privileges
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO postgres, anon, authenticated, service_role;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON FUNCTIONS TO postgres, anon, authenticated, service_role;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO postgres, anon, authenticated, service_role;

-- Auth specific setup (minimal to allow gotrue to run migrations)
ALTER ROLE supabase_admin SET search_path TO public, auth, extensions;
ALTER ROLE postgres SET search_path TO public, auth, extensions;

-- Event Triggers for Supabase (simplified)
-- (Normally Supabase has a lot of triggers for realtime, but the realtime service manages some)

-- Allow Realtime to read
GRANT SELECT ON ALL TABLES IN SCHEMA public TO postgres, anon, authenticated, service_role;
