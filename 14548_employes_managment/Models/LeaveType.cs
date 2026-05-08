namespace _14548_employes_managment.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxDaysPerYear { get; set; }
        public bool IsActive { get; set; } = true;

        // Relacionamento
        public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
