using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;

namespace TaskMaster.Infra.Repository
{
    public class ProjectRepository : IProjectRepository, IDisposable
    {
        private readonly ILogger _logger;
        private readonly RepositoryDBContext _context;

        public ProjectRepository(ILogger<ProjectRepository> logger, RepositoryDBContext context) 
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Project> Insert(Project project)
        {
            try
            {
                var existingUser = await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == project.User.Id)
                    ?? throw new Exception("User not found.");

                project.UserId = existingUser.Id;
                project.User = null;

                foreach (var task in project.Tasks)
                {
                    foreach (var taskChangeLog in task.TaskChangeLogs)
                    {
                        taskChangeLog.UserId = existingUser.Id; 
                        taskChangeLog.User = null; 
                    }
                }

                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in Insert while attempting to add project '{project.Name}' to the database.");
                throw;
            }
        }

        public async Task<IList<Project>> GetAllByUserId(int userId)
        {
            try
            {
                return await _context.Projects
                    .AsNoTracking()
                    .Include(p => p.User)
                    //.Include(p => p.Tasks)
                    .Where(p => p.User.Id == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of projects. Exception: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Project> Get(long projectId)
        {
            try
            {
                return await _context.Projects
                    .AsNoTracking()
                    .Include(p => p.Tasks)
                    .FirstAsync(t => t.Id == projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the project with ID {projectId}.");
                throw;
            }
        }

        public async Task<bool> Update(Project project)
        {
            try
            {
                var existingEntity = _context.ChangeTracker.Entries<Project>()
                    .FirstOrDefault(e => e.Entity.Id == project.Id);

                if (existingEntity is not null)
                {
                    _context.Projects.Entry(existingEntity.Entity).State = EntityState.Detached;
                }

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in Update while attempting to update project with ID {project.Id}.");

                throw;
            }
        }

        public async Task<bool> Delete(Project project)
        {
            try
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while attempting to delete project with ID {project.Id}.");
                throw;
            }
        }

        public async Task<bool> Exist(long projectId)
        {
            try
            {
                return await _context.Projects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == projectId) is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking the existence of the project with ID {projectId}", projectId);
                throw;
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
