using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FilesShareApi
{
    public class DbClient : IDbClient
    {
        private readonly IMongoCollection<FileEntity> files;
        private readonly IMongoCollection<ApplicationUser> users;

        public DbClient(IOptions<FilesShareApiDbConfig> filesDbConfig)
        {
            var client = new MongoClient(filesDbConfig.Value.Connection_String);
            var dataBase = client.GetDatabase(filesDbConfig.Value.Database_Name);
            files = dataBase.GetCollection<FileEntity>(filesDbConfig.Value.Files_Collection_Name);
            users = dataBase.GetCollection<ApplicationUser>(filesDbConfig.Value.Users_Collection_Name);
        }
        public IMongoCollection<FileEntity> GetFilesCollection()
        {
            return files;
        }

        public IMongoCollection<ApplicationUser> GetUsersCollection()
        {
            return users;
        }
    }
}
