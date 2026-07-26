using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IApprovalService
    {
        Task<List<ApprovalReadDTO>> GetAll(PaginationDTO pagination);

        Task<int> GetTotalApprovalRecord();
        Task<ApprovalReadDTO> GetById(int id);
        Task<ApprovalReadDTO> AddApproval( ApprovalWriteDTO dto);
        Task UpdateApproval( int id,ApprovalWriteDTO dto);
        Task DeleteApproval(int id);  
    }
}
