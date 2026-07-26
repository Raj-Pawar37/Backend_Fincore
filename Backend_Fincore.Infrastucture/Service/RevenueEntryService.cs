using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RevenueEntry;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Backend_Fincore.Infrastucture.Service
{
    public class RevenueEntryService : IRevenueEntryService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public RevenueEntryService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }


        //  get all with cache
        public async Task<ApiResponse<List<RevenueEntryDto>>> GetAllAsync(PaginationDTO pagination)
        {
            var cacheKey = $"revenueEntries_{pagination.PageNumber}_{pagination.PageSize}";


            //  Check Cache
            if (_cache.TryGetValue(cacheKey, out ApiResponse<List<RevenueEntryDto>>? cachedData))
            {
                Console.WriteLine("GET ALL FROM CACHE");
                return cachedData!;
            }


            Console.WriteLine("GET ALL FROM DATABASE");


            var totalRecord = await _context.RevenueEntry.CountAsync();


            var data = await _context.RevenueEntry
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();


            var dto = _mapper.Map<List<RevenueEntryDto>>(data);


            var response = new ApiResponse<List<RevenueEntryDto>>
            {
                Success = true,
                Message = "Revenue Entries fetched successfully",
                Data = dto,
                Error = null,
                Metadata = new Metadata
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecord / pagination.PageSize)
                },
                TotalNumberRecord = totalRecord
            };


            // Save Cache
            _cache.Set(
                cacheKey,
                response,
                TimeSpan.FromMinutes(5)
            );


            return response;
        }

        // Get By Id With Cache
        public async Task<RevenueEntryDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"revenueEntry_{id}";


            //Check cache
            if (_cache.TryGetValue(cacheKey, out RevenueEntryDto? cached))
            {
                Console.WriteLine("GET BY ID FROM CACHE");
                return cached;
            }


            Console.WriteLine("GET BY ID FROM DATABASE");


            var data = await _context.RevenueEntry.FindAsync(id);


            if (data == null)
                return null;


            var dto = _mapper.Map<RevenueEntryDto>(data);



            // Save Cache
            _cache.Set(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(10)
            );


            return dto;
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
            var entity = await _context.RevenueEntry
                .FindAsync(dto.RevenueEntryId);


            if (entity == null)
                return false;



            entity.CustomerId = dto.CustomerId;
            entity.ProfitCenterName = dto.ProfitCenterName;
            entity.Description = dto.Description;
            entity.Amount = dto.Amount;
            entity.RevenueDate = dto.RevenueDate;
            entity.Status = dto.Status;



            await _context.SaveChangesAsync();


            return true;
        }





        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.RevenueEntry
                .FindAsync(id);


            if (entity == null)
                return false;



            _context.RevenueEntry.Remove(entity);


            await _context.SaveChangesAsync();


            return true;
        }
    }
}