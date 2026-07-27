using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Customer;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        private readonly IMemoryCache cache;

        private const string CacheKeyList = "Cache_Customer_List_";
        private const string CacheKeySingle = "Cache_Customer_Id_";
        private const string CacheKeyCount = "Cache_Customer_Count_";

        public CustomerService(
            AppDbContext db,
            IMapper mapper,
            ICurrentUserService currentUser,
            IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
            this.cache = cache;
        }

        public async Task<CustomerReadDTO> AddCutomer(CustomerWriteDTO c)
        {
            var data = mapper.Map<Customer>(c);
            data.IsActive = 1;
            data.CreatedAt = DateTime.Now;
            data.CreatedBy = currentUser.UserId;

            await db.Customer.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Customer
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.CustomerId == data.CustomerId);

            ClearCustomerCache();

            return mapper.Map<CustomerReadDTO>(mdata);
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            var customer = await db.Customer
                .FirstOrDefaultAsync(x => x.CustomerId == id && x.IsActive == 1);

            if (customer == null)
                return false;

            bool hasRevenue = await db.RevenueEntry
                .AnyAsync(x => x.CustomerId == id);

            if (hasRevenue)
            {
                throw new InvalidOperationException("Customer cannot be deleted because it has revenue entries.");
            }

            bool hasInvoices = await db.ARInvoice
                .AnyAsync(x => x.CustomerId == id);

            if (hasInvoices)
            {
                throw new InvalidOperationException("Customer cannot be deleted because it has AR invoices.");
            }

            // Soft Delete
            customer.IsActive = 0;
            customer.ModifiedAt = DateTime.Now;
            customer.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();

            ClearCustomerCache();

            return true;
        }

        public async Task<List<CustomerReadDTO>> GetAll(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out List<CustomerReadDTO>? cachedList) && cachedList != null)
            {
                return cachedList;
            }

            var search = db.Customer
                .Include(x => x.Company)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.CustomerName.Contains(pagination.Search) ||
                    x.CustomerCode.Contains(pagination.Search) ||
                    x.Company.CompanyName.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var result = mapper.Map<List<CustomerReadDTO>>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<CustomerReadDTO> GetById(int id)
        {
            string cacheKey = $"{CacheKeySingle}{id}";

            if (cache.TryGetValue(cacheKey, out CustomerReadDTO? cachedItem) && cachedItem != null)
            {
                return cachedItem;
            }

            var data = await db.Customer
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.CustomerId == id && x.IsActive == 1);

            if (data == null)
                return null;

            var result = mapper.Map<CustomerReadDTO>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<int> GetTotalCustomerRecords(string? search)
        {
            string cacheKey = $"{CacheKeyCount}Search_{search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var data = db.Customer
                .Include(x => x.Company)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.CustomerName.Contains(search) ||
                    x.CustomerCode.Contains(search) ||
                    x.Company.CompanyName.Contains(search)
                );
            }

            int count = await data.CountAsync();

            cache.Set(cacheKey, count, TimeSpan.FromMinutes(10));

            return count;
        }

        public async Task<bool> UpdateCustomer(int id, CustomerWriteDTO c)
        {
            var data = await db.Customer
                .FirstOrDefaultAsync(x => x.CustomerId == id && x.IsActive == 1);

            if (data == null)
                return false;

            mapper.Map(c, data);
            data.ModifiedAt = DateTime.Now;
            data.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();

            ClearCustomerCache();

            return true;
        }

        private void ClearCustomerCache()
        {
            if (cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}