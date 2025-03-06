using MediatR;

namespace Exam1.Models.User
{
    public class UserGetByIdModel : IRequest<UserModel>
    {
        public required Guid UserId { get; set; }
    }
}
