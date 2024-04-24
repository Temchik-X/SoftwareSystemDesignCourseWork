using BlazorApp2.Server.Controllers;
using BlazorApp2.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp2.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Добавьте DbSet для каждой сущности, которая будет отображаться в базе данных
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HrProcess> HrProcesses { get; set; }
        public DbSet<IndividualGoal> IndividualGoals { get; set; }
        public DbSet<DepartmentGoal> DepartmentGoals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
