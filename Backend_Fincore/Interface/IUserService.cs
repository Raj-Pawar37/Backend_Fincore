using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IUserService
    {
        Task<List<UserReadDTO>> GetAll();
        Task<UserReadDTO> GetById(int id);

       Task<UserReadDTO> AddUser(UserWriteDTO u);

        Task<bool> UpdateUser(int id,UserWriteDTO u);

        Task<bool > DeleteUser(int id);
    }
}
