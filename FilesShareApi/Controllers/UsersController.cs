using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FilesShareApi
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="userToCreate">instance of UserEntity</param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([Required] UserCreateDto userToCreate)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return StatusCode(400, "{ Error: You are already registered }");
            }

            var newUser = new UserEntity
            {
                UserName = userToCreate.Name,
                Email = userToCreate.Email
            };

            var result = await userService.CreateOne(newUser, userToCreate.Password);

            if (result.Succeeded)
            {
                var user = await userService.FindOneByEmail(userToCreate.Email);

                await Login(UsersMapper.CreateUserLoginDto(userToCreate.Email, userToCreate.Password));

                return Ok(UsersMapper.CreateUserResponseDto(user));
            }

            return StatusCode(400,  $"Registration Failed: {result.Errors} ");
        }

        /// <summary>
        /// log in with email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginParameters)
        {
            var user = await userService.FindOneByEmail(loginParameters.Email);

            if (user != null)
            {
                var result = await userService.Login(user, loginParameters.Password);

                if (result.Succeeded)
                {
                    return Ok(UsersMapper.CreateUserResponseDto(user));
                }
            }

            return StatusCode(401, "{ Error: Invalid Email or Password }");
        }

        [HttpGet("logout")]
        [Authorize]
        public IActionResult LogOut()
        {
            userService.Logout();

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult GetAllUsers()
        {
            var users = userService.FindAllByAdmin();

            return Ok(UsersMapper.CreateDtoList(users));
        }

        [HttpGet("/users/1")]
        [Authorize]
        public async Task<IActionResult> FindUserByName([FromQuery(Name = "name")] string name)
        {
            var user = await userService.FindOneByName(name);

            if (user == null)
            {
                return Ok();
            }

            return Ok(UsersMapper.CreateUserResponseDto(user));
        }

        [HttpDelete("/users/1")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromQuery(Name = "id")] string id)
        {
            var userToDelete = await userService.FindOneById(id);

            if (userToDelete != null)
            {
                var result = await userService.DeleteOne(userToDelete);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return StatusCode(404, "{ Error: User not found }");
        }

    }
}
