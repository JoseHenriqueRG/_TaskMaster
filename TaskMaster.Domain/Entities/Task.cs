using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMaster.Domain.Entities
{
    [Table("Task")]
    public class Task(Priority priority)
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; private set; } = priority;

        public List<TaskChangeLog> TaskChangeLogs { get; set; }
    }
}
