using Backend_Fincore.DTOs;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface ICompanyService
    {

        Task<List<CompanyReadDTO>> GetAll();

        Task<CompanyReadDTO> GetById(int id);
        
        Task<CompanyReadDTO> AddCompany(CompanyWriteDTO c);

        Task<bool> UpdateCompany(int id,CompanyWriteDTO c);

        Task<bool> DeleteCompany(int id);
    }
}
