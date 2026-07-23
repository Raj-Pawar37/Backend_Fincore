using AutoMapper;
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

        public ApprovalService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ApprovalReadDTO> AddApproval(ApprovalWriteDTO dto)
        {
            if (dto.MinAmount > dto.MaxAmount)
            {
                throw new Exception(
                    "Minimum Amount cannot be greater than Maximum Amount.");
            }
            if (dto.ApprovalLevel <= 0)
            {
                throw new Exception(
                    "Approval Level must be greater than zero.");
            }

           // Validation to prevent overlapping approval ranges.
            bool isRangeExists = await db.Approval.AnyAsync(x =>
                                                     dto.MinAmount <= x.MaxAmount &&
                                                     dto.MaxAmount >= x.MinAmount);

            if (isRangeExists)
            {
                throw new Exception(
                    "Approval amount range already exists.");
            }
            var data = mapper.Map<Approval>(dto);
            data.CreatedBy = 2;//testing
            await db.Approval.AddAsync(data);
            await db.SaveChangesAsync();

            var res = await db.Approval.Include(x => x.Role)
                                        .Where(x => x.IsActive == 1)
                                        .FirstOrDefaultAsync(x => x.ApprovalId == data.ApprovalId);
            return mapper.Map<ApprovalReadDTO>(res);
        }

        public async Task DeleteApproval(int id)
        {
            var data = await db.Approval.FindAsync(id);
            if (data is null) {
                throw new Exception("Approval ID is not Found");
            }
            //db.Approval.Remove(data);
            data.IsActive = 0;
            data.ModifiedBy = 2;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }

        public async Task<List<ApprovalReadDTO>> GetAll()
        {
            var data = await db.Approval.Include(x => x.Role).Where(x => x.IsActive == 1).ToListAsync();
            var res = mapper.Map<List<ApprovalReadDTO>>(data);
            return res;
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

        public async Task UpdateApproval(int id, ApprovalWriteDTO dto)
        {
            // Validation to prevent overlapping approval ranges.
            bool isRangeExists = await db.Approval.AnyAsync(x =>
                                                     dto.MinAmount <= x.MaxAmount &&
                                                     dto.MaxAmount >= x.MinAmount);

            if (isRangeExists)
            {
                throw new Exception(
                    "Approval amount range already exists.");
            }
            if (dto.MinAmount > dto.MaxAmount)
            {
                throw new Exception(
                    "Minimum Amount cannot be greater than Maximum Amount.");
            }
            if (dto.ApprovalLevel <= 0)
            {
                throw new Exception(
                    "Approval Level must be greater than zero.");
            }
            var data = await db.Approval.FindAsync(id);
            if (data is null) {
                throw new Exception("Approval ID not found");
            }
         
            mapper.Map(dto, data);
            data.ModifiedBy = 2;//userid - empid , Admin ->who is updating 
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

        }
    }
}
