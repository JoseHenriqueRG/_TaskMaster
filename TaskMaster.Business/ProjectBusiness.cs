using Microsoft.Extensions.Logging;
using System.Text;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Business
{
    public class ProjectBusiness(IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ILogger<ProjectBusiness> logger) : IProjectBusiness
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly ILogger<ProjectBusiness> _logger = logger;

        #region project
        public async Task<ActionResult<IList<ProjectModel>>> GetProjectsByUserId(int userId)
        {
            try
            {
                var projects = await _projectRepository.GetAllByUserId(userId);

                var projectsModel = projects?.Select(p => new ProjectModel() { Id = p.Id, Name = p.Name, UserId = p.User.Id }).ToList();

                return new ActionResult<IList<ProjectModel>>
                {
                    Success = true,
                    Message = "Projects list loaded successfully.",
                    Result = projectsModel ?? []
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetProjects while retrieving the list of projects.");

                return new ActionResult<IList<ProjectModel>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the projects. Please try again later."
                };
            }
        }

        public async Task<ActionResult<ProjectModel>> CreateProject(ProjectModel projectModel)
        {
            try
            {
                Project project = new()
                {
                    Name = projectModel.Name,
                    User = new() { Id = projectModel.UserId },
                };

                if (projectModel.Tasks is not null && projectModel.Tasks.Count > 0)
                {
                    foreach (var taskModel in projectModel.Tasks)
                    {
                        var task = new Domain.Entities.Task(taskModel.Priority)
                        {
                            Title = taskModel.Title,
                            Description = taskModel.Description,
                            DueDate = taskModel.DueDate,
                            Status = taskModel.Status,
                            TaskChangeLogs =
                            [
                                new()
                                {
                                    ModificationDate = DateTime.UtcNow,
                                    ChangeDescription = "Create Task",
                                    User = new() { Id = projectModel.UserId },
                                    UserComments = taskModel.UserComments
                                }
                            ]
                        };

                        project.AddTask(task);
                    }
                }

                var result = await _projectRepository.Insert(project);

                if (result.Id > 0)
                {
                    projectModel.Id = result.Id;

                    return new ActionResult<ProjectModel>
                    {
                        Success = true,
                        Message = $"Project {projectModel.Name} saved successfully",
                        Result = projectModel
                    };
                }
                else
                {
                    return new ActionResult<ProjectModel>
                    {
                        Success = false,
                        Message = $"There was a problem saving the project {projectModel.Name}. Please try again soon."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in CreateProject while attempting to create project '{projectModel.Name}' for userId '{projectModel.UserId}'.");

                return new ActionResult<ProjectModel>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while creating the project '{projectModel.Name}'. Please try again later."
                };
            }
        }

        public async Task<ActionResult<ProjectModel>> DeleteProject(long projectId)
        {
            try
            {
                var project = await _projectRepository.Get(projectId);

                if (project.Tasks.Any(t => t.Status != Status.Completed))
                    return new ActionResult<ProjectModel>
                    {
                        Success = false,
                        Message = "The project cannot be deleted because it contains incomplete tasks. Please complete or remove all tasks before attempting to delete the project."
                    };

                await _projectRepository.Delete(project);

                return new ActionResult<ProjectModel> { Success = true, Message = "Project deleted successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in DeleteProject while attempting to delete project with ID {projectId}.");

                return new ActionResult<ProjectModel>
                {
                    Success = false,
                    Message = $"An error occurred while deleting the project with ID {projectId}. Please try again later."
                };
            }
        }
        #endregion

        #region Task
        public async Task<ActionResult<TaskModel>> CreateTask(int userId, long projectId, TaskModel taskModel)
        {
            try
            {
                if (!await _projectRepository.Exist(projectId))
                    return new ActionResult<TaskModel>
                    {
                        Success = false,
                        Message = $"Project with ID {projectId} does not found."
                    };

                var project = await _projectRepository.Get(projectId);

                var task = new Domain.Entities.Task(taskModel.Priority)
                {
                    Title = taskModel.Title,
                    Description = taskModel.Description,
                    DueDate = taskModel.DueDate,
                    Status = taskModel.Status,
                    TaskChangeLogs =
                    [
                        new()
                        {
                            ModificationDate = DateTime.UtcNow,
                            ChangeDescription = "Create Task",
                            UserId = userId ,
                            UserComments = taskModel.UserComments
                        }
                    ]
                };

                project.AddTask(task);

                await _projectRepository.Update(project);

                taskModel.Id = task.Id;

                return new ActionResult<TaskModel>
                {
                    Success = true,
                    Message = $"Task {taskModel.Title} saved successfully",
                    Result = taskModel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in CreateTask while attempting to create task '{taskModel.Title}' for userId '{userId}'.");

                return new ActionResult<TaskModel>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while creating the task '{taskModel.Title}'. Please try again later."
                };
            }
        }

        public async Task<ActionResult<TaskModel>> DeleteTask(long taskId)
        {
            try
            {
                var task = await _taskRepository.GetTask(taskId);

                if (task is null)
                    return new ActionResult<TaskModel>
                    {
                        Success = false,
                        Message = $"Task with ID {taskId} does not found."
                    };

                await _taskRepository.DeleteTask(task);

                return new ActionResult<TaskModel>
                {
                    Success = true,
                    Message = $"Task {taskId} deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in DeleteTask while attempting to delete task with ID {taskId}.");

                return new ActionResult<TaskModel>
                {
                    Success = false,
                    Message = $"An error occurred while deleting the task {taskId}. Please try again later."
                };
            }
        }

        public async Task<ActionResult<IList<TaskModel>>> GetTasksByProjectId(long projectId)
        {
            try
            {
                var project = await _projectRepository.Get(projectId);

                if (project.Id <= 0)
                    return new ActionResult<IList<TaskModel>>
                    {
                        Success = false,
                        Message = $"Project with ID {projectId} does not found."
                    };

                var tasksModel = project.Tasks
                    .Select(t => new TaskModel()
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Status = t.Status,
                        Priority = t.Priority,
                    })
                    .ToList();

                return new ActionResult<IList<TaskModel>>
                {
                    Success = true,
                    Message = "Task list loaded successfully",
                    Result = tasksModel ?? []
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in GetTasksByProjectId while retrieving tasks for project with ID {projectId}.");

                return new ActionResult<IList<TaskModel>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the tasks for the project with ID {projectId}"
                };
            }
        }

        public async Task<ActionResult<TaskModel>> UpdateTask(int userId, TaskModel taskModel)
        {
            try
            {
                var task = await _taskRepository.GetTask(taskModel.Id);

                if (task is null)
                    return new ActionResult<TaskModel>
                    {
                        Success = false,
                        Message = $"Task with ID {taskModel.Id} does not found."
                    };

                var changeDescription = new StringBuilder();

                if (!task.Title.Equals(taskModel.Title))
                {
                    task.Title = taskModel.Title;
                    changeDescription.AppendLine($"Title changed from '{task.Title}' to '{taskModel.Title}'.");
                }

                if (!task.Description.Equals(taskModel.Description))
                {
                    task.Description = taskModel.Description;
                    changeDescription.AppendLine($"Description changed from '{task.Description}' to '{taskModel.Description}'.");
                }

                if (!task.Status.Equals(taskModel.Status))
                {
                    task.Status = taskModel.Status;
                    changeDescription.AppendLine($"Status changed from '{task.Status}' to '{taskModel.Status}'.");
                }

                if (!task.DueDate.Equals(taskModel.DueDate))
                {
                    task.DueDate = taskModel.DueDate;
                    changeDescription.AppendLine($"DueDate changed from '{task.DueDate}' to '{taskModel.DueDate}'.");
                }

                if (!task.Priority.Equals(taskModel.Priority))
                {
                    return new ActionResult<TaskModel>
                    {
                        Success = false,
                        Message = "Unable to change the task priority."
                    };
                }

                if (!string.IsNullOrWhiteSpace(taskModel.UserComments))
                {
                    changeDescription.AppendLine($"User comments added: '{taskModel.UserComments}'.");
                }

                task.TaskChangeLogs ??= [];

                task.TaskChangeLogs.Add(new()
                {
                    ModificationDate = DateTime.UtcNow,
                    ChangeDescription = changeDescription.ToString(),
                    User = new() { Id = userId },
                    UserComments = taskModel.UserComments
                });

                await _taskRepository.UpdateTask(task);

                return new ActionResult<TaskModel> { Success = true, Message = $"Task {taskModel.Title} updated successfully", Result = taskModel };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in UpdateTask while updating task with ID {taskModel.Id}. Error: {ex.Message}");

                return new ActionResult<TaskModel>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the task. Please try again later."
                };
            }
        }

        #endregion
    }
}
