using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class City : BaseEntity
    {
        [Key]
        public int CityId { get; set; }

        public int StateId { get; set; }

        public string CityName { get; set; } = null!;

        // Navigation properties
        public State State { get; set; } = null!;

        public ICollection<Company> Companies { get; set; }
            = new List<Company>();
    }
}