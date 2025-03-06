using Exam1.Models.User;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.User
{
    public class DeleteUserHandler : IRequestHandler<UserDeleteModel, UserResponseModel>
    {
        private readonly UserService _service;
        public DeleteUserHandler(UserService service)
        {
            _service = service;
        }

        public Task<UserResponseModel> Handle(UserDeleteModel request, CancellationToken cancellationToken)
        {
            return _service.Delete(request.UserId);
        }
    }
}
