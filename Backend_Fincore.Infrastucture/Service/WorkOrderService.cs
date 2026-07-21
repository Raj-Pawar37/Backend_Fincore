using AutoMapper;
using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.Data;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public WorkOrderService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<WorkOrderReadDTO>> GetAll()
        {
            var data = await db.WorkOrder.ToListAsync();

            return mapper.Map<List<WorkOrderReadDTO>>(data);
        }

        public async Task<WorkOrderReadDTO?> GetById(int id)
        {
            var data = await db.WorkOrder.FindAsync(id);

            if (data == null)
                return null;

            return mapper.Map<WorkOrderReadDTO>(data);
        }

        public async Task<WorkOrderReadDTO> Create(WorkOrderWriteDTO dto)
        {
            var data = mapper.Map<WorkOrder>(dto);

            data.Status = "Draft";

            await db.WorkOrder.AddAsync(data);

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(data);
        }

        public async Task<WorkOrderReadDTO?> Update(int id, WorkOrderWriteDTO dto)
        {
            var data = await db.WorkOrder.FindAsync(id);

            if (data == null)
                return null;

            mapper.Map(dto, data);

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(data);
        }

        public async Task<bool> Delete(int id)
        {
            var data = await db.WorkOrder.FindAsync(id);

            if (data == null)
                return false;

            db.WorkOrder.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}