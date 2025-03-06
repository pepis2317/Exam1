using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Exam1.Models.User
{
    public class UserEditModel : IRequest<UserResponseModel>
    {
        public required Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public required string OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
