# Supabase Local Setup

Este diretório contém a configuração para executar Supabase localmente via Docker.

## Serviços Incluídos

- **PostgreSQL 15** com pgvector extension (porta 54322)
- **Supabase Studio** - Interface web de administração (porta 54323)
- **Kong** - API Gateway (porta 8000)
- **PostgREST** - API REST automática
- **Realtime** - WebSocket para subscriptions
- **Storage API** - Gerenciamento de arquivos
- **pg_meta** - Metadados do banco

## Quick Start

### 1. Subir os serviços

```bash
docker-compose up -d
```

### 2. Aguardar inicialização (30-60 segundos)

Verificar status:
```bash
docker-compose ps
```

### 3. Acessar Supabase Studio

Abrir no navegador: http://localhost:54323

### 4. Verificar PostgreSQL

Testar conexão direta:
```bash
psql -h localhost -p 54322 -U postgres -d postgres
# Senha: postgres
```

## Verificar pgvector

Após conectar ao banco, executar:

```sql
-- Habilitar extensão
CREATE EXTENSION IF NOT EXISTS vector;

-- Confirmar instalação
SELECT * FROM pg_extension WHERE extname = 'vector';
```

Deve retornar uma linha com informações da extensão vector.

## Endpoints da API

- **REST API**: http://localhost:8000/rest/v1
- **Realtime**: ws://localhost:8000/realtime/v1
- **Storage**: http://localhost:8000/storage/v1
- **Auth**: http://localhost:8000/auth/v1

## Credenciais (Desenvolvimento)

**PostgreSQL:**
- Host: localhost
- Port: 54322
- User: postgres
- Password: postgres
- Database: postgres

**Supabase API Keys:**
- Anon Key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6ImFub24iLCJleHAiOjE5ODM4MTI5OTZ9.CRXP1A7WOeoJeXxjNni43kdQwgnWNReilDMblYTn_I0`
- Service Role Key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImV4cCI6MTk4MzgxMjk5Nn0.EGIM96RAZx35lJzdJsyH-qQwv8Hdp7fsn3W0YpN81IU`

⚠️ **ATENÇÃO:** Estas credenciais são APENAS para desenvolvimento local. Nunca use em produção!

## Parar os serviços

```bash
docker-compose down
```

## Limpar dados (reset completo)

```bash
docker-compose down -v
```

## Troubleshooting

### Porta já em uso

Se as portas 54322, 54323 ou 8000 já estiverem em uso, edite o `docker-compose.yml` e altere as portas externas.

### Logs dos serviços

```bash
# Ver todos os logs
docker-compose logs

# Ver logs de um serviço específico
docker-compose logs db
docker-compose logs studio
```

### Serviços não inicializam

1. Verificar se Docker está rodando
2. Verificar espaço em disco
3. Consultar logs: `docker-compose logs`

## Estrutura de Volumes

Os dados são persistidos em volumes Docker:
- `db-data`: Dados do PostgreSQL
- `storage-data`: Arquivos do Storage API

## Próximos Passos

Após o Supabase estar rodando:
1. Criar migrations para as tabelas do projeto (US062)
2. Executar migrations (US063)
3. Verificar integridade dos schemas (US064)
4. Criar índices e constraints (US065)
