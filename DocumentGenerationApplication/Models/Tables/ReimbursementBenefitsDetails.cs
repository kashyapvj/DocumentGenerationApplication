namespace DocumentGenerationApplication.Models.Tables
{
    public class ReimbursementBenefitsDetails
    {

        public int Id { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;


        // Individual Reimbursable Benefits (Annual Amounts)
        public int ChildEducationAllowance { get; set; } = 0;

        public int ChildHostelAllowance { get; set; } = 0;

        public int LeaveTravelAllowance { get; set; } = 0;

        public int BooksPeriodicalsSelfCertification { get; set; } = 0;

        public int SodexoMealCoupon { get; set; } = 0;

        public int FuelCarReimbursement { get; set; } = 0;

        public int DriverReimbursement { get; set; } = 0;

        public int MobileReimbursement { get; set; } = 0;


        // Total Amount After Selection
        public int Total { get; set; } = 0;


        // Metadata
        public DateTime? CreatedOn { get; set; }// hidden at UI

        public string CreatedBy { get; set; } = string.Empty;// hidden at UI

        public DateTime? UpdatedOn { get; set; }// hidden at UI

        public string UpdatedBy { get; set; } = string.Empty;// hidden at UI
    }
}
