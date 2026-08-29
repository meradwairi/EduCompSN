using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, List<string>> _userConnections = new Dictionary<string, List<string>>();

        public override System.Threading.Tasks.Task OnConnected()
        {
            // جلب معرف المستخدم الذي أرسلته من العميل (Index.cshtml)
            string userId = Context.QueryString["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                lock (_userConnections)
                {
                    if (!_userConnections.ContainsKey(userId))
                        _userConnections[userId] = new List<string>();
                    if (!_userConnections[userId].Contains(Context.ConnectionId))
                        _userConnections[userId].Add(Context.ConnectionId);
                }
            }
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            string userId = Context.QueryString["userId"];
            if (!string.IsNullOrEmpty(userId) && _userConnections.ContainsKey(userId))
            {
                lock (_userConnections)
                {
                    _userConnections[userId].Remove(Context.ConnectionId);
                    if (_userConnections[userId].Count == 0)
                        _userConnections.Remove(userId);
                }
            }
            return base.OnDisconnected(stopCalled);
        }

        // دالة إرسال الرسالة الخاصة
        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.QueryString["userId"];
            if (string.IsNullOrEmpty(fromUserId)) return;

            // 1. حفظ الرسالة في قاعدة البيانات
            using (var db = new AppDbContext())
            {
                int fromId = int.Parse(fromUserId);
                int toId = int.Parse(toUserId);
                var chatMessage = new ChatMessage
                {
                    FromUserId = fromId,
                    ToUserId = toId,
                    Message = message,
                    SentAt = DateTime.Now,
                    IsRead = false
                };
                db.ChatMessages.Add(chatMessage);
                db.SaveChanges();

                var fromUser = db.Users.Find(fromId);
                var messageData = new
                {
                    Id = chatMessage.Id,
                    FromUserId = fromId,
                    FromUserName = fromUser?.FullName ?? "User",
                    Message = message,
                    SentAt = chatMessage.SentAt.ToString("HH:mm")
                };

                // 2. إرسال الرسالة إلى المرسل (لتحديث واجهته فورًا)
                Clients.Caller.onNewMessage(messageData);

                // 3. إرسال الرسالة إلى المستقبل إذا كان متصلاً
                if (_userConnections.ContainsKey(toUserId))
                {
                    foreach (var connectionId in _userConnections[toUserId])
                    {
                        Clients.Client(connectionId).onNewMessage(messageData);
                    }
                }
            }
        }
    }
}