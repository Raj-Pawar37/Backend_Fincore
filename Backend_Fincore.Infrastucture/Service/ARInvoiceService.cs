using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ARInvoice;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Backend_Fincore.Infrastucture.Service
{
    public class ARInvoiceService : IARInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public ARInvoiceService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }



        // GET ALL WITH CACHE
        public async Task<List<ARInvoiceDto>> GetAllAsync(PaginationDTO pagination)
        {
            var cacheKey = $"arInvoices_{pagination.PageNumber}_{pagination.PageSize}";


            // Check Cache
            if (_cache.TryGetValue(cacheKey, out List<ARInvoiceDto>? cachedData))
            {
                Console.WriteLine("AR INVOICE DATA FROM CACHE");
                return cachedData!;
            }


            Console.WriteLine("AR INVOICE DATA FROM DATABASE");


            var data = await _context.ARInvoice
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();


            var result = _mapper.Map<List<ARInvoiceDto>>(data);



            // Store Cache
            _cache.Set(
                cacheKey,
                result,
                TimeSpan.FromMinutes(5)
            );


            return result;
        }




        // GET BY ID WITH CACHE
        public async Task<ARInvoiceDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"arInvoice_{id}";


            // Check Cache
            if (_cache.TryGetValue(cacheKey, out ARInvoiceDto? cached))
            {
                Console.WriteLine("AR INVOICE ITEM FROM CACHE");
                return cached;
            }


            Console.WriteLine("AR INVOICE ITEM FROM DATABASE");


            var data = await _context.ARInvoice.FindAsync(id);


            if (data == null)
                return null;


            var result = _mapper.Map<ARInvoiceDto>(data);



            // Store Cache
            _cache.Set(
                cacheKey,
                result,
                TimeSpan.FromMinutes(10)
            );


            return result;
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
            var revenue = await _context.RevenueEntry
                .FirstOrDefaultAsync(x => x.RevenueEntryId == dto.RevenueEntryId);


            if (revenue == null)
                throw new Exception("Revenue Entry not found.");



            bool invoiceExists = await _context.ARInvoice
                .AnyAsync(x => x.RevenueEntryId == dto.RevenueEntryId);


            if (invoiceExists)
                throw new Exception("Invoice already generated for this Revenue Entry.");



            int count = await _context.ARInvoice.CountAsync();


            string invoiceNumber = $"INV-{DateTime.Now.Year}-{(count + 1):D4}";



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



            _context.ARInvoice.Add(invoice);


            await _context.SaveChangesAsync();


            return true;
        }
    }
}