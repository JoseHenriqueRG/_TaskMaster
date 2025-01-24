using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMaster.Domain.Entities
{
    [Table("Project")]
    public class Project
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        private List<Task> _tasks = [];
        private const int MaxTaskCount = 20;

        public List<Task> Tasks { get { return _tasks; } }

        /// <summary>
        /// Cada projeto permite no máximo 20 tarefas
        /// </summary>
        /// <param name="task"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddTask(Task task)
        {
            if(_tasks.Count >= MaxTaskCount)
            {
                throw new InvalidOperationException($"Cannot add more than {MaxTaskCount} works.");
            }

            _tasks.Add(task);
        }
    }
}
