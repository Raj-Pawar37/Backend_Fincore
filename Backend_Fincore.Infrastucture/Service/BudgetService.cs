using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class BudgetService : IBudgetService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public BudgetService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        //public async Task<BudgetReadDTO> AddBudget(BudgetWriteDTO bdg)
        //{
        //    var addBudget = mapper.Map<Budget>(bdg);
        //    await db.Budget.AddAsync(addBudget);
        //    await db.SaveChangesAsync();
        //    return mapper.Map<BudgetReadDTO>(addBudget);

        //}
        public async Task<BudgetReadDTO> AddBudget(BudgetWriteDTO dto)
        {
            bool companyExists = await db.Company
                .AnyAsync(x => x.CompanyId == dto.CompanyId);

            if (!companyExists)
            {
                throw new Exception("Company not found.");
            }

            bool departmentExists = await db.Department
                .AnyAsync(x =>
                    x.DepartmentId == dto.DepartmentId &&
                    x.CompanyId == dto.CompanyId);

            if (!departmentExists)
            {
                throw new Exception(
                    "Department not found or does not belong to the selected company.");
            }

            bool budgetExists = await db.Budget
                .AnyAsync(x =>
                    x.CompanyId == dto.CompanyId &&
                    x.DepartmentId == dto.DepartmentId &&
                    x.FinancialYear == dto.FinancialYear);

            if (budgetExists)
            {
                throw new Exception(
                    "Budget already exists for this company, department and financial year.");
            }

            var data = mapper.Map<Budget>(dto);

            await db.Budget.AddAsync(data);
            await db.SaveChangesAsync();

            var savedBudget = await db.Budget
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .FirstAsync(x => x.BudgetId == data.BudgetId);

            return mapper.Map<BudgetReadDTO>(savedBudget);
        }
        public async Task<bool> DeleteBudget(int id)
        {
            var data = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == id);

            if (data == null)
            {
                return false;
            }

            bool hasBudgetLines = await db.BudgetLine
                .AnyAsync(x => x.BudgetId == id);

            if (hasBudgetLines)
            {
                throw new Exception(
                    "Budget cannot be deleted because it contains budget lines.");
            }

            db.Budget.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetReadDTO>> GetAll()
        {
            var data = await db.Budget
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .ToListAsync();

            return mapper.Map<List<BudgetReadDTO>>(data);
        }

        public async Task<BudgetReadDTO?> GetById(int id)
        {
            var data = await db.Budget
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x => x.BudgetId == id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<BudgetReadDTO>(data);
        }

        public async Task<bool> UpdateBudget(int id, BudgetWriteDTO dto)
        {
            var data = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == id);

            if (data == null)
            {
                return false;
            }

            bool companyExists = await db.Company
                .AnyAsync(x => x.CompanyId == dto.CompanyId);

            if (!companyExists)
            {
                throw new Exception("Company not found.");
            }

            bool departmentExists = await db.Department
                .AnyAsync(x =>
                    x.DepartmentId == dto.DepartmentId &&
                    x.CompanyId == dto.CompanyId);

            if (!departmentExists)
            {
                throw new Exception(
                    "Department not found or does not belong to the selected company.");
            }

            bool duplicateBudget = await db.Budget
                .AnyAsync(x =>
                    x.CompanyId == dto.CompanyId &&
                    x.DepartmentId == dto.DepartmentId &&
                    x.FinancialYear == dto.FinancialYear &&
                    x.BudgetId != id);

            if (duplicateBudget)
            {
                throw new Exception(
                    "Budget already exists for this company, department and financial year.");
            }

            mapper.Map(dto, data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}