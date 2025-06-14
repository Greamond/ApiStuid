using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace ApiStuid.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column("is_public")]
        public bool IsPublic { get; set; }
        public int Creator { get; set; }
        [Column("is_archive")]
        public bool IsArchive { get; set; }
    }
}
