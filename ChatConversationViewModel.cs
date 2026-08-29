using System;

namespace EduCompSN_Clean.Models
{
    public class ChatConversationViewModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}