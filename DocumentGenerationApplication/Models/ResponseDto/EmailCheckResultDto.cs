using DocumentGenerationApplication.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.ResponseDto
{
    public class EmailCheckResultDto
    {
        public bool Exists { get; set; }
        public string Message { get; set; }=string.Empty;

        // Optional: fill when data exists

        public int DocumentType { get; set; }

        public string? EmployeeName { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Delayed;


        public decimal TotalCTC { get; set; }

        public string Department { get; set; } = string.Empty;

        public string Band { get; set; } = string.Empty;

        public string JobLocation { get; set; } = string.Empty;

        public int? GradeId { get; set; }
        public int? DesignationId { get; set; }

        public string Grade { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string PFApplicability { get; set; } = string.Empty;

        public bool IsMetro { get; set; }

        public bool OptedNPS { get; set; }
        public bool OptedVPF { get; set; }
        public decimal ChosenRFB { get; set; }

        public bool IsRFBSelected { get; set; }
        public string RupeesInWords { get; set; } = string.Empty;

        public DateOnly JoiningDate { get; set; } = new DateOnly();
        public DateOnly JoiningDatePlus3Months { get; set; } = new DateOnly();
        public DateOnly JoiningDatePlus6Months { get; set; } = new DateOnly();
        public DateOnly JoiningDatePlus10Months { get; set; } = new DateOnly();
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Address_Line1 { get; set; } = string.Empty;
        public string Address_Line2 { get; set; } = string.Empty;
        public string Address_Line3 { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;

        public bool _IsBonusApplicable { get; set; }=false;

        public string _BonusAmount=string.Empty;

        public DateTime? OfferValidTill { get; set; }
        public int OfferValidTill1 { get; set; }
        // Add more fields as needed
    }

}
