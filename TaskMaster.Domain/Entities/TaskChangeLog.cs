using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TaskMaster.Domain.Entities
{
    [Table("TaskChangeLog")]
    public class TaskChangeLog
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Data de modificação
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Usuário que fez a modificação
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// Descrição do que foi modificado
        /// </summary>
        [MaxLength(500)]
        public string ChangeDescription { get; set; }

        /// <summary>
        /// Comentários do usuário
        /// </summary>
        [MaxLength(500)]
        [AllowNull]
        public string? UserComments { get; set; }
    }
}
