using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<AppTask> Tasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Связь Project -> AppTask
            builder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Запрет удаления проекта, если есть связанные задачи

            // Связь AppTask -> User
            builder.Entity<AppTask>()
                .HasOne(t => t.User)
                .WithMany() // Если у пользователя не будет списка задач, оставляем пустым
                .HasForeignKey(t => t.UserId) // Внешний ключ UserId
                .OnDelete(DeleteBehavior.Cascade); // Удаление задач, связанных с пользователем
        }
    }
}
