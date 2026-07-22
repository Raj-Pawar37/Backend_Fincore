
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentWriteDTO
    {
        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public int MasterId { get; set; }

        [Required]
        public string MasterType { get; set; } = null!;

        //[Required]
        //public string FileName { get; set; } = null!;

        public string? Remarks { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
