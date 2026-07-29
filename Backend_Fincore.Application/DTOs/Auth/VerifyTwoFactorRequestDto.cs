using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Auth
{
    public class VerifyTwoFactorRequestDto
    {
        [Required]
        public int UserId { get; set; }


        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must contain exactly 6 digits.")]
        public string Otp { get; set; } = null!;
    }
}
