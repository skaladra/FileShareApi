namespace FilesShareApi
{
    public class ChatResponseDto
    {
        public string Id { get; set; }

        public string InterlocutorId { get; set; }

        public string InterlocutorName { get; set; }

        public MessageResponseDto LastMessage { get; set; }
    }
}
