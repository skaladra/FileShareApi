using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public static class MessageMapper
    {
        public static List<MessageDto> CreateListDto(IEnumerable<MessageEntity> messages)
        {
            var messagesList = new List<MessageDto>();

            foreach (var msg in messages)
            {
                messagesList.Add(CreateDto(msg));
            }
            return messagesList;
        }

        public static MessageDto CreateDto(MessageEntity message)
        {
            var text = CryptoService.Decrypt(message.EncryptedText);
            return new MessageDto()
            {
                Id = message.Id,
                SentTime = message.SentTime,
                Text = text,
                SentByName = message.SentByName,
                SentToName = message.SentToName
            };
        }
    }
}
