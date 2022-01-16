using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FilesShareApi
{
    public class MessageEntity
    {
        public MessageEntity
            (
            string id,
            string userId, 
            string userName, 
            string recipentId, 
            string recipentName, 
            byte[] encryptedText,
            string chatId,
            DateTime sentTimeUtc
            )
        {
            Id = id;
            SentById = userId;
            SentByName = userName;
            SentTimeUtc = sentTimeUtc;
            SentToId = recipentId;
            SentToName = recipentName;
            EncryptedText = encryptedText;
            ChatId = chatId;
        }

        [BsonId]
        public string Id { get; set; }
        public string ChatId { get; set; }
        public DateTime SentTimeUtc { get; set; }
        public byte[] EncryptedText { get; set; }
        public string SentById { get; set; }
        public string SentToId { get; set; }
        public string SentByName { get; set; }
        public string SentToName { get; set; }
    }
}
