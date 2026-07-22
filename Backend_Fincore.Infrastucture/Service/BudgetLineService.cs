using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public  class BudgetLineService : IBudgetLineService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public BudgetLineService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<BudgetLineReadDTO> AddBudgetLine(
            BudgetLineWriteDTO dto)
        {
            var budget = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == dto.BudgetId);

            if (budget == null)
            {
                throw new Exception("Budget not found.");
            }

            bool categoryExists = await db.BudgetCategory
                .AnyAsync(x =>
                    x.BudgetCategoryId == dto.BudgetCategoryId);

            if (!categoryExists)
            {
                throw new Exception("Budget category not found.");
            }

            if (dto.AllocatedAmount <= 0)
            {
                throw new Exception(
                    "Allocated amount must be greater than zero.");
            }

            bool duplicateLine = await db.BudgetLine
                .AnyAsync(x =>
                    x.BudgetId == dto.BudgetId &&
                    x.BudgetCategoryId == dto.BudgetCategoryId &&
                    x.CostCenter == dto.CostCenter);

            if (duplicateLine)
            {
                throw new Exception(
                    "Budget line already exists for this category and cost center.");
            }

            decimal alreadyAllocated = await db.BudgetLine
                .Where(x => x.BudgetId == dto.BudgetId)
                .SumAsync(x => x.AllocatedAmount);

            decimal newTotalAllocation =
                alreadyAllocated + dto.AllocatedAmount;

            if (newTotalAllocation > budget.TotalBudget)
            {
                throw new Exception(
                    "Allocated amount exceeds the total budget.");
            }

            var data = mapper.Map<BudgetLine>(dto);

            await db.BudgetLine.AddAsync(data);
            await db.SaveChangesAsync();

            var savedData = await db.BudgetLine
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Company)
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .FirstAsync(x =>
                    x.BudgetLineId == data.BudgetLineId);

            return mapper.Map<BudgetLineReadDTO>(savedData);
        }

        public async Task<bool> DeleteBudgetLine(int id)
        {
            var data = await db.BudgetLine
                .FirstOrDefaultAsync(x => x.BudgetLineId == id);

            if (data == null)
            {
                return false;
            }

            bool usedInCapex = await db.CapexRequest
                .AnyAsync(x => x.BudgetLineId == id);

            bool usedInOpex = await db.OpexRequest
                .AnyAsync(x => x.BudgetLineId == id);

            if (usedInCapex || usedInOpex)
            {
                throw new Exception(
                    "Budget line cannot be deleted because it is used in a CAPEX or OPEX request.");
            }

            db.BudgetLine.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetLineReadDTO>> GetAll()
        {
            var data = await db.BudgetLine
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Company)
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .ToListAsync();

            return mapper.Map<List<BudgetLineReadDTO>>(data);
        }

        public async Task<BudgetLineReadDTO?> GetById(int id)
        {
            var data = await db.BudgetLine
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Company)
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .FirstOrDefaultAsync(x => x.BudgetLineId == id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<BudgetLineReadDTO>(data);
        }

        public async Task<bool> UpdateBudgetLine(
    int id,
    BudgetLineWriteDTO dto)
        {
            var data = await db.BudgetLine
                .FirstOrDefaultAsync(x => x.BudgetLineId == id);

            if (data == null)
            {
                return false;
            }

            var budget = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == dto.BudgetId);

            if (budget == null)
            {
                throw new Exception("Budget not found.");
            }

            bool categoryExists = await db.BudgetCategory
                .AnyAsync(x =>
                    x.BudgetCategoryId == dto.BudgetCategoryId);

            if (!categoryExists)
            {
                throw new Exception("Budget category not found.");
            }

            if (dto.AllocatedAmount <= 0)
            {
                throw new Exception(
                    "Allocated amount must be greater than zero.");
            }

            bool duplicateLine = await db.BudgetLine
                .AnyAsync(x =>
                    x.BudgetId == dto.BudgetId &&
                    x.BudgetCategoryId == dto.BudgetCategoryId &&
                    x.CostCenter == dto.CostCenter &&
                    x.BudgetLineId != id);

            if (duplicateLine)
            {
                throw new Exception(
                    "Budget line already exists for this category and cost center.");
            }

            decimal otherAllocatedAmount = await db.BudgetLine
                .Where(x =>
                    x.BudgetId == dto.BudgetId &&
                    x.BudgetLineId != id)
                .SumAsync(x => x.AllocatedAmount);

            decimal newTotalAllocation =
                otherAllocatedAmount + dto.AllocatedAmount;

            if (newTotalAllocation > budget.TotalBudget)
            {
                throw new Exception(
                    "Allocated amount exceeds the total budget.");
            }

            mapper.Map(dto, data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}


