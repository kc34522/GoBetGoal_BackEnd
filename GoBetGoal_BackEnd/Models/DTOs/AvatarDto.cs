using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models.DTOs
{
    public class AvatarDto
    {
        public int Id { get; set; }
        public string AvatarImagePath { get; set; }
        public int AvatarPrice { get; set; }
        public bool IsLocked { get; set; }
        public bool IsUnlocked { get; set; }

    }
}