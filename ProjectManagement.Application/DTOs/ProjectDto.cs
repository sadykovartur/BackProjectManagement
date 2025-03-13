using System;
using System.Collections.Generic;

namespace ProjectManagement.Application.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();

    }
}
