using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            return await db.AccountMaster.CountAsync();
        }
        public async Task<List<AccountMasterReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.AccountMaster.AsQueryable();
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
            var data = await db.AccountMaster.FirstOrDefaultAsync(x =>x.AccountMasterId == id);

            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }

            return mapper.Map<AccountMasterReadDTO>(data);
        }
        public async Task<AccountMasterReadDTO>AddAccountMaster(AccountMasterWriteDTO dto)
        {
            var data = mapper.Map<AccountMaster>(dto);

            data.CreatedBy = currentUser.UserId;

            await db.AccountMaster.AddAsync(data);
            await db.SaveChangesAsync();
            return mapper.Map<AccountMasterReadDTO>(data);
        }
        public async Task UpdateAccountMaster( int id, AccountMasterWriteDTO dto)
        {
            var data = await db.AccountMaster.FindAsync(id);

            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }
            data.ModifiedBy = currentUser.UserId;
            mapper.Map(dto, data);
            await db.SaveChangesAsync();
        }
        public async Task DeleteAccountMaster(int id)
        {
            var data = await db.AccountMaster.FindAsync(id);
            if (data == null)
            {
                throw new Exception("Account Master not found.");
            }

            //db.AccountMaster.Remove(data);
            data.IsActive = 0;//soft delete by vikas 
            await db.SaveChangesAsync();
        }

      
    }
}
