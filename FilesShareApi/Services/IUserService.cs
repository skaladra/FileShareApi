using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IUserService
    {
        Task<IdentityResult> CreateOne(UserEntity user, string password);
        Task<SignInResult> Login(UserEntity user, string password);
        Task<UserEntity> FindOneByEmail(string email);
        Task<UserEntity> FindOneByName(string name);
        Task<UserEntity> FindOneById(string id);
        IEnumerable<UserEntity> FindAllByAdmin();
        void Logout();
        Task<IdentityResult> DeleteOne(UserEntity user);
    }
}
