using MongoDbGenericRepository.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.IO;

namespace FilesShareApi
{
    [CollectionName("Files")]
    public class FileEntity
    {
        public FileEntity(string userId, string contentType, string fileName, string link, bool deleteOnceDownload)
        {
            Id = Guid.NewGuid().ToString();
            Name = fileName;
            Url = link + $"/download?id={Id}";
            CreatedTimeUtc = DateTime.Now;
            CreatedTime = CreatedTimeUtc.ToLocalTime();
            DeleteAfterDownload = deleteOnceDownload;
            CreatorId = userId;
            DocumentType = contentType;
            S3Name = Id + Path.GetExtension(fileName);
            ToDelete = false;
        }

        public FileEntity(bool toDelete, string id)
        {
            Id = id;
            Name = null;
            Url = null;
            DeleteAfterDownload = true;
            CreatorId = null;
            DocumentType = null;
            S3Name = null;
            ToDelete = true;
        }

        /// <summary>
        /// File's identifier 
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Name of the file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url which transition leads to downloading a file
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Time of adding
        /// </summary>
        public DateTime CreatedTimeUtc { get; set; }

        /// <summary>
        /// Should the file be deleted once it was downloaded
        /// </summary>
        public DateTime CreatedTime { get; set; }
        public bool DeleteAfterDownload { get; set; }

        /// <summary>
        /// Identifier of User who has created this file
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Type of a file
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Name on S3 storage
        /// </summary>
        public string S3Name { get; set; }

        /// <summary>
        /// Should file be deleted by files cleaner
        /// </summary>
        public bool ToDelete { get; set; }
    }
}
