using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Backend_Fincore.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public UserService(AppDbContext db, IMapper mapper)
        {   this.db = db;
            this.mapper = mapper;
        }

        public async Task<UserReadDTO> AddUser(UserWriteDTO u)
        {
            var data = mapper.Map<User>(u);

            await db.User.AddAsync(data);
            await db.SaveChangesAsync();

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == data.UserId);

            return mapper.Map<UserReadDTO>(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var data=await db.User.FindAsync(id);
            if (data == null)
            {
                return false;
            }
            else
            {
                db.User.Remove(data);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<UserReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Company.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.CompanyName.Contains(pagination.Search)
                );
            }
            var data = await db.User
                .Include(x => x.Role)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<UserReadDTO>>(data);
        }

        public async Task<UserReadDTO?> GetById(int id)
        {
            var data = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (data == null)
                return null;

            return mapper.Map<UserReadDTO>(data);
        }

        public async Task<int> GetTotalUserRecords()
        {
            return await db.User.CountAsync();
        }

        public async Task<bool> UpdateUser(int id,UserWriteDTO u)
        {
            var data = await db.User.FindAsync(id);
            if (data == null)
            {
                return false;

            }
            var mdata = mapper.Map(u, data);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
