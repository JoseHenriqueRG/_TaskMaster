using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMaster.Domain.ValueObjects
{
    public class UserModel
    {
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }
    }
}
