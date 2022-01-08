using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

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
                return StatusCode(400, "You are already registered");
            }

            var newUser = new UserEntity
            {
                UserName = userToCreate.Name,
                Email = userToCreate.Email
            };

            var result = await userService.CreateUser(newUser, userToCreate.Password);

            if (result.Succeeded)
            {
                var user = await userService.FindByEmail(userToCreate.Email);

                await Login(user.Email, userToCreate.Password);

                return Ok($"User {userToCreate.Name} Created Successfully");
            }

            return StatusCode(400, $"Registration Failed: {result.Errors}");
        }

        /// <summary>
        /// log in with email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Required][EmailAddress] string email, [Required] string password)
        {
            var user = await userService.FindByEmail(email);

            if (user != null)
            {
                var result = await userService.Login(user, password);

                if (result.Succeeded)
                {
                    return Ok(UsersMapper.CreateDto(user));
                }
            }

            return StatusCode(401, "Login Failed: Invalid Email or Password");
        }

        [HttpPost("logout")]
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
            var users = userService.FindAll();

            return Ok(UsersMapper.CreateDtoList(users));
        }

        [HttpGet("/users/1")]
        [Authorize]
        public async Task<IActionResult> FindUserByName([FromQuery(Name = "name")] string name)
        {
            var user = await userService.FindByName(name);

            if (user == null)
            {
                return Ok();
            }

            return Ok(UsersMapper.CreateDto(user));
        }

        [HttpDelete("/users/1")]
        public async Task<IActionResult> DeleteUser([FromQuery(Name = "id")] string id)
        {
            var userToDelete = await userService.FindById(id);

            if (userToDelete != null)
            {
                var result = await userService.DeleteUser(userToDelete);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return StatusCode(404, "User not found");
        }

    }
}
