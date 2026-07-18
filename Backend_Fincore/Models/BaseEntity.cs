using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public abstract class BaseEntity
    {
        [Required]
        public byte IsActive { get; set; } = 1;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
