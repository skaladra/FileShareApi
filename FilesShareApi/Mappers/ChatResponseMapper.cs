using System.Collections.Generic;

namespace FilesShareApi
{
    public static class ChatResponseMapper
    {
        private static ChatResponseDto CreateDto
            (
            string chatId,
            MessageChatEntityDto lastMessage,
            (string, string) interlocutorInfo
            )
        {
            return new ChatResponseDto()
            {
                Id = chatId,
                InterlocutorId = interlocutorInfo.Item1, 
                InterlocutorName = interlocutorInfo.Item2,
                LastMessage = new MessageResponseDto()
                {
                    Id = lastMessage.Id,
                    SentByName = lastMessage.SentByName,
                    SentTime = lastMessage.SentTimeUtc,
                    Text = CryptoService.Decrypt(lastMessage.EncryptedText)
                }
            };
        }

        public static List<ChatResponseDto> CreateListDto(List<ChatEntity> chats, string userId)
        {
            
            var chatList = new List<ChatResponseDto>();
            foreach (var chat in chats)
            {
                var interlocutorInfo = GetInterlocutorInfo(userId, chat);
                chatList.Add(CreateDto(chat.Id, chat.LastMessage, interlocutorInfo));
            }
            return chatList;
        }

        private static (string, string) GetInterlocutorInfo(string userId, ChatEntity chat)
        {
            if (chat.FirstInterlocutorId == userId)
            {
                return (chat.SecondInterlocutorId, chat.SecondInterlocutorName);
            }
            return (chat.FirstInterlocutorId, chat.FirstInterlocutorName);
        }
    }
}
