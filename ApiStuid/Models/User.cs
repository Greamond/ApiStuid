using System;

namespace ApiStuid.Models
{
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
        public DateTime? LastActivity { get; set; }
    }
}
