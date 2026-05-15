namespace _14548_employes_managment.Models
{

    // enum sozinho nao gera migration, tem de ser usado dentro de uma classe que seja parte do modelo para ser criado na base de dados, sugestao do proprio c# kit a dizer isso, deu uma ajuda
    public enum Function
    {
        Administration,
        Engineer,
        Worker,
    }

    public class Employee: UserActivity
    {
        public int Id { get; set; }
        // Comeca vazio para o formulario poder preencher sem cair em null.
        public string EmpNo { get; set; } = string.Empty;
        // string.Empty e so uma string vazia; evita valores nulos nos campos de texto.
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Junta os nomes e limpa espacos extra quando o nome do meio nao existe.
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public Function Function { get; set; } = Function.Administration;
    }
}
