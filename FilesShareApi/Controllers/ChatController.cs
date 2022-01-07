using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [ApiController]
    [Route("chats")]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] string text,
            [FromQuery(Name ="recipent")] string recipentId)
        {
            var user = await userService.FindById(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var recipent = await userService.FindById(recipentId);
            if (recipent == null || user.Id == recipent.Id)
            {
                return StatusCode(400, "User doesn't exist.");
            }
            var result = chatService.SendMessage(text, user, recipent);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult DeleteMessage(string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            chatService.DeleteMessage(id, userId);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetIncomingMessages()
        {
            var messages = await chatService.GetMessages
                (
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

            return Ok(messages);
        }
    }
}
