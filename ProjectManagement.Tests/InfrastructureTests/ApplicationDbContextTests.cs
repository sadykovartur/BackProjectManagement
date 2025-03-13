using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Tests.InfrastructureTests
{
    public class ApplicationDbContextTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationDbContextTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddProjectAsync_ShouldSaveProject()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                var project = new Project { Title = "Test Project", Description = "Test Description", DueDate = DateTime.Now.AddDays(10) };
                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var project = await context.Projects.FirstOrDefaultAsync(p => p.Title == "Test Project");
                Assert.NotNull(project);
                Assert.Equal("Test Description", project.Description);
            }
        }
    }
}
