using System;

namespace CyberSecurityChatBotMainPOE.Models
{
    public class ActivityLogEntry
    {
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }

        public ActivityLogEntry(string action, string description)
        {
            Action = action;
            Description = description;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss} | {Action} | {Description}";
        }
    }
}