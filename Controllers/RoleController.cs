using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [ApiController]
    [Route("roles")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpPost("createRole")]
        [Authorize]
        public async Task<IActionResult> CreateRole([Required] string name)
        {

           IdentityResult result = await roleManager.CreateAsync(new ApplicationRole() { Name = name });
           if (result.Succeeded)
               return Ok($"Role {name} Created Successfully");
            return StatusCode(400, result.Errors);
        }

    }
}
