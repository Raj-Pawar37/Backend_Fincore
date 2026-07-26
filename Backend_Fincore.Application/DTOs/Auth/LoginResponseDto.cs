using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public bool Is2FAEnabled { get; set; }
        public bool Requires2FA { get; set; }
        public string Message { get; set; } = null!;
    }
}
