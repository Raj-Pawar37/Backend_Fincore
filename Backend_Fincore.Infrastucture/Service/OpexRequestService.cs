using AutoMapper;
using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Data;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class OpexRequestService : IOpexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public OpexRequestService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<OpexRequestReadDTO>> GetAll()
        {
            var data = await db.OpexRequest.ToListAsync();

            return mapper.Map<List<OpexRequestReadDTO>>(data);
        }

        public async Task<OpexRequestReadDTO?> GetById(int id)
        {
            var data = await db.OpexRequest.FindAsync(id);

            if (data == null)
                return null;

            return mapper.Map<OpexRequestReadDTO>(data);
        }
        public async Task<OpexRequestReadDTO> Create(OpexRequestWriteDTO dto)
        {
            var data = mapper.Map<OpexRequest>(dto);

            data.Status = "Pending";

            db.OpexRequest.Add(data);

            await db.SaveChangesAsync();

            return mapper.Map<OpexRequestReadDTO>(data);
        }

        public async Task<OpexRequestReadDTO> Update(int id, OpexRequestWriteDTO dto)
        {
            var data = await db.OpexRequest.FindAsync(id);

            if (data == null)
            {
                return null;
            }

            mapper.Map(dto, data);

            await db.SaveChangesAsync();
            return mapper.Map<OpexRequestReadDTO>(data);
        }

        public async Task<bool> Delete(int id)
        {
            var data = await db.OpexRequest.FindAsync(id);

            if (data == null)
                return false;

            db.OpexRequest.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }
    }

}