using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class BudgetCategoryWriteDTO
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CategoryCode { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
