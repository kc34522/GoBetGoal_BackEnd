using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class SuitForTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index("IX_SuitForTagName", IsUnique = true)]
        [StringLength(100)]
        public string TagName { get; set; }

        public virtual ICollection<TrialTemplate> TrialTemplates { get; set; }



    }
}