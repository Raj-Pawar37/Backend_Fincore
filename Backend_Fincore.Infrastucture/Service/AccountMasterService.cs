using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
{
    public class AccountMasterService: IAccountMasterService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public AccountMasterService( AppDbContext db, IMapper mapper,ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<int> GetAccountMasterCount()
        {
            return await db.AccountMaster.Where(x=>x.IsActive==1).CountAsync();
        }
        public async Task<List<AccountMasterReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.AccountMaster.Where(x=>x.IsActive==1).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.AccountName.Contains(pagination.Search) ||

                    x.AccountCode.Contains(pagination.Search) ||

                    x.AccountType.Contains(pagination.Search));
            }
            var data = await search
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();
            return mapper.Map<List<AccountMasterReadDTO>>(data);
        }
        public async Task<AccountMasterReadDTO> GetById(int id){
            var data = await db.AccountMaster
                .FirstOrDefaultAsync(x => x.AccountMasterId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }

            return mapper.Map<AccountMasterReadDTO>(data);
        }
        public async Task<AccountMasterReadDTO>AddAccountMaster(AccountMasterWriteDTO dto)
        {
            var data = mapper.Map<AccountMaster>(dto);
            bool accountCodeExists = await db.AccountMaster.AnyAsync(x =>x.AccountCode == dto.AccountCode);

            if (accountCodeExists)
            {
                throw new Exception("Account Code already exists.");
            }
            data.CreatedBy = currentUser.UserId;
            data.CreatedBy = currentUser.UserId;
            data.IsActive = 1;

            await db.AccountMaster.AddAsync(data);
            await db.SaveChangesAsync();
            return mapper.Map<AccountMasterReadDTO>(data);
        }
        public async Task UpdateAccountMaster( int id, AccountMasterUpdateDTO dto)
        {
            var data = await db.AccountMaster.FirstOrDefaultAsync(x => x.AccountMasterId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }
            bool accountCodeExists = await db.AccountMaster.AnyAsync(x =>
                          x.AccountCode == dto.AccountCode && x.AccountMasterId != id && x.IsActive == 1);

            if (accountCodeExists)
            {
                throw new Exception("Account Code already exists.");
            }
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            mapper.Map(dto, data);
            await db.SaveChangesAsync();
        }
        public async Task DeleteAccountMaster(int id)
        {
            var data = await db.AccountMaster
                .FirstOrDefaultAsync(x => x.AccountMasterId == id && x.IsActive == 1);
            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }

            data.IsActive = 0;//soft delete by vikas 
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }

      
    }
}
