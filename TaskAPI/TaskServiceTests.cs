using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskAPI.Controllers;
using TaskAPI.Data;
using TaskAPI.Models;
using Xunit;

namespace TaskAPI.Tests
{
    public class TasksControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new TasksController(_context);
        }

        [Fact]
        public async Task GetTasks_ReturnsOkResult()
        {
            var result = await _controller.GetTasks();
            Assert.True(result is OkObjectResult);


        }

        [Fact]
        public async Task GetTask_ReturnsNotFoundResult()
        {
            var result = await _controller.GetTask(0);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtActionResult()
        {
            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Description",
                DueDate = System.DateTime.Now.AddDays(1)
            };

            var result = await _controller.CreateTask(task);
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }
    }
}