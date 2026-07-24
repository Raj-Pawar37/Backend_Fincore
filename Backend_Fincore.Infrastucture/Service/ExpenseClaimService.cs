using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_Fincore.Service
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        //private readonly IHttpContextAccessor httpContextAccessor;

        public ExpenseClaimService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
            //this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetExpenseClaimCount(int userId, PaginationDTO pagination)
        {
            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            IQueryable<ExpenseClaim> query = db.ExpenseClaim
                .Include(x => x.ClaimedByUser);

         
            if (user.Role.RoleName == "CFO")
            {
                db.ExpenseClaim.ToList();
            }
            else if (user.Role.RoleName == "Manager")
            {
                query = query.Where(x => x.ClaimedByUser.RoleId == user.RoleId);
            }
            else
            {
                query = query.Where(x => x.ClaimedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.ClaimNumber.Contains(pagination.Search) ||
                    x.Status.Contains(pagination.Search));
            }

            return await query.CountAsync();
        }
        public async Task<List<ExpenseClaimReadDTO>> GetAll(int userId, PaginationDTO pagination)
        {
            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            IQueryable<ExpenseClaim> query = db.ExpenseClaim
                .Include(x => x.ClaimedByUser);

            // Role-wise filtering
            if (user.Role.RoleName == "CFO")
            {
                await db.ExpenseClaim.ToListAsync();
            }
            else if (user.Role.RoleName == "Manager")
            {
                query = query.Where(x => x.ClaimedByUser.RoleId == user.RoleId);
            }
            else
            {
                query = query.Where(x => x.ClaimedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.ClaimNumber.Contains(pagination.Search) ||
                    x.Status.Contains(pagination.Search));
            }

            int totalRecords = await query.CountAsync();
          
            var expenseClaims = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<ExpenseClaimReadDTO>>(expenseClaims);
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

        public async Task<ExpenseClaimReadDTO> Create(ExpenseClaimWriteDTO dto)
        {
            //var userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


        
            bool claimExists = await db.ExpenseClaim
                .AnyAsync(x => x.ClaimNumber == dto.ClaimNumber);

            if (claimExists)
                throw new Exception("Claim Number already exists.");

            if (string.IsNullOrWhiteSpace(dto.BillFilePath))
                throw new Exception("Bill File Path is required.");

          

            var expenseClaim = mapper.Map<ExpenseClaim>(dto);

            //expenseClaim.CreatedBy = userId;
            expenseClaim.CreatedAt= DateTime.Now;
            expenseClaim.Status = "Pending";
            expenseClaim.ApprovedBy = null;
            expenseClaim.ApprovedDate = null;
            expenseClaim.OpexRequestId = null;

            await db.ExpenseClaim.AddAsync(expenseClaim);
            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
        }

        public async Task<ExpenseClaimReadDTO> Update(int expenseClaimId,ExpenseClaimWriteDTO dto)
        {
            //var userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var expenseClaim = await db.ExpenseClaim
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == expenseClaimId);

            if (expenseClaim == null)
                throw new Exception("Expense Claim not found.");

            if (expenseClaim.Status == "Approved")
                throw new Exception("Approved Expense Claim cannot be updated.");

            bool claimNumberExists = await db.ExpenseClaim
                .AnyAsync(x =>
                    x.ClaimNumber == dto.ClaimNumber &&
                    x.ExpenseClaimId != expenseClaimId);

            if (claimNumberExists)
                throw new Exception("Claim Number already exists.");

            //expenseClaim.CreatedBy = userId;
            expenseClaim.ModifiedAt= DateTime.Now;
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

            if (expenseClaim.Status == "Approved")
                throw new Exception("Approved Expense Claim cannot be deleted.");


            db.ExpenseClaim.Remove(expenseClaim);
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

            if (expenseClaim.Status == "Approved")
                throw new Exception("Expense Claim is already approved.");

            if (dto.Status != "Approved" && dto.Status != "Rejected")
                throw new Exception("Status must be Approved or Rejected.");

            var approver = await db.User
                .FirstOrDefaultAsync(x => x.UserId == verifiedBy);

            if (approver == null)
                throw new Exception("Approver user not found.");

            await using var transaction =
                await db.Database.BeginTransactionAsync();


           
            if (dto.Status == "Rejected")
            {
                expenseClaim.Status = "Rejected";
                expenseClaim.ApprovedBy = verifiedBy;
                expenseClaim.ApprovedDate = DateTime.Now;

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
            }

           
            if (dto.BudgetLineId == null)
                throw new Exception(
                    "Budget Line is required to approve the Expense Claim.");

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == dto.BudgetLineId.Value);

            if (budgetLine == null)
                throw new Exception("Budget Line not found.");

            decimal usedAmount = await db.OpexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId.Value &&
                    x.Status != "Rejected")
                .SumAsync(x => x.Amount);

            decimal availableAmount =
                budgetLine.AllocatedAmount - usedAmount;

            if (expenseClaim.ExpenseAmount > availableAmount)
            {
                throw new Exception(
                    $"Expense Claim amount exceeds available budget of {availableAmount}.");
            }

         
            var opexRequest = new OpexRequest
            {
                BudgetLineId = dto.BudgetLineId.Value,
                Title = expenseClaim.Description ?? expenseClaim.ClaimNumber,
                Amount = expenseClaim.ExpenseAmount,
                RequestedBy = expenseClaim.ClaimedBy,
                Status = "Approved",
                ApprovedBy = verifiedBy,
                ApprovedDate = DateTime.Now
            };

            await db.OpexRequest.AddAsync(opexRequest);
            await db.SaveChangesAsync();

            expenseClaim.Status = "Approved";
            expenseClaim.ApprovedBy = verifiedBy;
            expenseClaim.ApprovedDate = DateTime.Now;
            expenseClaim.OpexRequestId =
                opexRequest.OpexRequestId;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);


        }
    }
}
