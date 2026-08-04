using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Approval;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Backend_Fincore.Infrastucture.Service
{
    public class ApprovalService:IApprovalService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public ApprovalService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        public async Task<ApprovalReadDTO> AddApproval(ApprovalWriteDTO dto)
        {
            if (dto.MinAmount < 0 || dto.MaxAmount < 0)
            {
                throw new Exception("Amount cannot be negative.");
            }
            if (dto.MinAmount >= dto.MaxAmount)
            {
                throw new Exception("Minimum Amount cannot be greater than Maximum Amount.");
            }
            //if (dto.MinAmount < dto.MaxAmount)
            //{
            //    throw new Exception("Maximun Amount cannot be greater than Minimum Amount.");
            //}
            if (dto.ApprovalLevel <= 0)
            {
                throw new Exception("Approval Level must be greater than zero.");
            }
            bool roleExists = await db.Role.AnyAsync(x =>x.RoleId == dto.RoleId && x.IsActive == 1);

            if (!roleExists)
            {
                throw new Exception("Invalid Role.");
            }
            // Validation to prevent overlapping approval ranges.
            bool isRangeExists = await db.Approval.AnyAsync(x => x.IsActive == 1&&
                                                     dto.MinAmount <= x.MaxAmount &&
                                                     dto.MaxAmount >= x.MinAmount);

            if (isRangeExists)
            {
                throw new Exception("Approval amount range already exists.");
            }
            var data = mapper.Map<Approval>(dto);
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;//testing
            data.CreatedAt = DateTime.Now;
            await db.Approval.AddAsync(data);
            await db.SaveChangesAsync();

            var res = await db.Approval.Include(x => x.Role)
                                        .Where(x => x.IsActive == 1)
                                        .FirstOrDefaultAsync(x => x.ApprovalId == data.ApprovalId);
            return mapper.Map<ApprovalReadDTO>(res);
        }

        public async Task DeleteApproval(int id)
        {
            var data = await db.Approval
                .FirstOrDefaultAsync(x => x.ApprovalId == id && x.IsActive == 1);

            if (data is null) {
                throw new Exception("Approval ID is not Found");
            }
            //db.Approval.Remove(data);
            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task<List<ApprovalReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search =  db.Approval.Where(x => x.IsActive == 1).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.ApprovalId.ToString().Contains(pagination.Search) ||

                    x.Role.RoleName.Contains(pagination.Search));
            }
            var data = await search.Include(x => x.Role)
                                        .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                        .Take(pagination.PageSize)
                                       .ToListAsync();
            var res = mapper.Map<List<ApprovalReadDTO>>(data);
            return res;
        }
        public async Task<int> GetTotalApprovalRecord()
        {
            var data = await db.Approval.Where(x => x.IsActive == 1).CountAsync();
            return data;
        }
        public async Task<ApprovalReadDTO> GetById(int id)
        {
            var data = await db.Approval.Include(x => x.Role)
                                        .Where(x => x.IsActive == 1)
                                        .FirstOrDefaultAsync(x=>x.ApprovalId==id);
            if (data is null) {
                throw new Exception("Approval not found ");
            }
            return mapper.Map<ApprovalReadDTO>(data);
        }

    

        public async Task UpdateApproval(int id, ApprovalUpdateDTO dto)
        {
           
            // Validation to prevent overlapping approval ranges.
            bool isRangeExists = await db.Approval.AnyAsync(x =>
                           x.IsActive == 1 &&
                           x.ApprovalId != id &&
                           dto.MinAmount <= x.MaxAmount &&
                           dto.MaxAmount >= x.MinAmount);

            if (isRangeExists)
            {
                throw new Exception(
                    "Approval amount range already exists.");
            }
            if (dto.MinAmount >= dto.MaxAmount)
            {
                throw new Exception(
                    "Minimum Amount cannot be greater than Maximum Amount.");
            }
            if (dto.ApprovalLevel <= 0)
            {
                throw new Exception(
                    "Approval Level must be greater than zero.");
            }
            bool roleExists = await db.Role.AnyAsync(x => x.RoleId == dto.RoleId && x.IsActive == 1);
            if (!roleExists)
            {
                throw new Exception("Invalid Role.");
            }
            var data = await db.Approval.FirstOrDefaultAsync(x =>x.ApprovalId == id &&x.IsActive == 1);
            if (data is null) {
                throw new Exception("Approval ID not found");
            }
         
            mapper.Map(dto, data);  
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

        }
    }
}
