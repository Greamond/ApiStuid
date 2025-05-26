using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("chapters_subtask")]
    public class ChapterSubtask
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
