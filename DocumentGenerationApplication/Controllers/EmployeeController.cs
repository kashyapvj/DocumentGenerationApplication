using DocumentGenerationApplication.Models.Employee;
using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.Tables;
using DocumentGenerationApplication.Models.UI_Model;
using DocumentGenerationApplication.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;
using Xceed.Words.NET;

namespace DocumentGenerationApplication.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DesignationSelectionViewModel
            {
                Bands = await _repo.GetBandsAsync()
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOfferLetters()
        {
            try
            {
                var employees = await _repo.GetAllEmployeesAsync();


                return View(employees);
            }
            catch (Exception ex)
            {
                // Optional: log error
                TempData["ErrorMessage"] = "Failed to load employee list: " + ex.Message;
                return View(new List<EmployeeDetails>()); // Return empty list if error
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditOfferLetters(int id)
        {

            try
            {
                var employee = await _repo.GetEmployeeByIdAsync(id);

                // Fetch dropdowns via repo
                ViewBag.BandId = await _repo.GetBandsAsync();
                ViewBag.GradeId = await _repo.GetGradesByBandIdAsync(employee.BandId);
                ViewBag.DepartmentId = await _repo.GetDepartmentsAsync();
                ViewBag.DesignationId = await _repo.GetDesignationsAsync(employee.BandId, employee.GradeId, employee.DepartmentId);

                if (employee == null)
                    return NotFound();

                return View(employee);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in EditOfferLetters: " + ex.Message, ex);
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditOfferLetters(EmployeeDetails model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch existing employee from DB
                    var existingEmployee = await _repo.GetEmployeeByIdAsync(model.Id);

                    if (existingEmployee == null)
                        return NotFound();

                    // ✅ Check if mobile number changed
                    if (!string.Equals(existingEmployee.MobileNumber, model.MobileNumber, StringComparison.Ordinal))
                    {
                        // Only check duplicates if number changed
                        var mobileExists = await _repo.IsMobileNumberExistsAsync(model.MobileNumber, model.Id);

                        if (mobileExists)
                        {
                            ModelState.AddModelError("MobileNumber", "This mobile number is already registered.");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        await _repo.UpdateEmployeeAsync(model);
                        return RedirectToAction(nameof(GetAllOfferLetters));
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error in EditOfferLetters_POST: " + ex.Message, ex);
                }
            }

            // Reload dropdowns if validation fails
            ViewBag.BandId = await _repo.GetBandsAsync();
            ViewBag.GradeId = await _repo.GetGradesByBandIdAsync(model.BandId);
            ViewBag.DepartmentId = await _repo.GetDepartmentsAsync();
            ViewBag.DesignationId = await _repo.GetDesignationsAsync(model.BandId, model.GradeId, model.DepartmentId);

            return View(model);
        }




       


        //[HttpPost]
        //public async Task<IActionResult> EditOfferLetters(EmployeeDetails model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _repo.UpdateEmployeeAsync(model);
        //            return RedirectToAction(nameof(GetAllOfferLetters));
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new ApplicationException("Error in EditOfferLetters_POST: " + ex.Message, ex);
        //        }
        //    }

        //    // Reload dropdowns if validation fails
        //    ViewBag.BandId = await _repo.GetBandsAsync();
        //    ViewBag.GradeId = await _repo.GetGradesByBandIdAsync(model.BandId);
        //    ViewBag.DepartmentId = await _repo.GetDepartmentsAsync();
        //    ViewBag.DesignationId = await _repo.GetDesignationsAsync(model.BandId, model.GradeId, model.DepartmentId);

        //    return View(model);
        //}



        [HttpGet]
        public async Task<IActionResult> GetEmployeeSalaryDetails(int id)
        {
            try
            {
                // Step 1: Fetch employee
                var employee = await _repo.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound("Employee not found.");

                // Step 2: Fetch salary details
                var salaryDetails = await _repo
                    .GetSalaryBreakdownsByEmailAndMobileAsync(employee.Email, employee.MobileNumber);

                if (salaryDetails == null || !salaryDetails.Any())
                    return NotFound("No salary details found for this employee.");


                // Step 3: Return modal partial view
                return PartialView("_EmployeeSalaryModal", salaryDetails);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetEmployeeSalaryDetails: " + ex.Message, ex);
            }
        }


        # region
        private string FormatAmount(decimal amount, CultureInfo culture)
        {

            try
            {
                return amount == 0 ? "-" : amount.ToString("N0", culture);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in FormatAmount: " + ex.Message, ex);
            }
        }

        public static string ConvertDecimalToINRWords(decimal amount)
        {

            try
            {
                if (amount == 0)
                    return "Zero Rupees Only";

                string[] unitsMap = {
                "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                "Eighteen", "Nineteen"};

                string[] tensMap = {
                "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};

                string ConvertToWords(long number)
                {
                    string words = "";

                    if ((number / 10000000) > 0)
                    {
                        words += ConvertToWords(number / 10000000) + " Crore ";
                        number %= 10000000;
                    }

                    if ((number / 100000) > 0)
                    {
                        words += ConvertToWords(number / 100000) + " Lakh ";
                        number %= 100000;
                    }

                    if ((number / 1000) > 0)
                    {
                        words += ConvertToWords(number / 1000) + " Thousand ";
                        number %= 1000;
                    }

                    if ((number / 100) > 0)
                    {
                        words += ConvertToWords(number / 100) + " Hundred ";
                        number %= 100;
                    }

                    if (number > 0)
                    {
                        if (!string.IsNullOrEmpty(words))
                            words += "and ";

                        if (number < 20)
                            words += unitsMap[number];
                        else
                        {
                            words += tensMap[number / 10];
                            if ((number % 10) > 0)
                                words += " " + unitsMap[number % 10];
                        }
                    }

                    return words.Trim();
                }

                long rupees = (long)Math.Floor(amount);
                int paise = (int)((amount - rupees) * 100);

                string result = ConvertToWords(rupees) + " Rupees";

                if (paise > 0)
                {
                    result += " and " + ConvertToWords(paise) + " Paise";
                }

                return result + " Only";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in ConvertDecimalToINRWords: " + ex.Message, ex);
            }


        }

        [Obsolete]
        public byte[] Generate_Pdf( FillPdfTemplateInput input)
        {

            try
            {
                decimal amount = input.employeeDetails.TotalCTC;
                input.employeeDetails.RupeesInWords = ConvertDecimalToINRWords(amount);

                var indianCulture = new System.Globalization.CultureInfo("en-IN");

                string templateFileName = input.employeeDetails.DocumentType switch
                {
                    1 => "Offer_Letter_Experienced.docx",
                    2 => "Offer_Letter_Fresher.docx",
                    3 => "Appointment_Letter_Experienced.docx",
                    4 => "Appointment_Letter_Fresher.docx",
                    _ => throw new InvalidOperationException("Invalid Document Type selected.")
                };

                string webRoot = _env.WebRootPath;
                string templatePath = Path.Combine(webRoot, "templates", "DOCs", templateFileName);
                string filledDocxPath = Path.Combine(webRoot, "templates", "PDFs", $"Filled_{templateFileName}");
                string outputPdfPath = Path.ChangeExtension(filledDocxPath, ".pdf");

                var doc = DocX.Load(templatePath);
                doc.ReplaceText("DD.MM.YYYY", DateTime.Now.ToString("dd-MM-yyyy", indianCulture));
                doc.ReplaceText("<<Employee Name>>", input.employeeDetails.EmployeeName);
                doc.ReplaceText("<<Designation>>", input.employeeDetails.Designation);
                doc.ReplaceText("<<Joining Date>>", input.employeeDetails.JoiningDate.ToString("dd-MM-yyyy", indianCulture));

                doc.ReplaceText("<<Address_Line1>>", input.employeeDetails.Address_Line1);
                doc.ReplaceText("<<Address_Line2>>", input.employeeDetails.Address_Line2);
                doc.ReplaceText("<<Address_Line3>>", input.employeeDetails.Address_Line3);

                doc.ReplaceText("<<Mob No>>", input.employeeDetails.MobileNumber);
                doc.ReplaceText("<<Mail id>>", input.employeeDetails.Email);
                string offerValidText = input.employeeDetails.OfferValidTill1 == 1 ? "today" : "two days";
                doc.ReplaceText("<<OfferLetterValidTill>>", offerValidText);
                doc.ReplaceText("Ref_No", input.employeeDetails.RefNo);
                doc.ReplaceText("<<Employee ID>>", input.employeeDetails.EmployeeId.ToString());
                doc.ReplaceText("<<Job Location>>", input.employeeDetails.JobLocation);
                doc.ReplaceText("<<CTC in no>>", FormatAmount(input.employeeDetails.TotalCTC, indianCulture));
                doc.ReplaceText("<<CTC in word>>", input.employeeDetails.RupeesInWords);
                doc.ReplaceText("<<Band>>", input.employeeDetails.Band);
                doc.ReplaceText("<<Grade>>", input.employeeDetails.Grade);
                doc.ReplaceText("<<PF Applicability>>", input.employeeDetails.PFApplicability);
                doc.ReplaceText("<<Probation date>>", input.employeeDetails.JoiningDatePlus3Months.ToString("dd-MM-yyyy", indianCulture));
                doc.ReplaceText("<<Permanent date>>", input.employeeDetails.JoiningDatePlus6Months.ToString("dd-MM-yyyy", indianCulture));

                // Salary replacements with 0 check
                var s = input.salaryValues;
                doc.ReplaceText("Basic_A", FormatAmount(s.Basic, indianCulture));
                doc.ReplaceText("Basic_M", FormatAmount(Math.Round(s.Basic / 12, 0), indianCulture));
                doc.ReplaceText("HRA_A", FormatAmount(s.HRA, indianCulture));
                doc.ReplaceText("HRA_M", FormatAmount(Math.Round(s.HRA / 12, 0), indianCulture));
                doc.ReplaceText("Statutory_A", FormatAmount(s.StatutoryBonus, indianCulture));
                doc.ReplaceText("Statutory_M", FormatAmount(Math.Round(s.StatutoryBonus / 12, 0), indianCulture));
                doc.ReplaceText("NPS_A", FormatAmount(s.NPS, indianCulture));
                doc.ReplaceText("NPS_M", FormatAmount(Math.Round(s.NPS / 12, 0), indianCulture));
                doc.ReplaceText("VPF_A", FormatAmount(s.VPF, indianCulture));
                doc.ReplaceText("VPF_M", FormatAmount(Math.Round(s.VPF / 12, 0), indianCulture));
                doc.ReplaceText("RFB_A", FormatAmount(s.RFB, indianCulture));
                doc.ReplaceText("RFB_M", FormatAmount(Math.Round(s.RFB / 12, 0), indianCulture));
                doc.ReplaceText("SA_A", FormatAmount(s.SpecialAllowance, indianCulture));
                doc.ReplaceText("SA_M", FormatAmount(Math.Round(s.SpecialAllowance / 12, 0), indianCulture));
                doc.ReplaceText("TF_A", FormatAmount(s.TotalFixedComponent, indianCulture));
                doc.ReplaceText("TF_M", FormatAmount(Math.Round(s.TotalFixedMonthlyComponent, 0), indianCulture));
                doc.ReplaceText("VP_A", FormatAmount(s.VariablePay, indianCulture));
                doc.ReplaceText("VP_M", FormatAmount(Math.Round(s.VariablePay / 12, 0), indianCulture));
                doc.ReplaceText("NS_A", FormatAmount(s.NetSalary, indianCulture));
                doc.ReplaceText("NS_M", FormatAmount(Math.Round(s.NetSalary / 12, 0), indianCulture));
                doc.ReplaceText("PFO_A", FormatAmount(s.PFEmployer, indianCulture));
                doc.ReplaceText("PFO_M", FormatAmount(Math.Round(s.PFEmployer / 12, 0), indianCulture));
                doc.ReplaceText("ESICO_A", FormatAmount(s.ESICEmployer, indianCulture));
                doc.ReplaceText("ESICO_M", FormatAmount(Math.Round(s.ESICEmployer / 12, 0), indianCulture));
                doc.ReplaceText("G_A", FormatAmount(s.Gratuity, indianCulture));
                doc.ReplaceText("G_M", FormatAmount(Math.Round(s.Gratuity / 12, 0), indianCulture));
                doc.ReplaceText("IC_A", FormatAmount(s.InsuranceCoverage, indianCulture));
                doc.ReplaceText("IC_M", FormatAmount(Math.Round(s.InsuranceCoverage / 12, 0), indianCulture));
                doc.ReplaceText("TB_A", FormatAmount(s.TotalAnnualBenefits, indianCulture));
                doc.ReplaceText("TB_M", FormatAmount(Math.Round(s.TotalAnnualBenefits / 12, 0), indianCulture));
                doc.ReplaceText("TC_A", FormatAmount(s.TotalCompensation, indianCulture));
                doc.ReplaceText("TC_M", FormatAmount(Math.Round(s.TotalCompensation / 12, 0), indianCulture));
                doc.ReplaceText("PFE_A", FormatAmount(s.PFEmployee, indianCulture));
                doc.ReplaceText("PFE_M", FormatAmount(Math.Round(s.PFEmployee / 12, 0), indianCulture));
                doc.ReplaceText("ESICE_A", FormatAmount(s.ESICEmployee, indianCulture));
                doc.ReplaceText("ESICE_M", FormatAmount(Math.Round(s.ESICEmployee / 12, 0), indianCulture));
                doc.ReplaceText("PT_A", FormatAmount(s.ProfessionalTax, indianCulture));
                doc.ReplaceText("PT_M", FormatAmount(Math.Round(s.ProfessionalTaxMonthly, 0), indianCulture));

                decimal d_m_value = Math.Round((s.PFEmployee / 12) + (s.ESICEmployee / 12) + s.ProfessionalTaxMonthly, 0);
                doc.ReplaceText("D_A", FormatAmount(s.TotalDeductions, indianCulture));
                doc.ReplaceText("D_M", FormatAmount(d_m_value, indianCulture));

                doc.SaveAs(filledDocxPath);

                // LibreOffice conversion
              // var libreOfficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
               var libreOfficePath = @"C:\Libre_Office\program\soffice.exe";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = libreOfficePath,
                        Arguments = $"--convert-to pdf \"{filledDocxPath}\" --outdir \"{Path.GetDirectoryName(filledDocxPath)}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!System.IO.File.Exists(outputPdfPath))
                    throw new Exception("PDF conversion failed: " + err);

                return System.IO.File.ReadAllBytes(outputPdfPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GeneratePdfFromWord: " + ex.Message, ex);
            }



        }

        #endregion




        [HttpGet]
        public async Task<IActionResult> DownloadOfferLetterPdf(int id, bool status, bool preview = false)
        {
            try
            {
                var employee = await _repo.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound("Employee not found.");

                var salaryDetails = await _repo
                    .GetSalaryBreakdownsByEmailAndMobileAsync(employee.Email, employee.MobileNumber);

                if (salaryDetails == null || !salaryDetails.Any())
                    return NotFound("No salary details found for this employee.");

                var latestSalary = salaryDetails.OrderByDescending(s => s.Id).FirstOrDefault();

                if (status)
                {
                    switch (employee.DocumentType)
                    {
                        case 1: employee.DocumentType = 3; break;
                        case 2: employee.DocumentType = 4; break;
                    }
                }

                var _salaryBreakdown = new FillPdfTemplateInput
                {
                    salaryValues = new SalaryBreakdown
                    {
                        EmployeeName = latestSalary.EmployeeName,
                        Email = latestSalary.Email,
                        MobileNumber = latestSalary.MobileNumber,
                        IsMetro = latestSalary.IsMetro,
                        TotalCompensation = latestSalary.TotalCompensation,
                        Basic = latestSalary.Basic,
                        HRA = latestSalary.HRA,
                        StatutoryBonus = latestSalary.StatutoryBonus,
                        NPS = latestSalary.NPS,
                        VPF = latestSalary.VPF,
                        RFB = latestSalary.RFB,
                        SpecialAllowance = latestSalary.SpecialAllowance,
                        TotalFixedComponent = latestSalary.TotalFixedComponent,
                        VariablePay = latestSalary.VariablePay,
                        PFEmployee = latestSalary.PFEmployee,
                        ESICEmployee = latestSalary.ESICEmployee,
                        ProfessionalTax = latestSalary.ProfessionalTax,
                        ProfessionalTaxMonthly = latestSalary.ProfessionalTaxMonthly,
                        TotalDeductions = latestSalary.TotalDeductions,
                        NetSalary = latestSalary.NetSalary,
                        PFEmployer = latestSalary.PFEmployer,
                        ESICEmployer = latestSalary.ESICEmployer,
                        Gratuity = latestSalary.Gratuity,
                        InsuranceCoverage = latestSalary.InsuranceCoverage,
                        CurrentDate = DateTime.Today
                    },
                    employeeDetails = new SalaryBreakdownInput
                    {
                        DocumentType = employee.DocumentType,
                        EmployeeId = employee.EmployeeId,
                        EmployeeName = employee.EmployeeName,
                        Email = employee.Email,
                        MobileNumber = employee.MobileNumber,
                        Department = employee.Department?.DepartmentName,
                        Band = employee.Band?.BandName,
                        Grade = employee.Grade?.GradeName,
                        Designation = employee.Designation?.DesignationName,
                        JobLocation = employee.JobLocation,
                        Address_Line1 = employee.Address_Line1,
                        Address_Line2 = employee.Address_Line2,
                        Address_Line3 = employee.Address_Line3,
                        RefNo = employee.RefNo,
                        JoiningDate = employee.JoiningDate,
                        JoiningDatePlus3Months = DateOnly.FromDateTime(employee.ProbationDate),
                        JoiningDatePlus6Months = DateOnly.FromDateTime(employee.PermanentDate),
                        TotalCTC = latestSalary.TotalCompensation,
                        Status = employee.Status,
                        PFApplicability = employee.PFApplicability,
                        OfferValidTill = employee.OfferValidTill
                    }
                };

                byte[] pdfBytes;
                //string fileName;

                pdfBytes = Generate_Pdf(_salaryBreakdown);


                string input = employee.RefNo;

                if (employee.DocumentType == 1 && status == false)
                {
                    var fileName1 = $"OfferLetter_Experience_{employee.EmployeeName}.pdf";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName1}\"");
                    return File(pdfBytes, "application/pdf");
                }
                else if (employee.DocumentType == 2 && status == false)
                {
                    var fileName1 = $"OfferLetter_Fresher_{employee.EmployeeName}.pdf";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName1}\"");
                    return File(pdfBytes, "application/pdf");
                }
                else if (employee.DocumentType == 3 && status == true && employee.EmployeeId != null && input.Substring(0, 11) == "Appointment")
                {
                    var safeEmployeeName = string.Join("_", employee.EmployeeName.Split(Path.GetInvalidFileNameChars()));
                    var safeEmployeeId = string.Join("_", employee.EmployeeId.ToString().Split(Path.GetInvalidFileNameChars()));
                    var fileName1 = $"AppointmentLetter_Experience_{safeEmployeeName}_{safeEmployeeId}.pdf";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName1}\"");
                    return File(pdfBytes, "application/pdf");
                }
                else if (employee.DocumentType == 4 && status == true && employee.EmployeeId != null && input.Substring(0, 11) == "Appointment")
                {
                    var safeEmployeeName = string.Join("_", employee.EmployeeName.Split(Path.GetInvalidFileNameChars()));
                    var safeEmployeeId = string.Join("_", employee.EmployeeId.ToString().Split(Path.GetInvalidFileNameChars()));
                    var fileName1 = $"AppointmentLetter_Fresher_{safeEmployeeName}_{safeEmployeeId}.pdf";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName1}\"");
                    return File(pdfBytes, "application/pdf");
                }
                else
                {
                    throw new ArgumentException("Invalid Document Type provided.", nameof(employee.DocumentType));
                }

      
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in DownloadOfferLetterPdf: " + ex.Message, ex);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentLetters()
        {
            try
            {
                var employees = await _repo.GetAllEmployeesAsync();
                //return View(employees);
                var AppointmentLetterList = employees
                .Where(e => e.EmployeeId != null && !string.IsNullOrEmpty(e.RefNo))
                .ToList();

                return View(AppointmentLetterList);
            }
            catch (Exception ex)
            {
                // Optional: log error
                TempData["ErrorMessage"] = "Failed to load employee list: " + ex.Message;
                return View(new List<EmployeeDetails>()); // Return empty list if error
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetGrades(int bandId)
        {
            var grades = await _repo.GetGradesByBandIdAsync(bandId);
            return Json(grades);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartments()
        {
            var departments = await _repo.GetDepartmentsAsync();
            return Json(departments);
        }

        [HttpGet]
        public async Task<JsonResult> GetDesignations(int bandId, int gradeId, int departmentId)
        {
            var designations = await _repo.GetDesignationsAsync(bandId, gradeId, departmentId);
            return Json(designations);
        }


        //[HttpGet]
        //public IActionResult ReadOfferLetter()
        //{
        //    //var designations = await _repo.GetOfferLetterAsync(bandId, gradeId, departmentId);
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult ReadAppointmentLetter()
        //{
        //    //var designations = await _repo.GetOfferLetterAsync(bandId, gradeId, departmentId);
        //    return View();
        //}

    }
}

