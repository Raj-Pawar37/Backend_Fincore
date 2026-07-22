using AutoMapper;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public ExpenseClaimService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<ExpenseClaimReadDTO>> GetAll()
        {
            var data = await db.ExpenseClaim
                .ToListAsync();

            return mapper.Map<List<ExpenseClaimReadDTO>>(data);
        }

        public async Task<ExpenseClaimReadDTO?> GetById(int id)
        {
            var data = await db.ExpenseClaim
                .FindAsync(id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<ExpenseClaimReadDTO>(data);
        }

        public async Task<ExpenseClaimReadDTO> Create(
            ExpenseClaimWriteDTO dto)
        {
            var data = mapper.Map<ExpenseClaim>(dto);

            data.Status = "Pending";
            data.ApprovedBy = null;
            data.ApprovedDate = null;

            await db.ExpenseClaim.AddAsync(data);
            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(data);
        }

        public async Task<ExpenseClaimReadDTO?> Update(
            int id,
            ExpenseClaimWriteDTO dto)
        {
            var data = await db.ExpenseClaim
                .FindAsync(id);

            if (data == null)
            {
                return null;
            }

            mapper.Map(dto, data);

            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(data);
        }

        public async Task<bool> Delete(int id)
        {
            var data = await db.ExpenseClaim
                .FindAsync(id);

            if (data == null)
            {
                return false;
            }

            db.ExpenseClaim.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
