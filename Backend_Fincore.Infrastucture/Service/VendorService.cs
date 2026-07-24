using AutoMapper;
using Backend_Fincore.Application.DTOs;
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

        public VendorService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<VendorReadDTO> AddVendor(VendorWriteDTO v)
        {
            var data = mapper.Map<Vendor>(v);

            await db.Vendor.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Vendor
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.VendorId == data.VendorId);

            return mapper.Map<VendorReadDTO>(mdata);
        }

        public async Task<List<VendorReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.Company.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.CompanyName.Contains(pagination.Search)
                );
            }

            var data = await db.Vendor
                .Include(x => x.Company)
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

        

        public async Task<int> GetTotalVendorRecord()
        {
               return await db.Vendor.CountAsync();
         }
    }
}