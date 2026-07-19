using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class EmployeeService :IEmployeeService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public EmployeeService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<EmployeeReadDTO> AddEmp(EmployeeWriteDTO e)
        {
            var data = mapper.Map<Employee>(e);

            await db.Employee.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Employee
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .FirstOrDefaultAsync(x => x.EmployeeId == data.EmployeeId);

            return mapper.Map<EmployeeReadDTO>(mdata);
        }

        public async Task<bool> delete(int id)
        {
            var data= await db.Employee.FindAsync(id);
            if (data == null)
            {
                return false;
            }
            else
            {
                db.Employee.Remove(data);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<EmployeeReadDTO>> GetAll()
        {
            var data = await db.Employee
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .ToListAsync();

            return mapper.Map<List<EmployeeReadDTO>>(data);
        }

        public async Task<EmployeeReadDTO> GetById(int id)
        {
            var data = await db.Employee
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

        public async Task<bool> update(int id, EmployeeWriteDTO e)
        {
            var data = await db.Employee.FindAsync(id);
            if (data == null)
            {
                return false;

            }
            var mdata = mapper.Map(e, data);
            await db.SaveChangesAsync();
            return true;
        }

    }
}
