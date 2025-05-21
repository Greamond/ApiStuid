using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    public class Participant
    {
        public int Id { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
