using MediatR;

namespace Exam1.Models.User
{
    public class UsersRequestModel : IRequest<List<UserModel>>
    {
    }
}
