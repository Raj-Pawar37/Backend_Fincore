using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;


namespace Backend_Fincore.Application.Interface
{
    public interface ICompanyService
    {

        Task<List<CompanyReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalCompanyRecords(string? search);

        Task<CompanyReadDTO> GetById(int id);
        
        Task<CompanyReadDTO> AddCompany(CompanyWriteDTO c);

        Task<bool> UpdateCompany(int id,CompanyWriteDTO c);

        Task<bool> DeleteCompany(int id);
    }
}
