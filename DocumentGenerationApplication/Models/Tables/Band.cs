using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.Tables
{
    public class Band
    {
        [Key]
        public int Id { get; set; }
        public string BandName { get; set; } = string.Empty;
        public decimal CoverAmount { get; set; }
        public decimal? PremiumAmount { get; set; } // Nullable because some rows have no premium
    }
}
