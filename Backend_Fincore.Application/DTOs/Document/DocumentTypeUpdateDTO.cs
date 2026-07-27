using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Document
{
    public class DocumentTypeUpdateDTO
    {
        public string DocumentTypeName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
