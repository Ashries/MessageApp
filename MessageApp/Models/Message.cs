using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApp.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsPublic => ReceiverId == null;

        // Foreign keys
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; } // Null for public messages
        public int? PreviousMessageId { get; set; } // For message threads

        // Navigation properties
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }

        [ForeignKey("PreviousMessageId")]
        public virtual Message? PreviousMessage { get; set; }

        // Collection of replies to this message
        public virtual ICollection<Message> Replies { get; set; } = new List<Message>();
    }
}