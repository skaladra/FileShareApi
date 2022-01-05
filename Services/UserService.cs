using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi.Services
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

        public async Task<IdentityResult> CreateUser(UserEntity user, string password)
        {
            IdentityResult result = await userManager.CreateAsync(user, password);

            return result;
        }

        public async Task<IdentityResult> DeleteUser(UserEntity user)
        {
            var result = await userManager.DeleteAsync(user);

            return result;
        }

        public async Task<UserEntity> FindByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user;
        }

        public async Task<UserEntity> FindById(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            return user;
        }

        public async Task<SignInResult> Login(UserEntity user, string password)
        {
            var result = await signInManager.PasswordSignInAsync(user, password, false, false);

            return result;
        }

        public async void Logout()
        {
            await signInManager.SignOutAsync();
        }
    }
}
