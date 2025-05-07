using System.Collections.Generic;

namespace ApiStuid.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public List<TaskResponsible> ResponsibleUsers { get; set; } = new List<TaskResponsible>();
        public List<Subtask> Subtasks { get; set; } = new List<Subtask>();
    }
}
