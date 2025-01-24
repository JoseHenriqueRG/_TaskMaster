using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectBusiness _projectBusiness;
        private readonly IUserBusiness _userBusiness;

        public ProjectsController(IProjectBusiness business, IUserBusiness userBusiness)
        {
            _projectBusiness = business;
            _userBusiness = userBusiness;
        }

        // GET: api/Projects/5
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetProjectsByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest($"User ID {userId} is not valid.");
            }

            var result = await _userBusiness.CheckUserExists(userId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(await _projectBusiness.GetProjectsByUserId(userId));
        }

        // POST: api/Projects
        [HttpPost]
        public async Task<IActionResult> PostProject(ProjectModel project)
        {
            var result = await _userBusiness.CheckUserExists(project.UserId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(await _projectBusiness.CreateProject(project));
        }

        // DELETE: api/Projects/5
        [HttpDelete("{projectId:long}")]
        public async Task<IActionResult> DeleteProject(long projectId)
        {
            return Ok(await _projectBusiness.DeleteProject(projectId));
        }

        // GET: api/Projects/Tasks/5
        [HttpGet("Tasks/{projectId:long}")]
        public async Task<IActionResult> GetTasksByProject(long projectId)
        {
            return Ok(await _projectBusiness.GetTasksByProjectId(projectId));
        }

        // PUT: api/Projects/Tasks/5/1
        [HttpPut("Tasks/{userId:int}/{taskId:long}")]
        public async Task<IActionResult> PutTask(int userId, long taskId, TaskModel taskModel)
        {
            if (taskId != taskModel.Id)
            {
                return BadRequest();
            }

            if (userId <= 0)
            {
                return BadRequest($"User ID {userId} is not valid.");
            }

            var result = await _userBusiness.CheckUserExists(userId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(await _projectBusiness.UpdateTask(userId, taskModel));
        }

        // POST: api/Projects/Tasks/5/39920
        [HttpPost("Tasks/{userId:int}/{projectId:long}")]
        public async Task<IActionResult> PostTask(int userId, long projectId, TaskModel task)
        {
            if (userId <= 0)
            {
                return BadRequest($"User ID {userId} is not valid.");
            }

            var result = await _userBusiness.CheckUserExists(userId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(await _projectBusiness.CreateTask(userId, projectId, task));
        }

        // DELETE api/Projects/Tasks/5
        [HttpDelete("Tasks/{taskId:long}")]
        public async Task<IActionResult> DeleteTask(long taskId)
        {
            return Ok(await _projectBusiness.DeleteTask(taskId));
        }
    }
}
    