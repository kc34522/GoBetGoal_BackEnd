using GoBetGoal_BackEnd.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace GoBetGoal_BackEnd.Models
{
    public class TrialTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TrialTitle { get; set; }

        [Required]
        [StringLength(200)]
        public string TrialDescription { get; set; }

        [Required]
        public int TrialFrequency { get; set; }

        [Required]
        public int StageCount { get; set; }

        [Required]
        public TrialCategory TrialCategory { get; set; }

        [Required]
        public int MaxUser {  get; set; }

        [Required]
        public bool IsAi { get; set; }

        [Required]
        public int TrialPrice { get; set; } = 0;

        //public ProductType ProductType { get; set; }

        [Required]
        [StringLength(200)]
        public string CardImagePath { get; set; }
        [Required]
        [StringLength(10)]
        public string CardColor { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<SuitForTag> SuitForTags { get; set; } = new List<SuitForTag>();
        public virtual ICollection<NotSuitForTag> NotSuitForTags { get; set; } = new List<NotSuitForTag>();
        public virtual ICollection<TrialRule> TrialRules { get; set; } = new List<TrialRule>();
        public virtual ICollection<TrialCaution> TrialCautions { get; set; } = new List<TrialCaution>();
        public virtual ICollection<TrialEffection> TrialEffections { get; set; } = new List<TrialEffection>();

    }
}