using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace FilesShareApi
{
    [CollectionName("Files")]
    public class FileEntity
    {
        /// <summary>
        /// Guid converted to srting
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
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Should the file be deleted once it was downloaded
        /// </summary>
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
