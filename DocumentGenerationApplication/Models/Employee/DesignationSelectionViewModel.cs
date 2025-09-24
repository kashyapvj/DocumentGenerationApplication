using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocumentGenerationApplication.Models.Employee
{
    public class DesignationSelectionViewModel
    {
        public int BandId { get; set; }
        public int GradeId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }

        public List<SelectListItem> Bands { get; set; } = new();
        public List<SelectListItem> Grades { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> Designations { get; set; } = new();
    }
}
