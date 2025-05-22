namespace Domain
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string RecipientId { get; set; }
        public AppUser Recipient { get; set; }
    }
}
