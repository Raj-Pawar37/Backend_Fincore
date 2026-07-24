using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Country
{
    public class StateFilterDTO : PaginationDTO
    {
        public int CountryId { get; set; }

    }
}
