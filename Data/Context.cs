using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoTasks.Models;

namespace ToDoTasks.Data
{
    public class Context: IdentityDbContext<AppUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<AppTask> AppTasks { get; set; }
    }
}
