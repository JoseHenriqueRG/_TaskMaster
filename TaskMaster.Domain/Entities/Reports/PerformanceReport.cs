namespace TaskMaster.Domain.Entities.Reports
{
    public class PerformanceReport
    {
        /// <summary>
        /// O número total de usuários considerados no relatório.
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// A data final do período do relatório (últimos 30 dias).
        /// </summary>
        public DateTime ReportEndDate { get; set; }

        /// <summary>
        /// A data inicial do período do relatório (últimos 30 dias).
        /// </summary>
        public DateTime ReportStartDate { get; set; }

        /// <summary>
        /// Lista de detalhes de tarefas concluídas por cada usuário.
        /// </summary>
        public List<UserTaskCompletionDetail> UserTaskCompletionDetails { get; set; }
    }
}
