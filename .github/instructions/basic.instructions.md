---
applyTo: '**'
---
 - Voce pode compilar e executar os testes, mas quem coloca os servicos no ar para testar eh o usuario. Peca sempre para ele.
 - Use o Chrome Devtools para testar a interface, console logs, network calls, etc, se necessario
 - Use o MongoDB Mcp Server se necessario
 - Os logs estao na pasta logs. Verifique o nome do arquivo que voce quer consultar antes.
 - NUNCA USE O TERMINAL PARA FAZER APPEND DE ARQUIVOS!
 - Quando for executar o build ou o lint do frontend nunca use --silent. Verifique o caminho correto antes de executar
 - NAO TEMOS TESTES UNITARIOS PARA FRONTEND AINDA.
 - NAO CRIE NENHUM RESUMO, README, DOCUMENTACAO, ETC Que nao tenha sido solicitada. Ex. Vou criar um Readme documentando a Implementacao. Vou criar um documento mostrando como corrigi esse erro.
 - SEMPRE LEIA lessons.instructions.md para nao cometer os erros aprendidos.
 - Para decisões arquiteturais importantes, crie ADR (Architecture Decision Record) em docs/architecture/ADRs/ seguindo o formato: Status, Contexto, Decisão, Consequências.
 - **IMPORTANTE**: Nenhuma estimativa de tempo ou story points deve ser feita, em nenhuma hipotese.
 - **OBRIGATORIO**:  PROIBIDO usar qualquer comando no no terminal para editar arquivos

## GERENCIAMENTO DE SERVIÇOS

### Script manage-services.sh

**Comandos disponíveis:**
- `./manage-services.sh status` - Mostrar status dos serviços
- `./manage-services.sh start [service]` - Iniciar serviço(s): all, backend, frontend
- `./manage-services.sh stop [service]` - Parar serviço(s)
- `./manage-services.sh restart [service]` - Reiniciar serviço(s)
- `./manage-services.sh rebuild [service]` - Recompilar e reiniciar serviço(s)
- `./manage-services.sh logs <service> [type]` - Ver logs (runtime, build)

**Exemplos:**
```bash
./manage-services.sh restart backend
./manage-services.sh rebuild all
./manage-services.sh logs backend runtime
```

### ⚠️ ALERTA CRÍTICO SOBRE LOGS

**NUNCA use `tail -f` ou comandos interativos via parâmetros no terminal!**

Comandos que ficam travados esperando input (tail -f, grep sem saída) NÃO FUNCIONAM corretamente via run_in_terminal.

✅ **CORRETO:**
```bash
./manage-services.sh logs backend runtime  # Usa tail -100 internamente
tail -100 logs/backend-runtime.log  # Mostra últimas 100 linhas
cat logs/backend-runtime.log | grep "ERROR" | head -20
```

❌ **INCORRETO:**
```bash
tail -f logs/backend-runtime.log  # Fica travado!
grep "pattern" arquivo  # Sem saída definida, pode travar
```
 