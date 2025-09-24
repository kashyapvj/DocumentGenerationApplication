using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentGenerationApplication.Models.Tables
{

    [Index(nameof(EmployeeId), IsUnique = true)]
    public class SalaryBreakdown
    {
        [Key]
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
 
        public string EmployeeName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public bool IsMetro { get; set; }
        public decimal TotalCompensation { get; set; }
        public decimal Basic { get; set; }
        public decimal HRA { get; set; }
        public decimal StatutoryBonus { get; set; }
        public decimal NPS { get; set; }
        public decimal VPF { get; set; }
        public decimal RFB { get; set; } // Reimbursable Flexible Benefits
        public decimal SpecialAllowance { get; set; }

        //public decimal TotalFixedComponent => Basic + HRA + StatutoryBonus + NPS + VPF + RFB + SpecialAllowance;
        public decimal TotalFixedComponent { get; set; }

        public decimal TotalFixedMonthlyComponent => TotalFixedComponent / 12;
        public decimal VariablePay { get; set; } = 0;
        public decimal PFEmployee { get; set; }
        public decimal ESICEmployee { get; set; }
        public decimal ProfessionalTax { get; set; } = 2500;
        public decimal ProfessionalTaxMonthly { get; set; } = 200;

        //public decimal TotalDeductions => PFEmployee + ESICEmployee + ProfessionalTax;
        public decimal TotalDeductions { get; set; }

        //public decimal NetSalary => TotalFixedComponent + VariablePay - TotalDeductions;
        public decimal NetSalary {  get; set; } 

        public decimal PFEmployer { get; set; }
        public decimal ESICEmployer { get; set; }
        public decimal Gratuity { get; set; }
        public decimal InsuranceCoverage { get; set; }

        public decimal TotalAnnualBenefits => PFEmployer + ESICEmployer + Gratuity + InsuranceCoverage;
        public decimal TotalAnnualCTC => NetSalary + TotalAnnualBenefits;

        public DateTime CurrentDate { get; set; }= DateTime.Today;

    }

}
