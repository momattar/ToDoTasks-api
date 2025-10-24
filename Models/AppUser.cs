using Microsoft.AspNetCore.Identity;

namespace ToDoTasks.Models
{
    public class AppUser : IdentityUser
    {
        public List<AppTask> Tasks { get; set; } = new List<AppTask>();
    }
}