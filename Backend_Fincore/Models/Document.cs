using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Document : BaseEntity
    {
        [Key]
        public int DocumentId { get; set; }

        public int DocumentTypeId { get; set; }

        public int MasterId { get; set; }

        // Employee / Vendor / Customer
        public string MasterType { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public string FileType { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string? Remarks { get; set; }

        // Navigation property
        public DocumentType DocumentType { get; set; } = null!;
    }
}