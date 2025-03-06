using Exam1.Entities;
using Exam1.Models.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.User
{
    public class UserSignInModelValidator : AbstractValidator<UserSignInModel>
    {
        private readonly AccelokaContext _db;
        public UserSignInModelValidator(AccelokaContext db)
        {
            _db = db;
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email must be filled");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password must be filled");
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage("Invalid email format");
            RuleFor(x => x).MustAsync(CheckUniqueEmail).WithMessage("Email already in use");
        }
        private async Task<bool> CheckUniqueEmail(UserSignInModel request, CancellationToken token)
        {
            var user = await _db.Users.Where(q => q.Email == request.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                return false;
            }
            return true;
        }
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
