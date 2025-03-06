using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class GetUserHandler : IRequestHandler<UserRequestModel, UserModel?>
    {
        private readonly UserService _service;
        public GetUserHandler(UserService service)
        {
            _service = service;
        }
        public Task<UserModel?> Handle(UserRequestModel request, CancellationToken cancellationToken)
        {
            return _service.Get(request);
        }
    }
}
