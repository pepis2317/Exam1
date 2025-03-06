using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class PutUserHandler : IRequestHandler<UserEditModel, UserResponseModel?>
    {
        private readonly UserService _service;
        public PutUserHandler(UserService service)
        {
            _service = service;
        }
        public Task<UserResponseModel?> Handle(UserEditModel request, CancellationToken cancellationToken)
        {
            return _service.Put(request);
        }
    }
}
