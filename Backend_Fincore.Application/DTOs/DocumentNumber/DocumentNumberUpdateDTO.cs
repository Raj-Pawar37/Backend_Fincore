using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.DocumentNumber
{
    public class DocumentNumberUpdateDTO
    {
        public int DocumentNumberId { get; set; }
        public string DocumentName { get; set; } 
        public string TableName { get; set; } 
        public string ColumnName { get; set; } 
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public string? Separator { get; set; }
        public int NumberLength { get; set; }
        public string? FyYearFormat { get; set; }
    }
}
