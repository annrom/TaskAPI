using Microsoft.EntityFrameworkCore;
using TaskAPI.Data;
using TaskAPI.Interfaces;

namespace TaskAPI.Services
{
    public class TaskSchedulerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public TaskSchedulerService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task CheckTasksDueSoon(int hours)
        {
            var now = DateTime.UtcNow;
            var tasksDueSoon = await _context.Tasks
                .Where(t => !t.IsCompleted && t.DueDate <= now.AddHours(hours) && !t.IsDeleted)
                .ToListAsync();

            foreach (var task in tasksDueSoon)
            {
                await _emailService.SendEmailAsync("user@example.com", "Task Reminder", $"Task '{task.Title}' is due soon!");
            }
        }
    }
}