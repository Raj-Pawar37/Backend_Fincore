using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public class BudgetCategoryService : IBudgetCategoryService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public BudgetCategoryService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        public async Task<BudgetCategoryReadDTO> AddBudgetCategory(BudgetCategoryWriteDTO dto)
        {
            bool exists = await db.BudgetCategory.AnyAsync(x => x.CategoryCode == dto.CategoryCode);

            if (exists)
            {
                throw new Exception("Category code already exists.");
            }

            var data = mapper.Map<BudgetCategory>(dto);

            //int userId = 1;
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

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

            //db.BudgetCategory.Remove(data);
            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetCategoryReadDTO>> GetAll(PaginationDTO pagination)
        {

            var search = db.BudgetCategory.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.CategoryName.Contains(pagination.Search));

                  
            }

            var data = await search
                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                 .Take(pagination.PageSize)
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

        public async Task<int> GetTotalRecord()
        {
           return await db.BudgetCategory.CountAsync();
        }

        public async Task<bool> UpdateBudgetCategory(int id, BudgetCategoryUpdateDTO dto)
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

            //int userId = 1;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }
    }
}