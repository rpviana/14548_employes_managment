namespace _14548_employes_managment.Models
{
    public class ErrorViewModel
    {
        // Guarda o identificador tecnico do pedido para suporte.
        public string? RequestId { get; set; }

        // So mostra o RequestId quando existe um valor real.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
