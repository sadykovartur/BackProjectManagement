using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Domain.Entities
{
    public class AppTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
