using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDoTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly Data.Context _context;

        public TaskController(Data.Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.AppTasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(Models.CreateTaskDto taskDto)
        {
            if (string.IsNullOrWhiteSpace(taskDto.Title))
                return BadRequest("Title cannot be empty");

            if (taskDto.Title.Length > 200)
                return BadRequest("Title cannot exceed 200 characters");
            var task = new Models.AppTask
            {
                Title = taskDto.Title,
                IsCompleted = taskDto.IsCompleted
            };
            await _context.AppTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.AppTasks.FindAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id)
        {
            var task = await _context.AppTasks.FindAsync(id);
            if (task == null) return NotFound();

            task.IsCompleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.AppTasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.AppTasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}