namespace BusinessObjects.Models
{
    public enum MessageType { Text = 0, Image = 1, System = 2 }
    public enum MessageStatus { Pending = 0, Sent = 1, Failed = 2 }

    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageBase64 { get; set; }
        public string? AttachmentUrl { get; set; }

        public MessageType MessageType { get; set; } = MessageType.Text;
        public MessageStatus Status { get; set; } = MessageStatus.Sent;
        public string? Metadata { get; set; } 

        public string SenderId { get; set; } = "";
        public virtual User Sender { get; set; }  

        public string ReceiverId { get; set; } = "";
        public virtual User Receiver { get; set; }
    }
}
