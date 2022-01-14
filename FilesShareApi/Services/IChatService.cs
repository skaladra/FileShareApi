using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IChatService
    {
        Task<MessageEntity> CreateOne(byte[] text, UserEntity user, UserEntity recipent);
        Task DeleteOne(string id, string userId);
        Task<List<MessageEntity>> GetAll(string userId);
    }
}
