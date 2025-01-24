using Microsoft.EntityFrameworkCore;
using TaskMaster.Business;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Infra.Repository;
using TaskMaster.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Business
builder.Services.AddScoped<IProjectBusiness, ProjectBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IReportsBusiness, ReportsBusiness>();
// Repository
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<RepositoryDBContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("TaskMasterDatabase"),
            b => b.MigrationsAssembly("TaskMaster.Infra")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "docs";
});

DatabaseManagementService.MigrationInitialization(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
