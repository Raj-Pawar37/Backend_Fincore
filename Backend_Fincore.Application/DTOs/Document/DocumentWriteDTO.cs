
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentWriteDTO
    {
        [Required]
        public int DocumentTypeId { get; set; }

        public string? Remarks { get; set; }

        //[Required]
        //public bool IsActive { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
