using Microsoft.EntityFrameworkCore;
using TaskMaster.Infra.Repository;

namespace TaskMaster.WebApi.Services
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialization(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var serviceDb = serviceScope.ServiceProvider.GetService<RepositoryDBContext>();

            serviceDb.Database.Migrate();
        }
    }
}
