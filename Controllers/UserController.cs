using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace FilesShareApi.Controllers 
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([Required] UserEntity userInst)
        {
            if (this.User.Identity.IsAuthenticated) return Ok("You are already registered");
            var user = new ApplicationUser
            {
                UserName = userInst.Name,
                Email = userInst.Email
            };

            IdentityResult result = await userManager.CreateAsync(user, userInst.Password);
            if (result.Succeeded)
            {
                return Ok($"User {userInst.Name} Created Successfully");
            }
            return StatusCode(400, $"Registration Failed: {result.Errors}");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Required][EmailAddress] string email, [Required] string password)
        {
            ApplicationUser appUser = await userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, password, false, false);
                if (result.Succeeded)
                {
                    return Ok($"Welcome, {appUser.UserName}");
                }
            }
            return StatusCode(401, "Login Failed: Invalid Email or Password");
        }

        [HttpPost]
        [Route ("logout")]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return Ok("You have successfully logged out");
        }

    }
}
