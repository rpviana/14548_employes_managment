namespace _14548_employes_managment.Models
{
    public class SystemCodeDetail
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Foreign Key
        public int SystemCodeId { get; set; }
        public SystemCode SystemCode { get; set; }
    }
}
