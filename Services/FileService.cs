using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

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
        public List<FileEntity> GetFiles(string id)
        {
            return files.Find(file => file.CreatorId == id).ToList();
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
            var fileToDelete = new FileEntity()
            {
                Id = file.Id,
                Name = file.Name,
                Url = null,
                DeleteAfterDownload = true,
                CreatorId = null,
                DocumentType = null,
                S3Name = file.S3Name,
                ToDelete = true
            };

            var filter = new BsonDocument("_id", file.Id);

            files.ReplaceOneAsync(filter, fileToDelete);
        }
    }
}
