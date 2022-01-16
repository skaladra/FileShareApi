using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IMessageService
    {
        Task<MessageEntity> CreateOne(string id, byte[] text, List<UserChatEntityDto> userDtos, string chatId, DateTime sentTimeUtc);
        Task DeleteOne(string id, string userId);
        Task<List<MessageEntity>> GetAll(string userId, string chatId);
    }
}
