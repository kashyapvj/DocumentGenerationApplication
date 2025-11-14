using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.ResponseDto;
using DocumentGenerationApplication.Models.Tables;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DocumentGenerationApplication.Repository
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly AppDbContext _context;

        public SalaryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDetails>> GetAllEmployeesAsync()
        {

            try
            {
                return await _context.EmployeeDetails
                             .Include(e => e.Band)
                             .Include(e => e.Grade)
                             .Include(e => e.Department)
                             .Include(e => e.Designation)
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetAllEmployeesAsync: " + ex.Message, ex);
            }

        }

        public async Task SaveEmployeeDetailsAsync(FillPdfTemplateInput model)
        {
            try
            {
                if (model == null || model.salaryValues == null || model.employeeDetails == null)
                    throw new ArgumentNullException(nameof(model), "Model or its properties cannot be null.");

                if (model.employeeDetails.DocumentType == 1 || model.employeeDetails.DocumentType == 2)
                {
                    // Check for duplicate Email or MobileNumber
                    bool emailExists = await _context.EmployeeDetails.AnyAsync(e => e.Email == model.employeeDetails.Email);
                    bool mobileExists = await _context.EmployeeDetails.AnyAsync(e => e.MobileNumber == model.employeeDetails.MobileNumber);



                    if (emailExists || mobileExists)
                    {
                        string errorMsg = "Duplicate entry found for: ";
                        if (emailExists) errorMsg += "Email ";
                        if (mobileExists) errorMsg += "Mobile Number ";


                        throw new InvalidOperationException(errorMsg.Trim());
                    }
                }
                //Duplicacy check for EmployeeId, RefNo
                if (model.employeeDetails.DocumentType == 3 || model.employeeDetails.DocumentType == 4)
                {
                    bool employeeIdExistsInSalaryBreakdown = await _context.SalaryBreakdowns.AnyAsync(e => e.EmployeeId == model.employeeDetails.EmployeeId);
                    bool employeeIdExistsInEmployeedetails = await _context.EmployeeDetails.AnyAsync(e => e.EmployeeId == model.employeeDetails.EmployeeId);
                    bool refNoExistsInEmployeedetails = await _context.EmployeeDetails.AnyAsync(e => e.RefNo == model.employeeDetails.RefNo);

                    if (employeeIdExistsInSalaryBreakdown || employeeIdExistsInEmployeedetails || refNoExistsInEmployeedetails)
                    {
                        string errorMsg = "Duplicate entry found for: ";
                        if (employeeIdExistsInSalaryBreakdown) errorMsg += "EmployeeId_ExistsInSalaryBreakdownTable ";
                        if (employeeIdExistsInEmployeedetails) errorMsg += "EmployeeId_ExistsInEmployeedetailsTable ";
                        if (refNoExistsInEmployeedetails) errorMsg += "RefNo_ExistsInEmployeedetailsTable";
                        throw new InvalidOperationException(errorMsg.Trim());
                    }
                }

                var band = await _context.Bands.FirstOrDefaultAsync(b => b.BandName == model.employeeDetails.Band);
                var grade = await _context.Grades.FirstOrDefaultAsync(g => g.GradeName == model.employeeDetails.Grade);
                var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == model.employeeDetails.Department);
                //var designation = await _context.Designations.FirstOrDefaultAsync(ds => ds.DesignationName == model.employeeDetails.Designation);

                if (band == null || grade == null || department == null)
                    throw new Exception("One or more referenced entities were not found in the database.");
                var designation = await _context.Designations.FirstOrDefaultAsync(ds => ds.DesignationName == model.employeeDetails.Designation &&
                                                                                  ds.BandId == band.Id &&
                                                                                  ds.GradeId == grade.Id &&
                                                                                  ds.DepartmentId == department.Id);
                if (band == null || grade == null || department == null || designation == null)
                    throw new Exception("One or more referenced entities were not found in the database.");


                //here employee (Offer Letter experienced) data will be inserted
                if (model.employeeDetails.DocumentType == 1)
                {
                    //var band = await _context.Bands.FirstOrDefaultAsync(b => b.BandName == model.employeeDetails.Band);
                    //var grade = await _context.Grades.FirstOrDefaultAsync(g => g.GradeName == model.employeeDetails.Grade);
                    //var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == model.employeeDetails.Department);
                    ////var designation = await _context.Designations.FirstOrDefaultAsync(ds => ds.DesignationName == model.employeeDetails.Designation);

                    //if (band == null || grade == null || department == null)
                    //    throw new Exception("One or more referenced entities were not found in the database.");
                    //var designation = await _context.Designations.FirstOrDefaultAsync(ds => ds.DesignationName == model.employeeDetails.Designation &&
                    //                                                                  ds.BandId == band.Id &&
                    //                                                                  ds.GradeId == grade.Id &&
                    //                                                                  ds.DepartmentId == department.Id);
                    //if (band == null || grade == null || department == null || designation == null)
                    //    throw new Exception("One or more referenced entities were not found in the database.");

                    var _employeeDetails = new EmployeeDetails
                    {
                        EmployeeId = null,
                        RefNo = Guid.NewGuid().ToString("N"),
                        EmployeeName = model.employeeDetails.EmployeeName,
                        TotalCompensation = model.salaryValues.TotalCompensation,
                        BandId = band.Id,
                        GradeId = grade.Id,
                        DepartmentId = department.Id,
                        DesignationId = designation.Id,
                        JoiningDate = model.employeeDetails.JoiningDate,
                        Email = model.employeeDetails.Email,
                        Address_Line1 = model.employeeDetails.Address_Line1,
                        Address_Line2 = model.employeeDetails.Address_Line2,
                        Address_Line3 = model.employeeDetails.Address_Line3,
                        JobLocation = model.employeeDetails.JobLocation,
                        MobileNumber = model.employeeDetails.MobileNumber,
                        Status = model.employeeDetails.Status,
                        OfferValidTill = DateTime.Now.AddDays(2),
                        DocumentType = model.employeeDetails.DocumentType,
                        PFApplicability = model.employeeDetails.PFApplicability,
                        PermanentDate = model.employeeDetails.JoiningDatePlus6Months.ToDateTime(TimeOnly.MinValue),
                        ProbationDate = model.employeeDetails.JoiningDatePlus3Months.ToDateTime(TimeOnly.MinValue),
                        BonusAmount = model.employeeDetails.IsBonusApplicable ? model.employeeDetails.BonusAmount : "Not Applicable"

                    };

                    await _context.EmployeeDetails.AddAsync(_employeeDetails);
                    await _context.SaveChangesAsync();

                    var _salaryBreakdown = new SalaryBreakdown
                    {
                        EmployeeId = null,
                        EmployeeName = model.employeeDetails.EmployeeName,
                        IsMetro = model.salaryValues.IsMetro,
                        TotalCompensation = model.salaryValues.TotalCompensation,
                        Basic = model.salaryValues.Basic,
                        HRA = model.salaryValues.HRA,
                        StatutoryBonus = model.salaryValues.StatutoryBonus,
                        NPS = model.salaryValues.NPS,
                        VPF = model.salaryValues.VPF,
                        RFB = model.salaryValues.RFB,
                        SpecialAllowance = model.salaryValues.SpecialAllowance,
                        TotalFixedComponent = model.salaryValues.TotalFixedComponent,
                        VariablePay = model.salaryValues.VariablePay,
                        PFEmployee = model.salaryValues.PFEmployee,
                        ESICEmployee = model.salaryValues.ESICEmployee,
                        ProfessionalTax = model.salaryValues.ProfessionalTax,
                        ProfessionalTaxMonthly = model.salaryValues.ProfessionalTaxMonthly,
                        TotalDeductions = model.salaryValues.TotalDeductions,
                        NetSalary = model.salaryValues.NetSalary,
                        PFEmployer = model.salaryValues.PFEmployer,
                        ESICEmployer = model.salaryValues.ESICEmployer,
                        Gratuity = model.salaryValues.Gratuity,
                        InsuranceCoverage = model.salaryValues.InsuranceCoverage,
                        CurrentDate = model.salaryValues.CurrentDate,
                        Email = model.employeeDetails.Email,
                        MobileNumber = model.employeeDetails.MobileNumber
                    };

                    await _context.SalaryBreakdowns.AddAsync(_salaryBreakdown);
                    await _context.SaveChangesAsync();
                }


                //here employee (Offer Letter fresher) data will be inserted 

                if (model.employeeDetails.DocumentType == 2)
                {
                    var employeeDetails = new EmployeeDetails
                    {
                        EmployeeId = null,
                        RefNo = Guid.NewGuid().ToString("N"),
                        EmployeeName = model.employeeDetails.EmployeeName,
                        TotalCompensation = model.salaryValues.TotalCompensation,
                        //BandId = 1,
                        //GradeId = 2,
                        //DepartmentId = 1,
                        //DesignationId = 2,
                        BandId = band.Id,
                        GradeId = grade.Id,
                        DepartmentId = department.Id,
                        DesignationId = designation.Id,
                        JoiningDate = model.employeeDetails.JoiningDate,
                        Email = model.employeeDetails.Email,
                        Address_Line1 = model.employeeDetails.Address_Line1,
                        Address_Line2 = model.employeeDetails.Address_Line2,
                        Address_Line3 = model.employeeDetails.Address_Line3,
                        JobLocation = model.employeeDetails.JobLocation,
                        MobileNumber = model.employeeDetails.MobileNumber,
                        Status = model.employeeDetails.Status,
                        //OfferValidTill = model.employeeDetails.JoiningDate.AddDays(model.employeeDetails.OfferValidTill1)
                       // OfferValidTill = model.employeeDetails.OfferValidTill,
                        OfferValidTill = DateTime.Now.AddDays(2),
                        DocumentType = model.employeeDetails.DocumentType,
                        PFApplicability = model.employeeDetails.PFApplicability,
                        PermanentDate = model.employeeDetails.JoiningDatePlus6Months.ToDateTime(TimeOnly.MinValue),
                        ProbationDate = model.employeeDetails.JoiningDatePlus3Months.ToDateTime(TimeOnly.MinValue),
                        BonusAmount = model.employeeDetails.IsBonusApplicable ? model.employeeDetails.BonusAmount : "Not Applicable"



                    };

                    await _context.EmployeeDetails.AddAsync(employeeDetails);
                    await _context.SaveChangesAsync();

                    var salaryBreakdown = new SalaryBreakdown
                    {
                        EmployeeId = null,
                        EmployeeName = model.employeeDetails.EmployeeName,
                        IsMetro = model.salaryValues.IsMetro,
                        TotalCompensation = model.salaryValues.TotalCompensation,
                        Basic = model.salaryValues.Basic,
                        HRA = model.salaryValues.HRA,
                        StatutoryBonus = model.salaryValues.StatutoryBonus,
                        NPS = model.salaryValues.NPS,
                        VPF = model.salaryValues.VPF,
                        RFB = model.salaryValues.RFB,
                        SpecialAllowance = model.salaryValues.SpecialAllowance,
                        TotalFixedComponent = model.salaryValues.TotalFixedComponent,
                        VariablePay = model.salaryValues.VariablePay,
                        PFEmployee = model.salaryValues.PFEmployee,
                        ESICEmployee = model.salaryValues.ESICEmployee,
                        ProfessionalTax = model.salaryValues.ProfessionalTax,
                        ProfessionalTaxMonthly = model.salaryValues.ProfessionalTaxMonthly,
                        TotalDeductions = model.salaryValues.TotalDeductions,
                        NetSalary = model.salaryValues.NetSalary,
                        PFEmployer = model.salaryValues.PFEmployer,
                        ESICEmployer = model.salaryValues.ESICEmployer,
                        Gratuity = model.salaryValues.Gratuity,
                        InsuranceCoverage = model.salaryValues.InsuranceCoverage,
                        CurrentDate = model.salaryValues.CurrentDate,
                        Email = model.employeeDetails.Email,
                        MobileNumber = model.employeeDetails.MobileNumber
                    };

                    await _context.SalaryBreakdowns.AddAsync(salaryBreakdown);
                    await _context.SaveChangesAsync();
                }

                if (model.employeeDetails.DocumentType == 3)
                {
                    // 🔹 Update EmployeeDetails
                    var existingEmployee = await _context.EmployeeDetails
                        .FirstOrDefaultAsync(e => e.Email == model.employeeDetails.Email);

                    if (existingEmployee != null)
                    {
                        // update only what you need
                        existingEmployee.RefNo = model.employeeDetails.RefNo;
                        existingEmployee.EmployeeId = model.employeeDetails.EmployeeId;
                        existingEmployee.Status = 0;
                        existingEmployee.WorkingDays=model.employeeDetails.WorkingDays;
                        existingEmployee.Probation = model.employeeDetails.IsProbationApplicable ? "Yes" : "No";

                        _context.EmployeeDetails.Update(existingEmployee);
                        await _context.SaveChangesAsync();
                    }

                    // 🔹 Update SalaryBreakdown
                    var existingSalary = await _context.SalaryBreakdowns
                        .FirstOrDefaultAsync(s => s.Email == model.employeeDetails.Email);

                    if (existingSalary != null)
                    {
                        // update only what you need
                        existingSalary.EmployeeId = model.employeeDetails.EmployeeId;

                        _context.SalaryBreakdowns.Update(existingSalary);
                        await _context.SaveChangesAsync();
                    }
                }

                if (model.employeeDetails.DocumentType == 4)
                {
                    // 🔹 Update EmployeeDetails
                    var existingEmployee = await _context.EmployeeDetails
                         .FirstOrDefaultAsync(e => e.Email == model.employeeDetails.Email);

                    if (existingEmployee != null)
                    {
                        // update only the fields you need
                        existingEmployee.RefNo = model.employeeDetails.RefNo;
                        existingEmployee.EmployeeId = model.employeeDetails.EmployeeId;
                        existingEmployee.Status = 0;
                        existingEmployee.WorkingDays = model.employeeDetails.WorkingDays;
                        existingEmployee.Probation = model.employeeDetails.IsProbationApplicable ? "Yes" : "No";

                        _context.EmployeeDetails.Update(existingEmployee);
                        await _context.SaveChangesAsync();
                    }

                    // 🔹 Update SalaryBreakdown
                    var existingSalary = await _context.SalaryBreakdowns
                        .FirstOrDefaultAsync(s => s.EmployeeId == model.employeeDetails.EmployeeId);

                    if (existingSalary != null)
                    {
                        // update only the fields you need
                        existingSalary.EmployeeId = model.employeeDetails.EmployeeId;

                        _context.SalaryBreakdowns.Update(existingSalary);
                        await _context.SaveChangesAsync();
                    }
                }


            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in SaveEmployeeDetailsAsync: " + ex.Message, ex);
            }


        }






        public async Task<EmailCheckResultDto> CheckEmailAsync(string email, int docType)
        {

            try
            {
                if (docType == 1 || docType == 2)
                {
                    var employee = await _context.EmployeeDetails
                        .Where(e => e.Email == email)
                        .FirstOrDefaultAsync();

                    if (employee != null)
                    {
                        return new EmailCheckResultDto
                        {
                            Exists = true,
                            Message = $"Email already exists with {employee.EmployeeName}.",
                            Email = employee.Email,
                            EmployeeName = employee.EmployeeName,
                            EmployeeId = employee.EmployeeId
                        };
                    }

                    return new EmailCheckResultDto
                    {
                        Exists = false
                    };
                }
                else if (docType == 3 || docType == 4)
                {

                    var employee = await _context.EmployeeDetails
                       .Where(e => e.Email == email)
                       .FirstOrDefaultAsync();

                    if (employee!=null && employee.EmployeeId != null && !string.IsNullOrEmpty(employee.RefNo))
                    {
                        return new EmailCheckResultDto
                        {
                            Exists = true,
                            Message = $"Email already exists with {employee.EmployeeName} in Appointment letters",
                            Email = employee.Email,
                            EmployeeName = employee.EmployeeName,
                            EmployeeId = employee.EmployeeId
                        };
                    }


                    var EmployeeDetailsTable = await _context.EmployeeDetails
                        .Where(o => o.Email == email)
                        .FirstOrDefaultAsync();

                    var SalaryBreakdownsTable = await _context.SalaryBreakdowns
                       .Where(o => o.Email == email)
                       .FirstOrDefaultAsync();


                    if (EmployeeDetailsTable != null || SalaryBreakdownsTable != null)
                    {
                        // EmployeeDetails
                  

                        var band = await _context.Bands
                           .Where(d => d.Id == EmployeeDetailsTable.BandId)
                           .Select(d => d.BandName)
                           .FirstOrDefaultAsync();

                        var grade = await _context.Grades
                           .Where(d => d.Id == EmployeeDetailsTable.GradeId)
                           .Select(d => d.GradeName)
                           .FirstOrDefaultAsync();

                        //var grade = await _context.Grades
                        // .Where(d => d.Id == EmployeeDetailsTable.GradeId)
                        // .Select(d => d.Id)
                        // .FirstOrDefaultAsync();

                        var department = await _context.Departments
                         .Where(d => d.Id == EmployeeDetailsTable.DepartmentId)
                         .Select(d => d.DepartmentName)
                         .FirstOrDefaultAsync();

                        var designation = await _context.Designations
                           .Where(d => d.Id == EmployeeDetailsTable.DesignationId)
                           .Select(d => d.DesignationName)
                           .FirstOrDefaultAsync();

                        //var designation = await _context.Designations
                        //.Where(d => d.Id == EmployeeDetailsTable.DesignationId)
                        //.Select(d => d.Id)
                        //.FirstOrDefaultAsync();

                        return new EmailCheckResultDto
                        {
                            Exists = true,
                            Message = "Employee data found and auto filled.",
                            EmployeeName = EmployeeDetailsTable.EmployeeName,                         
                            TotalCTC = SalaryBreakdownsTable.TotalCompensation,
                            Band = band,
                            Grade= grade,
                            Department = department,
                            Designation= designation,
                            JobLocation = EmployeeDetailsTable.JobLocation,
                            PFApplicability = EmployeeDetailsTable.PFApplicability,
                            IsMetro = SalaryBreakdownsTable.IsMetro,
                            OptedNPS = SalaryBreakdownsTable.NPS != 0 ? true : false,
                            OptedVPF = SalaryBreakdownsTable.VPF != 0 ? true : false,
                            JoiningDate = DateOnly.FromDateTime(EmployeeDetailsTable.JoiningDate),
                            JoiningDatePlus3Months = DateOnly.FromDateTime(EmployeeDetailsTable.ProbationDate),
                            JoiningDatePlus6Months = DateOnly.FromDateTime(EmployeeDetailsTable.PermanentDate),
                            Email = EmployeeDetailsTable.Email,
                            MobileNumber = EmployeeDetailsTable.MobileNumber,
                            Address_Line1 = EmployeeDetailsTable.Address_Line1,
                            Address_Line2 = EmployeeDetailsTable.Address_Line2,
                            Address_Line3 = EmployeeDetailsTable.Address_Line3
                            //RefNo = offerLetter.RefNo,
                            //OfferValidTill = offerLetter.OfferValidTill
                            // Add more fields here
                        };
                    }
                    return new EmailCheckResultDto
                    {
                        Exists = false,
                        Message = "Offer Letter does not exist. Kindly generate Offer Letter first."
                    };
                }

                return new EmailCheckResultDto
                {
                    Exists = false,
                    Message = "Invalid document type."
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in CheckEmailAsync: " + ex.Message, ex);
            }

        }



        //public List<SelectListItem> GetAllBands()
        //{         
        //    try
        //    {
        //        return _context.Bands
        //       .Select(b => new SelectListItem { Text = b.BandName, Value = b.BandName })
        //       .ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in GetAllBands: " + ex.Message, ex);
        //    }       
        //}

        public List<SelectListItem> GetAllBands(int? documentType)
        {
            try
            {
                var query = _context.Bands.AsQueryable();

                if (documentType == 2)
                {
                    query = query.Where(b => b.BandName == "T"); // Only T
                }

                return query
                    .Select(b => new SelectListItem { Text = b.BandName, Value = b.BandName })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetAllBands: " + ex.Message, ex);
            }
        }


        public List<SelectListItem> GetAllDepartments()
        {
          
            try
            {
                return _context.Departments
               .Select(d => new SelectListItem { Text = d.DepartmentName, Value = d.DepartmentName })
               .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetAllDepartments: " + ex.Message, ex);
            }
           
        }

        public List<SelectListItem> GetGradesByBandName(string bandName)
        {
         
            try
            {
                var bandId = _context.Bands
                    .Where(b => b.BandName == bandName)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                return _context.Grades
                    .Where(g => g.BandId == bandId)
                    .Select(g => new SelectListItem { Text = g.GradeName, Value = g.GradeName })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetGradesByBandName: " + ex.Message, ex);
            }
         
        }

        public List<SelectListItem> GetDesignationsByNames(string bandName, string gradeName, string departmentName)
        {

            try
            {
                var bandId = _context.Bands
                    .Where(b => b.BandName == bandName)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                var gradeId = _context.Grades
                    .Where(g => g.GradeName == gradeName && g.BandId == bandId)
                    .Select(g => g.Id)
                    .FirstOrDefault();

                var departmentId = _context.Departments
                    .Where(d => d.DepartmentName == departmentName)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                return _context.Designations
                    .Where(d => d.BandId == bandId && d.GradeId == gradeId && d.DepartmentId == departmentId)
                    .Select(d => new SelectListItem { Text = d.DesignationName, Value = d.DesignationName })
                    .ToList();
                //.Select(d => new SelectListItem { Text = d.DesignationName, Value = d.Id.ToString() })

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetDesignationsByNames: " + ex.Message, ex);
            }
         
        }

    }

}
