using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IEmployeeService
    {
        Task<List<EmployeeReadDTO>> GetAll();

        Task<EmployeeReadDTO> GetById(int id);

        Task<EmployeeReadDTO> AddEmp(EmployeeWriteDTO e);

        Task<bool> update(int id, EmployeeWriteDTO e);

        Task<bool> delete(int id);


    }
}
