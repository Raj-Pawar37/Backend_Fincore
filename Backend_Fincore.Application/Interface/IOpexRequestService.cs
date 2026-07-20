using Backend_Fincore.DTOs;


namespace Backend_Fincore.Interface
{
    public interface IOpexRequestService
    {
        Task<List<OpexRequestDto>> GetAllRequests();
        Task Create(OpexRequestDto opd);

        Task<OpexRequestDto> GetById(int id);

        Task Update(OpexRequestDto dto);

        Task Delete(int id);
    }
}
