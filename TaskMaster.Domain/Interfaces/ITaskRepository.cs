namespace TaskMaster.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<Entities.Task> GetTask(long taskId);
        Task<IList<Entities.Task>> GetTasksByDateRange(DateTime startDate, DateTime endDate);
        Task<bool> DeleteTask(Entities.Task task);
        Task<bool> UpdateTask(Entities.Task task);
    }
}
