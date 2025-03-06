using MediatR;

namespace Exam1.Models.User
{
    public class UserDeleteModel : IRequest<UserResponseModel>
    {
        public required Guid UserId { get; set; }
    }
}
