using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IChatService
    {
        MessageEntity CreateOne(byte[] text, UserEntity user, UserEntity recipent);
        void DeleteOne(string id, string userId);
        Task<List<MessageEntity>> GetAll(string userId);
    }
}
