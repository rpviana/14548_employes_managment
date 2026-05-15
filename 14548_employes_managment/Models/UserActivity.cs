namespace _14548_employes_managment.Models
{
    public class UserActivity
    {
        // Guarda quem criou e quem alterou o registo, para auditoria simples.
        public string? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedById { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
