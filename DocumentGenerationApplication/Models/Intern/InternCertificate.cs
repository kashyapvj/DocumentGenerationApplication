using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.InternCertificate
{
    public class InternCertificate
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Intern Name is required.")]
        [StringLength(25, ErrorMessage = "Intern Name cannot exceed 25 characters.")]
        public string InternName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Created On date is required.")]
        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "Duration in months is required.")]
        [Range(1, 24, ErrorMessage = "Duration should be between 1 and 24 months.")]
        public int DurationInMonths { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Domain is required.")]
        [StringLength(20, ErrorMessage = "Domain cannot exceed 20 characters.")]
        public string Domain { get; set; } = string.Empty;
    }
}
