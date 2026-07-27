using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.AccountMaster
{
    public  class AccountMasterUpdateDTO
    {
        [Required]
        [StringLength(50)]
        [RegularExpression(
          @"^[a-zA-Z0-9_-]+$", ErrorMessage = "Account Code can contain only letters, numbers, hyphen (-) and underscore (_).")]
        public string AccountCode { get; set; } = null!;


        [Required]
        [StringLength(100)]
        [RegularExpression(
            @"^[a-zA-Z0-9\s&.,'()/\-]+$", ErrorMessage = "Account Name contains invalid characters.")]
        public string AccountName { get; set; } = null!;


        [Required]
        [StringLength(50)]
        [RegularExpression(
            @"^[a-zA-Z ]+$", ErrorMessage = "Account Type can contain only letters and spaces.")]
        public string AccountType { get; set; } = null!;

        public byte IsActive { get; set; } 



        [RegularExpression(
    @"^[a-zA-Z0-9\s&.,'()/:\-_]+$", ErrorMessage = "Description contains invalid characters. Emojis and unsupported special characters are not allowed.")]
        public string? Description { get; set; }

    }
}
