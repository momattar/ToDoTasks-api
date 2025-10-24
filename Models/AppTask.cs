namespace ToDoTasks.Models
{
    public class AppTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
