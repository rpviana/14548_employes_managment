namespace _14548_employes_managment.Models
{
    public class SystemCode
    {
        public int Id { get; set; }
        // Codigo tecnico usado para agrupar varios valores do mesmo tipo.
        public string Code { get; set; }
        // Nome legivel que aparece na administracao.
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Lista dos detalhes associados a este codigo.
        public ICollection<SystemCodeDetail> SystemCodeDetails { get; set; }
    }
}
