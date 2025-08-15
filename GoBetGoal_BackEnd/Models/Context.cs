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

    }
}