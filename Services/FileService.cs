using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class FileService : IFileService
    {
        private readonly IMongoCollection<FileEntity> files;

        public FileService(IDbClient dbClient)
        {
            files = dbClient.GetFilesCollection();
        }

        /// <summary>
        /// Gets lists of files that user has
        /// </summary>
        /// <param name="id">file's identifier </param>
        /// <returns></returns>
        public async Task<List<FileEntity>> GetFiles(string id)
        {
            var filesList = await files.FindAsync(file => file.CreatorId == id);

            return await filesList.ToListAsync();
        }

        /// <summary>
        /// Deletes file created by user by it's identifier 
        /// </summary>
        /// <param name="id">file's identifier </param>
        /// <param name="creatorId">author's id</param>
        /// <returns></returns>
        public FileEntity DeleteFile(string id, string creatorId)
        {
            var file = files.FindOneAndDelete( file => file.CreatorId == creatorId && file.Id == id);

            return file;

        }
        /// <summary>
        /// Find file by identifier  
        /// </summary>
        /// <param name="id">file's identifier </param>
        /// <returns></returns>
        public FileEntity GetFileById(string id)
        {
            return files.Find(file => file.Id == id).First();
        }

        /// <summary>
        /// Add file reference to database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string AddFile(FileEntity file)
        {
            files.InsertOne(file);

            return file.Id;
        }

        /// <summary>
        /// Get list of files that were marked to be deleted
        /// </summary>
        /// <returns></returns>
        public List<FileEntity> GetFilesToDelete() 
        { 
            return files.Find(file => file.ToDelete == true).ToList(); 
        }

        /// <summary>
        /// Mark file to be deleted
        /// </summary>
        /// <param name="file"></param>
        public void SetFileToDelete(FileEntity file)
        {
            var toDelete = true;

            var fileToDelete = new FileEntity(toDelete, file.Id);

            var filter = new BsonDocument("_id", file.Id);

            files.ReplaceOneAsync(filter, fileToDelete);
        }
    }
}
