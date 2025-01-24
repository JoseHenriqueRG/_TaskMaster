using TaskMaster.Domain.Entities;

namespace TaskMaster.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> Insert(Project project);
        Task<IList<Project>> GetAllByUserId(int userId);
        Task<Project> Get(long projectId);
        Task<bool> Update(Project project);
        Task<bool> Delete(Project project);
        Task<bool> Exist(long projectId);
    }
}
