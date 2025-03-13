using Moq;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Tests.ApplicationTests
{
    public class AppTaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly TaskService _taskService;

        public AppTaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnTasks()
        {
            // Arrange
            var tasks = new List<Domain.Entities.AppTask>
            {
                new Domain.Entities.AppTask { Id = 1, Title = "Task 1", Description = "Description 1", ProjectId = 1 },
                new Domain.Entities.AppTask { Id = 2, Title = "Task 2", Description = "Description 2", ProjectId = 2 }
            };

            _mockTaskRepository.Setup(repo => repo.GetTasksAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetTasksAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Task 1", result.First().Title);
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTask()
        {
            // Arrange
            var AppTaskDto = new TaskDto { Title = "New Task", Description = "New Description", ProjectId = 1 };

            // Act
            await _taskService.AddTaskAsync(AppTaskDto);

            // Assert
            _mockTaskRepository.Verify(repo => repo.AddTaskAsync(It.Is<Domain.Entities.AppTask>(t => t.Title == "New Task")), Times.Once);
        }
    }
}
