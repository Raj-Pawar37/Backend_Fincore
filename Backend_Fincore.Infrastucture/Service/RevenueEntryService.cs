using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastructure.Service
{
    public class RevenueEntryService : IRevenueEntryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RevenueEntryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RevenueEntryDto>> GetAllAsync()
        {
            var data = await _context.RevenueEntry.ToListAsync();

            return _mapper.Map<List<RevenueEntryDto>>(data);
        }

        public async Task<RevenueEntryDto?> GetByIdAsync(int id)
        {
            var data = await _context.RevenueEntry.FindAsync(id);

            if (data == null)
                return null;

            return _mapper.Map<RevenueEntryDto>(data);
        }

        public async Task<bool> AddAsync(RevenueEntryCreateDto dto)
        {
            var entity = _mapper.Map<RevenueEntry>(dto);

            await _context.RevenueEntry.AddAsync(entity);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(RevenueEntryUpdateDto dto)
        {
            var entity = await _context.RevenueEntry.FindAsync(dto.RevenueEntryId);

            if (entity == null)
                return false;

            entity.CustomerId = dto.CustomerId;
            entity.ProfitCenterName = dto.ProfitCenterName;
            entity.Description = dto.Description;
            entity.Amount = dto.Amount;
            entity.RevenueDate = dto.RevenueDate;
            entity.Status = dto.Status;

            _context.RevenueEntry.Update(entity);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.RevenueEntry.FindAsync(id);

            if (entity == null)
                return false;

            _context.RevenueEntry.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}