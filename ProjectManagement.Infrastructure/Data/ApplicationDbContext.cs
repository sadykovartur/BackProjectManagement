using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<AppTask> Tasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ����� Project -> AppTask
            builder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // ������ �������� �������, ���� ���� ��������� ������

            // ����� AppTask -> User
            builder.Entity<AppTask>()
                .HasOne(t => t.User)
                .WithMany() // ���� � ������������ �� ����� ������ �����, ��������� ������
                .HasForeignKey(t => t.UserId) // ������� ���� UserId
                .OnDelete(DeleteBehavior.Cascade); // �������� �����, ��������� � �������������
        }
    }
}
