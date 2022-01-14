using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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
        Task Logout();
        Task<IdentityResult> DeleteOne(UserEntity user);
    }
}
