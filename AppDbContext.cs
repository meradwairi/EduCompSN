using System.Data.Entity;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Models
{
    public class AppDbContext : DbContext
    {
        // الجداول الأساسية
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }

        // جداول البروفايل المتكامل
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }

        // جداول التفاعل والإشعارات
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // جدول الدردشة
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public AppDbContext() : base("DefaultConnection")
        {
            // تعطيل التهيئة التلقائية لقاعدة البيانات (نتحكم يدويًا عبر Migrations)
            Database.SetInitializer<AppDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== حل مشكلة "Multiple cascade paths" في ChatMessages =====
            // تعطيل الحذف التتالي (Cascade Delete) لكلا المفتاحين الخارجيين
            modelBuilder.Entity<ChatMessage>()
                .HasRequired(cm => cm.FromUser)
                .WithMany()
                .HasForeignKey(cm => cm.FromUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMessage>()
                .HasRequired(cm => cm.ToUser)
                .WithMany()
                .HasForeignKey(cm => cm.ToUserId)
                .WillCascadeOnDelete(false);

            // إذا لم تكن لديك خصائص FromUser و ToUser في نموذج ChatMessage،
            // يمكنك استخدام الطريقة التالية (بدون تحديد العلاقة العكسية):
            /*
            modelBuilder.Entity<ChatMessage>()
                .HasRequired(cm => cm.FromUser)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMessage>()
                .HasRequired(cm => cm.ToUser)
                .WithMany()
                .WillCascadeOnDelete(false);
            */

            // يمكنك إضافة تكوينات إضافية هنا (مثل تعيين أسماء الجداول، الفهارس، إلخ)
        }
    }
}