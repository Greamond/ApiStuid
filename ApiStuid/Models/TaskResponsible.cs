using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("task_responsible")]
    public class TaskResponsible
    {
        public int Id { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
    }
}
