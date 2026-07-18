using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Country : BaseEntity
    {
        [Key]
        public int CountryId { get; set; }

        public string CountryName { get; set; } = null!;

        public string CountryCode { get; set; } = null!;

        // Navigation properties
        public ICollection<State> States { get; set; }
            = new List<State>();

        public ICollection<Company> Companies { get; set; }
            = new List<Company>();
    }
}