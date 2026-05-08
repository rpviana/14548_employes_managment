namespace _14548_employes_managment.Models
{
    public class ApprovalActivity : UserActivity
    {
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedOn { get; set; }
        public string RejectionReason { get; set; }
    }
}
