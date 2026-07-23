using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Payment
{
    public class SearchByMasterTypeReqDTO
    {
        public string MasterType { get; set; } = string.Empty;

        public string? SearchText { get; set; }
    }
}
