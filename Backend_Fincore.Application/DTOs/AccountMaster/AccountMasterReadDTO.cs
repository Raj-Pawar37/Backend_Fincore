using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.AccountMaster
{
    public class AccountMasterReadDTO
    {
        public int AccountMasterId { get; set; }

        public string AccountCode { get; set; } = null!;

        public string AccountName { get; set; } = null!;

        public string AccountType { get; set; } = null!;

        public string? Description { get; set; }
   
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }


    }
}
