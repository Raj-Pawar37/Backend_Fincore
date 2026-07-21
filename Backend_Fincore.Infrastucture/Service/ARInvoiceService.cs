using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastructure.Service
{
    public class ARInvoiceService : IARInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ARInvoiceService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ARInvoiceDto>> GetAllAsync()
        {
            var data = await _context.ARInvoice.ToListAsync();

            return _mapper.Map<List<ARInvoiceDto>>(data);
        }

        public async Task<ARInvoiceDto?> GetByIdAsync(int id)
        {
            var data = await _context.ARInvoice.FindAsync(id);

            if (data == null)
                return null;

            return _mapper.Map<ARInvoiceDto>(data);
        }

        public async Task<bool> AddAsync(ARInvoiceCreateDto dto)
        {
            var entity = _mapper.Map<ARInvoice>(dto);

            await _context.ARInvoice.AddAsync(entity);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(ARInvoiceUpdateDto dto)
        {
            var entity = await _context.ARInvoice.FindAsync(dto.ARInvoiceId);

            if (entity == null)
                return false;

            entity.CustomerId = dto.CustomerId;
            entity.RevenueEntryId = dto.RevenueEntryId;
            entity.InvoiceNumber = dto.InvoiceNumber;
            entity.InvoiceAmount = dto.InvoiceAmount;
            entity.InvoiceDate = dto.InvoiceDate;
            entity.Status = dto.Status;
            entity.PONumber = dto.PONumber;

            _context.ARInvoice.Update(entity);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ARInvoice.FindAsync(id);

            if (entity == null)
                return false;

            _context.ARInvoice.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}