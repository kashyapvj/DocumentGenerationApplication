using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.Intern
{
    public class InternOfferLetter
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string InternName { get; set; }=string.Empty;

        [Required]
        public string Address_Line_1 { get; set; } = string.Empty;

        [Required]
        public string Address_Line_2 { get; set; } = string.Empty;

        [Required]
        public string Address_Line_3 { get; set; } = string.Empty;

        [Required]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Designation { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }

        [Required]
        public DateTime RelievingDate { get; set; }

        [Required]
        public string Department { get; set; }=string.Empty;
    }
}
