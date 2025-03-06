using Exam1.Entities;
using Exam1.Models.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Exam1.Services
{
    public class UserService
    {
        private readonly AccelokaContext _db;
        private readonly IDataProtector _protector;
        public UserService(AccelokaContext db, IDataProtectionProvider provider)
        {
            _db = db;
            _protector = provider.CreateProtector("CredentialsProtector");
        }
        public async Task<List<UserModel>> Get()
        {
            var users = await _db.Users.Select(q=>new UserModel
            {
                UserId = q.UserId,
                UserName = q.UserName,
                Email = q.Email,
                Role = q.Role
            }).ToListAsync();
            return users;
        }
        public async Task<UserModel?> Get(UserRequestModel request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.Email == request.Email);
            if (user != null && _protector.Unprotect(user.Password) == request.Password)
            {
                var data = new UserModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = user.Password,
                    Role = user.Role
                };
                return data;
            }
            return null;
        }
        public async Task<UserModel?> Get(Guid UserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if (user != null)
            {
                var data = new UserModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = user.Password,
                    Role = user.Role
                };
                return data;
            }
            return null;
        }
        public async Task<UserResponseModel> Post(UserSignInModel request)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                Password = _protector.Protect(request.Password),
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return new UserResponseModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email
            };
        }
        public async Task<UserResponseModel?> Put(UserEditModel request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == request.UserId);
            if (user != null)
            {
                user.UserName = string.IsNullOrEmpty(request.UserName) ? user.UserName : request.UserName;
                user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;
                user.Password = string.IsNullOrEmpty(request.NewPassword)? user.Password: _protector.Protect(request.NewPassword);
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return new UserResponseModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            return null;
        }
        public async Task<UserResponseModel?> Delete(Guid UserId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.UserId == UserId);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return new UserResponseModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            return null;
        }
    }
}
