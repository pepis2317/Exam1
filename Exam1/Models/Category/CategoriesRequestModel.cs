using MediatR;

namespace Exam1.Models.Category
{
    public class CategoriesRequestModel : IRequest<List<string>>
    {
    }
}
