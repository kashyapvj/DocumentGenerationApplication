using DocumentGenerationApplication.Models.Tables;
using DocumentGenerationApplication.Models.UI_Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace DocumentGenerationApplication.Models.ParentModel
{
    public class FillPdfTemplateInput
    {
        
        public SalaryBreakdown salaryValues { get; set; } = new SalaryBreakdown();
        public SalaryBreakdownInput employeeDetails { get; set; } = new SalaryBreakdownInput();
    }
}
