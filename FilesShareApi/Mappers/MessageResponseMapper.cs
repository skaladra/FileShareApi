using System.Collections.Generic;

namespace FilesShareApi
{
    public static class MessageResponseMapper
    {
        public static List<MessageResponseDto> CreateListDto(IEnumerable<MessageEntity> messages)
        {
            var messagesList = new List<MessageResponseDto>();

            foreach (var msg in messages)
            {
                messagesList.Add(CreateDto(msg));
            }
            return messagesList;
        }

        public static MessageResponseDto CreateDto(MessageEntity message)
        {
            var text = CryptoService.Decrypt(message.EncryptedText);
            return new MessageResponseDto()
            {
                Id = message.Id,
                SentTime = message.SentTimeUtc,
                Text = text,
                SentByName = message.SentByName,
                SentToName = message.SentToName
            };
        }
    }
}
