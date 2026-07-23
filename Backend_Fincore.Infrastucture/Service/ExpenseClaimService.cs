using AutoMapper;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public ExpenseClaimService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<ExpenseClaimReadDTO>> GetAll(int userId)
        {
            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            List<ExpenseClaim> expenseClaims;

            if (user.Role.RoleName == "CFO")
            {
                expenseClaims = await db.ExpenseClaim.ToListAsync();
            }
            else if (user.Role.RoleName == "Manager")
            {
                expenseClaims = await db.ExpenseClaim
                    .Include(x => x.ClaimedByUser)
                    .Where(x => x.ClaimedByUser.RoleId == user.RoleId)
                    .ToListAsync();
            }
            else
            {
                expenseClaims = await db.ExpenseClaim
                    .Where(x => x.ClaimedBy == userId)
                    .ToListAsync();
            }

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
            // Check duplicate Claim Number
            bool claimExists = await db.ExpenseClaim
                .AnyAsync(x => x.ClaimNumber == dto.ClaimNumber);

            if (claimExists)
                throw new Exception("Claim Number already exists.");

            // Bill File Path validation
            if (string.IsNullOrWhiteSpace(dto.BillFilePath))
                throw new Exception("Bill File Path is required.");

            var expenseClaim = mapper.Map<ExpenseClaim>(dto);

            // Default values
            expenseClaim.Status = "Pending";
            expenseClaim.ApprovedBy = null;
            expenseClaim.ApprovedDate = null;
            //expenseClaim.OpexRequestId = null;

            await db.ExpenseClaim.AddAsync(expenseClaim);
            await db.SaveChangesAsync();

            return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
        }

        public async Task<ExpenseClaimReadDTO> Update(
      int expenseClaimId,
      ExpenseClaimWriteDTO dto)
        {
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

        public async Task<ExpenseClaimReadDTO> Verify(int expenseClaimId,int verifiedBy,ExpenseClaimVerifyDTO dto)
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

          
                // When claim is rejected, only update its status
                if (dto.Status == "Rejected")
                {
                    expenseClaim.Status = "Rejected";
                    expenseClaim.ApprovedBy = verifiedBy;
                    expenseClaim.ApprovedDate = DateTime.Now;

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
                }

                // BudgetLineId is required while approving
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

                // Create new OPEX request
                var opexRequest = new OpexRequest
                {
                    BudgetLineId = dto.BudgetLineId.Value,
                    Title = expenseClaim.Description
                            ?? expenseClaim.ClaimNumber,
                    Amount = expenseClaim.ExpenseAmount,
                    RequestedBy = expenseClaim.ClaimedBy,
                    Status = "Approved",
                    ApprovedBy = verifiedBy,
                    ApprovedDate = DateTime.Now
                };

                await db.OpexRequest.AddAsync(opexRequest);
                await db.SaveChangesAsync();

                // Update Expense Claim with status and OPEX ID
                expenseClaim.Status = "Approved";
                expenseClaim.ApprovedBy = verifiedBy;
                expenseClaim.ApprovedDate = DateTime.Now;
                expenseClaim.OpexRequestId =
                    //opexRequest.OpexRequestId;

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return mapper.Map<ExpenseClaimReadDTO>(expenseClaim);
        
           
        }
    }
}
