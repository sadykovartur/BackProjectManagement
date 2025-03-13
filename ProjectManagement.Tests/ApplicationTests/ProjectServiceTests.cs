using Moq;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Tests.ApplicationTests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _projectService = new ProjectService(_mockProjectRepository.Object, _mockTaskRepository.Object);
        }

        [Fact]
        public async Task GetProjectsAsync_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Title = "Project 1", Description = "Description 1", DueDate = DateTime.Now.AddDays(10) },
                new Project { Id = 2, Title = "Project 2", Description = "Description 2", DueDate = DateTime.Now.AddDays(20) }
            };

            _mockProjectRepository.Setup(repo => repo.GetProjectsAsync()).ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetProjectsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Project 1", result.First().Title);
        }
               
    }
}
