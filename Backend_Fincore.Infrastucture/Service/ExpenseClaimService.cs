using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_Fincore.Infrastucture.Service
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService current;

        public ExpenseClaimService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;

        }

        public async Task<int> GetExpenseClaimCount(PaginationDTO pagination)
        {
            int userId = current.UserId;

            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<ExpenseClaim> query = db.ExpenseClaim
                .Include(x => x.ClaimedByUser)
                .Where(x => x.IsActive == 1);

            if (user.Role.RoleId == 1 || user.Role.RoleId == 2)
            {
                // CFO and allowed manager see all active claims.
            }
            else if (user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x => x.ClaimedByUser.RoleId == user.Role.RoleId);
            }
            else
            {
                query = query.Where(x => x.ClaimedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string search = pagination.Search.Trim();

                query = query.Where(x =>
                    x.ClaimNumber.Contains(search) ||
                    x.Status.Contains(search));
            }

            return await query.CountAsync();
        }
        public async Task<List<ExpenseClaimReadDTO>> GetAll(PaginationDTO pagination)
        {
            int userId = current.UserId;

            if (pagination.PageNumber <= 0)
                pagination.PageNumber = 1;

            if (pagination.PageSize <= 0)
                pagination.PageSize = 10;

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<ExpenseClaim> query = db.ExpenseClaim
                .Include(x => x.ClaimedByUser)
                .Where(x => x.IsActive == 1);

            // Role Filtering
            if (user.Role.RoleId == 1 || user.Role.RoleId == 2)
            {
                // CFO/Admin - View all active records
            }
            else if (user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x => x.ClaimedByUser.RoleId == user.Role.RoleId);
            }
            else
            {
                query = query.Where(x => x.ClaimedBy == userId);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string search = pagination.Search.Trim();

                query = query.Where(x => x.ClaimNumber.Contains(search) || x.Status.Contains(search));
            }

            var expenseClaims = await query
                .OrderByDescending(x => x.ExpenseClaimId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<ExpenseClaimReadDTO>>(expenseClaims);
        }
        public async Task<ExpenseClaimReadDTO> GetById(int id)
        {
            var data = await db.ExpenseClaim.FirstOrDefaultAsync(x => x.ExpenseClaimId == id && x.IsActive == 1);

            if (data == null)
                throw new Exception("Expense Claim not found or has been deleted.");

            return mapper.Map<ExpenseClaimReadDTO>(data);
        }

        public async Task<ExpenseClaimReadDTO> Create(ExpenseClaimWriteDTO dto)
        {
            bool claimExists = await db.ExpenseClaim
                .AnyAsync(x =>
                    x.ClaimNumber == dto.ClaimNumber &&
                    x.IsActive == 1);

            if (claimExists)
                throw new Exception("Claim Number already exists.");

            if (string.IsNullOrWhiteSpace(dto.BillFilePath))
                throw new Exception("Bill File Path is required.");

            var expenseClaim = mapper.Map<ExpenseClaim>(dto);

            expenseClaim.CreatedBy = current.UserId;
            expenseClaim.CreatedAt = DateTime.Now;

            expenseClaim.IsActive = 1;
            expenseClaim.ClaimedBy = current.UserId;
            expenseClaim.Status = "Pending";
            expenseClaim.ApprovedBy = null;
            expenseClaim.ApprovedDate = null;
            expenseClaim.OpexRequestId = null;

            await db.ExpenseClaim.AddAsync(expenseClaim);
            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
        }
        public async Task<ExpenseClaimReadDTO> Update(int expenseClaimId, ExpenseClaimWriteDTO dto)
        {
            var expenseClaim = await db.ExpenseClaim.FirstOrDefaultAsync(x => x.ExpenseClaimId == expenseClaimId);

            if (expenseClaim == null)
                throw new Exception("Expense Claim not found.");

            if (expenseClaim.IsActive == 0)
                throw new Exception("Expense Claim has been deleted.");

            if (expenseClaim.Status == "Approved")
                throw new Exception("Approved Expense Claim cannot be updated.");

            bool claimNumberExists = await db.ExpenseClaim
                .AnyAsync(x =>
                    x.ClaimNumber == dto.ClaimNumber &&
                    x.ExpenseClaimId != expenseClaimId &&
                    x.IsActive == 1);

            if (claimNumberExists)
                throw new Exception("Claim Number already exists.");

            expenseClaim.ModifiedBy = current.UserId;
            expenseClaim.ModifiedAt = DateTime.Now;

            expenseClaim.ClaimNumber = dto.ClaimNumber;
            expenseClaim.ExpenseAmount = dto.ExpenseAmount;
            expenseClaim.ExpenseDate = dto.ExpenseDate;
            expenseClaim.Description = dto.Description;
            expenseClaim.BillFilePath = dto.BillFilePath;
            expenseClaim.ClaimedBy = dto.ClaimedBy;

            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
        }
        public async Task<bool> Delete(int expenseClaimId)
        {
            var expenseClaim = await db.ExpenseClaim
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == expenseClaimId);

            if (expenseClaim == null)
                throw new Exception("Expense Claim not found.");

            if (expenseClaim.IsActive == 0)
                throw new Exception("Expense Claim has already been deleted.");

            if (expenseClaim.Status == "Approved")
                throw new Exception("Approved Expense Claim cannot be deleted.");

            expenseClaim.IsActive = 0;
            expenseClaim.ModifiedBy = current.UserId;
            expenseClaim.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<ExpenseClaimReadDTO> Verify(int expenseClaimId, int verifiedBy, ExpenseClaimVerifyDTO dto)
        {
            var expenseClaim = await db.ExpenseClaim
                .FirstOrDefaultAsync(x =>
                    x.ExpenseClaimId == expenseClaimId);

            if (expenseClaim == null)
                throw new Exception("Expense Claim not found.");

            if (expenseClaim.IsActive == 0)
                throw new Exception("Inactive Expense Claim cannot be verified.");

            if (expenseClaim.Status == "Approved")
                throw new Exception("Expense Claim is already approved.");

            if (dto.Status != "Approved" && dto.Status != "Rejected")
            {
                throw new Exception("Status must be Approved or Rejected.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == verifiedBy &&
                    x.IsActive == 1);

            if (approver == null)
                throw new Exception("Approver user not found or inactive.");

            await using var transaction = await db.Database.BeginTransactionAsync();


            if (dto.Status == "Rejected")
            {
                expenseClaim.Status = "Rejected";
                expenseClaim.ApprovedBy = verifiedBy;
                expenseClaim.ApprovedDate = DateTime.Now;
                expenseClaim.ModifiedBy = current.UserId;
                expenseClaim.ModifiedAt = DateTime.Now;

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
            }

            if (dto.BudgetLineId == null)
            {
                throw new Exception("Budget Line is required to approve the Expense Claim.");
            }

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == dto.BudgetLineId.Value &&
                    x.IsActive == 1);

            if (budgetLine == null)
            {
                throw new Exception("Budget Line not found or inactive.");
            }

            decimal usedAmount = await db.OpexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId.Value &&
                    x.Status != "Rejected" &&
                    x.IsActive == 1)
                .SumAsync(x => x.Amount);

            decimal availableAmount = budgetLine.AllocatedAmount - usedAmount;

            if (expenseClaim.ExpenseAmount > availableAmount)
            {
                throw new Exception($"Expense Claim amount exceeds available budget of {availableAmount}.");
            }

            var opexRequest = new OpexRequest
            {
                BudgetLineId = dto.BudgetLineId.Value,
                Title = expenseClaim.Description ?? expenseClaim.ClaimNumber,
                Amount = expenseClaim.ExpenseAmount,
                RequestedBy = expenseClaim.ClaimedBy,

                Status = "Pending",
                ApprovedBy = verifiedBy,
                ApprovedDate = DateTime.Now,

                IsActive = 1,
                CreatedBy = current.UserId,
                CreatedAt = DateTime.Now
            };

            await db.OpexRequest.AddAsync(opexRequest);
            await db.SaveChangesAsync();

            expenseClaim.Status = "Approved";
            expenseClaim.ApprovedBy = verifiedBy;
            expenseClaim.ApprovedDate = DateTime.Now;
            expenseClaim.OpexRequestId = opexRequest.OpexRequestId;

            expenseClaim.ModifiedBy = current.UserId;
            expenseClaim.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);

        }
        public async Task<List<ExpenseClaimDropdownDTO>> GetDropdown()
        {
            var data = await db.ExpenseClaim
                .Where(x => x.IsActive == 1)
                .OrderBy(x => x.ClaimNumber)
                .Select(x => new ExpenseClaimDropdownDTO
                {
                    ExpenseClaimId = x.ExpenseClaimId,
                    ClaimNumber = x.ClaimNumber
                })
                .ToListAsync();

            return data;
        }
    }
}
