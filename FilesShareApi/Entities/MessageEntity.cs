using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FilesShareApi
{
    public class MessageEntity
    {
        public MessageEntity
            (
            string userId, 
            string userName, 
            string recipentId, 
            string recipentName, 
            byte[] encryptedText
            )
        {
            Id = Guid.NewGuid().ToString();
            SentById = userId;
            SentByName = userName;
            SentTimeUtc = DateTime.Now;
            SentTime = SentTimeUtc.ToLocalTime();
            SentToId = recipentId;
            SentToName = recipentName;
            EncryptedText = encryptedText;
        }

        [BsonId]
        public string Id { get; set;  }
        public DateTime SentTimeUtc { get; set; }
        public DateTime SentTime { get; set; } 
        public byte[] EncryptedText { get; set; }
        public string SentById { get; set; }
        public string SentToId { get; set; }
        public string SentByName { get; set; }
        public string SentToName { get; set; }
    }
}
