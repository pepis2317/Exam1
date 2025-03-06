using Exam1.Entities;
using Exam1.Models.User;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Exam1.Validators.User
{
    public class UserRequestModelValidator : AbstractValidator<UserRequestModel>
    {
        private readonly AccelokaContext _db;
        private readonly IDataProtector _protector;
        public UserRequestModelValidator(AccelokaContext db, IDataProtectionProvider provider)
        {
            _db = db;
            _protector = provider.CreateProtector("CredentialsProtector");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email must be filled");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password must be filled");
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage("Invalid email format");
            RuleFor(x => x).MustAsync(ValidCredentials).WithMessage("Invalid credentials");
        }

        private async Task<bool> ValidCredentials(UserRequestModel request, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.Email == request.Email, token);
            if (user == null)
            {
                return false;
            }
            return _protector.Unprotect(user.Password) == request.Password;
        }
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
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
