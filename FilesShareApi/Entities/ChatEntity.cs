using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FilesShareApi
{
    public class ChatEntity
    {
        public ChatEntity(List<UserChatEntityDto> interlocutors, MessageChatEntityDto message)
        {
            Id = Guid.NewGuid().ToString();
            FirstInterlocutorId = interlocutors[0].Id;
            FirstInterlocutorName = interlocutors[0].Name;
            SecondInterlocutorId = interlocutors[1].Id;
            SecondInterlocutorName = interlocutors[1].Name;
            LastMessage = message;
        }

        [BsonId]
        public string Id { get; set; }
        public string FirstInterlocutorId { get; set; }
        public string SecondInterlocutorId { get; set; }
        public string FirstInterlocutorName { get; set; }
        public string SecondInterlocutorName { get; set; }
        public MessageChatEntityDto LastMessage { get; set; }
    }
}
