using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public VendorService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
       

        public async Task<VendorReadDTO> AddVendor(VendorWriteDTO v)
        {
            var data = mapper.Map<Vendor>(v);
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

            await db.Vendor.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Vendor
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.VendorId == data.VendorId);

            return mapper.Map<VendorReadDTO>(mdata);
        }

        public async Task<List<VendorReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Vendor
                .Include(x => x.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.VendorName.Contains(pagination.Search) ||
                    x.VendorCode.Contains(pagination.Search) ||
                    x.Company.CompanyName.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<VendorReadDTO>>(data);
        }

        public async Task<VendorReadDTO> GetById(int id)
        {
            var data = await db.Vendor
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.VendorId == id);

            if (data == null)
            {
                return null;
            }

            return mapper.Map<VendorReadDTO>(data);
        }

        public async Task<bool> UpdateVendor(int id, VendorWriteDTO v)
        {
            var data = await db.Vendor.FindAsync(id);

            if (data == null)
            {
                return false;
            }

            mapper.Map(v, data);
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVendor(int id)
        {
            var data = await db.Vendor.FindAsync(id);

            if (data == null)
            {
                return false;
            }

            bool isUsed =
                await db.RFQVendor.AnyAsync(x => x.VendorId == id) ||
                await db.PurchaseOrder.AnyAsync(x => x.VendorId == id) ||
                await db.APInvoice.AnyAsync(x => x.VendorId == id) ||
                await db.WorkOrder.AnyAsync(x => x.VendorId == id);

            if (isUsed)
            {
                throw new Exception("Vendor cannot be deleted because it is associated with other records.");
            }

            db.Vendor.Remove(data);

            await db.SaveChangesAsync();

            return true;
        }



        public async Task<int> GetTotalVendorRecord(string? search)
        {
            var data = db.Vendor
                .Include(x => x.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.VendorName.Contains(search) ||
                    x.VendorCode.Contains(search) ||
                   
                    x.Company.CompanyName.Contains(search));
            }

            return await data.CountAsync();
        }
    }
}