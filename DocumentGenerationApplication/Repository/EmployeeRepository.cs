using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.Tables;
using DocumentGenerationApplication.Models.UI_Model;
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

      


        public async Task<SalaryBreakdownInput?> GetEmployeeCompleteDataByIdAsync(int id)
        {
            try
            {
                var employeeDetails = await _context.EmployeeDetails
                    .Include(e => e.Band)
                    .Include(e => e.Grade)
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employeeDetails == null)
                    return null;

                // Define list of metro cities
                var metroCities = new List<string>
                    {
                        "Navi Mumbai",
                        "Delhi",
                        "Chennai",
                        "Kolkata",
                        "Bengaluru",
                        "Hyderabad",
                        "Ahmedabad",
                        "Nagpur"
                    };

                // Check if employee's job location is a metro city
                bool isMetroCity = metroCities.Any(city =>
                    string.Equals(city, employeeDetails.JobLocation, StringComparison.OrdinalIgnoreCase));

                var salaryValues = await _context.SalaryBreakdowns
                    .FirstOrDefaultAsync(e => e.Email == employeeDetails.Email && e.MobileNumber==employeeDetails.MobileNumber);

                var completeData = new SalaryBreakdownInput
                {
                    EmployeeName = employeeDetails.EmployeeName,

                    BandId = employeeDetails.BandId,
                    GradeId = employeeDetails.GradeId,
                    DepartmentId = employeeDetails.DepartmentId,
                    DesignationId = employeeDetails.DesignationId,

                    Band = employeeDetails.Band?.BandName ?? string.Empty,
                    Grade = employeeDetails.Grade?.GradeName ?? string.Empty,
                    Department = employeeDetails.Department?.DepartmentName ?? string.Empty,
                    Designation = employeeDetails.Designation?.DesignationName ?? string.Empty,

                    TotalCTC = salaryValues?.TotalCompensation ?? 0,
                    BonusAmount = employeeDetails.BonusAmount ?? "Not Applicable",
                    JobLocation = employeeDetails.JobLocation ?? string.Empty,
                    PFApplicability = employeeDetails.PFApplicability != "Full" ? "Fixed" : "Full",
                    Address_Line1 = employeeDetails.Address_Line1 ?? string.Empty,
                    Address_Line2 = employeeDetails.Address_Line2 ?? string.Empty,
                    Address_Line3 = employeeDetails.Address_Line3 ?? string.Empty,
                    Email = employeeDetails.Email ?? string.Empty,
                    MobileNumber = employeeDetails.MobileNumber ?? string.Empty,
                    JoiningDate = employeeDetails.JoiningDate,
                    OfferValidTill = employeeDetails.OfferValidTill,
                    OptedNPS = salaryValues?.NPS != null && salaryValues?.NPS != 0,
                    OptedVPF = salaryValues?.VPF != null && salaryValues?.VPF != 0,
                    Status = employeeDetails.Status,
                    IsMetro = isMetroCity,
                    DocumentType = employeeDetails.DocumentType,
                    //JoiningDatePlus3Months = employeeDetails.ProbationDate == DateTime.MinValue ? "--/--/----" : DateOnly.FromDateTime(employeeDetails.ProbationDate),
                    //JoiningDatePlus6Months = employeeDetails.PermanentDate == DateTime.MinValue ? "--/--/----" : DateOnly.FromDateTime(employeeDetails.PermanentDate)

                    JoiningDatePlus3Months = DateOnly.FromDateTime(employeeDetails.ProbationDate),
                    JoiningDatePlus6Months = DateOnly.FromDateTime(employeeDetails.PermanentDate)


                };


                return completeData;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetEmployeeCompleteDataByIdAsync: " + ex.Message, ex);
            }
        }

        public async Task SaveEmployeeCompleteData(SalaryBreakdownInput employeeDetails, SalaryBreakdown salaryValues)
        {
            try
            {
                // 🧱 Fetch the existing employee record
                var employee = await _context.EmployeeDetails
                    .FirstOrDefaultAsync(e => e.Id == employeeDetails.Id);

                if (employee == null)
                    throw new ApplicationException("Employee not found.");

                // 🧩 Check for duplicate mobile number (if it's changed)
                if (!string.Equals(employee.MobileNumber, employeeDetails.MobileNumber, StringComparison.OrdinalIgnoreCase))
                {
                    bool mobileExists = await _context.EmployeeDetails
                        .AnyAsync(e => e.MobileNumber == employeeDetails.MobileNumber && e.EmployeeId != employeeDetails.EmployeeId);

                    if (mobileExists)
                        throw new ApplicationException("Mobile number already exists for another employee.");
                }

                // 📧 Check for duplicate email (if it's changed)
                if (!string.Equals(employee.Email, employeeDetails.Email, StringComparison.OrdinalIgnoreCase))
                {
                    bool emailExists = await _context.EmployeeDetails
                        .AnyAsync(e => e.Email == employeeDetails.Email && e.Id != employee.Id);

                    if (emailExists)
                        throw new ApplicationException("Email address already exists for another employee.");
                }

                // 🧾 Update EmployeeDetails fields

                employee.EmployeeName = employeeDetails.EmployeeName;

                // 🧱 Convert Band (string → int)
                if (int.TryParse(employeeDetails.Band, out int bandId))
                {
                    employee.BandId = bandId;
                }
                else
                {
                    throw new ApplicationException("Invalid Band value. Expected numeric ID.");
                }

                // 🧱 Convert Grade (string → int)
                if (int.TryParse(employeeDetails.Grade, out int gradeId))
                {
                    employee.GradeId = gradeId;
                }
                else
                {
                    throw new ApplicationException("Invalid Grade value. Expected numeric ID.");
                }

                // 🧱 Convert Department (string → int)
                if (int.TryParse(employeeDetails.Department, out int departmentId))
                {
                    employee.DepartmentId = departmentId;
                }
                else
                {
                    throw new ApplicationException("Invalid Department value. Expected numeric ID.");
                }

                // 🧱 Convert Designation (string → int)
                if (int.TryParse(employeeDetails.Designation, out int designationId))
                {
                    employee.DesignationId = designationId;
                }
                else
                {
                    throw new ApplicationException("Invalid Designation value. Expected numeric ID.");
                }
                employee.JoiningDate = employeeDetails.JoiningDate;
                employee.Email = employeeDetails.Email;
                employee.Address_Line1 = employeeDetails.Address_Line1;
                employee.Address_Line2 = employeeDetails.Address_Line2;
                employee.Address_Line3 = employeeDetails.Address_Line3;
                employee.JobLocation = employeeDetails.JobLocation;
                employee.MobileNumber = employeeDetails.MobileNumber;
                employee.Status = employeeDetails.Status;
                employee.OfferValidTill = employeeDetails.OfferValidTill;
                employee.PFApplicability = employeeDetails.PFApplicability;
                employee.PermanentDate = employeeDetails.JoiningDatePlus6Months.ToDateTime(TimeOnly.MinValue);
                employee.ProbationDate = employeeDetails.JoiningDatePlus3Months.ToDateTime(TimeOnly.MinValue);
                employee.BonusAmount = employeeDetails.BonusAmount;
                employee.OfferValidTill = employeeDetails.OfferValidTill;
                employee.TotalCompensation = salaryValues.TotalCompensation;
                employee.RefNo = GuidBase64.NewId();



                // 🧮 Update or Insert SalaryBreakdown
                var salary = await _context.SalaryBreakdowns
                    .FirstOrDefaultAsync(s => s.Email == employeeDetails.Email && s.MobileNumber==employeeDetails.MobileNumber);

                if (salary != null)
                {
                    // Update existing record
                    salary.TotalCompensation = salaryValues.TotalCompensation;
                    salary.Basic = salaryValues.Basic;
                    salary.HRA = salaryValues.HRA;
                    salary.StatutoryBonus = salaryValues.StatutoryBonus;
                    salary.NPS = salaryValues.NPS;
                    salary.VPF = salaryValues.VPF;
                    salary.RFB = salaryValues.RFB;
                    salary.PFEmployer = salaryValues.PFEmployer;
                    salary.PFEmployee = salaryValues.PFEmployee;
                    salary.Gratuity = salaryValues.Gratuity;
                    salary.ESICEmployer = salaryValues.ESICEmployer;
                    salary.ESICEmployee = salaryValues.ESICEmployee;
                    salary.SpecialAllowance = salaryValues.SpecialAllowance;
                    salary.TotalFixedComponent = salaryValues.TotalFixedComponent;
                    salary.VariablePay = salaryValues.VariablePay;
                    salary.NetSalary = salaryValues.NetSalary;
                    salary.TotalDeductions = salaryValues.TotalDeductions;
                    salary.IsMetro = salaryValues.IsMetro;
                    salary.MobileNumber= employeeDetails.MobileNumber;
                    salary.Email = employeeDetails.Email;
                    salary.EmployeeName = employeeDetails.EmployeeName;
                    salary.EmployeeId= employeeDetails.EmployeeId;
   
                }
                else
                {
                    throw new ApplicationException("Salary record not found for the specified employee.");
                }

                // 💾 Commit all changes
                await _context.SaveChangesAsync();
            }
            catch (ApplicationException)
            {
                throw; // rethrow known validation errors as-is
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in SaveEmployeeCompleteData: " + ex.Message, ex);
            }
        }

        public static class GuidBase64
        {
            // returns 22-char base64url string (no padding)
            public static string NewId()
            {
                var guid = Guid.NewGuid();
                // 16 bytes
                string b64 = Convert.ToBase64String(guid.ToByteArray()); // 24 chars with '==' padding
                                                                         // convert to base64url and remove padding
                b64 = b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
                return b64; // 22 characters typically
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

        public async Task<IEnumerable<EmployeeDetails>> GetAllEmployeesAsync()
        {

            try
            {
                return await _context.EmployeeDetails
                 .Include(e => e.Band)         // Navigation property for Band
                 .Include(e => e.Grade)        // Navigation property for Grade
                 .Include(e => e.Department)   // Navigation property for Department
                 .Include(e => e.Designation)  // Navigation property for Designation
                 .OrderBy(e => e.Id)
                 .ToListAsync();

                //return await _context.EmployeeDetails.ToListAsync();


            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetAllEmployeesAsync: " + ex.Message, ex);
            }

        }





        public async Task<List<EmployeeOfferLettersViewModel>> GetEmployeesAsync()
        {

           try
           {
                    var result = await (
                    from e in _context.EmployeeDetails.AsNoTracking()
                   .Include(e => e.Band)
                   .Include(e => e.Grade)
                   .Include(e => e.Department)
                   .Include(e => e.Designation)

                   join s in _context.SalaryBreakdowns.AsNoTracking()
                   on e.Email equals s.Email into salaryGroup
                   from s in salaryGroup.DefaultIfEmpty()

                  //join r in _context.ReimbursementBenefitsDetails.AsNoTracking()
                  //on e.Email equals r.Email into rfbGroup
                  //from r in rfbGroup.DefaultIfEmpty()

                  orderby e.Id descending
                  select new EmployeeOfferLettersViewModel
                  {
                      Employee = e,
                      Salary = s,
                     // Rfb = r
                  }
                  ).ToListAsync();

                 return result;


           }
           catch (Exception ex)
           {
               throw new ApplicationException("Error in GetEmployeesAsync: " + ex.Message, ex);
           }

        }

        //public async Task<List<EmployeeOfferLetterVM>> GetEmployeeOfferLettersAsync()
        //{
        //    var result = await _context.EmployeeDetails
        //        .AsNoTracking()
        //        .Include(e => e.Band)
        //        .Include(e => e.Grade)
        //        .Include(e => e.Department)
        //        .Include(e => e.Designation)
        //        .GroupJoin(
        //            _context.SalaryBreakdowns.AsNoTracking(),
        //            e => e.EmployeeId,
        //            s => s.EmployeeId,
        //            (e, s) => new EmployeeOfferLetterVM
        //            {
        //                Employee = e,
        //                Salary = s.FirstOrDefault()
        //            })
        //        .OrderByDescending(x => x.Employee.JoiningDate)
        //        .ToListAsync();

        //    return result;
        //}


    }
}
