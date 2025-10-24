using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoTasks.Data;
using ToDoTasks.Models;

namespace ToDoTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // PROTECT ALL ENDPOINTS
    public class TaskController : ControllerBase
    {
        private readonly Context _context;

        public TaskController(Context context)
        {
            _context = context;
        }

        // Get current user's ID from JWT token
        private string GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Debug logging
            Console.WriteLine($"User ID from token: {userId}");
            Console.WriteLine($"Is authenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Username: {User.Identity?.Name}");

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _context.AppTasks
                .Where(t => t.UserId == userId)  // Only current user's tasks
                .Select(t => new TaskResponseDto
                {
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto taskDto)
        {
            if (string.IsNullOrWhiteSpace(taskDto.Title))
                return BadRequest("Title cannot be empty");

            var userId = GetCurrentUserId();

            // before adding the task
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
            {
                return Unauthorized("Authenticated user not found in database.");
            }

            var task = new AppTask
            {
                Title = taskDto.Title,
                IsCompleted = taskDto.IsCompleted,
                UserId = userId  // Link to current user
            };

            await _context.AppTasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.AppTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);  // Check ownership

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.AppTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);  // Check ownership

            if (task == null)
                return NotFound();

            task.IsCompleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.AppTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);  // Check ownership

            if (task == null)
                return NotFound();

            _context.AppTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
