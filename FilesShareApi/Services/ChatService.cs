using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<ChatEntity> chats;

        public ChatService(IDbClient dbClient)
        {
            chats = dbClient.GetChatsCollection();
        }

        public async Task<string> GetOne(List<UserChatEntityDto> interlocutors, MessageChatEntityDto message)
        {
            var chat = new ChatEntity(interlocutors, message);
            var chatResult = await chats.FindOneAndUpdateAsync
                (
                (x => x.FirstInterlocutorId == interlocutors[0].Id 
                && x.SecondInterlocutorId == interlocutors[1].Id 
                || x.FirstInterlocutorId == interlocutors[1].Id
                && x.SecondInterlocutorId == interlocutors[0].Id),
                Builders<ChatEntity>.Update.Set(y => y.LastMessage, message)
                );
            if (chatResult == null)
            {
                await chats.InsertOneAsync(chat);
            }
            return chat.Id;
        }

        public async Task<List<ChatEntity>> GetAll(string userId)
        {
            var chatsCursor = await chats.FindAsync(x => x.FirstInterlocutorId == userId 
            || x.SecondInterlocutorId == userId);
            return await chatsCursor.ToListAsync();
        }

        public async Task DeleteOne(string chatId, string userId)
        {
            await chats.FindOneAndDeleteAsync((x => x.Id == chatId 
            && x.SecondInterlocutorId == userId 
            || x.FirstInterlocutorId == userId));

        }
    }
}
