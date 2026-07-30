//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Backend_Fincore.Application.DTOs
//{
//    internal class BudgetCategoryDto
//    {
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs
{
    public class CreateBudgetCategoryDto
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

    public class UpdateBudgetCategoryDto
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

    public class BudgetCategoryResponseDto
    {
        public int BudgetCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryCode { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}