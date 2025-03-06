
using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class PostUserHandler : IRequestHandler<UserSignInModel, UserResponseModel>
    {
        private readonly UserService _service;
        public PostUserHandler(UserService service)
        {
            _service = service;
        }

        public Task<UserResponseModel> Handle(UserSignInModel request, CancellationToken cancellationToken)
        {
            return _service.Post(request);
        }
    }
}
