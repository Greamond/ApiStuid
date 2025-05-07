using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiStuid.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}
