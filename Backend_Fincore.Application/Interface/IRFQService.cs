using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQService
    {
        Task<ApiResponse<List<RFQResponseDto>>> GetAll();
        Task<ApiResponse<RFQResponseDto>> GetAllById(int id);
        Task<ApiResponse<RFQResponseDto>> Create(RFQCreateDto dto);



    }
}
