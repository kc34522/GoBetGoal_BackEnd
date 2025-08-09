using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class UserAvatar
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int AvatarId { get; set; }
        [ForeignKey("AvatarId")]
        public virtual Avatar Avatar { get; set; }

        public bool IsCurrent { get; set; }=false;

        [Column(TypeName ="datetime2")]
        public DateTime AcquiredAt { get; set; }
    }
}