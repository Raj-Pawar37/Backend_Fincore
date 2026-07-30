using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentUpdateDTO
    {
        [Required]
        public int DocumentTypeId { get; set; }

        public string? Remarks { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
