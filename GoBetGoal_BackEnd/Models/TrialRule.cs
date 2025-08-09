using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class TrialRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_RuleDescription", IsUnique = true)]
        [StringLength(200)]
        public string RuleDescription { get; set; }

        public virtual ICollection<TrialTemplate> TrialTemplates { get; set; }
    }
}