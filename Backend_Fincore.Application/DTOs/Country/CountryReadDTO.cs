using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Country
{
    public class CountryReadDTO
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; } = null!;

        public string CountryCode { get; set; } = null!;
    }
}
