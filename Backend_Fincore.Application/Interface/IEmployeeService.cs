using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface IEmployeeService
    {
        Task<List<EmployeeReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalEmployeeRecords(string? search);

        Task<EmployeeReadDTO> GetById(int id);

        Task<EmployeeReadDTO> AddEmp(EmployeeWriteDTO e);

        Task<bool> update(int id, EmployeeWriteDTO e);

        Task<bool> delete(int id);


    }
}
