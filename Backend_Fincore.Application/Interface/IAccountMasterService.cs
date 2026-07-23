using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IAccountMasterService
    {
        Task<List<AccountMasterReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetAccountMasterCount();

        Task<AccountMasterReadDTO> GetById(int id);

        Task<AccountMasterReadDTO> AddAccountMaster(AccountMasterWriteDTO dto);

        Task UpdateAccountMaster(int id, AccountMasterWriteDTO dto);

        Task DeleteAccountMaster(int id);
    }
}
