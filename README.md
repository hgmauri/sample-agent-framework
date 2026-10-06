# sample-agent-framework

Exemplo de agente de IA com o **Microsoft Agent Framework** no .NET 10: um assistente de atendimento de logística que consulta pedidos, calcula prazos de entrega, mantém o contexto da conversa e é monitorado com OpenTelemetry.

Artigo completo: [Construindo agentes de IA com o Microsoft Agent Framework no .NET](https://henriquemauri.net/construindo-agentes-de-ia-com-o-microsoft-agent-framework-no-net/)

## O que o exemplo mostra

- Criação de um agente com `AIProjectClient` e `AsAIAgent` (Microsoft Foundry)
- Ferramentas (function calling) com `AIFunctionFactory` e `[Description]`
- Conversa com múltiplos turnos usando `AgentSession`
- Resposta em streaming com `RunStreamingAsync`
- Observabilidade com `UseOpenTelemetry`

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Projeto no [Microsoft Foundry](https://ai.azure.com) com um modelo publicado (ex.: `gpt-4o-mini`)
- Azure CLI autenticado (`az login`)

## Como executar

```bash
export AZURE_OPENAI_ENDPOINT="https://seu-projeto.services.ai.azure.com"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"

# Opcional: exibe os traces do OpenTelemetry no console
export HABILITAR_TRACES=true

dotnet run --project src/Logistica.Agente
```

Exemplo de conversa:

```
Você: Qual o status do pedido PED-1003?
Agente: O pedido PED-1003 está aguardando coleta, com destino a Belo Horizonte/MG.

Você: E qual o prazo de entrega para o CEP 30110000?
Agente: Para o CEP 30110000, o prazo estimado é de 2 dias úteis após a coleta.
```

## Observações

- O pacote `Microsoft.Agents.AI.Foundry` ainda é publicado como *prerelease*. O `.csproj` usa a versão flutuante `1.*-*`; após o primeiro `dotnet restore`, recomenda-se fixar a versão resolvida.
- `DefaultAzureCredential` é usado para facilitar o desenvolvimento. Em produção, prefira `ManagedIdentityCredential`.
- Os dados de pedidos são simulados em memória na classe `PedidoTools`.

## Referências

- [Microsoft Agent Framework (GitHub)](https://github.com/microsoft/agent-framework)
- [Documentação oficial](https://learn.microsoft.com/agent-framework/)
