using Microsoft.EntityFrameworkCore;
using ToDoTasks.Models;

namespace ToDoTasks.Data
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<AppTask> AppTasks { get; set; }
    }
}
