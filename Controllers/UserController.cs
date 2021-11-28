using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using Core.Services;
using Core.Entity;

namespace FilesShareApi.Controllers 
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserEntity userInst)
        {

            var user = new ApplicationUser
            {
                UserName = userInst.Name,
                Email = userInst.Email
            };

            IdentityResult result = await userManager.CreateAsync(user, userInst.Password);
            if (result.Succeeded)
            {
                return Ok("User Created Successfully");
            }
            return Ok(result.Errors);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetUsers(string Email)
        //{
        //    return Ok(await userManager.GetUserAsync());
        //}

    }
}
