using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.Tables;
using DocumentGenerationApplication.Models.UI_Model;
using DocumentGenerationApplication.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Xceed.Words.NET;


namespace DocumentGenerationApplication.Controllers
{
    [Authorize]
    [Route("Salary")]
    public class SalaryController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly ISalaryRepository _repository;


        public SalaryController(IWebHostEnvironment env, ISalaryRepository repository)
        {
            _env = env;
            _repository = repository;
        }


        [HttpGet("GetBands")]
        public IActionResult GetBands(int? documentType)
        {
            try
            {
                var bands = _repository.GetAllBands(documentType);
                return Json(bands);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetBands: " + ex.Message, ex);
            }
        }


        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {

            try
            {
                var departments = _repository.GetAllDepartments();
                return Json(departments);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetDepartments: " + ex.Message, ex);
            }

         
        }

        [HttpGet("GetGrades")]
        public IActionResult GetGrades(string bandName)
        {
            try
            {
                var grades = _repository.GetGradesByBandName(bandName);
                return Json(grades);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetGrades: " + ex.Message, ex);
            }

          
        }

        [HttpGet("GetDesignations")]
        public IActionResult GetDesignations(string bandName, string gradeName, string departmentName)
        {
            try
            {
                var designations = _repository.GetDesignationsByNames(bandName, gradeName, departmentName);
                return Json(designations);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in GetDesignations: " + ex.Message, ex);
            }

           
        }


        //------------------------------------------------------------------------>>>>
        //public static string ConvertDecimalToINRWords(decimal amount)
        //{

        //    try
        //    {
        //        if (amount == 0)
        //            return "Zero Rupees Only";

        //        string[] unitsMap = {
        //        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        //        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
        //        "Eighteen", "Nineteen"};

        //        string[] tensMap = {
        //        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};

        //        string ConvertToWords(long number)
        //        {
        //            string words = "";

        //            if ((number / 10000000) > 0)
        //            {
        //                words += ConvertToWords(number / 10000000) + " Crore ";
        //                number %= 10000000;
        //            }

        //            if ((number / 100000) > 0)
        //            {
        //                words += ConvertToWords(number / 100000) + " Lakh ";
        //                number %= 100000;
        //            }

        //            if ((number / 1000) > 0)
        //            {
        //                words += ConvertToWords(number / 1000) + " Thousand ";
        //                number %= 1000;
        //            }

        //            if ((number / 100) > 0)
        //            {
        //                words += ConvertToWords(number / 100) + " Hundred ";
        //                number %= 100;
        //            }

        //            if (number > 0)
        //            {
        //                if (!string.IsNullOrEmpty(words))
        //                    words += "and ";

        //                if (number < 20)
        //                    words += unitsMap[number];
        //                else
        //                {
        //                    words += tensMap[number / 10];
        //                    if ((number % 10) > 0)
        //                        words += " " + unitsMap[number % 10];
        //                }
        //            }

        //            return words.Trim();
        //        }

        //        long rupees = (long)Math.Floor(amount);
        //        int paise = (int)((amount - rupees) * 100);

        //        string result = ConvertToWords(rupees) + " Rupees";

        //        if (paise > 0)
        //        {
        //            result += " and " + ConvertToWords(paise) + " Paise";
        //        }

        //        return result + " Only";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in ConvertDecimalToINRWords: " + ex.Message, ex);
        //    }


        //}


        #region
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
        public byte[] Generate_Pdf(FillPdfTemplateInput input)
        {

            try
            {
                //decimal amount = input.employeeDetails.TotalCTC;
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
                //doc.ReplaceText("<<Joining Date>>", input.employeeDetails.JoiningDate.ToString("dd-MM-yyyy", indianCulture));
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




        // GET: /Salary/CalculateSalaryBreakdown
        [HttpGet("CalculateSalaryBreakdown")]
        public IActionResult CalculateSalaryBreakdown()
        {

            try
            {
                // Optional: Provide a pre-filled model
                var model = new SalaryBreakdownInput();
       
                return View(model); // Return a view if it's an MVC app
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in CalculateSalaryBreakdown_GET: " + ex.Message, ex);
            }

        }


        //[HttpPost("CalculateSalaryBreakdown")]
        //public async Task<IActionResult> CalculateSalaryBreakdown([FromBody] SalaryBreakdownInput inputModel)
        //{
        //    try
        //    {

        //        decimal amount = inputModel.TotalCTC;
        //        inputModel.RupeesInWords = ConvertDecimalToINRWords(amount);

        //        var today = DateTime.Today;
        //        var daysDifference = (inputModel.JoiningDate.Date - today).Days;


        //        inputModel.OfferValidTill1 = daysDifference > 1 ? 2 : 1;

        //        var model = new SalaryBreakdown
        //        {
        //            TotalCompensation = inputModel.TotalCTC,
        //            Basic = Math.Round(inputModel.TotalCTC * 0.32M, 0),
        //            IsMetro = inputModel.IsMetro
        //        };

        //        model.HRA = inputModel.IsMetro ? Math.Round(model.Basic * 0.50M, 0) : Math.Round(model.Basic * 0.40M, 0);
        //        model.StatutoryBonus = (model.Basic < 252000) ? Math.Round(model.Basic * 0.0833M, 0) : 0;
        //        model.NPS = inputModel.OptedNPS ? Math.Round(model.Basic * 0.10M, 0) : 0;
        //        model.VPF = inputModel.OptedVPF ? Math.Round(model.Basic * 0.12M, 0) : 0;
        //        model.RFB = (model.TotalCompensation > 600000) ? 183400 : 0;

        //        model.InsuranceCoverage = inputModel.Band switch
        //        {
        //            "T" or "A" => 13000,
        //            "B" => 20000,
        //            "C" => 25000,
        //            "D" => 30000,
        //            "E" => 35000,
        //            _ => 0
        //        };

        //        decimal pfBase = model.Basic >= 180000 && inputModel.PFApplicability != "Full" ? 180000 : model.Basic;
        //        model.PFEmployer = Math.Round(pfBase * 0.12M, 0);
        //        model.PFEmployee = Math.Round(pfBase * 0.12M, 0);
        //        model.Gratuity = Math.Round(model.Basic * 0.048M, 0);

        //        // ESIC Calculation
        //        bool isEligibleForESIC = (model.IsMetro && inputModel.TotalCTC < 254000) || (!model.IsMetro && inputModel.TotalCTC < 295250);
        //        bool isUnderWageLimit = model.TotalFixedMonthlyComponent < 21000;
        //        if (isEligibleForESIC && isUnderWageLimit)
        //        {
        //            decimal employerBase = model.TotalCompensation - model.PFEmployer - model.Gratuity - model.InsuranceCoverage;
        //            model.ESICEmployer = Math.Round((employerBase * 0.0325M) / 1.0325M, 0);
        //        }
        //        else
        //        {
        //            model.ESICEmployer = 0;
        //        }

        //        model.VariablePay = 0;
        //        decimal fixedUsed = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB
        //                                + model.PFEmployer + model.ESICEmployer + model.Gratuity + model.InsuranceCoverage;

        //        model.SpecialAllowance = Math.Round(inputModel.TotalCTC - fixedUsed, 0);
        //        model.TotalFixedComponent = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB + model.SpecialAllowance;

        //        model.VariablePay = inputModel.TotalCTC switch
        //        {
        //            <= 1000000 => 0,
        //            <= 2000000 => Math.Round(0.05M * model.TotalFixedComponent, 0),
        //            _ => Math.Round(0.10M * model.TotalFixedComponent, 0)
        //        };

        //        if (model.VariablePay > 0)
        //        {
        //            fixedUsed = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB
        //                                    + model.PFEmployer + model.ESICEmployer + model.Gratuity + model.InsuranceCoverage + model.VariablePay;

        //            model.SpecialAllowance = Math.Round(inputModel.TotalCTC - fixedUsed, 0);
        //            model.TotalFixedComponent = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB + model.SpecialAllowance;
        //            model.NetSalary = model.TotalFixedComponent + model.VariablePay;
        //        }
        //        else
        //        {
        //            model.NetSalary = model.TotalFixedComponent;
        //        }

        //        if (isEligibleForESIC && isUnderWageLimit)
        //            model.ESICEmployee = Math.Round(model.TotalFixedComponent * 0.0075M, 0);
        //        else
        //            model.ESICEmployee = 0;

        //        model.EmployeeId = inputModel.EmployeeId;
        //        model.TotalDeductions = model.PFEmployee + model.ESICEmployee + model.ProfessionalTax;

        //        var _salaryBreakdown = new FillPdfTemplateInput
        //        {
        //            salaryValues = model,
        //            employeeDetails = inputModel
        //        };

        //        // ✅ Only Save in DB (no PDF generation)
        //        await _repository.SaveEmployeeDetailsAsync(_salaryBreakdown);

        //        // Return success response
        //        return Ok(new { message = "Data saved successfully." });
        //    }
        //    catch (InvalidOperationException ex) // e.g., duplicate constraints
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (ApplicationException ex) // repo wrapped errors
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //    catch (Exception ex) // unexpected
        //    {
        //        return StatusCode(500, new { message = "Unexpected error: " + ex.Message });
        //    }

        //}


        private FillPdfTemplateInput CalculateBreakdown(SalaryBreakdownInput inputModel)
        {
            decimal amount = inputModel.TotalCTC;
            inputModel.RupeesInWords = ConvertDecimalToINRWords(amount);

            var today = DateTime.Today;
            var daysDifference = (inputModel.JoiningDate.Date - today).Days;


            inputModel.OfferValidTill1 = daysDifference > 1 ? 2 : 1;

            var model = new SalaryBreakdown
            {
                TotalCompensation = inputModel.TotalCTC,
                Basic = Math.Round(inputModel.TotalCTC * 0.32M, 0),
                IsMetro = inputModel.IsMetro
            };

            model.HRA = inputModel.IsMetro ? Math.Round(model.Basic * 0.50M, 0) : Math.Round(model.Basic * 0.40M, 0);
            model.StatutoryBonus = (model.Basic < 252000) ? Math.Round(model.Basic * 0.0833M, 0) : 0;
            model.NPS = inputModel.OptedNPS ? Math.Round(model.Basic * 0.10M, 0) : 0;
            model.VPF = inputModel.OptedVPF ? Math.Round(model.Basic * 0.12M, 0) : 0;
            model.RFB = (model.TotalCompensation > 600000) ? 183400 : 0;

            model.InsuranceCoverage = inputModel.Band switch
            {
                "T" or "A" => 13000,
                "B" => 20000,
                "C" => 25000,
                "D" => 30000,
                "E" => 35000,
                _ => 0
            };

            decimal pfBase = model.Basic >= 180000 && inputModel.PFApplicability != "Full" ? 180000 : model.Basic;
            model.PFEmployer = Math.Round(pfBase * 0.12M, 0);
            model.PFEmployee = Math.Round(pfBase * 0.12M, 0);
            model.Gratuity = Math.Round(model.Basic * 0.048M, 0);

            // ESIC Calculation
            bool isEligibleForESIC = (model.IsMetro && inputModel.TotalCTC < 254000) || (!model.IsMetro && inputModel.TotalCTC < 295250);
            bool isUnderWageLimit = model.TotalFixedMonthlyComponent < 21000;
            if (isEligibleForESIC && isUnderWageLimit)
            {
                decimal employerBase = model.TotalCompensation - model.PFEmployer - model.Gratuity - model.InsuranceCoverage;
                model.ESICEmployer = Math.Round((employerBase * 0.0325M) / 1.0325M, 0);
            }
            else
            {
                model.ESICEmployer = 0;
            }

            model.VariablePay = 0;
            decimal fixedUsed = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB
                                    + model.PFEmployer + model.ESICEmployer + model.Gratuity + model.InsuranceCoverage;

            model.SpecialAllowance = Math.Round(inputModel.TotalCTC - fixedUsed, 0);
            model.TotalFixedComponent = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB + model.SpecialAllowance;

            model.VariablePay = inputModel.TotalCTC switch
            {
                <= 1000000 => 0,
                <= 2000000 => Math.Round(0.05M * model.TotalFixedComponent, 0),
                _ => Math.Round(0.10M * model.TotalFixedComponent, 0)
            };

            if (model.VariablePay > 0)
            {
                fixedUsed = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB
                                        + model.PFEmployer + model.ESICEmployer + model.Gratuity + model.InsuranceCoverage + model.VariablePay;

                model.SpecialAllowance = Math.Round(inputModel.TotalCTC - fixedUsed, 0);
                model.TotalFixedComponent = model.Basic + model.HRA + model.StatutoryBonus + model.NPS + model.VPF + model.RFB + model.SpecialAllowance;
                model.NetSalary = model.TotalFixedComponent + model.VariablePay;
            }
            else
            {
                model.NetSalary = model.TotalFixedComponent;
            }

            if (isEligibleForESIC && isUnderWageLimit)
                model.ESICEmployee = Math.Round(model.TotalFixedComponent * 0.0075M, 0);
            else
                model.ESICEmployee = 0;

            model.EmployeeId = inputModel.EmployeeId;
            model.TotalDeductions = model.PFEmployee + model.ESICEmployee + model.ProfessionalTax;

            var _salaryBreakdown = new FillPdfTemplateInput
            {
                salaryValues = model,
                employeeDetails = inputModel
            };

            return _salaryBreakdown;
        }



        [HttpPost("PreviewSalaryBreakdown")]
        public IActionResult PreviewSalaryBreakdown([FromBody] SalaryBreakdownInput inputModel)
        {
            try
            {
                // Step 1: calculate salary
                var model = CalculateBreakdown(inputModel);

                // Step 2: generate preview PDF
                            
                var pdfBytes = Generate_Pdf(model); 
               

                // Step 3: return PDF as response
                return File(pdfBytes, "application/pdf", "SalaryBreakdownPreview.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating preview: " + ex.Message });
            }
        }

        [HttpPost("SubmitSalaryBreakdown")]
        public async Task<IActionResult> SubmitSalaryBreakdown([FromBody] SalaryBreakdownInput inputModel)
        {
            try
            {
                var model = CalculateBreakdown(inputModel);

                // ✅ Save only when user confirms
                await _repository.SaveEmployeeDetailsAsync(model);

                return Ok(new { message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error while saving: " + ex.Message });
            }
        }










        //[Obsolete]
        //public byte[] GeneratePdfFromWord([FromBody] FillPdfTemplateInput input)
        //{

        //    try
        //    {
        //        var indianCulture = new System.Globalization.CultureInfo("en-IN");

        //        string templateFileName = input.employeeDetails.DocumentType switch
        //        {
        //            1 => "Offer_Letter_Experienced.docx",
        //            2 => "Offer_Letter_Fresher.docx",
        //            3 => "Appointment_Letter_Experienced.docx",
        //            4 => "Appointment_Letter_Fresher.docx",
        //            _ => throw new InvalidOperationException("Invalid Document Type selected.")
        //        };

        //        string webRoot = _env.WebRootPath;
        //        string templatePath = Path.Combine(webRoot, "templates", "DOCs", templateFileName);
        //        string filledDocxPath = Path.Combine(webRoot, "templates", "PDFs", $"Filled_{templateFileName}");
        //        string outputPdfPath = Path.ChangeExtension(filledDocxPath, ".pdf");

        //        var doc = DocX.Load(templatePath);
        //        doc.ReplaceText("DD.MM.YYYY", DateTime.Now.ToString("dd-MM-yyyy", indianCulture));
        //        doc.ReplaceText("<<Employee Name>>", input.employeeDetails.EmployeeName);
        //        doc.ReplaceText("<<Designation>>", input.employeeDetails.Designation);
        //        doc.ReplaceText("<<Joining Date>>", input.employeeDetails.JoiningDate.ToString("dd-MM-yyyy", indianCulture));

        //        doc.ReplaceText("<<Address_Line1>>", input.employeeDetails.Address_Line1);
        //        doc.ReplaceText("<<Address_Line2>>", input.employeeDetails.Address_Line2);
        //        doc.ReplaceText("<<Address_Line3>>", input.employeeDetails.Address_Line3);

        //        doc.ReplaceText("<<Mob No>>", input.employeeDetails.MobileNumber);
        //        doc.ReplaceText("<<Mail id>>", input.employeeDetails.Email);
        //        string offerValidText = input.employeeDetails.OfferValidTill1 == 1 ? "today" : "two days";
        //        doc.ReplaceText("<<OfferLetterValidTill>>", offerValidText);
        //        doc.ReplaceText("Ref_No", input.employeeDetails.RefNo);
        //        doc.ReplaceText("<<Employee ID>>", input.employeeDetails.EmployeeId.ToString());
        //        doc.ReplaceText("<<Job Location>>", input.employeeDetails.JobLocation);
        //        doc.ReplaceText("<<CTC in no>>", FormatAmount(input.employeeDetails.TotalCTC, indianCulture));
        //        doc.ReplaceText("<<CTC in word>>", input.employeeDetails.RupeesInWords);
        //        doc.ReplaceText("<<Band>>", input.employeeDetails.Band);
        //        doc.ReplaceText("<<Grade>>", input.employeeDetails.Grade);
        //        doc.ReplaceText("<<PF Applicability>>", input.employeeDetails.PFApplicability);
        //        doc.ReplaceText("<<Probation date>>", input.employeeDetails.JoiningDatePlus3Months.ToString("dd-MM-yyyy", indianCulture));
        //        doc.ReplaceText("<<Permanent date>>", input.employeeDetails.JoiningDatePlus6Months.ToString("dd-MM-yyyy", indianCulture));

        //        // Salary replacements with 0 check
        //        var s = input.salaryValues;
        //        doc.ReplaceText("Basic_A", FormatAmount(s.Basic, indianCulture));
        //        doc.ReplaceText("Basic_M", FormatAmount(Math.Round(s.Basic / 12, 0), indianCulture));
        //        doc.ReplaceText("HRA_A", FormatAmount(s.HRA, indianCulture));
        //        doc.ReplaceText("HRA_M", FormatAmount(Math.Round(s.HRA / 12, 0), indianCulture));
        //        doc.ReplaceText("Statutory_A", FormatAmount(s.StatutoryBonus, indianCulture));
        //        doc.ReplaceText("Statutory_M", FormatAmount(Math.Round(s.StatutoryBonus / 12, 0), indianCulture));
        //        doc.ReplaceText("NPS_A", FormatAmount(s.NPS, indianCulture));
        //        doc.ReplaceText("NPS_M", FormatAmount(Math.Round(s.NPS / 12, 0), indianCulture));
        //        doc.ReplaceText("VPF_A", FormatAmount(s.VPF, indianCulture));
        //        doc.ReplaceText("VPF_M", FormatAmount(Math.Round(s.VPF / 12, 0), indianCulture));
        //        doc.ReplaceText("RFB_A", FormatAmount(s.RFB, indianCulture));
        //        doc.ReplaceText("RFB_M", FormatAmount(Math.Round(s.RFB / 12, 0), indianCulture));
        //        doc.ReplaceText("SA_A", FormatAmount(s.SpecialAllowance, indianCulture));
        //        doc.ReplaceText("SA_M", FormatAmount(Math.Round(s.SpecialAllowance / 12, 0), indianCulture));
        //        doc.ReplaceText("TF_A", FormatAmount(s.TotalFixedComponent, indianCulture));
        //        doc.ReplaceText("TF_M", FormatAmount(Math.Round(s.TotalFixedMonthlyComponent, 0), indianCulture));
        //        doc.ReplaceText("VP_A", FormatAmount(s.VariablePay, indianCulture));
        //        doc.ReplaceText("VP_M", FormatAmount(Math.Round(s.VariablePay / 12, 0), indianCulture));
        //        doc.ReplaceText("NS_A", FormatAmount(s.NetSalary, indianCulture));
        //        doc.ReplaceText("NS_M", FormatAmount(Math.Round(s.NetSalary / 12, 0), indianCulture));
        //        doc.ReplaceText("PFO_A", FormatAmount(s.PFEmployer, indianCulture));
        //        doc.ReplaceText("PFO_M", FormatAmount(Math.Round(s.PFEmployer / 12, 0), indianCulture));
        //        doc.ReplaceText("ESICO_A", FormatAmount(s.ESICEmployer, indianCulture));
        //        doc.ReplaceText("ESICO_M", FormatAmount(Math.Round(s.ESICEmployer / 12, 0), indianCulture));
        //        doc.ReplaceText("G_A", FormatAmount(s.Gratuity, indianCulture));
        //        doc.ReplaceText("G_M", FormatAmount(Math.Round(s.Gratuity / 12, 0), indianCulture));
        //        doc.ReplaceText("IC_A", FormatAmount(s.InsuranceCoverage, indianCulture));
        //        doc.ReplaceText("IC_M", FormatAmount(Math.Round(s.InsuranceCoverage / 12, 0), indianCulture));
        //        doc.ReplaceText("TB_A", FormatAmount(s.TotalAnnualBenefits, indianCulture));
        //        doc.ReplaceText("TB_M", FormatAmount(Math.Round(s.TotalAnnualBenefits / 12, 0), indianCulture));
        //        doc.ReplaceText("TC_A", FormatAmount(s.TotalCompensation, indianCulture));
        //        doc.ReplaceText("TC_M", FormatAmount(Math.Round(s.TotalCompensation / 12, 0), indianCulture));
        //        doc.ReplaceText("PFE_A", FormatAmount(s.PFEmployee, indianCulture));
        //        doc.ReplaceText("PFE_M", FormatAmount(Math.Round(s.PFEmployee / 12, 0), indianCulture));
        //        doc.ReplaceText("ESICE_A", FormatAmount(s.ESICEmployee, indianCulture));
        //        doc.ReplaceText("ESICE_M", FormatAmount(Math.Round(s.ESICEmployee / 12, 0), indianCulture));
        //        doc.ReplaceText("PT_A", FormatAmount(s.ProfessionalTax, indianCulture));
        //        doc.ReplaceText("PT_M", FormatAmount(Math.Round(s.ProfessionalTaxMonthly, 0), indianCulture));

        //        decimal d_m_value = Math.Round((s.PFEmployee / 12) + (s.ESICEmployee / 12) + s.ProfessionalTaxMonthly, 0);
        //        doc.ReplaceText("D_A", FormatAmount(s.TotalDeductions, indianCulture));
        //        doc.ReplaceText("D_M", FormatAmount(d_m_value, indianCulture));

        //        doc.SaveAs(filledDocxPath);

        //        // LibreOffice conversion
        //         var libreOfficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
        //        //var libreOfficePath = @"C:\Libre_Office\program\soffice.exe";
        //        var process = new Process
        //        {
        //            StartInfo = new ProcessStartInfo
        //            {
        //                FileName = libreOfficePath,
        //                Arguments = $"--headless --convert-to pdf \"{filledDocxPath}\" --outdir \"{Path.GetDirectoryName(filledDocxPath)}\"",
        //                RedirectStandardOutput = true,
        //                RedirectStandardError = true,
        //                UseShellExecute = false,
        //                CreateNoWindow = true
        //            }
        //        };

        //        process.Start();
        //        string output = process.StandardOutput.ReadToEnd();
        //        string err = process.StandardError.ReadToEnd();
        //        process.WaitForExit();

        //        if (!System.IO.File.Exists(outputPdfPath))
        //            throw new Exception("PDF conversion failed: " + err);

        //        return System.IO.File.ReadAllBytes(outputPdfPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in GeneratePdfFromWord: " + ex.Message, ex);
        //    }



        //}

        //private string FormatAmount(decimal amount, CultureInfo culture)
        //{

        //    try
        //    {
        //        return amount == 0 ? "-" : amount.ToString("N0", culture);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error in FormatAmount: " + ex.Message, ex);
        //    }
        //}

        [HttpGet("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string email, int docType)
        {

            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email is required.");

                var result = await _repository.CheckEmailAsync(email, docType);

                if(result.Exists)
                ViewBag.FailureAlert = result.Message;
                if(!result.Exists)
                ViewBag.FailureAlert=result.Message;

                return Json(result);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in CheckEmail: " + ex.Message, ex);
            }

        }

    }
}
