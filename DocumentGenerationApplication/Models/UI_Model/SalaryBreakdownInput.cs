//using DocumentGenerationApplication.Models.Enums;
//using DocumentGenerationApplication.Models.Tables;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.Runtime.CompilerServices;

//namespace DocumentGenerationApplication.Models.UI_Model
//{
//    public class SalaryBreakdownInput : IValidatableObject
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [Display(Name = "Document Type")]
//        public int DocumentType { get; set; }

//        [Display(Name = "Employee Name")]
//        [StringLength(30, ErrorMessage = "Employee name cannot exceed 30 characters.")]
//        public string EmployeeName { get; set; } = string.Empty;

//        public int? EmployeeId { get; set; }

//        public EmployeeStatus Status { get; set; } = EmployeeStatus.Delayed;


//        [Display(Name = "Total CTC (INR)")]
//        public decimal TotalCTC { get; set; }

//        public string Department { get; set; } = string.Empty;
//        public string Band { get; set; } = string.Empty;

//        [StringLength(20, ErrorMessage = "Location cannot exceed 20 characters.")]
//        [Display(Name = "Location")]
//        public string JobLocation { get; set; } = string.Empty;

//        public string Grade { get; set; } = string.Empty;
//        public string Designation { get; set; } = string.Empty;
//        public string PFApplicability { get; set; } = string.Empty;

//        [Display(Name = "Is Metro Location")]
//        public bool IsMetro { get; set; }

//        [Display(Name = "Opted for NPS")]
//        public bool OptedNPS { get; set; }

//        [Display(Name = "Opted for VPF")]
//        public bool OptedVPF { get; set; }
//        public decimal ChosenRFB { get; set; }

//        public bool IsRFBSelected { get; set; }
//        public string RupeesInWords { get; set; } = string.Empty;

//        //public DateOnly JoiningDate { get; set; } = new DateOnly();
//        public DateTime JoiningDate { get; set; } = new DateTime();
//        public DateOnly JoiningDatePlus3Months { get; set; } = new DateOnly();
//        public DateOnly JoiningDatePlus6Months { get; set; } = new DateOnly();
//        public DateOnly JoiningDatePlus10Months { get; set; } = new DateOnly();

//        [EmailAddress(ErrorMessage = "Invalid email address.")]
//        [StringLength(30, ErrorMessage = "Email cannot exceed 30 characters.")]
//        public string Email { get; set; } = string.Empty;

//        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
//        public string MobileNumber { get; set; } = string.Empty;

//        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
//        public string Address_Line1 { get; set; } = string.Empty;

//        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
//        public string Address_Line2 { get; set; } = string.Empty;

//        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
//        public string Address_Line3 { get; set; } = string.Empty;

//        [StringLength(40, ErrorMessage = "Reference number cannot exceed 40 characters.")]
//        public string RefNo { get; set; } = string.Empty;

//        public DateTime? OfferValidTill { get; set; }
//        public int OfferValidTill1 { get; set; }

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            // Shared required fields
//            bool Require(params string[] values) => values.All(string.IsNullOrWhiteSpace);

//            switch (DocumentType)
//            {
//                case 1:
//                    if (Require(EmployeeName)) yield return new ValidationResult("Employee Name is required.", new[] { nameof(EmployeeName) });
//                    if (JoiningDate == default) yield return new ValidationResult("Joining Date is required.", new[] { nameof(JoiningDate) });
//                    if (TotalCTC <= 0) yield return new ValidationResult("Total CTC must be a positive number.", new[] { nameof(TotalCTC) });
//                    if (Require(Band)) yield return new ValidationResult("Band is required.", new[] { nameof(Band) });
//                    if (Require(Grade)) yield return new ValidationResult("Grade is required.", new[] { nameof(Grade) });
//                    if (Require(Department)) yield return new ValidationResult("Department is required.", new[] { nameof(Department) });
//                    if (Require(Designation)) yield return new ValidationResult("Designation is required.", new[] { nameof(Designation) });
//                    if (Require(Email)) yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
//                    if (Require(Address_Line1)) yield return new ValidationResult("Address is required.", new[] { nameof(Address_Line1) });
//                    if (Require(JobLocation)) yield return new ValidationResult("Location is required.", new[] { nameof(JobLocation) });
//                    if (Require(MobileNumber)) yield return new ValidationResult("Mobile Number is required.", new[] { nameof(MobileNumber) });
//                    if (Require(PFApplicability)) yield return new ValidationResult("PF Applicability is required.", new[] { nameof(PFApplicability) });
//                    if (ChosenRFB < 0) yield return new ValidationResult("Chosen RFB must be greater than 0.", new[] { nameof(ChosenRFB) });
//                    break;

//                case 2:
//                    if (Require(EmployeeName)) yield return new ValidationResult("Employee Name is required.", new[] { nameof(EmployeeName) });
//                    if (Require(Address_Line1)) yield return new ValidationResult("Address is required.", new[] { nameof(Address_Line1) });
//                    if (Require(MobileNumber)) yield return new ValidationResult("Mobile Number is required.", new[] { nameof(MobileNumber) });
//                    if (Require(Email)) yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
//                    if (JoiningDate == default) yield return new ValidationResult("Joining Date is required.", new[] { nameof(JoiningDate) });
//                    if (JoiningDatePlus3Months == default) yield return new ValidationResult("Probation Date is required.", new[] { nameof(JoiningDatePlus3Months) });
//                    if (JoiningDatePlus6Months == default) yield return new ValidationResult("Permanent Date is required.", new[] { nameof(JoiningDatePlus6Months) });
//                    break;

//                case 3:
//                    if (Require(EmployeeName)) yield return new ValidationResult("Employee Name is required.", new[] { nameof(EmployeeName) });
//                    if (JoiningDate == default) yield return new ValidationResult("Joining Date is required.", new[] { nameof(JoiningDate) });
//                    if (EmployeeId <= 0) yield return new ValidationResult("Employee ID must be a positive number.", new[] { nameof(EmployeeId) });
//                    if (TotalCTC <= 0) yield return new ValidationResult("Total CTC must be a positive number.", new[] { nameof(TotalCTC) });
//                    if (Require(Band)) yield return new ValidationResult("Band is required.", new[] { nameof(Band) });
//                    if (Require(Grade)) yield return new ValidationResult("Grade is required.", new[] { nameof(Grade) });
//                    if (Require(Department)) yield return new ValidationResult("Department is required.", new[] { nameof(Department) });
//                    if (Require(Designation)) yield return new ValidationResult("Designation is required.", new[] { nameof(Designation) });
//                    if (Require(Email)) yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
//                    if (Require(Address_Line1)) yield return new ValidationResult("Address is required.", new[] { nameof(Address_Line1) });
//                    if (Require(JobLocation)) yield return new ValidationResult("Location is required.", new[] { nameof(JobLocation) });
//                    if (Require(MobileNumber)) yield return new ValidationResult("Mobile Number is required.", new[] { nameof(MobileNumber) });
//                    if (Require(PFApplicability)) yield return new ValidationResult("PF Applicability is required.", new[] { nameof(PFApplicability) });
//                    if (ChosenRFB < 0) yield return new ValidationResult("Chosen RFB must be greater than 0.", new[] { nameof(ChosenRFB) });
//                    if (Require(RefNo)) yield return new ValidationResult("Ref No is required.", new[] { nameof(RefNo) });
//                    break;

//                case 4:
//                    if (Require(EmployeeName)) yield return new ValidationResult("Employee Name is required.", new[] { nameof(EmployeeName) });
//                    if (Require(Address_Line1)) yield return new ValidationResult("Address is required.", new[] { nameof(Address_Line1) });
//                    if (Require(MobileNumber)) yield return new ValidationResult("Mobile Number is required.", new[] { nameof(MobileNumber) });
//                    if (Require(Email)) yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
//                    if (JoiningDate == default) yield return new ValidationResult("Joining Date is required.", new[] { nameof(JoiningDate) });
//                    if (EmployeeId <= 0) yield return new ValidationResult("Employee ID must be a positive number.", new[] { nameof(EmployeeId) });
//                    if (Require(RefNo)) yield return new ValidationResult("Ref No is required.", new[] { nameof(RefNo) });
//                    if (JoiningDatePlus3Months == default) yield return new ValidationResult("Probation Date is required.", new[] { nameof(JoiningDatePlus3Months) });
//                    if (JoiningDatePlus6Months == default) yield return new ValidationResult("Permanent Date is required.", new[] { nameof(JoiningDatePlus6Months) });
//                    break;

//                default:
//                    yield return new ValidationResult("Invalid Document Type.");
//                    break;
//            }
//        }
//    }


//}


using DocumentGenerationApplication.Models.Enums;
using DocumentGenerationApplication.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.UI_Model
{
    public class SalaryBreakdownInput : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Document Type is required.")]
        [Display(Name = "Document Type")]
        public int DocumentType { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        [Display(Name = "Employee Name")]
        [StringLength(30, ErrorMessage = "Employee name cannot exceed 30 characters.")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee ID is required.")]
        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Delayed;

        [Required(ErrorMessage = "Total CTC is required.")]
        [Display(Name = "Total CTC (INR)")]
        public decimal TotalCTC { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Band is required.")]
        public string Band { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(20, ErrorMessage = "Location cannot exceed 20 characters.")]
        [Display(Name = "Location")]
        public string JobLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required.")]
        public string Grade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "PF Applicability is required.")]
        public string PFApplicability { get; set; } = string.Empty;

        [Required(ErrorMessage = "Is Metro Location must be specified.")]
        [Display(Name = "Is Metro Location")]
        public bool IsMetro { get; set; }

        [Required(ErrorMessage = "Opted NPS must be specified.")]
        [Display(Name = "Opted for NPS")]
        public bool OptedNPS { get; set; }

        [Required(ErrorMessage = "Opted VPF must be specified.")]
        [Display(Name = "Opted for VPF")]
        public bool OptedVPF { get; set; }

        [Required(ErrorMessage = "Chosen RFB is required.")]
        public decimal ChosenRFB { get; set; }

        [Required(ErrorMessage = "RFB selection is required.")]
        public bool IsRFBSelected { get; set; }

        [Required(ErrorMessage = "Rupees in words is required.")]
        public string RupeesInWords { get; set; } = string.Empty;

        [Required(ErrorMessage = "Joining Date is required.")]
        public DateTime JoiningDate { get; set; } = new DateTime();

        [Required(ErrorMessage = "Probation Date is required.")]
        public DateOnly JoiningDatePlus3Months { get; set; } = new DateOnly();

        [Required(ErrorMessage = "Permanent Date is required.")]
        public DateOnly JoiningDatePlus6Months { get; set; } = new DateOnly();

        [Required(ErrorMessage = "JoiningDatePlus10Months is required.")]
        public DateOnly JoiningDatePlus10Months { get; set; } = new DateOnly();

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(30, ErrorMessage = "Email cannot exceed 30 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address_Line1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Line 2 is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address_Line2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Line 3 is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address_Line3 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reference number is required.")]
        [StringLength(40, ErrorMessage = "Reference number cannot exceed 40 characters.")]
        public string RefNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Offer Valid Till is required.")]
        public DateTime? OfferValidTill { get; set; }

        [Required(ErrorMessage = "Offer Valid Till (int) is required.")]
        public int OfferValidTill1 { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Keep your conditional validation logic here
            if (TotalCTC <= 0)
                yield return new ValidationResult("Total CTC must be a positive number.", new[] { nameof(TotalCTC) });

            if (EmployeeId.HasValue && EmployeeId <= 0)
                yield return new ValidationResult("Employee ID must be a positive number.", new[] { nameof(EmployeeId) });

            if (ChosenRFB < 0)
                yield return new ValidationResult("Chosen RFB must be greater than or equal to 0.", new[] { nameof(ChosenRFB) });
        }
    }
}
