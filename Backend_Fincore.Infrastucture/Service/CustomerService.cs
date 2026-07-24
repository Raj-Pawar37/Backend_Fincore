using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        public CustomerService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser )
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        public async Task<CustomerReadDTO> AddCutomer(CustomerWriteDTO c)
        {
            var data = mapper.Map<Customer>(c);
            data.CreatedAt= DateTime.Now;
            data.CreatedBy = currentUser.UserId;
            await db.Customer.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Customer
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.CustomerId == data.CustomerId);

            return mapper.Map<CustomerReadDTO>(mdata);
        }

      

        public async Task<bool> DeleteCustomer(int id)
        {
            var customer = await db.Customer.FindAsync(id);

            if (customer == null)
                return false;

            bool hasRevenue = await db.RevenueEntry
                .AnyAsync(x => x.CustomerId == id);

            if (hasRevenue)
            {
                throw new Exception("Customer cannot be deleted because it has revenue entries.");
            }

            bool hasInvoices = await db.ARInvoice
                .AnyAsync(x => x.CustomerId == id);

            if (hasInvoices)
            {
                throw new Exception("Customer cannot be deleted because it has AR invoices.");
            }

            db.Customer.Remove(customer);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<CustomerReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Customer
                .Include(x => x.Company)
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

            return mapper.Map<List<CustomerReadDTO>>(data);
        }



        public async Task<CustomerReadDTO> GetById(int id)
        {
            var data = await db.Customer
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (data == null)
                return null;

            return mapper.Map<CustomerReadDTO>(data);
        }

        public async Task<int> GetTotalCustomerRecords(string? search)
        {
            var data = db.Customer
                .Include(x => x.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.CustomerName.Contains(search) ||
                    x.CustomerCode.Contains(search) ||
                    x.Company.CompanyName.Contains(search)
                );
            }

            return await data.CountAsync();
        }

        public async Task<bool> UpdateCustomer(int id, CustomerWriteDTO c)
        {
            var data = await db.Customer.FindAsync(id);

            if (data == null)
                return false;

            mapper.Map(c, data);
            data.ModifiedAt = DateTime.Now;
            data.ModifiedBy = currentUser.UserId;
            await db.SaveChangesAsync();

            return true;
        }
    }
}