using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Auth
{
    public class SetupTwoFactorResponseDto
    {
        public int UserId { get; set; }
        public string QrCodeBase64 { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
