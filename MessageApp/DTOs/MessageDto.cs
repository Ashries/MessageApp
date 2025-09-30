namespace MessageApp.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsPublic { get; set; }
        public UserDto Sender { get; set; } = null!;
        public UserDto? Receiver { get; set; }
        public int? PreviousMessageId { get; set; }
    }

    public class CreateMessageDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int? ReceiverId { get; set; } // Null for public messages
        public int? PreviousMessageId { get; set; } // For replies
    }

    public class UpdateMessageDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}