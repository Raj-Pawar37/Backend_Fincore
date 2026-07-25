using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ARInvoice;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
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

        public async Task<List<ARInvoiceDto>> GetAllAsync(PaginationDTO pagination)
        {
            var data = await _context.ARInvoice
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

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

        public Task<ARInvoiceDto?> GenerateInvoiceAsync(GenerateInvoiceDto dto)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> GenerateInvoiceAsync(ARInvoiceGenerateDto dto)
        {
            // 1. Find Revenue Entry
            var revenue = await _context.RevenueEntry
                .FirstOrDefaultAsync(x => x.RevenueEntryId == dto.RevenueEntryId);

            if (revenue == null)
                throw new Exception("Revenue Entry not found.");

            // 2. Check whether invoice already exists
            bool invoiceExists = await _context.ARInvoice
                .AnyAsync(x => x.RevenueEntryId == dto.RevenueEntryId);

            if (invoiceExists)
                throw new Exception("Invoice already generated for this Revenue Entry.");

            // 3. Generate Invoice Number
            int count = await _context.ARInvoice.CountAsync();

            string invoiceNumber = $"INV-{DateTime.Now.Year}-{(count + 1):D4}";

            // 4. Create Invoice
            var invoice = new ARInvoice
            {
                CustomerId = revenue.CustomerId,
                RevenueEntryId = revenue.RevenueEntryId,
                InvoiceNumber = invoiceNumber,
                InvoiceAmount = revenue.Amount,
                InvoiceDate = DateTime.Now,
                Status = "Pending",
                PONumber = null
            };

            // 5. Save
            _context.ARInvoice.Add(invoice);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
