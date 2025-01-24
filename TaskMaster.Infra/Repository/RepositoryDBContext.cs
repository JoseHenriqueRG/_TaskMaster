using Microsoft.EntityFrameworkCore;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Infra.Repository
{
    public class RepositoryDBContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<TaskChangeLog> TaskChangeLogs { get; set; }
        public DbSet<User> Users { get; set; }

        public RepositoryDBContext(DbContextOptions<RepositoryDBContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskChangeLog>()
                .HasOne(p => p.User)
                .WithMany(u => u.TaskChangeLogs)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
