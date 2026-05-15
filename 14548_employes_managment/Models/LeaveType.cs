namespace _14548_employes_managment.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        // Nome curto do tipo de ausencia.
        public string Name { get; set; }
        // Explica quando este tipo deve ser usado.
        public string Description { get; set; }
        // Limite anual permitido para este tipo.
        public int MaxDaysPerYear { get; set; }
        public bool IsActive { get; set; } = true;

        // Pedidos que usam este tipo de ausencia.
        public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
