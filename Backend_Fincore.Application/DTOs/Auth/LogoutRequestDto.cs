using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Auth
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}
