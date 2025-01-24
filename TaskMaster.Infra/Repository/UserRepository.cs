using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;

namespace TaskMaster.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger _logger;
        private readonly RepositoryDBContext _context;

        public UserRepository(ILogger<UserRepository> logger, RepositoryDBContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> Insert(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in Insert while attempting to add user '{user.UserName}' to the database.");
                throw;
            }
        }

        public async Task<bool> Exists(int userId)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId) is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking the existence of the user with ID {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> Find(int userId)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking the existence of the user with ID {UserId}", userId);
                throw;
            }
        }
    }
}
