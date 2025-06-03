using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    public class Subtask
    {
        public int Id { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Responsible { get; set; }
        [Column("creator_id")]
        public int CreatorId { get; set; }
        [Column("chapter_id")]
        public int ChapterId { get; set; }
        public int Position { get; set; }

    }
}
