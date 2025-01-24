using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskMaster.Business;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Tests
{
    public class ProjectBusinessTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<ILogger<ProjectBusiness>> _loggerMock;
        private readonly ProjectBusiness _projectBusiness;

        public ProjectBusinessTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _loggerMock = new Mock<ILogger<ProjectBusiness>>();
            _projectBusiness = new ProjectBusiness(_projectRepositoryMock.Object, _taskRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async void GetProjectsByUserId_ShouldReturnProjects_WhenProjectsExist()
        {
            // Arrange
            var userId = 1;
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project 1", User = new User { Id = userId } },
                new Project { Id = 2, Name = "Project 2", User = new User { Id = userId } }
            };
            _projectRepositoryMock.Setup(repo => repo.GetAllByUserId(userId)).ReturnsAsync(projects);

            // Act
            var result = await _projectBusiness.GetProjectsByUserId(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result[0].Name.Should().Be("Project 1");
            result.Result[1].Name.Should().Be("Project 2");
        }

        [Fact]
        public async void CreateProject_ShouldReturnSuccess_WhenProjectIsCreated()
        {
            // Arrange
            var projectModel = new ProjectModel { Name = "New Project", UserId = 1 };
            var project = new Project { Id = 1, Name = "New Project" };
            _projectRepositoryMock.Setup(repo => repo.Insert(It.IsAny<Project>())).ReturnsAsync(project);

            // Act
            var result = await _projectBusiness.CreateProject(projectModel);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Id.Should().Be(1);
            result.Message.Should().Be($"Project {projectModel.Name} saved successfully");
        }

        [Fact]
        public async void DeleteProject_ShouldReturnSuccess_WhenProjectIsDeleted()
        {
            // Arrange
            var projectId = 1;  
            var project = new Project { Id = projectId };
            _projectRepositoryMock.Setup(repo => repo.Get(projectId)).ReturnsAsync(project);
            _projectRepositoryMock.Setup(repo => repo.Delete(project)).Verifiable();

            // Act
            var result = await _projectBusiness.DeleteProject(projectId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Project deleted successfully.");
            _projectRepositoryMock.Verify(repo => repo.Delete(project), Times.Once);
        }

        [Fact]
        public async void CreateTask_ShouldReturnSuccess_WhenTaskIsCreated()
        {
            // Arrange
            var userId = 1;
            var projectId = 1;
            var taskModel = new TaskModel { Title = "New Task", Priority = Priority.High };
            var project = new Project { Id = projectId };
            _projectRepositoryMock.Setup(repo => repo.Exist(projectId)).ReturnsAsync(true);
            _projectRepositoryMock.Setup(repo => repo.Get(projectId)).ReturnsAsync(project);

            // Act
            var result = await _projectBusiness.CreateTask(userId, projectId, taskModel);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be($"Task {taskModel.Title} saved successfully");
        }

        [Fact]
        public async void DeleteTask_ShouldReturnSuccess_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = 1;
            var taskModel = new TaskModel { Id = 1, Title = "New Task", Priority = Priority.High };
            var task = new Domain.Entities.Task(Priority.Low) { Id = taskId };
            _taskRepositoryMock.Setup(repo => repo.GetTask(taskId)).ReturnsAsync(task);

            // Act
            var result = await _projectBusiness.DeleteTask(taskId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be($"Task {taskId} deleted successfully.");
            _taskRepositoryMock.Verify(repo => repo.DeleteTask(task), Times.Once);
        }

        [Fact]
        public async void GetTasksByProjectId() 
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = 1, Name = "Project 1", User = new User { Id = 1 } };
            project.AddTask(new Domain.Entities.Task(Priority.Low) { Id = 1, Title = "Task Title", Description = "Task description" });
            _projectRepositoryMock.Setup(repo => repo.Get(projectId)).ReturnsAsync(project);

            // Act
            var result = await _projectBusiness.GetTasksByProjectId(projectId);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(1);
            result.Result[0].Title.Should().Be("Task Title");
        }
    }
}

