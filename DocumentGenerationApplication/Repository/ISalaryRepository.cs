using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.ResponseDto;
using DocumentGenerationApplication.Models.Tables;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocumentGenerationApplication.Repository
{
    public interface ISalaryRepository
    {
        Task<EmailCheckResultDto> CheckEmailAsync(string email, int docType);
        Task<IEnumerable<EmployeeDetails>> GetAllEmployeesAsync();
        // List<SelectListItem> GetAllBands();
        List<SelectListItem> GetAllBands(int? documentType);

        List<SelectListItem> GetGradesByBandName(string bandName);
        List<SelectListItem> GetAllDepartments();
        List<SelectListItem> GetDesignationsByNames(string bandName, string gradeName, string departmentName);


        Task SaveEmployeeDetailsAsync(FillPdfTemplateInput model);
        //Task<bool> SaveEmployeeDetailsAsync(FillPdfTemplateInput model);

    }
}