using Exam1.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Services
{
    public class CategoryService
    {
        private readonly AccelokaContext _db;
        public CategoryService(AccelokaContext db)
        {
            _db = db;
        }
        public async Task<List<string>> Get()
        {
            var categories = await _db.Categories.Select(q=>q.CategoryName).ToListAsync();
            return categories;
        }

    }
}
