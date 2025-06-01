using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("chapters_subtask")]
    public class ChapterSubtask
    {
        public int Id { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        public string Name { get; set; }
    }
}
