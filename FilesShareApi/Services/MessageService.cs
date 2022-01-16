using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<MessageEntity> messages;

        public MessageService(IDbClient dbClient)
        {
            messages = dbClient.GetMessagesCollection();
        }

        public async Task DeleteOne(string id, string userId)
        {
            await messages.FindOneAndDeleteAsync(m => m.Id == id && m.SentById == userId);
        }

        public async Task<List<MessageEntity>> GetAll(string userId, string chatId)
        {
            var msgs = await messages.FindAsync(m => m.SentToId == userId || m.SentById == userId 
            && m.ChatId == chatId);
            return await msgs.ToListAsync();
        }

        public async Task<MessageEntity> CreateOne
            (
            string id,
            byte[] encryptedText,
            List<UserChatEntityDto> userDtos, 
            string chatId,
            DateTime sentTimeUtc
            )
        {
            var msg = new MessageEntity
                (
                id,
                userDtos[0].Id,
                userDtos[0].Name,
                userDtos[1].Id,
                userDtos[1].Name,
                encryptedText,
                chatId,
                sentTimeUtc
                );
            await messages.InsertOneAsync(msg);
            return msg;
        }
    }
}
