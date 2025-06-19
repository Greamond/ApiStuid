using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("tasks")]
    public class Task
    {
        public int Id { get; set; }

        [Column("project_id")]
        public int ProjectId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [Column("chapter_id")]
        public int Chapter { get; set; }
        [Column("creator_id")]
        public int CreatorId { get; set; }
        public int Position { get; set; }
    }
}
