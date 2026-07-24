    using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CompanyService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<CompanyReadDTO> AddCompany(CompanyWriteDTO c)
        {
            var data = mapper.Map<Company>(c);

            await db.Company.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Company
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.CompanyId == data.CompanyId);

            return mapper.Map<CompanyReadDTO>(mdata);
        }

        public async Task<bool> DeleteCompany(int id)
        {
            var company = await db.Company.FindAsync(id);

            if (company == null)
                return false;

            bool hasCustomers = await db.Customer
                .AnyAsync(x => x.CompanyId == id);

            if (hasCustomers)
            {
                throw new Exception("Company cannot be deleted because it has customer records.");
            }

            db.Company.Remove(company);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<CompanyReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Company.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search)) {
                search = search.Where(x =>
                    x.CompanyName.Contains(pagination.Search)
                );
            }
            var data = await search
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var mdata = mapper.Map<List<CompanyReadDTO>>(data);

            return mdata;
        }

        public async Task<CompanyReadDTO> GetById(int id)
        {
            var gid = await db.Company
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.CompanyId == id);

            if (gid == null)
            {
                return null;
            }

            var mdata = mapper.Map<CompanyReadDTO>(gid);

            return mdata;
        }

        public async Task<int> GetTotalCompanyRecords()
        {
            return await db.Company.CountAsync();
        }

        public async Task<bool> UpdateCompany(int id, CompanyWriteDTO c)
        {
            var data = await db.Company.FindAsync(id);

            if (data == null)
            {
                return false;
            }

            mapper.Map(c, data);

            await db.SaveChangesAsync();

            return true;
        }
    }
}