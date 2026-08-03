using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.DTOs.GRN;

namespace Backend_Fincore.Application.Interface
{
    public interface IGRNService
    {
        Task<List<GRNDTO>> GetAllGrns(PaginationDTO pagination);

        Task<int> GetAllGRNCount();

        Task<GRNDTO> GetGrnById(int id);

        Task AddGrn(GRNCreate grn);

        Task UpdateGRN(GRNCUDTO grn, int id);

        Task DeletegrnById(int id);

        Task UpdateGRNStatus(int id, GrnStatusDTO dto);

        Task<List<GRNDTO>> FetchDraftGRN();
    }
}
