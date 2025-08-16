using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class Context : DbContext
    {
        public Context() : base("name=Context")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<UserAvatar> UserAvatars { get; set; }
        public DbSet<TrialTemplate> TrialTemplates { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Trial> Trials { get; set; }
        public DbSet<UserStage> UserStages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Trial -> User
            modelBuilder.Entity<Trial>()
               .HasRequired(us => us.User)
               .WithMany(t => t.Trials)
               .HasForeignKey(us => us.UserId)
               .WillCascadeOnDelete(false);

            // Trial -> TrialTemplate
            modelBuilder.Entity<Trial>()
               .HasRequired(us => us.TrialTemplate)
               .WithMany(t => t.Trials)
               .HasForeignKey(us => us.TrialTemplateId)
               .WillCascadeOnDelete(false);

            // UserStage → Trial
            modelBuilder.Entity<UserStage>()
                .HasRequired(us => us.Trial)
                .WithMany(t => t.UserStages)
                .HasForeignKey(us => us.TrialId)
                .WillCascadeOnDelete(false);

            // UserStage → User
            modelBuilder.Entity<UserStage>()
                .HasRequired(us => us.User)
                .WithMany(u => u.UserStages)
                .HasForeignKey(us => us.UserId)
                .WillCascadeOnDelete(false);

            // UserStage → Stage
            modelBuilder.Entity<UserStage>()
                .HasRequired(us => us.Stage)
                .WithMany(u => u.UserStages)
                .HasForeignKey(us => us.StageId)
                .WillCascadeOnDelete(false);


            // UserAvatar → User
            modelBuilder.Entity<UserAvatar>()
                .HasRequired(ua => ua.User)
                .WithMany(u => u.UserAvatars)
                .HasForeignKey(ua => ua.UserId)
                .WillCascadeOnDelete(false);

            // UserAvatar → Avatar
            modelBuilder.Entity<UserAvatar>()
                .HasRequired(ua => ua.Avatar)
                .WithMany(a => a.UserAvatars)
                .HasForeignKey(ua => ua.AvatarId)
                .WillCascadeOnDelete(false);

            // Stage → TrialTemplate (新增)
            modelBuilder.Entity<Stage>()
                .HasRequired(s => s.TrialTemplate)
                .WithMany(tt => tt.Stages)
                .HasForeignKey(s => s.TrialTemplateId)
                .WillCascadeOnDelete(false);
        }



    }
}