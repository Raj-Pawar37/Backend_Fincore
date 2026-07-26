using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
{
    public class CapexRequestService : ICapexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        public CapexRequestService(AppDbContext db,IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(string? searchText,int? departmentId)
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

                var dto = new BudgetLineDropdownDTO
                {
                    BudgetLineId = item.BudgetLineId,
                    DisplayName = item.CostCenter + " - " + item.BudgetCategory.CategoryName,
                    AllocatedAmount = item.AllocatedAmount,
                    AvailableAmount = item.AllocatedAmount - approvedAmount
                };

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

            CapexRequest data =mapper.Map<CapexRequest>(dto);

            data.Status = "Pending";
            data.ApprovedBy = null;
            data.ApprovedDate = null;
            //int userId = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

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

        public async Task<List<CapexReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.CapexRequest.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                var keyword = pagination.Search.Trim();

                search = search.Where(x =>
                    x.Title.Contains(keyword) ||
                    x.Status.Contains(keyword));
                   
            }

            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }


            var data = await search
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Where(x => x.RequestedBy == currentUser.UserId)
                .OrderByDescending(x => x.CapexRequestId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
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

        public async Task<bool> UpdateCapexRequest(int capexRequestId,CapexWriteDTO dto)
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

            if (capex.RequestedBy != currentUser.UserId)
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
            //int userId = 1;
            capex.ModifiedBy = currentUser.UserId;
            capex.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCapexRequest(int capexRequestId,int userId)
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
                throw new Exception(
                    "CAPEX request not found.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId);

            if (approver == null)
            {
                throw new Exception(
                    "Approver not found.");
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

            // Find approval rule according to CAPEX amount
            var approval = await db.Approval
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    capex.Amount >= x.MinAmount &&
                    capex.Amount <= x.MaxAmount);                       

            if (approval == null)
            {
                throw new Exception(
                    "Approval rule not found for this CAPEX amount.");
            }

            if (approver.RoleId != approval.RoleId)
            {
                throw new Exception(
                    "You cannot approve this CAPEX request. " +
                    "It must be approved by the " +
                    approval.Role.RoleName + ".");
            }

            if (dto.Status == "Approved")
            {
                decimal approvedAmount = await db.CapexRequest
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

                bool prExists = await db.PurchaseRequisition
                    .AnyAsync(x =>
                        x.CapexRequestId ==
                        capex.CapexRequestId);

                if (prExists)
                {
                    throw new Exception(
                        "Purchase Requisition already exists " +
                        "for this CAPEX request.");
                }

                var pr = new PurchaseRequisition
                {
                    CapexRequestId =
                        capex.CapexRequestId,

                    PRNumber =
                        "PR-" +
                        DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                    Title =
                        capex.Title,

                    Description =
                        "Created automatically from CAPEX request.",

                    Status =
                        "Pending"
                };

                await db.PurchaseRequisition
                    .AddAsync(pr);

                capex.Status = "Approved";
                capex.ApprovedBy = currentUser.UserId;
                capex.ApprovedDate = DateTime.Now;
                capex.ModifiedBy = currentUser.UserId;
                capex.ModifiedAt = DateTime.Now;
            }
            else
            {
                capex.Status = "Rejected";
                capex.ApprovedBy = null;
                capex.ApprovedDate = null;
                capex.ModifiedBy = currentUser.UserId;
                capex.ModifiedAt = DateTime.Now;
            }

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetTotalRecord()
        {
            return await db.CapexRequest.CountAsync();
            //return await db.CapexRequest.CountAsync(x => x.RequestedBy == currentUser.UserId);
        }
    }
}
