using System;

namespace FilesShareApi
{
    public class MessageChatMapper
    {
        public static MessageChatEntityDto CreateDto
            (
            string text, 
            string sentByName
            )
        {
            return new MessageChatEntityDto()
            {
                Id = Guid.NewGuid().ToString(),
                SentTimeUtc = DateTime.Now,
                EncryptedText = CryptoService.Encrypt(text),
                SentByName = sentByName,
            };
        }
    }
}
