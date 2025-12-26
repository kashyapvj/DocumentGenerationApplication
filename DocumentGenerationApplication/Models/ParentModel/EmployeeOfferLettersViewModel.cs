using DocumentGenerationApplication.Models.Tables;

namespace DocumentGenerationApplication.Models.ParentModel
{
    public class EmployeeOfferLettersViewModel
    {
        public EmployeeDetails Employee { get; set; } = new();
        public SalaryBreakdown Salary { get; set; } = new();
        public ReimbursementBenefitsDetails? Rfb { get; set; } = new();
    }
}
