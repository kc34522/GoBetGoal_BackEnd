using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoBetGoal_BackEnd.Models
{
    public class Stage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StageIndex { get; set; }

        [StringLength(200)]
        public string StageSampleImagePath { get; set; }
        
        [Required]
        [StringLength(200)]
        public string StageDescription { get; set; }

        //[Required]
        //[StringLength(200)]
        //public string StageTitle { get; set; }

        //[Required]
        //[StringLength(200)]
        //public string StageContent { get; set; }

        [Required]
        public int TrialTemplateId { get; set; }

        [ForeignKey("TrialTemplateId")]
        public virtual TrialTemplate TrialTemplate { get; set; }

        //[Required]
        //public bool IsActive { get; set; } = true;

        //[Required]
        //[Column(TypeName = "datetime2")]
        //public DateTime CreatedAt { get; set; } = DateTime.Now;




        //public virtual ICollection<UserStage> UserStages { get; set; } = new List<UserStage>();
    }
}