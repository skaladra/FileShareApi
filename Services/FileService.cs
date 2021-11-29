using MongoDB.Bson;
using MongoDB.Driver;
using System;
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
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FileEntity> GetFiles(string id)
        {
            var filter = new BsonDocument("CreatorId", id);
            return files.Find(filter).ToList();
        }

        /// <summary>
        /// Deletes file created by user by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public FileEntity DeleteFile(string id, string creatorId)
        {
            var filter = new BsonDocument("_id", id);
            var file = files.FindOneAndDelete( file => file.CreatorId == creatorId && file.Id == id);
            return file;

        }

        public FileEntity GetFileById(string id)
        {
            var filter = new BsonDocument("_id", id);
            return files.Find(filter).First();
        }

        public string AddFile(FileEntity file)
        {

            files.InsertOne(file);
            return file.Id;
        }

    }
}
