using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class TrialEffection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_EffectionDescription", IsUnique = true)]
        [StringLength(150)]
        public string EffectionDescription { get; set; }

        public virtual ICollection<TrialTemplate> TrialTemplates { get; set; }
    }
}