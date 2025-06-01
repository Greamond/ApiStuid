using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("chapters_task")]
    public class ChapterTask
    {
        public int Id { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        public string Name { get; set; }
    }
}
