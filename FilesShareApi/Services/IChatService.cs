using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IChatService
    {
        Task<string> GetOne(List<UserChatEntityDto> interlocutors, MessageChatEntityDto message);

        Task<List<ChatEntity>> GetAll(string userId);

        Task DeleteOne(string chatId, string userId);
    }
}
