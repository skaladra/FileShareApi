using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FilesShareApi
{
    public class MessageChatEntityDto
    {
        [BsonId]
        public string Id { get; set; }
        public DateTime SentTimeUtc { get; set; }
        public byte[] EncryptedText { get; set; }
        public string SentByName { get; set; }
    }
}
