using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class GetUserByIdHandler : IRequestHandler<UserGetByIdModel, UserModel?>
    {
        private readonly UserService _service;
        public GetUserByIdHandler(UserService service)
        {
            _service = service;
        }

        public Task<UserModel?> Handle(UserGetByIdModel request, CancellationToken cancellationToken)
        {
            return _service.Get(request.UserId);
        }
    }
}
