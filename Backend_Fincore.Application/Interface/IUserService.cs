using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IUserService
    {
        Task<List<UserReadDTO>> GetAll(PaginationDTO pagination);

        Task<int> GetTotalUserRecords(string? search);

        Task<UserReadDTO?> GetById(int id);

        
        Task<UserReadDTO> AddUser(UserCreateDTO u);

        
        Task<bool> UpdateUser(int id, UserUpdateDTO u);

       
        Task<bool> DeleteUser(int id);
    }
}