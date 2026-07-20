using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class State : BaseEntity
    {
        [Key]
        public int StateId { get; set; }

        public int CountryId { get; set; }

        public string StateCode { get; set; } = null!;

        public string StateName { get; set; } = null!;

        // Navigation properties
        public Country Country { get; set; } = null!;

        public ICollection<City> Cities { get; set; }
            = new List<City>();

        public ICollection<Company> Companies { get; set; }
            = new List<Company>();
    }
}