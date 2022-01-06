using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<MessageEntity> messages;

        public ChatService(IDbClient dbClient)
        {
            messages = dbClient.GetMessagesCollection();
        }

        public void DeleteMessage(string id, string userId)
        {
            messages.FindOneAndDeleteAsync(m => m.Id == id && m.SentById == userId);
        }

        public async Task<List<MessageEntity>> GetMessages(string userId)
        {
            var msgs = await messages.FindAsync(m => m.SentToId == userId || m.SentById == userId);

            return await msgs.ToListAsync();
        }

        public MessageEntity SendMessage(string text, UserEntity user, UserEntity recipent)
        {
            var msg = new MessageEntity
                (
                user.Id.ToString(), 
                user.UserName, 
                recipent.Id.ToString(), 
                recipent.UserName, 
                text
                );
            messages.InsertOne(msg);
            return msg;
        }
    }
}
