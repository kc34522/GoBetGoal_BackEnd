using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class Avatar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string AvatarImagePath {  get; set; }

        [Required]
        public int AvatarPrice { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserAvatar> UserAvatars { get; set; }

    }
}