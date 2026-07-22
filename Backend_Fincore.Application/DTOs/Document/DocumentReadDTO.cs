using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentReadDTO
    {
        public int DocumentId { get; set; }

        public string DocumentTypeName { get; set; } = null!;

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public string FileType { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }
    }
}
