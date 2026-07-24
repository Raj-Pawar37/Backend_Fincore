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

        public CustomerService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<CustomerReadDTO> AddCutomer(CustomerWriteDTO c)
        {
            var data = mapper.Map<Customer>(c);

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

            var search =  db.Customer.AsQueryable();
            if(!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x => x.Company.CompanyName.Contains(pagination.Search));
            }

            var data = await db.Customer
                .Include(x => x.Company)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var mdata= mapper.Map<List<CustomerReadDTO>>(data);
            return mdata;
        }

        public Task<List<CustomerReadDTO>> GetAll(LoginDto dto)
        {
            throw new NotImplementedException();
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

        public async Task<int> GetTotalCustomerRecords()
        {
            return await db.Customer.CountAsync(); 
        }

        public async Task<bool> UpdateCustomer(int id, CustomerWriteDTO c)
        {
            var data = await db.Customer.FindAsync(id);

            if (data == null)
                return false;

            mapper.Map(c, data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}