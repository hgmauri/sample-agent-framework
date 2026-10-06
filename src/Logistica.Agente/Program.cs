using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;

const string SourceName = "Logistica.Agente";

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Configure a variável AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";
var habilitarTraces = Environment.GetEnvironmentVariable("HABILITAR_TRACES") == "true";

// Observabilidade com OpenTelemetry (traces no console quando HABILITAR_TRACES=true)
var tracerBuilder = Sdk.CreateTracerProviderBuilder()
    .AddSource(SourceName)
    .AddSource("*Microsoft.Extensions.Agents*");

if (habilitarTraces)
    tracerBuilder.AddConsoleExporter(); // Em produção: AddOtlpExporter() ou Azure Monitor

using var tracerProvider = tracerBuilder.Build();

// Ferramentas que o agente pode chamar
var tools = new PedidoTools();

// DefaultAzureCredential é prático no desenvolvimento.
// Em produção, prefira ManagedIdentityCredential.
AIAgent agent = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential())
    .AsAIAgent(
        model: deploymentName,
        name: "AgenteLogistica",
        instructions: """
            Você é um assistente de atendimento de uma empresa de logística.
            Use as ferramentas disponíveis para consultar pedidos e prazos.
            Nunca invente informações sobre pedidos.
            """,
        tools:
        [
            AIFunctionFactory.Create(tools.ConsultarPedido),
            AIFunctionFactory.Create(tools.CalcularPrazoEntrega)
        ]);

// Envolve o agente com instrumentação OpenTelemetry.
// EnableSensitiveData = false evita registrar prompts e respostas nos traces (LGPD).
AIAgent agentMonitorado = agent
    .AsBuilder()
    .UseOpenTelemetry(sourceName: SourceName, configure: cfg => cfg.EnableSensitiveData = false)
    .Build();

// Sessão para manter o contexto da conversa
AgentSession session = await agentMonitorado.CreateSessionAsync();

Console.WriteLine("Agente de logística iniciado. Digite 'sair' para encerrar.");

while (true)
{
    Console.Write("\nVocê: ");
    var mensagem = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(mensagem) || mensagem.Equals("sair", StringComparison.OrdinalIgnoreCase))
        break;

    Console.Write("Agente: ");
    await foreach (var update in agentMonitorado.RunStreamingAsync(mensagem, session))
    {
        Console.Write(update);
    }
    Console.WriteLine();
}
