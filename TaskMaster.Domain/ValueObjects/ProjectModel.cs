using TaskMaster.Domain.Entities;

namespace TaskMaster.Domain.ValueObjects
{
    public class ProjectModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public List<TaskModel>? Tasks { get; set; }
    }
}
