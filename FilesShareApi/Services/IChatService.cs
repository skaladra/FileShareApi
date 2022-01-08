using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public interface IChatService
    {
        MessageEntity SendMessage(byte[] text, UserEntity user, UserEntity recipent);
        void DeleteMessage(string id, string userId);
        Task<List<MessageEntity>> GetMessages(string userId);
    }
}
