using DocumentGenerationApplication.Models.Employee;
using DocumentGenerationApplication.Models.UI_Model;

namespace DocumentGenerationApplication.Models.ParentModel
{
    public class SalaryDesignationViewModel
    {
        public SalaryBreakdownInput SalaryBreakdown { get; set; } = new();
        public DesignationSelectionViewModel DesignationSelection { get; set; } = new();
    }

}
