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
    public class CapexRequestService : ICapexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CapexRequestService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        public async Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(
    string? searchText,
    int? departmentId)
        {
            var budgetLines = await db.BudgetLine
                .Include(x => x.Budget)
                    .ThenInclude(x => x.Department)
                .Include(x => x.BudgetCategory)
                .ToListAsync();

            if (departmentId != null)
            {
                budgetLines = budgetLines
                    .Where(x => x.Budget.DepartmentId == departmentId)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                budgetLines = budgetLines
                    .Where(x =>
                        x.CostCenter.Contains(searchText) ||
                        x.BudgetCategory.CategoryName.Contains(searchText) ||
                        x.Budget.Department.DepartmentName.Contains(searchText))
                    .Take(20)
                    .ToList();
            }

            List<BudgetLineDropdownDTO> data = new();

            foreach (var item in budgetLines)
            {
                decimal approvedAmount = await db.CapexRequest
                    .Where(x =>
                        x.BudgetLineId == item.BudgetLineId &&
                        x.Status == "Approved")
                    .SumAsync(x => (decimal?)x.Amount) ?? 0;

                BudgetLineDropdownDTO dto = new BudgetLineDropdownDTO();

                dto.BudgetLineId = item.BudgetLineId;
                dto.DisplayName = item.CostCenter + " - " + item.BudgetCategory.CategoryName;
                dto.AllocatedAmount = item.AllocatedAmount;
                dto.AvailableAmount = item.AllocatedAmount - approvedAmount;

                data.Add(dto);
            }

            return data;
        }

        public async Task<CapexReadDTO> AddCapexRequest(CapexWriteDTO dto)
        {
            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == dto.BudgetLineId);

            if (budgetLine == null)
            {
                throw new Exception("Budget line not found.");
            }

            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == dto.RequestedBy);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (dto.Amount <= 0)
            {
                throw new Exception(
                    "CAPEX amount must be greater than zero.");
            }

            decimal approvedAmount = await db.CapexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.Status == "Approved")
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal availableAmount =
                budgetLine.AllocatedAmount - approvedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception(
                    "CAPEX request amount exceeds the available budget.");
            }

            CapexRequest data =
                mapper.Map<CapexRequest>(dto);

            data.Status = "Pending";
            data.ApprovedBy = null;
            data.ApprovedDate = null;

            await db.CapexRequest.AddAsync(data);
            await db.SaveChangesAsync();

            var result = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == data.CapexRequestId);

            return mapper.Map<CapexReadDTO>(result);
        }

        //public async Task<List<CapexReadDTO>> GetAll(
        //        int userId,
        //        int pageNumber,
        //        int pageSize)
        //{
        //    var user = await db.User
        //        .FirstOrDefaultAsync(x => x.UserId == userId);

        //    if (user == null)
        //    {
        //        throw new Exception("User not found.");
        //    }

        //    var query = db.CapexRequest
        //        .Include(x => x.BudgetLine)
        //            .ThenInclude(x => x.BudgetCategory)
        //        .Include(x => x.BudgetLine)
        //            .ThenInclude(x => x.Budget)
        //                .ThenInclude(x => x.Department)
        //        .Include(x => x.RequestedByUser)
        //        .Include(x => x.ApprovedByUser)
        //        .AsQueryable();

        //    if (user.RoleId == 1)
        //    {
        //        //query = query.Where(x =>
        //        //    x.RequestedBy == userId);
        //    }

        //    else if (user.RoleId == 2)
        //    {
        //        var manager = await db.Employee.FirstOrDefaultAsync(x =>
        //            x.EmployeeId == user.MasterId);

        //        if (manager == null)
        //        {
        //            throw new Exception("Manager employee not found.");
        //        }

        //        query = query.Where(x =>
        //            x.BudgetLine.Budget.DepartmentId ==
        //            manager.DepartmentId);
        //    }

        //    else if (user.RoleId == 3)
        //    {
        //        // CFO can view all requests
        //    }

        //    else
        //    {
        //        throw new Exception("Invalid user role.");
        //    }

        //    if (pageNumber <= 0)
        //    {
        //        pageNumber = 1;
        //    }

        //    if (pageSize <= 0)
        //    {
        //        pageSize = 10;
        //    }

        //    var data = await query
        //        .OrderByDescending(x => x.CapexRequestId)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return mapper.Map<List<CapexReadDTO>>(data);
        //}

        public async Task<List<CapexReadDTO>> GetAll(
                int userId,
                int pageNumber,
                int pageSize)
        {
            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            var data = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Where(x => x.RequestedBy == userId)
                .OrderByDescending(x => x.CapexRequestId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return mapper.Map<List<CapexReadDTO>>(data);
        }

        public async Task<CapexReadDTO?> GetById(int capexRequestId)
        {
            var data = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == capexRequestId);

            if (data == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            return mapper.Map<CapexReadDTO>(data);
        }

        public async Task<bool> UpdateCapexRequest(
                int capexRequestId,
                int userId,
                CapexWriteDTO dto)
        {
            var capex = await db.CapexRequest
                .FirstOrDefaultAsync(x => x.CapexRequestId == capexRequestId);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception("Only pending CAPEX requests can be updated.");
            }

            if (capex.RequestedBy != userId)
            {
                throw new Exception("You can update only your own CAPEX request.");
            }

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x => x.BudgetLineId == dto.BudgetLineId);

            if (budgetLine == null)
            {
                throw new Exception("Budget line not found.");
            }

            decimal approvedAmount = await db.CapexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.Status == "Approved")
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal availableAmount = budgetLine.AllocatedAmount - approvedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception("CAPEX amount exceeds available budget.");
            }

            capex.BudgetLineId = dto.BudgetLineId;
            capex.Title = dto.Title;
            capex.Amount = dto.Amount;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCapexRequest(
    int capexRequestId,
    int userId)
        {
            var capex = await db.CapexRequest
                .FirstOrDefaultAsync(x => x.CapexRequestId == capexRequestId);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            if (capex.RequestedBy != userId)
            {
                throw new Exception("You can delete only your own CAPEX request.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception("Only pending CAPEX requests can be deleted.");
            }

            db.CapexRequest.Remove(capex);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyCapexRequest(CapexVerifyDTO dto)
        {
            var capex = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == dto.CapexRequestId);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == dto.UserId);

            if (approver == null)
            {
                throw new Exception("Approver not found.");
            }

            if (approver.RoleId != 2 &&
                approver.RoleId != 3)
            {
                throw new Exception(
                    "Only Manager or CFO can verify CAPEX request.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception(
                    "CAPEX request is already verified.");
            }

            if (dto.Status != "Approved" &&
                dto.Status != "Rejected")
            {
                throw new Exception(
                    "Status must be Approved or Rejected.");
            }

            // Manager can verify only their department requests
            if (approver.RoleId == 2)
            {
                var manager = await db.Employee
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == approver.MasterId);

                if (manager == null)
                {
                    throw new Exception(
                        "Manager employee record not found.");
                }

                if (manager.DepartmentId !=
                    capex.BudgetLine.Budget.DepartmentId)
                {
                    throw new Exception(
                        "You can verify only your department requests.");
                }
            }

            if (dto.Status == "Approved")
            {
                decimal approvedAmount =
                    await db.CapexRequest
                        .Where(x =>
                            x.BudgetLineId == capex.BudgetLineId &&
                            x.Status == "Approved")
                        .SumAsync(x => (decimal?)x.Amount) ?? 0;

                decimal availableAmount =
                    capex.BudgetLine.AllocatedAmount -
                    approvedAmount;

                if (capex.Amount > availableAmount)
                {
                    throw new Exception(
                        "Insufficient available budget.");
                }

                bool prExists = await db.PurchaseRequisition.
                    AnyAsync(x => x.CapexRequestId == capex.CapexRequestId);

                if (!prExists)
                {
                    var pr = new PurchaseRequisition
                    {
                        CapexRequestId = capex.CapexRequestId,
                        PRNumber = "PR-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        Title = capex.Title,
                        Description = "Created automatically from CAPEX request.",
                        Status = "Pending"
                    };

                    await db.PurchaseRequisition.AddAsync(pr);
                }

                capex.Status = "Approved";
                capex.ApprovedBy = dto.UserId;
                capex.ApprovedDate = DateTime.Now;
            }

            else if (dto.Status == "Rejected")
            {
                capex.Status = "Rejected";

                // Keep approval fields empty for rejected requests
                capex.ApprovedBy = null;
                capex.ApprovedDate = null;
            }

            await db.SaveChangesAsync(); 

            return true;
        }

    }
}
