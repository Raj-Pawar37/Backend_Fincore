using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Service
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public APInvoiceService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<APInvoiceDTO>> GetAllAPInvoice()
        {
            var res = await db.APInvoice.Include(x => x.Vendor).ToListAsync();

            var data = mapper.Map<List<APInvoiceDTO>>(res);

            return data;
        }



        public async Task<APInvoiceDTO?> GetAPInvoiceById(int id)
        {
            var res = await db.APInvoice.Include(x => x.Vendor).FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if (res == null)
            {
                return null;
            }

            var data = mapper.Map<APInvoiceDTO>(res);

            return data;
        }

        public async Task AddAPInvoice(APInvoiceCUDTO AP)
        {
          
            var vendor = await db.Vendor.FirstOrDefaultAsync(x => x.VendorId == AP.VendorId);

            if (vendor == null)
            {
                throw new Exception("Vendor not found.");
            }

            var invoice = mapper.Map<APInvoice>(AP);

   
            var currentYear = DateTime.Now.Year;

            var lastINV = await db.APInvoice.Where(x => x.CreatedAt.Year == currentYear)
                                  .OrderByDescending(x => x.APInvoiceId)
                                  .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastINV != null)
            {
                var parts = lastINV.InvoiceNumber.Split('-');
                nextNumber = int.Parse(parts[2]) + 1;
            }

            invoice.InvoiceNumber = $"AST-{currentYear}-{nextNumber:D4}";

            // Temporary until JWT Authentication
            //invoice.CreatedBy=userid
            invoice.CreatedBy = AP.CreatedBy;

            await db.APInvoice.AddAsync(invoice);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAPInvoice(APInvoiceCUDTO AP, int id)
        {
            var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if (invoice == null)
            {
                throw new Exception("AP Invoice not found.");
            }
           
            var vendor = await db.Vendor.FirstOrDefaultAsync(x => x.VendorId == AP.VendorId);

            if (vendor == null)
            {
                throw new Exception("Vendor not found.");
            }

            invoice.VendorId = AP.VendorId;
            invoice.MasterId = AP.MasterId;
            invoice.MasterType = AP.MasterType;
            invoice.InvoiceAmount = AP.InvoiceAmount;
            invoice.InvoiceDate = AP.InvoiceDate;
            invoice.Status = AP.Status;

            // Temporary until JWT Authentication
            //invoice.ModifiedBy=userid

            invoice.ModifiedBy = AP.ModifiedBy;
            invoice.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task<bool> DeleteInvoiceById(int id)
        {
            var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if(invoice != null)
            {
                db.APInvoice.Remove(invoice);
                await db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
