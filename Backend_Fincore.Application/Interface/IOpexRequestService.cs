using Backend_Fincore.Application.DTOs.OpexRequest;

namespace Backend_Fincore.Interface
{
    public interface IOpexRequestService
    {
        Task<List<OpexRequestReadDTO>> GetAll();

        Task<OpexRequestReadDTO?> GetById(int id);

        Task<OpexRequestReadDTO> Create(OpexRequestWriteDTO dto);

        Task<OpexRequestReadDTO> Update(int id, OpexRequestWriteDTO dto);

        Task<bool> Delete(int id);

    }
}
