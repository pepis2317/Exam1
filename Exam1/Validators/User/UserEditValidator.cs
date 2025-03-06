using DocumentFormat.OpenXml.Spreadsheet;
using Exam1.Entities;
using Exam1.Models.User;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Validators.User
{
    public class UserEditValidator : AbstractValidator<UserEditModel>
    {
        private readonly AccelokaContext _db;
        private readonly IDataProtector _protector;
        public UserEditValidator(AccelokaContext db, IDataProtectionProvider provider)
        {
            _db = db;
            _protector = _protector = provider.CreateProtector("CredentialsProtector");
            RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("User id must be provided");
            RuleFor(x => x.OldPassword).NotNull().NotEmpty().WithMessage("Old password must be filled");
            RuleFor(x => x).MustAsync(async (x, cancellation) => await ValidateOldPassword(x.UserId, x.OldPassword, cancellation)).WithMessage("Invalid old password.");
            RuleFor(x => x.Email).Must(IsValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format");
            RuleFor(x => x.UserName).MustAsync(IsValidUserName).When(x => !string.IsNullOrEmpty(x.UserName)).WithMessage("Username already in use");
            RuleFor(x => x).MustAsync(CheckNewEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email already in use by another user");
            RuleFor(x => x.UserId).MustAsync(ValidUserId).WithMessage("Invalid user id");
        }
        private async Task<bool> IsValidUserName(string username, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user != null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidUserId(Guid UserId, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidateOldPassword(Guid UserId, string oldPassword, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (user == null)
            {
                return false;
            }
            return _protector.Unprotect(user.Password) == oldPassword;
            //return user.Password == oldPassword;
        }
        private async Task<bool> CheckNewEmail(UserEditModel request, CancellationToken token)
        {
            var user = await _db.Users.Where(q => q.Email == request.Email).FirstOrDefaultAsync();
            if (user != null && user.UserId != request.UserId)
            {
                return false;
            }
            return true;
        }
        private bool IsValidEmail(string? email)
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
