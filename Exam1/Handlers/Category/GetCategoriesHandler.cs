using Exam1.Models.Category;
using Exam1.Services;
using MediatR;

namespace Exam1.Handlers.Category
{
    public class GetCategoriesHandler : IRequestHandler<CategoriesRequestModel, List<string>>
    {
        private readonly CategoryService _service;
        public GetCategoriesHandler(CategoryService service)
        {
            _service = service;
        }
        public Task<List<string>> Handle(CategoriesRequestModel request, CancellationToken cancellationToken)
        {
            return _service.Get();
        }
    }
}
