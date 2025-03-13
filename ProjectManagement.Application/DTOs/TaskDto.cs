using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
    }
}
