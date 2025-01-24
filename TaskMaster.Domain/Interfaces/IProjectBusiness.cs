using TaskMaster.Domain.Entities;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Domain.Interfaces
{
    public interface IProjectBusiness
    {
        Task<ActionResult<IList<ProjectModel>>> GetProjectsByUserId(int userId);
        Task<ActionResult<ProjectModel>> CreateProject(ProjectModel projectModel);
        Task<ActionResult<ProjectModel>> DeleteProject(long projectId);
        Task<ActionResult<IList<TaskModel>>> GetTasksByProjectId(long projectId);
        Task<ActionResult<TaskModel>> CreateTask(int userId, long projectId, TaskModel taskModel);
        Task<ActionResult<TaskModel>> UpdateTask(int userId, TaskModel taskModel);
        Task<ActionResult<TaskModel>> DeleteTask(long taskId);
    }
}
