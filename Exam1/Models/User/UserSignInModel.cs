
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.User
{
    public class UserSignInModel : IRequest<UserResponseModel>
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
