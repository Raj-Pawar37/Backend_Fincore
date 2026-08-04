using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public  class BudgetLineService : IBudgetLineService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        public BudgetLineService(AppDbContext db, IMapper mapper,ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        //public async Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(string? searchText,int? departmentId, string? costCenter)
        //{
        //    var query = db.BudgetLine
        //        .Include(x => x.Budget)
        //            .ThenInclude(x => x.Department)
        //        .Include(x => x.BudgetCategory)
        //        .Where(x =>
        //            x.IsActive == 1 &&
        //            x.Budget.IsActive == 1 &&
        //             x.BudgetCategory.IsActive == 1 &&
        //            x.Budget.ApprovedBy != null &&
        //            x.Budget.ApprovedDate != null)
        //        .AsQueryable();

        //    if (departmentId.HasValue)
        //    {
        //        query = query.Where(x =>x.Budget.DepartmentId == departmentId.Value);
        //    }
        //    if (!string.IsNullOrWhiteSpace(costCenter))
        //    {
        //        costCenter = costCenter.Trim().ToUpper();

        //        query = query.Where(x =>
        //            x.CostCenter.ToUpper() == costCenter);
        //    }
        //    if (!string.IsNullOrWhiteSpace(searchText))
        //    {
        //        searchText = searchText.Trim().ToLower();

        //        query = query.Where(x =>
        //            x.CostCenter.ToLower().Contains(searchText) ||
        //            x.BudgetCategory.CategoryName.ToLower().Contains(searchText) ||
        //            x.BudgetCategory.CategoryCode.ToLower().Contains(searchText));
        //    }

        //    var budgetLines = await query
        //        .OrderBy(x => x.CostCenter)
        //        .Take(20)
        //        .ToListAsync();

        //    List<BudgetLineDropdownDTO> data = new();

        //    foreach (var item in budgetLines)
        //    {
        //        decimal approvedAmount = await db.CapexRequest
        //            .Where(x =>
        //                x.BudgetLineId == item.BudgetLineId &&
        //                x.Status == "Approved" &&
        //                x.IsActive == 1)
        //            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        //        var dto = new BudgetLineDropdownDTO
        //        {
        //            BudgetLineId = item.BudgetLineId,

        //            DisplayName =
        //                item.CostCenter + " - " +
        //                item.BudgetCategory.CategoryName,

        //            AllocatedAmount = item.AllocatedAmount,

        //            AvailableAmount =item.AllocatedAmount - approvedAmount
        //        };
        //        data.Add(dto);
        //    }
        //    return data;
        //}//modify

        public async Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(
    string? searchText,
    int? departmentId,
    string? costCenter)
        {
            var query = db.BudgetLine
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .Where(x =>
                    x.IsActive == 1 &&
                    x.Budget.IsActive == 1 &&
                    x.BudgetCategory.IsActive == 1 &&
                    x.Budget.ApprovedBy != null &&
                    x.Budget.ApprovedDate != null)
                .AsQueryable();

            if (departmentId.HasValue)
            {
                query = query.Where(x =>
                    x.Budget.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(costCenter))
            {
                costCenter = costCenter.Trim().ToUpper();

                if (costCenter != "CAPEX" &&
                    costCenter != "OPEX")
                {
                    throw new Exception(
                        "Cost center must be CAPEX or OPEX.");
                }

                query = query.Where(x =>
                    x.CostCenter.ToUpper() == costCenter);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();

                query = query.Where(x =>
                    x.CostCenter.ToLower().Contains(searchText) ||
                    x.BudgetCategory.CategoryName
                        .ToLower()
                        .Contains(searchText) ||
                    x.BudgetCategory.CategoryCode
                        .ToLower()
                        .Contains(searchText));
            }

            var budgetLines = await query
                .OrderBy(x => x.CostCenter)
                .ThenBy(x => x.BudgetCategory.CategoryName)
                .Take(20)
                .ToListAsync();

            List<BudgetLineDropdownDTO> data = new();

            foreach (var item in budgetLines)
            {
                decimal usedAmount = 0;

                if (item.CostCenter.Equals(
                    "CAPEX",
                    StringComparison.OrdinalIgnoreCase))
                {
                    usedAmount = await db.CapexRequest
                        .Where(x =>
                            x.BudgetLineId == item.BudgetLineId &&
                            x.Status == "Approved" &&
                            x.IsActive == 1)
                        .SumAsync(x => (decimal?)x.Amount) ?? 0;
                }
                else if (item.CostCenter.Equals(
                    "OPEX",
                    StringComparison.OrdinalIgnoreCase))
                {
                    usedAmount = await db.OpexRequest
                        .Where(x =>
                            x.BudgetLineId == item.BudgetLineId &&
                            x.Status == "Approved" &&
                            x.IsActive == 1)
                        .SumAsync(x => (decimal?)x.Amount) ?? 0;
                }

                data.Add(new BudgetLineDropdownDTO
                {
                    BudgetLineId = item.BudgetLineId,

                    DisplayName =
                        item.CostCenter + " - " +
                        item.BudgetCategory.CategoryName,

                    AllocatedAmount = item.AllocatedAmount,

                    AvailableAmount =
                        item.AllocatedAmount - usedAmount
                });
            }

            return data;
        }

        public async Task<BudgetLineReadDTO> AddBudgetLine(
            BudgetLineWriteDTO dto)
        {
            var budget = await db.Budget.FirstOrDefaultAsync(x =>
                            x.BudgetId == dto.BudgetId &&
                            x.IsActive == 1);

            if (budget == null)
            {
                throw new Exception("Budget not found.");
            }

            if (budget.ApprovedBy == null || budget.ApprovedDate == null)
            {
                throw new Exception(
                    "Budget must be verified before creating budget lines.");
            }

            bool categoryExists = await db.BudgetCategory.AnyAsync(x =>
                     x.BudgetCategoryId == dto.BudgetCategoryId &&
                     x.IsActive == 1);

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
                x.IsActive == 1);

            if (duplicateLine)
            {
                throw new Exception(
                    "Budget line already exists for this category and cost center.");
            }

            decimal alreadyAllocated = await db.BudgetLine
                .Where(x =>
                x.BudgetId == dto.BudgetId &&
                x.IsActive == 1)
                .SumAsync(x => x.AllocatedAmount);

            decimal newTotalAllocation = alreadyAllocated + dto.AllocatedAmount;

            if (newTotalAllocation > budget.TotalBudget)
            {
                throw new Exception("Allocated amount exceeds the total budget.");
            }

            var data = mapper.Map<BudgetLine>(dto);
            // int userId = 1;
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

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
            var data = await db.BudgetLine.FirstOrDefaultAsync(x => x.BudgetLineId == id &&x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            bool usedInCapex = await db.CapexRequest.AnyAsync(x => x.BudgetLineId == id && x.IsActive == 1);

            bool usedInOpex = await db.OpexRequest.AnyAsync(x => x.BudgetLineId == id && x.IsActive == 1);

            if (usedInCapex || usedInOpex)
            {
                throw new Exception(
                    "Budget line cannot be deleted because it is used in a CAPEX or OPEX request.");
            }
            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<BudgetLineReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.BudgetLine.Where(x => x.IsActive == 1).AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                var keyword = pagination.Search.Trim();

                search = search.Where(x =>
                    x.CostCenter.Contains(keyword) ||
                    x.Budget.Company.CompanyName.Contains(keyword) ||
                    x.Budget.Department.DepartmentName.Contains(keyword));
            }

            var data = await search
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Company)
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
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
                .FirstOrDefaultAsync(x => x.BudgetLineId == id && x.IsActive == 1);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<BudgetLineReadDTO>(data);
        }

        public async Task<int> GetTotalRecord()
        {
            return await db.BudgetLine.CountAsync(x => x.IsActive == 1);
        }

        public async Task<bool> UpdateBudgetLine(int id,BudgetLineWriteDTO dto)
        {
            var data = await db.BudgetLine.FirstOrDefaultAsync(x => x.BudgetLineId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }
            var budget = await db.Budget.FirstOrDefaultAsync(x => x.BudgetId == dto.BudgetId && x.IsActive == 1);

            if (budget == null)
            {
                throw new Exception("Budget not found.");
            }
            if (budget.ApprovedBy == null || budget.ApprovedDate == null)
            {
                throw new Exception(
                    "Budget must be verified before updating budget lines.");
            }

            bool categoryExists = await db.BudgetCategory.AnyAsync(x =>
                    x.BudgetCategoryId == dto.BudgetCategoryId && x.IsActive == 1);

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
                    x.BudgetLineId != id && x.IsActive == 1);

            if (duplicateLine)
            {
                throw new Exception(
                    "Budget line already exists for this category and cost center.");
            }

            decimal otherAllocatedAmount = await db.BudgetLine
                .Where(x =>
                    x.BudgetId == dto.BudgetId &&
                    x.BudgetLineId != id && x.IsActive == 1)
                .SumAsync(x => x.AllocatedAmount);

            decimal newTotalAllocation =
                otherAllocatedAmount + dto.AllocatedAmount;

            if (newTotalAllocation > budget.TotalBudget)
            {
                throw new Exception(
                    "Allocated amount exceeds the total budget.");
            }

            mapper.Map(dto, data);
       
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }
    }
}


