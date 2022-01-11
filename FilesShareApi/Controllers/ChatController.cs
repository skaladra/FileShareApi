using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [ApiController]
    [Route("/chats")]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }


        [HttpPost("/chats/1")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] string text,
            [FromQuery(Name ="recipent")] string recipentId)
        {
            var user = await userService.FindOneById(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var recipent = await userService.FindOneById(recipentId);

            if (recipent == null || user.Id == recipent.Id)
            {
                return StatusCode(400, "User doesn't exist.");
            }

            var encryptedText = CryptoService.Encrypt(text);

            var message = chatService.CreateOne(encryptedText, user, recipent);

            return Ok(MessageMapper.CreateDto(message));
        }

        [HttpDelete("/chats/1")]
        [Authorize]
        public IActionResult DeleteMessage([FromQuery(Name = "id")] string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            chatService.DeleteOne(id, userId);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetIncomingMessages()
        {
            var encryptedMessages = await chatService.GetAll
                (
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

            return Ok(MessageMapper.CreateListDto(encryptedMessages));
        }
    }
}
