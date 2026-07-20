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
        public ExpenseClaimService(AppDbContext db)
        {
            this.db = db;
        }

        public Task Create(ExpenseClaimDto opd)
        {
            var ExpenseClaim = new ExpenseClaim()
            {
                OpexRequestId = opd.OpexRequestId,
                ClaimNumber = opd.ClaimNumber,
                ExpenseAmount = opd.ExpenseAmount,
                ExpenseDate = opd.ExpenseDate,
                Description = opd.Description,
                BillFilePath = opd.BillFilePath,
                ClaimedBy = opd.ClaimedBy,
                Status = opd.Status,
                ApprovedBy = opd.ApprovedBy,
                ApprovedDate = opd.ApprovedDate,



            };
            db.ExpenseClaim.Add(ExpenseClaim);
            db.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task Delete(int id)
        {
            var dt = await db.ExpenseClaim.FindAsync(id);
            if (dt != null)
            {
                db.ExpenseClaim.Remove(dt);
                db.SaveChanges();

            }
        }

        //public async Task<List<ExpenseClaim>> GetAllExpenseClaims()
        //{
        //   return await db.ExpenseClaim.ToListAsync();
            
        //}

        //public async Task<ExpenseClaim> GetById(int id)
        //{
           
        //    var dt = await db.ExpenseClaim.FindAsync(id);
        //    return dt;
                    
        //}

        public async Task Update(ExpenseClaimDto ec)
        {
            var id = await db.ExpenseClaim.FindAsync(ec.ExpenseClaimId);
            if (id != null)
            {
                id.ClaimNumber = ec.ClaimNumber;
                id.ExpenseAmount = ec.ExpenseAmount;
                id.ExpenseDate = ec.ExpenseDate;
                id.Description = ec.Description;
                id.BillFilePath = ec.BillFilePath;
                id.ClaimedBy = ec.ClaimedBy;
                id.Status = ec.Status;
                id.ApprovedBy = ec.ApprovedBy;
                id.ApprovedDate = ec.ApprovedDate
;


            }
            ;
            await db.SaveChangesAsync();

        }

        Task<List<ExpenseClaimDto>> IExpenseClaimService.GetAllExpenseClaims()
        {
            throw new NotImplementedException();
        }

        Task<ExpenseClaimDto> IExpenseClaimService.GetById(int id)
        {
            throw new NotImplementedException();
        }
        //        public Task Create(ExpenseClaimDto opd)
        //        {
        //            var ExpenseClaim = new ExpenseClaim()
        //            {
        //              OpexRequestId = opd.OpexRequestId,
        //              ClaimNumber = opd.ClaimNumber,
        //              ExpenseAmount = opd.ExpenseAmount,    
        //              ExpenseDate = opd.ExpenseDate,
        //              Description = opd.Description,
        //              BillFilePath = opd.BillFilePath,
        //              ClaimedBy = opd.ClaimedBy,
        //              Status = opd.Status,
        //              ApprovedBy = opd.ApprovedBy,
        //              ApprovedDate = opd.ApprovedDate,



        //            };
        //            db.ExpenseClaim.Add(ExpenseClaim);
        //            db.SaveChanges();
        //            return Task.CompletedTask;
        //        }



        //        public async Task Update(ExpenseClaimDto ec)
        //        {
        //            var id = await db.ExpenseClaim.FindAsync(ec.ExpenseClaimId);
        //            if (id != null)
        //            {
        //                id.ClaimNumber = ec.ClaimNumber;
        //                id.ExpenseAmount = ec.ExpenseAmount;
        //                id.ExpenseDate = ec.ExpenseDate;
        //                id.Description = ec.Description;
        //                id.BillFilePath = ec.BillFilePath;
        //                id.ClaimedBy = ec.ClaimedBy;
        //                id.Status = ec.Status;
        //                id.ApprovedBy = ec.ApprovedBy;
        //                id.ApprovedDate = ec.ApprovedDate
        //;


        //            };
        //            await db.SaveChangesAsync();

        //        }

        //        public async Task<ExpenseClaim> GetById(int id)
        //        {
        //            var dt = await db.ExpenseClaim.FindAsync(id);
        //            return dt;
        //        }

        //        public async Task Delete(int id)
        //        {
        //            var dt = await db.ExpenseClaim.FindAsync(id);
        //            if (dt != null)
        //            {
        //                db.ExpenseClaim.Remove(dt);
        //                db.SaveChanges();

        //            }
        //        }


    }
}
