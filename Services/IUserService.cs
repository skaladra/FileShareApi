using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUser(UserEntity user, string password);
        Task<SignInResult> Login(UserEntity user, string password);
        Task<UserEntity> FindByEmail(string email);
        Task<UserEntity> FindById(string id);
        IEnumerable<UserEntity> FindAll();
        void Logout();
        Task<IdentityResult> DeleteUser(UserEntity user);
    }
}
