using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class CompanyWriteDTO
    {
        [Required]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string CompanyCode { get; set; } = null!;

        public string? GSTNo { get; set; }

        [Required]
        public string PANNo { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } = null!;

        [Required]
        public int CountryId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }
    }
}