using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FilesShareApi
{
    public class DbClient : IDbClient
    {
        private readonly IMongoDatabase dataBase;
        private readonly string filesConnectionName;
        private readonly string usersConnectionName;
        private readonly string messagesConnectionName;

        public DbClient(IOptions<FilesShareApiDbConfig> filesDbConfig)
        {
            var client = new MongoClient(filesDbConfig.Value.Connection_String);
            dataBase = client.GetDatabase(filesDbConfig.Value.Database_Name);
            filesConnectionName = filesDbConfig.Value.Files_Collection_Name;
            usersConnectionName = filesDbConfig.Value.Users_Collection_Name;
            messagesConnectionName = filesDbConfig.Value.Messages_Collection_Name;
        }
        public IMongoCollection<FileEntity> GetFilesCollection()
        {
            return dataBase.GetCollection<FileEntity>(filesConnectionName);
        }

        public IMongoCollection<UserEntity> GetUsersCollection()
        {
            return dataBase.GetCollection<UserEntity>(usersConnectionName);
        }

        public IMongoCollection<MessageEntity> GetMessagesCollection()
        {
            return dataBase.GetCollection<MessageEntity>(messagesConnectionName);
        }
    }
}
