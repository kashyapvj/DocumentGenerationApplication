using DocumentGenerationApplication.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentGenerationApplication.Models.Tables
{
    [Index(nameof(EmployeeId), IsUnique = true)]
    [Index(nameof(RefNo), IsUnique = true)]
    public class EmployeeDetails
    {
        [Key]
        public int Id { get; set; }
        public int? EmployeeId { get; set; }

        public int DocumentType { get; set; }

        public string RefNo { get; set; }=string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int BandId { get; set; } //Foreign Key
        public int GradeId { get; set; } //Foreign Key
 
        public int DepartmentId { get; set; }//Foreign Key
 
        public int DesignationId { get; set; }//Foreign Key
      

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ProbationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PermanentDate { get; set; }

        public decimal? TotalCompensation { get; set; }

        public string? WorkingDays { get; set; } = string.Empty;

        [NotMapped]
        public bool IsBonusApplicable { get; set; }
        public string? BonusAmount { get; set; } = "Not Applicable";

        [NotMapped]
        public bool IsProbationApplicable { get; set; }
        public string? Probation { get; set; } = string.Empty;

        public string PFApplicability { get; set; } = string.Empty;
        public string Email {  get; set; }= string.Empty;
        public string Address_Line1 { get; set; } = string.Empty;
        public string Address_Line2 { get; set; } = string.Empty;
        public string Address_Line3 { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public EmployeeStatus Status { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OfferValidTill { get; set; }
        public int OfferValidTill1 { get; set; }

        [ForeignKey(nameof(BandId))]
        public Band? Band { get; set; }

        [ForeignKey(nameof(GradeId))]
        public Grade? Grade { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        [ForeignKey(nameof(DesignationId))]
        public Designation? Designation { get; set; }

    }
}
