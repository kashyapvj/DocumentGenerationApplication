using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentGenerationApplication.Models.Tables
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to Band
        public int BandId { get; set; }

        // Navigation property (with FK explicitly defined, optional)
        [ForeignKey(nameof(BandId))]
        public Band? Band { get; set; }

        public string GradeName { get; set; } = string.Empty;
        public string ExperienceRange { get; set; } = string.Empty;
    }
}
