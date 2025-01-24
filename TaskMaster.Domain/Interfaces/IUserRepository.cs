using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Exists(int id);
        Task<User?> Find(int id);
        Task<User> Insert(User user);
    }
}
