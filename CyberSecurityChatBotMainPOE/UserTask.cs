using System;

namespace CyberSecurityChatBotMainPOE.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserTask()
        {
            Status = "Pending";
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            string statusEmoji = Status == "Completed" ? "✅" : "⏳";
            return $"{Id} | {statusEmoji} {Title} | {Status}";
        }
    }
}