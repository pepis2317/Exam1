using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.User
{
    public class UserModel
    {
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public required string Role { get; set; }

    }
}
