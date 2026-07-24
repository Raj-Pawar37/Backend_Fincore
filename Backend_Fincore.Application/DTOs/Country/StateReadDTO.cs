using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Country
{
    public class StateReadDTO
    {
        public int StateId { get; set; }

        public string StateName { get; set; } = null!;

        public string StateCode { get; set; } = null!;

        public int CountryId { get; set; }
    }
}
