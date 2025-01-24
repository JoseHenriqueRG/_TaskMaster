using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMaster.Domain.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string Role { get; set; }

        public List<Project> Projects { get; set; }

        public List<TaskChangeLog> TaskChangeLogs { get; set; }
    }
}
