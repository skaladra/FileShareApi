using MongoDB.Driver;

namespace FilesShareApi
{
    public interface IDbClient
    {
        IMongoCollection<FileEntity> GetFilesCollection();
        IMongoCollection<UserEntity> GetUsersCollection();
        IMongoCollection<MessageEntity> GetMessagesCollection();
        IMongoCollection<ChatEntity> GetChatsCollection();
    }
}
