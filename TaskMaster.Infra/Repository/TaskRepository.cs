using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;

namespace TaskMaster.Infra.Repository
{
    public class TaskRepository : ITaskRepository, IDisposable
    {
        private readonly ILogger _logger;
        private readonly RepositoryDBContext _context;

        public TaskRepository(ILogger<ProjectRepository> logger, RepositoryDBContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Domain.Entities.Task> GetTask(long taskId)
        {
            try
            {
                return await _context.Tasks
                    .Include(t => t.TaskChangeLogs)
                    .FirstAsync(t => t.Id == taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of tasks. Exception: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteTask(Domain.Entities.Task task)
        {
            try
            {
                if (task.TaskChangeLogs.Any())
                {
                    _context.TaskChangeLogs.RemoveRange(task.TaskChangeLogs);
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while attempting to delete task with ID {task.Id}.");
                throw;
            }
        }

        public async Task<bool> UpdateTask(Domain.Entities.Task task)
        {
            try
            {
                var existingEntity = _context.ChangeTracker.Entries<User>()
                    .FirstOrDefault(e => e.Entity.Id == task.TaskChangeLogs.Last().User.Id);

                if (existingEntity is not null)
                {
                    _context.Users.Entry(existingEntity.Entity).State = EntityState.Detached;
                }

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating task with ID {task.Id}. Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<IList<Domain.Entities.Task>> GetTasksByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Tasks
                                .Include(t => t.TaskChangeLogs)
                                    .ThenInclude(tcl => tcl.User)
                                .Where(t => t.Status.Equals(Status.Completed) && 
                                            t.TaskChangeLogs.Any(tcl => tcl.ModificationDate > startDate &&
                                                                        tcl.ModificationDate < endDate))
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of tasks. Exception: {Message}", ex.Message);
                throw;
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
