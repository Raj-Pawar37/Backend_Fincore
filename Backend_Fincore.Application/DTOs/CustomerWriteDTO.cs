using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public  class CustomerWriteDTO
    {
        public int CustomerId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public string CustomerName { get; set; } = null!;
        [Required]
        public string CustomerCode { get; set; } = null!;
        [Required]
        [StringLength(20)]
        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }
    }
}
