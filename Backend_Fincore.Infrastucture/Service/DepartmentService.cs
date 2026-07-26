using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Department;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models.Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class DepartmentService:IDepartmentService
    {
        AppDbContext db;
        IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public DepartmentService(AppDbContext db,IMapper mapper,ICurrentUserService currentUser) {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<int> GetTotalRecordDepartment()
        {
            return await db.Department.CountAsync();
        }
        public async Task<List<DepartmentReadDTO>>GetAll(PaginationDTO pagination)
        {
            var search = db.Department.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search)) {
                search = search.Where(x =>
                    x.DepartmentName.Contains(pagination.Search)||
                    x.DepartmentCode.Contains(pagination.Search)
                    );
            
            }
            var data = await search.Include(x => x.Company)
                                          .Where(x => x.IsActive == 1)
                                          .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                          .Take(pagination.PageSize)
                                          .ToListAsync();
            //.Where(x => x.IsActive == 1) if we want the departement who's status are active 
            return mapper.Map<List<DepartmentReadDTO>>(data);
        }
        public async Task<DepartmentReadDTO>GetById(int id)
        {
            var data = await db.Department.Include(x => x.Company).FirstOrDefaultAsync( x => x.DepartmentId == id );

            if (data == null)
            {
                throw new Exception("Department not found.");
            }

            return mapper.Map<DepartmentReadDTO>(data);
        }
        public async Task<DepartmentReadDTO>AddDepartment(DepartmentWriteDTO dto)
        {
            var companyExists = await db.Company.AnyAsync(x =>
                    x.CompanyId == dto.CompanyId &&
                    x.IsActive == 1);


            if (!companyExists)
            {
                throw new Exception("Invalid Company Id.");
            }
            var data = mapper.Map<Department>(dto);

            data.CreatedBy = currentUser.UserId;

            data.CreatedAt = DateTime.Now;

            await db.Department.AddAsync(data);

            await db.SaveChangesAsync();
            data = await db.Department.Include(x => x.Company).FirstOrDefaultAsync(x => x.DepartmentId == data.DepartmentId);
            return mapper.Map<DepartmentReadDTO>(data);
        }
        public async Task UpdateDepartment(int id,DepartmentWriteDTO dto)
        {
            var data = await db.Department .FindAsync(id);
            if (data == null)
            {
                throw new Exception("Department not found.");
            }
            mapper.Map(dto, data);
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }
        public async Task DeleteDepartment( int id)
        {
            var data = await db.Department.FindAsync(id);

            if (data == null)
            {
                throw new Exception( "Department not found." );
            }

            data.IsActive = 0;

            data.ModifiedBy = currentUser.UserId;

            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task<List<DepartmentDropdownDTO>>GetDepartmentDropdown(string? searchText)
        {
            var search = db.Department.Where(x => x.IsActive == 1).AsQueryable();


            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x =>x.DepartmentName.Contains(searchText));
            }


            var data = await search
                            .OrderBy(x => x.DepartmentName)
                            .Take(20)
                            .Select(x => new DepartmentDropdownDTO
                            {
                                DepartmentId = x.DepartmentId,
                                DepartmentName = x.DepartmentName
                            })
                            .ToListAsync();
            return data;
        }
      

    }
}
