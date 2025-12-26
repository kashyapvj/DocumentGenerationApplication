using Microsoft.EntityFrameworkCore;
using DocumentGenerationApplication.Models.Tables;
using DocumentGenerationApplication.Models.ParentModel;
using DocumentGenerationApplication.Models.UI_Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DocumentGenerationApplication.Models.UserModel;

namespace DocumentGenerationApplication.Data
{
    public class AppDbContext: IdentityDbContext <ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<FillPdfTemplateInput>().HasNoKey();
        //    base.OnModelCreating(modelBuilder);
        //}

        // Define DbSet for each of your models to map to tables
        public DbSet<Band> Bands { get; set; }
        public DbSet<Department> Departments { get; set; } 
        public DbSet<Designation> Designations { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<ReimbursableBenefit> ReimbursableBenefits { get; set; }
        public DbSet<ReimbursementBenefitsDetails> ReimbursementBenefitsDetails { get; set; }
        public DbSet<SalaryBreakdown> SalaryBreakdowns { get; set; }

    }
}
