using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
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
        private readonly ICurrentUserService current;

        public UserService(AppDbContext db, IMapper mapper,ICurrentUserService current)
        {   this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        public async Task<UserReadDTO> AddUser(UserCreateDTO u)
        {
            var data = mapper.Map<User>(u);

            data.PasswordHash = BCrypt.Net.BCrypt.HashPassword(u.Password);

            data.CreatedAt = DateTime.Now;
            data.CreatedBy = current.UserId;
            data.LastLoginDate = null;

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
            var search = db.User
                .Include(x => x.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.Username.Contains(pagination.Search) ||
                    x.Email.Contains(pagination.Search)
                );
            }

            var data = await search
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

        public async Task<int> GetTotalUserRecords(string? search)
        {
            var query = db.User.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Username.Contains(search) ||
                    x.Email.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<bool> UpdateUser(int id, UserUpdateDTO u)
        {
            var data = await db.User.FindAsync(id);

            if (data == null)
            {
                return false;
            }

            mapper.Map(u, data);

            data.ModifiedBy = current.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
            return true;
        }
    }
}
