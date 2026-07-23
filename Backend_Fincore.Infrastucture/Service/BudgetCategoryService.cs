using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class BudgetCategoryService : IBudgetCategoryService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public BudgetCategoryService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<BudgetCategoryReadDTO> AddBudgetCategory(BudgetCategoryWriteDTO dto)
        {
            bool exists = await db.BudgetCategory.AnyAsync(x => x.CategoryCode == dto.CategoryCode);

            if (exists)
            {
                throw new Exception("Category code already exists.");
            }

            var data = mapper.Map<BudgetCategory>(dto);

            await db.BudgetCategory.AddAsync(data);
            await db.SaveChangesAsync();

            return mapper.Map<BudgetCategoryReadDTO>(data);
        }

        public async Task<bool> DeleteBudgetCategory(int id)
        {
            var data = await db.BudgetCategory
                .FirstOrDefaultAsync(x => x.BudgetCategoryId == id);

            if (data == null)
            {
                return false;
            }

            bool isUsed = await db.BudgetLine
                .AnyAsync(x => x.BudgetCategoryId == id);

            if (isUsed)
            {
                throw new Exception(
                    "Budget category cannot be deleted because it is used in budget lines.");
            }

            db.BudgetCategory.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetCategoryReadDTO>> GetAll()
        {
            var data = await db.BudgetCategory
                .ToListAsync();

            return mapper.Map<List<BudgetCategoryReadDTO>>(data);
        }

        public async Task<BudgetCategoryReadDTO?> GetById(int id)
        {
            var data = await db.BudgetCategory
                .FirstOrDefaultAsync(x => x.BudgetCategoryId == id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<BudgetCategoryReadDTO>(data);
        }

        public async Task<bool> UpdateBudgetCategory(int id,BudgetCategoryWriteDTO dto)
        {
            var data = await db.BudgetCategory
                .FirstOrDefaultAsync(x => x.BudgetCategoryId == id);

            if (data == null)
            {
                return false;
            }

            bool duplicateCode = await db.BudgetCategory
                .AnyAsync(x =>
                    x.CategoryCode == dto.CategoryCode &&
                    x.BudgetCategoryId != id);

            if (duplicateCode)
            {
                throw new Exception("Category code already exists.");
            }

            mapper.Map(dto, data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}