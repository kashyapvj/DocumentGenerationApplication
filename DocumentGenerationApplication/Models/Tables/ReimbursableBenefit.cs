using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.Tables
{
    public class ReimbursableBenefit
    {
        [Key]
        public int Id { get; set; }
        public string BenefitName { get; set; } = string.Empty;
        public decimal AnnualAmount { get; set; }
        public decimal? MonthlyAmount { get; set; } // Nullable in case there's no monthly breakdown
        public string Remark { get; set; } = string.Empty;
        public string DeclarationOrProofRequirement { get; set; } = string.Empty;
    }
}
