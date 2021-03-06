using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserEntity> userManager;
        private readonly SignInManager<UserEntity> signInManager;

        public UserService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateOne(UserEntity user, string password)
        {
            IdentityResult result = await userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<IdentityResult> DeleteOne(UserEntity user)
        {
            var result = await userManager.DeleteAsync(user);

            return result;
        }

        public async Task<UserEntity> FindOneByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user;
        }

        public async Task<UserEntity> FindOneByName(string name)
        {
            var user = await userManager.FindByNameAsync(name);

            return user;
        }

        public async Task<UserEntity> FindOneById(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            return user;
        }

        public IEnumerable<UserEntity> FindAllByAdmin()
        {
            var result = (from users in userManager.Users select users).AsEnumerable();
            return result;
        }

        public async Task<SignInResult> Login(UserEntity user, string password)
        {
            var result = await signInManager.PasswordSignInAsync(user, password, false, false);

            return result;
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }
    }
}
