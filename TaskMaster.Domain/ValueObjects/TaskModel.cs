using TaskMaster.Domain.Entities;

namespace TaskMaster.Domain.ValueObjects
{
    public class TaskModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public string? UserComments { get; set; }
    }
}