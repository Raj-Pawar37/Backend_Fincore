using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Country
{
    public class CityReadDTO
    {
        public int CityId { get; set; }

        public string CityName { get; set; } = null!;

        public int StateId { get; set; }
    }
}
