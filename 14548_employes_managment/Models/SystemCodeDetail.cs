namespace _14548_employes_managment.Models
{
    public class SystemCodeDetail
    {
        public int Id { get; set; }
        // Texto que identifica o valor dentro do codigo principal.
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Liga este detalhe ao SystemCode pai.
        public int SystemCodeId { get; set; }
        // Navegacao para consultar o grupo inteiro quando preciso.
        public SystemCode SystemCode { get; set; }
    }
}
