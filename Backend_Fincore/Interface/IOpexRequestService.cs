using Backend_Fincore.DTOs;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface IOpexRequestService
    {
        Task<List<OpexRequest>> GetAllRequests();
        Task Create(OpexRequestDto opd);

        Task<OpexRequest> GetById(int id);

        Task Update(OpexRequestDto dto);

        Task Delete(int id);
    }
}
