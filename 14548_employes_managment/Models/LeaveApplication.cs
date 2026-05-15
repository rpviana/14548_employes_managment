using System.ComponentModel.DataAnnotations;

namespace _14548_employes_managment.Models
{
    public class LeaveApplication : ApprovalActivity
    {
        public int Id { get; set; }

        // Empregado que vai ficar em ausencia.
        [Display(Name = "Employee Name")]
        public int EmployeeId { get; set; }
        // Navegacao para mostrar o nome completo no frontend.
        public Employee? Employee { get; set; }

        // Data de inicio do pedido.
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        // Data de fim do pedido.
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        // Calculado com base nas datas escolhidas.
        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

        // Indica se e dia inteiro, meio dia ou outra duracao.
        [Display(Name = "Duration")]
        public int DurationId { get; set; }
        // Detalhe do codigo que mostra a duracao por extenso.
        public SystemCodeDetail? Duration { get; set; }

        // Tipo de ausencia, como ferias ou baixa.
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }
        // Navegacao para consultar o nome do tipo.
        public LeaveType? LeaveType { get; set; }

        // Ficheiro ou referencia a anexo, se existir.
        [Display(Name = "Attachment")]
        public string? Attachment { get; set; }

        // Observacoes adicionais do pedido.
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // Estado atual do pedido, por exemplo Pending ou Approved.
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        // Navegacao para mostrar o estado legivel.
        public SystemCodeDetail? Status { get; set; }
    }
}
