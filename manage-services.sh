#!/bin/bash

# Script interativo para gerenciar todos os serviços do AutorLLM
# Uso: ./manage-services.sh

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color
BOLD='\033[1m'

# Configurações de portas
BACKEND_PORT=5011
FRONTEND_PORT=5173
POSTGRES_PORT=54322
STUDIO_PORT=54323

# Diretórios dos serviços
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACKEND_DIR="$SCRIPT_DIR/src/backend/AutorLLM.Api"
FRONTEND_DIR="$SCRIPT_DIR/src/frontend"

# Log files
LOGS_DIR="$SCRIPT_DIR/logs"
mkdir -p "$LOGS_DIR"

# PIDs file
PIDS_FILE="$LOGS_DIR/service-pids.txt"

# Função para log colorido
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[✓]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[⚠]${NC} $1"
}

log_error() {
    echo -e "${RED}[✗]${NC} $1"
}

log_step() {
    echo -e "${CYAN}${BOLD}► $1${NC}"
}

# Função para salvar PIDs
save_pids() {
    local service=$1
    local pid=$2
    
    # Criar arquivo se não existir
    touch "$PIDS_FILE"
    
    # Remover linha antiga do serviço se existir
    sed -i "/^$service=/d" "$PIDS_FILE" 2>/dev/null || true
    
    # Adicionar nova linha
    echo "$service=$pid" >> "$PIDS_FILE"
}

# Função para obter PID de um serviço
get_pid() {
    local service=$1
    
    if [ -f "$PIDS_FILE" ]; then
        grep "^$service=" "$PIDS_FILE" 2>/dev/null | cut -d'=' -f2
    fi
}

# Função para limpar PID de um serviço
clear_pid() {
    local service=$1
    
    if [ -f "$PIDS_FILE" ]; then
        sed -i "/^$service=/d" "$PIDS_FILE" 2>/dev/null || true
    fi
}

# Função para matar processos em uma porta específica
kill_port() {
    local PORT=$1
    local SERVICE_NAME=$2
    
    # Busca PIDs usando a porta
    local PIDS=$(lsof -ti:$PORT 2>/dev/null || true)
    
    if [ -n "$PIDS" ]; then
        log_warning "Encerrando processos na porta $PORT ($SERVICE_NAME)..."
        echo "$PIDS" | xargs kill -9 2>/dev/null || true
        sleep 1
        log_success "Porta $PORT liberada"
    fi
}

# Função para verificar se um serviço está rodando
is_service_running() {
    local SERVICE=$1
    
    # Primeiro verifica se existe um PID salvo
    local PID=$(get_pid "$SERVICE")
    
    if [ -n "$PID" ]; then
        # Verifica se o processo ainda está vivo
        if kill -0 "$PID" 2>/dev/null; then
            return 0
        else
            # PID morto, limpar
            clear_pid "$SERVICE"
        fi
    fi
    
    # Fallback: verificar pela porta ou container
    case $SERVICE in
        postgres)
            docker ps | grep -q "autor_llm_postgres" && return 0
            ;;
        backend)
            lsof -Pi :$BACKEND_PORT -t >/dev/null 2>&1 && return 0
            ;;
        frontend)
            lsof -Pi :$FRONTEND_PORT -t >/dev/null 2>&1 && return 0
            ;;
    esac
    
    return 1
}

# Função para verificar se um serviço está respondendo
check_service_health() {
    local PORT=$1
    local MAX_ATTEMPTS=30
    local ATTEMPT=0
    
    while [ $ATTEMPT -lt $MAX_ATTEMPTS ]; do
        if curl -f -s -o /dev/null "http://localhost:$PORT" 2>/dev/null || \
           curl -f -s -o /dev/null "http://localhost:$PORT/health" 2>/dev/null || \
           curl -s -o /dev/null "http://localhost:$PORT" 2>/dev/null; then
            return 0
        fi
        
        if lsof -Pi :$PORT -t >/dev/null 2>&1; then
            return 0
        fi
        
        ATTEMPT=$((ATTEMPT + 1))
        sleep 1
    done
    
    return 1
}

# Função para exibir status dos serviços
show_status() {
    clear
    echo ""
    echo -e "${CYAN}${BOLD}╔═══════════════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}${BOLD}║                     Status dos Serviços AutorLLM                              ║${NC}"
    echo -e "${CYAN}${BOLD}╚═══════════════════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    
    # Cabeçalho
    echo -e "${BOLD}SERVIÇO              STATUS          PID        URL${NC}"
    echo "────────────────────────────────────────────────────────────────────────────────────"
    
    # PostgreSQL
    if is_service_running "postgres"; then
        echo -e "PostgreSQL           ${GREEN}● RODANDO${NC}       -          localhost:$POSTGRES_PORT"
    else
        echo -e "PostgreSQL           ${RED}● PARADO${NC}        -          -"
    fi
    
    # Backend API
    if is_service_running "backend"; then
        local backend_pid=$(get_pid "backend")
        echo -e "Backend API          ${GREEN}● RODANDO${NC}       $backend_pid      http://localhost:$BACKEND_PORT"
    else
        echo -e "Backend API          ${RED}● PARADO${NC}        -          -"
    fi
    
    # Frontend
    if is_service_running "frontend"; then
        local frontend_pid=$(get_pid "frontend")
        echo -e "Frontend             ${GREEN}● RODANDO${NC}       $frontend_pid      http://localhost:$FRONTEND_PORT"
    else
        echo -e "Frontend             ${RED}● PARADO${NC}        -          -"
    fi
    
    echo ""
}

# Função para parar um serviço específico
stop_service() {
    local SERVICE=$1
    local SERVICE_NAME=""
    
    case $SERVICE in
        postgres)
            SERVICE_NAME="PostgreSQL"
            log_step "Parando PostgreSQL (Docker)..."
            cd "$SCRIPT_DIR"
            docker-compose stop db 2>/dev/null || true
            log_success "PostgreSQL parado"
            return 0
            ;;
        backend)
            SERVICE_NAME="Backend API"
            log_step "Parando Backend API..."
            
            # Tentar usar o PID salvo primeiro
            local PID=$(get_pid "$SERVICE")
            if [ -n "$PID" ] && kill -0 "$PID" 2>/dev/null; then
                kill -9 "$PID" 2>/dev/null || true
                sleep 1
            fi
            
            # Matar pela porta como fallback
            kill_port $BACKEND_PORT "$SERVICE_NAME"
            
            # Limpar PID salvo
            clear_pid "$SERVICE"
            
            log_success "Backend API parado"
            ;;
        frontend)
            SERVICE_NAME="Frontend"
            log_step "Parando Frontend..."
            
            # Tentar usar o PID salvo primeiro
            local PID=$(get_pid "$SERVICE")
            if [ -n "$PID" ] && kill -0 "$PID" 2>/dev/null; then
                kill -9 "$PID" 2>/dev/null || true
                sleep 1
            fi
            
            # Matar processos Vite específicos
            pkill -f "vite" 2>/dev/null || true
            
            # Matar pela porta como fallback
            kill_port $FRONTEND_PORT "$SERVICE_NAME"
            
            # Limpar PID salvo
            clear_pid "$SERVICE"
            
            log_success "Frontend parado"
            ;;
        *)
            log_error "Serviço desconhecido: $SERVICE"
            return 1
            ;;
    esac
}

# Função para parar todos os serviços
stop_all_services() {
    echo ""
    log_step "Parando todos os serviços..."
    echo ""
    
    stop_service "frontend"
    stop_service "backend"
    stop_service "postgres"
    
    echo ""
    log_success "Todos os serviços foram parados"
}

# Função para compilar um serviço
build_service() {
    local SERVICE=$1
    
    case $SERVICE in
        postgres)
            log_info "PostgreSQL roda via Docker - nenhum build necessário"
            return 0
            ;;
        backend)
            log_step "Compilando Backend API..."
            cd "$BACKEND_DIR"
            dotnet build > "$LOGS_DIR/backend-build.log" 2>&1
            if [ $? -eq 0 ]; then
                log_success "Backend API compilado com sucesso"
            else
                log_error "Falha ao compilar Backend API (veja $LOGS_DIR/backend-build.log)"
                return 1
            fi
            ;;
        frontend)
            log_step "Instalando dependências do Frontend..."
            cd "$FRONTEND_DIR"
            npm install > "$LOGS_DIR/frontend-install.log" 2>&1
            if [ $? -eq 0 ]; then
                log_success "Dependências instaladas"
            else
                log_error "Falha ao instalar dependências (veja $LOGS_DIR/frontend-install.log)"
                return 1
            fi
            log_info "Frontend rodará em modo desenvolvimento (sem build necessário)"
            ;;
        *)
            log_error "Serviço desconhecido: $SERVICE"
            return 1
            ;;
    esac
    
    return 0
}

# Função para iniciar um serviço específico
start_service() {
    local SERVICE=$1
    local SERVICE_NAME=""
    local PID=0
    
    case $SERVICE in
        postgres)
            SERVICE_NAME="PostgreSQL"
            log_step "Iniciando PostgreSQL (Docker)..."
            cd "$SCRIPT_DIR"
            docker-compose up -d db
            if [ $? -eq 0 ]; then
                log_success "PostgreSQL iniciado"
                log_info "Aguardando PostgreSQL inicializar..."
                sleep 5
                if docker ps | grep -q "autor_llm_postgres"; then
                    log_success "PostgreSQL está rodando na porta $POSTGRES_PORT"
                else
                    log_error "PostgreSQL não iniciou corretamente"
                    return 1
                fi
            else
                log_error "Falha ao iniciar PostgreSQL"
                return 1
            fi
            ;;
        backend)
            SERVICE_NAME="Backend API"
            log_step "Iniciando Backend API na porta $BACKEND_PORT..."
            cd "$BACKEND_DIR"
            ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://localhost:$BACKEND_PORT" nohup dotnet run --project AutorLLM.Api.csproj > "$LOGS_DIR/backend-runtime.log" 2>&1 &
            PID=$!
            save_pids "$SERVICE" "$PID"
            log_success "Backend API iniciado (PID: $PID)"
            
            log_info "Aguardando Backend API inicializar..."
            if check_service_health $BACKEND_PORT; then
                log_success "Backend API está respondendo na porta $BACKEND_PORT"
            else
                log_error "Backend API não respondeu em tempo hábil (veja $LOGS_DIR/backend-runtime.log)"
                return 1
            fi
            ;;
        frontend)
            SERVICE_NAME="Frontend"
            log_step "Iniciando Frontend em modo desenvolvimento na porta $FRONTEND_PORT..."
            cd "$FRONTEND_DIR"
            nohup npm run dev -- --port $FRONTEND_PORT --host > "$LOGS_DIR/frontend-runtime.log" 2>&1 &
            PID=$!
            save_pids "$SERVICE" "$PID"
            log_success "Frontend iniciado (PID: $PID)"
            
            log_info "Aguardando Frontend inicializar..."
            if check_service_health $FRONTEND_PORT; then
                log_success "Frontend está respondendo na porta $FRONTEND_PORT"
            else
                log_error "Frontend não respondeu em tempo hábil (veja $LOGS_DIR/frontend-runtime.log)"
                return 1
            fi
            ;;
        *)
            log_error "Serviço desconhecido: $SERVICE"
            return 1
            ;;
    esac
    
    return 0
}

# Função para iniciar todos os serviços
start_all_services() {
    echo ""
    log_step "Iniciando todos os serviços..."
    echo ""
    
    local FAILED_SERVICES=()
    
    if ! start_service "postgres"; then
        FAILED_SERVICES+=("PostgreSQL")
    fi
    echo ""
    
    if ! start_service "backend"; then
        FAILED_SERVICES+=("Backend")
    fi
    echo ""
    
    if ! start_service "frontend"; then
        FAILED_SERVICES+=("Frontend")
    fi
    
    echo ""
    if [ ${#FAILED_SERVICES[@]} -eq 0 ]; then
        log_success "Todos os serviços foram iniciados com sucesso!"
    else
        log_warning "Alguns serviços falharam ao iniciar: ${FAILED_SERVICES[*]}"
        log_info "Verifique os logs para mais detalhes"
    fi
    return 0
}

# Função para rebuildar e reiniciar um serviço
rebuild_and_restart_service() {
    local SERVICE=$1
    
    echo ""
    log_step "Rebuilding e reiniciando $SERVICE..."
    echo ""
    
    # Parar serviço
    stop_service "$SERVICE"
    echo ""
    
    # Compilar serviço
    if ! build_service "$SERVICE"; then
        log_error "Falha ao compilar $SERVICE"
        read -p "Pressione ENTER para continuar..."
        return 0
    fi
    echo ""
    
    # Iniciar serviço
    if ! start_service "$SERVICE"; then
        log_error "Falha ao iniciar $SERVICE"
        read -p "Pressione ENTER para continuar..."
        return 0
    fi
    
    echo ""
    log_success "$SERVICE rebuilded e reiniciado com sucesso!"
    read -p "Pressione ENTER para continuar..."
    return 0
}

# Função para rebuildar e reiniciar todos os serviços
rebuild_and_restart_all() {
    echo ""
    log_step "Rebuilding e reiniciando todos os serviços..."
    echo ""
    
    # Parar todos
    stop_all_services
    echo ""
    
    # Compilar todos
    log_step "Compilando todos os serviços..."
    echo ""
    
    local FAILED_BUILD=()
    
    if ! build_service "backend"; then
        log_error "Falha ao compilar Backend API"
        FAILED_BUILD+=("Backend")
    fi
    echo ""
    
    if ! build_service "frontend"; then
        log_error "Falha ao compilar Frontend"
        FAILED_BUILD+=("Frontend")
    fi
    echo ""
    
    # Iniciar todos (mesmo que alguns builds tenham falhado)
    start_all_services
    
    echo ""
    if [ ${#FAILED_BUILD[@]} -eq 0 ]; then
        log_success "Todos os serviços foram rebuilded e reiniciados com sucesso!"
    else
        log_warning "Alguns serviços falharam ao compilar: ${FAILED_BUILD[*]}"
        log_info "Serviços com build bem-sucedido foram iniciados"
    fi
    read -p "Pressione ENTER para continuar..."
    return 0
}

# Submenu para selecionar serviço
show_service_menu() {
    local ACTION=$1
    local ACTION_NAME=$2
    
    clear
    echo ""
    echo -e "${CYAN}${BOLD}╔═══════════════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}${BOLD}║                  Qual serviço você deseja ${ACTION_NAME}?                      ║${NC}"
    echo -e "${CYAN}${BOLD}╚═══════════════════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${BOLD}[0] Todos${NC}"
    echo -e "${BOLD}[1] PostgreSQL${NC} [porta: $POSTGRES_PORT]"
    echo -e "${BOLD}[2] Backend API${NC} [porta: $BACKEND_PORT]"
    echo -e "${BOLD}[3] Frontend${NC} [porta: $FRONTEND_PORT]"
    echo ""
    echo -e "${BOLD}[9] Voltar${NC}"
    echo ""
    read -p "Escolha uma opção: " service_choice
    
    local SERVICE=""
    
    case $service_choice in
        0)
            SERVICE="all"
            ;;
        1)
            SERVICE="postgres"
            ;;
        2)
            SERVICE="backend"
            ;;
        3)
            SERVICE="frontend"
            ;;
        9)
            return
            ;;
        *)
            log_error "Opção inválida"
            read -p "Pressione ENTER para continuar..."
            return
            ;;
    esac
    
    # Executar ação no serviço selecionado
    case $ACTION in
        start)
            if [ "$SERVICE" = "all" ]; then
                start_all_services
            else
                echo ""
                if ! start_service "$SERVICE"; then
                    log_error "Falha ao iniciar $SERVICE"
                fi
                echo ""
            fi
            read -p "Pressione ENTER para continuar..."
            ;;
        stop)
            if [ "$SERVICE" = "all" ]; then
                stop_all_services
            else
                echo ""
                stop_service "$SERVICE"
                echo ""
            fi
            read -p "Pressione ENTER para continuar..."
            ;;
        rebuild)
            if [ "$SERVICE" = "all" ]; then
                rebuild_and_restart_all
            else
                rebuild_and_restart_service "$SERVICE"
            fi
            ;;
        logs)
            if [ "$SERVICE" = "all" ]; then
                log_error "Opção 'Todos' não disponível para logs. Selecione um serviço específico."
                read -p "Pressione ENTER para continuar..."
            else
                show_service_logs "$SERVICE"
            fi
            ;;
    esac
}

# Função para exibir logs de um serviço específico
show_service_logs() {
    local SERVICE=$1
    
    clear
    echo ""
    echo -e "${CYAN}${BOLD}╔═══════════════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}${BOLD}║                      Logs de ${SERVICE}                                        ║${NC}"
    echo -e "${CYAN}${BOLD}╚═══════════════════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    
    local LOG_OPTIONS=()
    local LOG_FILES=()
    
    case $SERVICE in
        postgres)
            echo ""
            log_info "Exibindo logs do PostgreSQL (Docker)"
            log_info "Pressione Ctrl+C para voltar ao menu"
            echo ""
            sleep 2
            docker logs -f autor_llm_postgres 2>&1 || true
            return
            ;;
        backend)
            LOG_OPTIONS=("Runtime" "Build")
            LOG_FILES=("$LOGS_DIR/backend-runtime.log" "$LOGS_DIR/backend-build.log")
            ;;
        frontend)
            LOG_OPTIONS=("Runtime" "Install")
            LOG_FILES=("$LOGS_DIR/frontend-runtime.log" "$LOGS_DIR/frontend-install.log")
            ;;
    esac
    
    for i in "${!LOG_OPTIONS[@]}"; do
        echo -e "${BOLD}[$i] ${LOG_OPTIONS[$i]}${NC}"
    done
    echo ""
    echo -e "${BOLD}[9] Voltar${NC}"
    echo ""
    read -p "Escolha o tipo de log: " log_choice
    
    if [ "$log_choice" = "9" ]; then
        return
    fi
    
    if [ "$log_choice" -ge 0 ] && [ "$log_choice" -lt "${#LOG_FILES[@]}" ]; then
        local LOG_FILE="${LOG_FILES[$log_choice]}"
        
        if [ -f "$LOG_FILE" ]; then
            echo ""
            log_info "Exibindo: $LOG_FILE"
            log_info "Pressione Ctrl+C para voltar ao menu"
            echo ""
            sleep 2
            tail -f "$LOG_FILE" || true
        else
            echo ""
            log_warning "Arquivo de log não encontrado: $LOG_FILE"
            read -p "Pressione ENTER para continuar..."
        fi
    else
        log_error "Opção inválida"
        read -p "Pressione ENTER para continuar..."
    fi
}

# Menu principal
show_menu() {
    show_status
    
    echo -e "${CYAN}${BOLD}╔═══════════════════════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}${BOLD}║                          Gerenciador de Serviços                              ║${NC}"
    echo -e "${CYAN}${BOLD}╚═══════════════════════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${BOLD}(1) Iniciar | (2) Parar | (3) Recompilar e Reiniciar | (4) Ver Logs | (9) Sair${NC}"
    echo ""
    read -p "Escolha uma opção: " choice
    
    case $choice in
        1)
            show_service_menu "start" "Iniciar"
            ;;
        2)
            show_service_menu "stop" "Parar"
            ;;
        3)
            show_service_menu "rebuild" "Recompilar e Reiniciar"
            ;;
        4)
            show_service_menu "logs" "Ver Logs"
            ;;
        9)
            echo ""
            log_info "Saindo..."
            exit 0
            ;;
        *)
            log_error "Opção inválida"
            read -p "Pressione ENTER para continuar..."
            ;;
    esac
}

# Trap Ctrl+C
trap '' SIGINT

# Banner inicial
clear
echo ""
echo -e "${GREEN}${BOLD}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}${BOLD}║         AutorLLM - Service Manager                        ║${NC}"
echo -e "${GREEN}${BOLD}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""
sleep 1

# Loop do menu
while true; do
    show_menu
done
