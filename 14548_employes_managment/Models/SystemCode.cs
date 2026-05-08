namespace _14548_employes_managment.Models
{
    public class SystemCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Relacionamento
        public ICollection<SystemCodeDetail> SystemCodeDetails { get; set; }
    }
}
