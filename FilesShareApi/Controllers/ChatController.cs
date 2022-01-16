using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilesShareApi.Controllers
{
    [ApiController]
    [Route("/chats")]
    public class ChatController : Controller
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;
        private readonly IChatService chatService;

        public ChatController
            (
            IMessageService messageService, 
            IUserService userService, 
            IChatService chatService
            )
        {
            this.messageService = messageService;
            this.userService = userService;
            this.chatService = chatService;
        }


        [HttpPost("/chats/1")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] string text,
            [FromQuery(Name = "recipent")] string recipentId)
        {
            var user = await userService.FindOneById(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var recipent = await userService.FindOneById(recipentId);

            if (recipent == null || user.Id == recipent.Id)
            {
                return StatusCode(400, "{ Error: User doesn't exist. }");
            }

            var userDtos = UserChatMapper.CreateDtoList(new List<UserEntity> { user, recipent });
            var messageDto = MessageChatMapper.CreateDto(text, user.UserName);
            var chatId = await chatService.GetOne(userDtos, messageDto);
            var message = await messageService.CreateOne
                (
                messageDto.Id, 
                messageDto.EncryptedText, 
                userDtos, 
                chatId, 
                messageDto.SentTimeUtc
                );

            return Ok(MessageResponseMapper.CreateDto(message));
        }

        [HttpDelete("/chats/message/1")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage([FromQuery(Name = "id")] string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await messageService.DeleteOne(id, userId);

            return Ok();
        }

        [HttpDelete("/chats/1")]
        [Authorize]
        public async Task<IActionResult> DeleteChat(string chatId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await chatService.DeleteOne(chatId, userId);

            return Ok();
        }

        [HttpGet("/chats/1")]
        [Authorize]
        public async Task<IActionResult> GetChatMessages(string id)
        {
            var messages = await messageService.GetAll
                (
                this.User.FindFirstValue(ClaimTypes.NameIdentifier), id
                );

            return Ok(MessageResponseMapper.CreateListDto(messages));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllChats()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chats = await chatService.GetAll
                (
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

            return Ok(ChatResponseMapper.CreateListDto(chats, userId));
        }
    }
}
