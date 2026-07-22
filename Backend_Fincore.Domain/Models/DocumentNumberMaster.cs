using Backend_Fincore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Domain.Models
{
    public class DocumentNumberMaster : BaseEntity
    {
        [Key]
        public int DocumentNumberId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ColumnName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Prefix { get; set; }

        [MaxLength(20)]
        public string? Suffix { get; set; }

        [MaxLength(5)]
        public string? Separator { get; set; }

        [Range(1, 20)]
        public int NumberLength { get; set; }

        [MaxLength(10)]
        public string? FyYearFormat { get; set; }

    }
}
