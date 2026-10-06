using System.ComponentModel;

public class PedidoTools
{
    private static readonly Dictionary<string, (string Status, string Destino)> _pedidos = new()
    {
        ["PED-1001"] = ("Em trânsito", "Vitória/ES"),
        ["PED-1002"] = ("Entregue", "São Paulo/SP"),
        ["PED-1003"] = ("Aguardando coleta", "Belo Horizonte/MG")
    };

    [Description("Consulta o status atual de um pedido pelo número.")]
    public string ConsultarPedido(
        [Description("Número do pedido no formato PED-0000.")] string numeroPedido)
    {
        return _pedidos.TryGetValue(numeroPedido.ToUpperInvariant(), out var pedido)
            ? $"Pedido {numeroPedido}: {pedido.Status}, destino {pedido.Destino}."
            : $"Pedido {numeroPedido} não encontrado.";
    }

    [Description("Calcula o prazo estimado de entrega em dias úteis para um CEP de destino.")]
    public int CalcularPrazoEntrega(
        [Description("CEP de destino, somente números.")] string cep)
    {
        // Regra simplificada: CEPs da região Sudeste (0 a 3) têm prazo menor
        return cep.Length > 0 && cep[0] is >= '0' and <= '3' ? 2 : 5;
    }
}
