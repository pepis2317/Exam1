using Exam1.Entities;
using Exam1.Models.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.User
{
    public class UserDeleteValidator : AbstractValidator<UserDeleteModel>
    {
        private readonly AccelokaContext _db;
        public UserDeleteValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).MustAsync(CheckId).WithMessage("Invalid user id");
        }
        private async Task<bool> CheckId(Guid UserId, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
