using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Models.Tables;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DocumentGenerationApplication.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetBandsAsync()
        {
            return await _context.Bands
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BandName })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetGradesByBandIdAsync(int bandId)
        {
            return await _context.Grades
                .Where(g => g.BandId == bandId)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.GradeName })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName.ToLower())
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.DepartmentName })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDesignationsAsync(int bandId, int gradeId, int departmentId)
        {
            return await _context.Designations
                .OrderBy(d => d.DesignationName)
                .Where(d => d.BandId == bandId && d.GradeId == gradeId && d.DepartmentId == departmentId)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.DesignationName })
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeDetails>> GetAllEmployeesAsync()
        {

            try
            {
                return await _context.EmployeeDetails
                 .Include(e => e.Band)         // Navigation property for Band
                 .Include(e => e.Grade)        // Navigation property for Grade
                 .Include(e => e.Department)   // Navigation property for Department
                 .Include(e => e.Designation)  // Navigation property for Designation
                 .ToListAsync();

                //return await _context.EmployeeDetails.ToListAsync();


            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetAllEmployeesAsync: " + ex.Message, ex);
            }

        }

        public async Task<EmployeeDetails?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                return await _context.EmployeeDetails
                    .Include(e => e.Band)
                    .Include(e => e.Grade)
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetEmployeeByIdAsync: " + ex.Message, ex);
            }
        }

        public async Task<bool> IsMobileNumberExistsAsync(string mobileNumber, int excludeEmployeeId)
        {
            return await _context.EmployeeDetails
                .AnyAsync(e => e.MobileNumber == mobileNumber && e.Id != excludeEmployeeId);
        }



        //public async Task UpdateEmployeeAsync(EmployeeDetails employee)
        //{
        //    try
        //    {
        //        _context.EmployeeDetails.Update(employee);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in UpdateEmployeeAsync: " + ex.Message, ex);
        //    }
        //}


        //public async Task UpdateEmployeeAsync(EmployeeDetails employee)
        //{
        //    try
        //    {
        //        //check mobile number duplicacy before saving


        //        // Update Employee table
        //        _context.EmployeeDetails.Update(employee);

        //        // Fetch related SalaryBreakdown(s) for this employee
        //        var salaryBreakdowns = await _context.SalaryBreakdowns
        //            .Where(s => s.Email == employee.Email)
        //            .ToListAsync();

        //        if (salaryBreakdowns != null && salaryBreakdowns.Any())
        //        {
        //            foreach (var salary in salaryBreakdowns)
        //            {
        //                salary.EmployeeName = employee.EmployeeName;
        //                salary.MobileNumber = employee.MobileNumber;

        //                _context.SalaryBreakdowns.Update(salary);
        //            }
        //        }

        //        // Save changes in one transaction
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in UpdateEmployeeAsync: " + ex.Message, ex);
        //    }
        //}


        public async Task UpdateEmployeeAsync(EmployeeDetails employee)
        {
            try
            {
                // Fetch the existing entity from DB
                var existingEmployee = await _context.EmployeeDetails
                    .FirstOrDefaultAsync(e => e.Id == employee.Id);

                if (existingEmployee == null)
                    throw new ApplicationException("Employee not found");

                // Check mobile number duplicacy if it's updated
                if (existingEmployee.MobileNumber != employee.MobileNumber)
                {
                    bool mobileExists = await _context.EmployeeDetails
                        .AnyAsync(e => e.MobileNumber == employee.MobileNumber && e.Id != employee.Id);

                    if (mobileExists)
                        throw new ApplicationException("Mobile number already exists for another employee");
                }

                // Update properties
                existingEmployee.EmployeeName = employee.EmployeeName;
                existingEmployee.MobileNumber = employee.MobileNumber;
                existingEmployee.BandId = employee.BandId;
                existingEmployee.GradeId = employee.GradeId;
                existingEmployee.DepartmentId = employee.DepartmentId;
                existingEmployee.DesignationId = employee.DesignationId;
                existingEmployee.JoiningDate = employee.JoiningDate;
                existingEmployee.ProbationDate = employee.ProbationDate;
                existingEmployee.PermanentDate = employee.PermanentDate;
                existingEmployee.OfferValidTill = employee.OfferValidTill;
                existingEmployee.Address_Line1 = employee.Address_Line1;
                existingEmployee.Address_Line2 = employee.Address_Line2;
                existingEmployee.Address_Line3 = employee.Address_Line3;
                existingEmployee.JobLocation = employee.JobLocation;
                existingEmployee.Status = employee.Status;
                
                // Add other fields you need

                // Update related SalaryBreakdowns
                var salaryBreakdowns = await _context.SalaryBreakdowns
                    .Where(s => s.Email == existingEmployee.Email)
                    .ToListAsync();

                foreach (var salary in salaryBreakdowns)
                {
                    salary.EmployeeName = existingEmployee.EmployeeName;
                    salary.MobileNumber = existingEmployee.MobileNumber;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in UpdateEmployeeAsync: " + ex.Message, ex);
            }
        }



        public async Task<List<SalaryBreakdown>> GetSalaryBreakdownsByEmailAndMobileAsync(string email, string mobileNumber) 
        {
            return await _context.SalaryBreakdowns
                .Where(s => s.Email == email && s.MobileNumber == mobileNumber)
                .ToListAsync();
        }


    }
}
