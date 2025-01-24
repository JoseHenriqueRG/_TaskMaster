using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMaster.Domain.Entities.Reports
{
    public class UserTaskCompletionDetail
    {
        /// <summary>
        /// ID do usuário.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Número total de tarefas concluídas pelo usuário nos últimos 30 dias.
        /// </summary>
        public int CompletedTasks { get; set; }
    }
}
