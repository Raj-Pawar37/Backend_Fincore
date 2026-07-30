using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public class BudgetService : IBudgetService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        public BudgetService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        public async Task<List<BudgetDropdownDTO>> GetBudgetDropdown(string? searchText)
        {
            var data = await db.Budget
                .Include(x => x.Company)
                .Where(x =>
                    x.IsActive == 1 &&
                     x.Company.IsActive == 1 &&
                    x.ApprovedBy != null &&
                    x.ApprovedDate != null && 
                    (string.IsNullOrEmpty(searchText) ||
                    x.Company.CompanyName.Contains(searchText))) 
                    .OrderBy(x => x.Company.CompanyName)
                    .Select(x => new BudgetDropdownDTO
                    {
                        BudgetId = x.BudgetId,
                        CompanyName = x.Company.CompanyName
                    })
                    .Take(20)
                    .ToListAsync();

            return data;
        }
        public async Task<BudgetReadDTO> AddBudget(BudgetWriteDTO dto)
        {
            bool companyExists = await db.Company
                .AnyAsync(x => x.CompanyId == dto.CompanyId &&
            x.IsActive == 1);

            if (!companyExists)
            {
                throw new Exception("Company not found.");
            }

            bool departmentExists = await db.Department
                .AnyAsync(x =>
                    x.DepartmentId == dto.DepartmentId &&
                    x.CompanyId == dto.CompanyId &&
            x.IsActive == 1);

            if (!departmentExists)
            {
                throw new Exception(
                    "Department not found or does not belong to the selected company.");
            }

            bool budgetExists = await db.Budget
                .AnyAsync(x =>
                    x.CompanyId == dto.CompanyId &&
                    x.DepartmentId == dto.DepartmentId &&
                    x.FinancialYear == dto.FinancialYear &&
            x.IsActive == 1);

            if (budgetExists)
            {
                throw new Exception(
                    "Budget already exists for this company, department and financial year.");
            }

            var data = mapper.Map<Budget>(dto);
            //int userId = 2;
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

            await db.Budget.AddAsync(data);
            await db.SaveChangesAsync();

            var savedBudget = await db.Budget
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .FirstAsync(x => x.BudgetId == data.BudgetId &&
            x.IsActive == 1);

            return mapper.Map<BudgetReadDTO>(savedBudget);
        }
        public async Task<bool> DeleteBudget(int id)
        {
            var data = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            bool hasBudgetLines = await db.BudgetLine
                .AnyAsync(x => x.BudgetId == id &&
            x.IsActive == 1);

            if (hasBudgetLines)
            {
                throw new Exception(
                    "Budget cannot be deleted because it contains budget lines.");
            }

            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Budget.Where(x => x.IsActive == 1).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.FinancialYear.Contains(pagination.Search));
            }

            var data = await search
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<BudgetReadDTO>>(data);
        }

        public async Task<BudgetReadDTO?> GetById(int id)
        {
            var data = await db.Budget
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x => x.BudgetId == id &&
            x.IsActive == 1);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<BudgetReadDTO>(data);
        }

        public async Task<int> GetTotalRecord()
        {
            return await db.Budget.CountAsync(x => x.IsActive == 1);
        }

        public async Task<bool> UpdateBudget(int id, BudgetWriteDTO dto)
        {
            var data = await db.Budget
                .FirstOrDefaultAsync(x => x.BudgetId == id &&
            x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            bool companyExists = await db.Company
                .AnyAsync(x => x.CompanyId == dto.CompanyId &&
            x.IsActive == 1);

            if (!companyExists)
            {
                throw new Exception("Company not found.");
            }

            bool departmentExists = await db.Department
                .AnyAsync(x =>
                    x.DepartmentId == dto.DepartmentId &&
                    x.CompanyId == dto.CompanyId &&
            x.IsActive == 1);

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
                    x.BudgetId != id &&
            x.IsActive == 1);

            if (duplicateBudget)
            {
                throw new Exception(
                    "Budget already exists for this company, department and financial year.");
            }

            mapper.Map(dto, data);
            //int userId = 1;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyBudget(int budgetId)
        {
            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId);

            if (user == null)
            {
                throw new Exception("Logged-in user not found.");
            }

            if (user.Role == null || user.Role.RoleName != "CFO")
            {
                throw new Exception("Only CFO can verify the budget.");
            }

            var budget = await db.Budget
                .FirstOrDefaultAsync(x =>
                    x.BudgetId == budgetId &&
                    x.IsActive == 1);

            if (budget == null)
            {
                return false;
            }

            if (budget.ApprovedBy != null)
            {
                throw new Exception("Budget is already verified.");
            }

            budget.ApprovedBy = currentUser.UserId;
            budget.ApprovedDate = DateTime.Now;

            budget.ModifiedBy = currentUser.UserId;
            budget.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

    }
}