using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentGenerationApplication.Models.Tables
{
    public class Designation
    {
        [Key]
        public int Id { get; set; }
        public int BandId {  get; set; }
        public int GradeId {  get; set; }
        public int DepartmentId {  get; set; }

        [ForeignKey(nameof(BandId))]
        public Band? Band { get; set; }

        [ForeignKey(nameof(GradeId))]
        public Grade? Grade { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
        public string DesignationName { get; set; } = string.Empty;

    }
}
