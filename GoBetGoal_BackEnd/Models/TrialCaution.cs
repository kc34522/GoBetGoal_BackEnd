using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class TrialCaution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_CautionDescription", IsUnique = true)]
        [StringLength(150)]
        public string CautionDescription { get; set; }

        public virtual ICollection<TrialTemplate> TrialTemplates { get; set; }
    }
}