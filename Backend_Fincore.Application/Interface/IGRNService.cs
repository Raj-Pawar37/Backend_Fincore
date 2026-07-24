using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.DTOs.GRN;

namespace Backend_Fincore.Interface
{
    public interface IGRNService
    {
        Task<List<GRNDTO>> GetAllGrns(string masterType, int masterId, GrnStatusDTO dto);

        Task<GRNDTO> GetGrnById(int id);

        Task AddGrn(GRNCUDTO grn);

        Task UpdateGRN(GRNCUDTO grn, int id);

        Task DeletegrnById(int id);

        Task UpdateGRNStatus(int id, GrnStatusDTO dto);
    }
}
