using DocumentGenerationApplication.Models.Tables;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocumentGenerationApplication.Repository
{
    public interface IEmployeeRepository
    {
        Task<List<SelectListItem>> GetBandsAsync();
        Task<List<SelectListItem>> GetDepartmentsAsync();
        Task<List<SelectListItem>> GetDesignationsAsync(int bandId, int gradeId, int departmentId);
        Task<List<SelectListItem>> GetGradesByBandIdAsync(int bandId);

        Task<IEnumerable<EmployeeDetails>> GetAllEmployeesAsync();
        Task<EmployeeDetails?> GetEmployeeByIdAsync(int id);

        Task UpdateEmployeeAsync(EmployeeDetails employee);

        Task<List<SalaryBreakdown>> GetSalaryBreakdownsByEmailAndMobileAsync(string email, string mobileNumber);

        Task<bool> IsMobileNumberExistsAsync(string mobileNumber, int excludeEmployeeId);




    }
}