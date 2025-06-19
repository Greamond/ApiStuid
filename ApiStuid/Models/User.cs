using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiStuid.Models
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Description { get; set; }
        public byte[] Photo { get; set; }
        [Column("last_activity")]
        public DateTime? LastActivity { get; set; }
        [Column("fcm_token")]
        public string FCMToken{ get; set; }
    }
}
