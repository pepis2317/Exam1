using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.User
{
    public class UserRequestModel : IRequest<UserModel?>
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
