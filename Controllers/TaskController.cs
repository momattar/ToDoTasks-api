using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetTasks()
        {
            var tasks = _context.AppTasks.ToList();
            return Ok(tasks);
        }

        [HttpPost]
        public IActionResult CreateTask(Models.CreateTaskDto taskDto)
        {
            var task = new Models.AppTask
            {
                Title = taskDto.Title,
                IsCompleted = taskDto.IsCompleted
            };

            _context.AppTasks.Add(task);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var task = _context.AppTasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id)
        {
            var task = _context.AppTasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = true;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.AppTasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.AppTasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}