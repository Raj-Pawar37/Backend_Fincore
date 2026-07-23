using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.AccountMaster
{
    public class AccountMasterWriteDTO
    {
        [Required]
        public string AccountCode { get; set; } = null!;

        [Required]
        public string AccountName { get; set; } = null!;

        [Required]
        public string AccountType { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
