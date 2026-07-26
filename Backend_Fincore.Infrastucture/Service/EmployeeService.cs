using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public EmployeeService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        public async Task<EmployeeReadDTO> AddEmp(EmployeeWriteDTO e)
        {
            var data = mapper.Map<Employee>(e);
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = currentUser.UserId;

            await db.Employee.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .FirstOrDefaultAsync(x => x.EmployeeId == data.EmployeeId);

            return mapper.Map<EmployeeReadDTO>(mdata);
        }

        public async Task<bool> delete(int id)
        {
            var data = await db.Employee.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            db.Employee.Remove(data);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<EmployeeReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.FirstName.Contains(pagination.Search) ||
                    x.LastName.Contains(pagination.Search) ||
                    x.EmployeeCode.Contains(pagination.Search) ||
                    x.Company.CompanyName.Contains(pagination.Search) ||
                    x.Department.DepartmentName.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<EmployeeReadDTO>>(data);
        }

        public async Task<EmployeeReadDTO> GetById(int id)
        {
            var data = await db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .FirstOrDefaultAsync(x => x.EmployeeId == id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<EmployeeReadDTO>(data);
        }

        public async Task<int> GetTotalEmployeeRecords(string? search)
        {
            var query = db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.EmployeeCode.Contains(search) ||
                    x.Company.CompanyName.Contains(search) ||
                    x.Department.DepartmentName.Contains(search)
                );
            }

            return await query.CountAsync();
        }

        public async Task<bool> update(int id, EmployeeWriteDTO e)
        {
            var data = await db.Employee.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            mapper.Map(e, data);
            data.ModifiedAt = DateTime.UtcNow;
            data.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();
            return true;
        }
    }
}