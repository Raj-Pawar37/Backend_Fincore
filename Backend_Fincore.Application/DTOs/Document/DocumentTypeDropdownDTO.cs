using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentTypeDropdownDTO
    {
        public int DocumentTypeId { get; set; }

        public string DocumentTypeName { get; set; } = null!;
    }
}
