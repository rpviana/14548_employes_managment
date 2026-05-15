namespace _14548_employes_managment.Models
{
    public class ApprovalActivity : UserActivity
    {
        // Quem aprovou o pedido, quando aplicavel.
        public string ApprovedBy { get; set; }
        // Quando a aprovacao aconteceu.
        public DateTime? ApprovedOn { get; set; }
        // Quem rejeitou o pedido, quando aplicavel.
        public string RejectedBy { get; set; }
        // Quando a rejeicao aconteceu.
        public DateTime? RejectedOn { get; set; }
        // Motivo da rejeicao para contexto futuro.
        public string RejectionReason { get; set; }
    }
}
