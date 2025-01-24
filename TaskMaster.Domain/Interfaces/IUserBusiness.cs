using TaskMaster.Domain.Entities;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Domain.Interfaces
{
    public interface IUserBusiness
    {
        Task<ActionResult<User>> CreateUser(UserModel user);
        Task<ActionResult<User>> CheckUserRole(int userId);
        Task<ActionResult<User>> CheckUserExists(int userId);
    }
}
