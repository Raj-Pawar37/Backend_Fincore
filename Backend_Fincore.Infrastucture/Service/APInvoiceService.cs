using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

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

            int currentYear = DateTime.Now.Year;

            int count = await db.APInvoice.CountAsync(x => x.CreatedAt.Year == currentYear);

            invoice.InvoiceNumber = $"AP-{currentYear}-{(count + 1):D4}";

            // Temporary until JWT Authentication
            //invoice.CreatedBy=userid
            invoice.CreatedBy = 1;

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

            invoice.ModifiedBy = 1;
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
