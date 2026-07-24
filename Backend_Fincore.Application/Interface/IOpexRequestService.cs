using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.OpexRequest;

namespace Backend_Fincore.Interface
{
    public interface IOpexRequestService
    {
        Task<List<OpexRequestReadDTO>> GetAll(int userId,PaginationDTO pagination);

        Task<OpexRequestReadDTO?> GetById(int id);

        Task<OpexRequestReadDTO> Create(OpexRequestWriteDTO dto);

        Task<OpexRequestReadDTO> Update(int opexRequestId,OpexRequestWriteDTO dto);

        Task<bool> Delete(int id);

        Task<OpexRequestReadDTO> Verify(int opexRequestId,int approvedBy,OpexRequestVerifyDTO dto);

        Task<List<OpexRequestReadDTO>> SearchOpex(OpexSearchDTO dto);

        Task<int> GetOpexRequestCount(int userId , PaginationDTO pagination);


    }
}
