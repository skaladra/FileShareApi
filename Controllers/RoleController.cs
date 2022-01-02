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
        private readonly RoleManager<RoleEntity> roleManager;

        public RoleController(RoleManager<RoleEntity> roleManager)
        {
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Create new application role. Allowed to administation right users only
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize("Admin")]
        public async Task<IActionResult> CreateRole([Required] string name)
        {

           IdentityResult result = await roleManager.CreateAsync(new RoleEntity() { Name = name });
           if (result.Succeeded)
               return Ok($"Role {name} Created Successfully");
            return StatusCode(400, result.Errors);
        }

    }
}
