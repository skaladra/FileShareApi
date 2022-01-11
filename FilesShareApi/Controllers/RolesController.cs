using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FilesShareApi
{
    [ApiController]
    [Route("/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRoles()
        {
            var roles = roleService.GetAll();

            return Ok(roles);
        }

        [HttpPut("/roles/user/1")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToRole(string id, string role)
        {
            var result = await roleService.AddUser(id, role);

            if (result.Succeeded) 
            {
                return Ok();
            }

            return StatusCode(400, result.Errors);
        }

        /// <summary>
        /// Create new application role. Allowed to administation right users only
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("/roles/1")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> CreateRole([FromQuery(Name ="name")][Required] string name)
        {  
           var result = await roleService.CreateOne(name);

           if (result.Succeeded) 
            {
                return Ok($"Role {name} Created Successfully");
            }

            return StatusCode(400, result.Errors);
        }

    }
}
