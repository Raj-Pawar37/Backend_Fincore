using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class OpexRequestService : IOpexRequestService
    {
        private readonly AppDbContext db;
        public OpexRequestService(AppDbContext db)
        {
            this.db = db;
            
        }

        public  Task Create(OpexRequestDto opd)
        {
            var OpexRequest =  new OpexRequest()
            {
                BudgetLineId = opd.BudgetLineId,
                Title = opd.Title,
                Amount = opd.Amount,
                RequestedBy = opd.RequestedBy,
                Status = opd.Status,
                ApprovedBy= opd.ApprovedBy,
                ApprovedDate= opd.ApprovedDate,


            };
           db.OpexRequest.Add(OpexRequest);
            db.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<List<OpexRequest>> GetAllRequests()
        {
          var data =  await db.OpexRequest.ToListAsync();
            return data;
        }

        public async Task Update(OpexRequestDto dto)
        {
            var id = await db.OpexRequest.FindAsync(dto.OpexRequestId);
            if (id != null)
            {
              id.BudgetLineId = dto.BudgetLineId;
                id.Title = dto.Title;
                id.Amount = dto.Amount;
                id.RequestedBy = dto.RequestedBy;
                id.Status = dto.Status;
                id.ApprovedBy = dto.ApprovedBy;
                id.ApprovedDate = dto.ApprovedDate;

            }
           await  db.SaveChangesAsync();
             
        }

        public async Task<OpexRequest>GetById(int id)
        {
            var dt = await db.OpexRequest.FindAsync(id);
            return dt;
        }

        public async Task Delete(int id)
        {
            var dt = await db.OpexRequest.FindAsync(id);
            if(dt != null)
            {
                db.OpexRequest.Remove(dt);
                db.SaveChanges();

            }
        }

        Task<List<OpexRequestDto>> IOpexRequestService.GetAllRequests()
        {
            throw new NotImplementedException();
        }

        Task<OpexRequestDto> IOpexRequestService.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
