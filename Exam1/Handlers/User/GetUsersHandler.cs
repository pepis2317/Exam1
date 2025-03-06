using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class GetUsersHandler : IRequestHandler<UsersRequestModel, List<UserModel>>
    {
        private readonly UserService _service;
        public GetUsersHandler(UserService service)
        {
            _service = service;
        }
        public Task<List<UserModel>> Handle(UsersRequestModel request, CancellationToken cancellationToken)
        {
            return _service.Get();
        }
    }
}
