using System.ComponentModel.DataAnnotations;

namespace TaskManagementCommon.Models
{
    public class TaskItem
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Title is required.")] 
        [MaxLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")] 
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? DueDate { get; set; } = DateTime.UtcNow.AddDays(14);

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; 
    }
}
