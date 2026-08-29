using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        // تأكد من أن اسم DbContext صحيح (AppDbContext أو ما يعادله)
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int? withUserId)
        {
            int currentUserId = (int)Session["UserId"];
            ViewBag.WithUserId = withUserId;
            var conversations = db.ChatMessages
                .Where(m => m.FromUserId == currentUserId || m.ToUserId == currentUserId)
                .GroupBy(m => m.FromUserId == currentUserId ? m.ToUserId : m.FromUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMsg = g.OrderByDescending(m => m.SentAt).FirstOrDefault().Message,
                    LastTime = g.OrderByDescending(m => m.SentAt).FirstOrDefault().SentAt
                })
                .ToList()
                .Select(x => new ChatConversationViewModel
                {
                    UserId = x.UserId,
                    UserFullName = db.Users.Find(x.UserId)?.FullName ?? "Unknown",
                    LastMessage = x.LastMsg ?? "",
                    LastMessageTime = x.LastTime == default(DateTime) ? DateTime.Now : x.LastTime
                }).ToList();
            return View(conversations);
        }

        [HttpPost]
        public JsonResult SendMessage(int toUserId, string message)
        {
            int fromUserId = (int)Session["UserId"];
            var msg = new ChatMessage
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Message = message,
                SentAt = DateTime.Now,
                IsRead = false
            };
            db.ChatMessages.Add(msg);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult GetConversation(int withUserId)
        {
            int currentUserId = (int)Session["UserId"];
            var messages = db.ChatMessages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == withUserId) ||
                            (m.FromUserId == withUserId && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList()
                .Select(m => new
                {
                    m.Id,
                    m.FromUserId,
                    m.Message,
                    SentAt = m.SentAt.ToString("HH:mm"),
                    FromUserFullName = db.Users.Find(m.FromUserId)?.FullName ?? "Unknown"
                });
            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewMessages(int withUserId, int lastMessageId)
        {
            int currentUserId = (int)Session["UserId"];
            var newMessages = db.ChatMessages
                .Where(m => ((m.FromUserId == currentUserId && m.ToUserId == withUserId) ||
                             (m.FromUserId == withUserId && m.ToUserId == currentUserId)) && m.Id > lastMessageId)
                .OrderBy(m => m.SentAt)
                .ToList()
                .Select(m => new
                {
                    m.Id,
                    m.FromUserId,
                    m.Message,
                    SentAt = m.SentAt.ToString("HH:mm"),
                    FromUserName = db.Users.Find(m.FromUserId)?.FullName ?? "Unknown"
                });
            return Json(newMessages, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}