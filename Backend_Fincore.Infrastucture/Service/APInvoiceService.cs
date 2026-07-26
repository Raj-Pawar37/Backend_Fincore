using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Models.Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Service
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public APInvoiceService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }














        //Create  API 
        public async Task AddAPInvoice(APInvoiceCUDTO AP)
        {

            var vendor = await db.Vendor.FirstOrDefaultAsync(x => x.VendorId == AP.VendorId);
            if (vendor == null)
            {
                throw new Exception("Vendor not found.");
            }

            var exist2 = await db.APInvoice.FirstOrDefaultAsync(x => x.MasterId == AP.MasterId && x.MasterType == AP.MasterType);
            if (exist2 != null)
            {
                throw new Exception($"API Invoice of this {AP.MasterType} has been Already Created");
            }

            if (AP.MasterType == "PurchaseOrder")
            {
                var purchaseOrder = await db.PurchaseOrder.FindAsync(AP.MasterId);
                if (purchaseOrder != null)
                {
                    throw new Exception("Purchase Order not found.");
                }
                if (purchaseOrder?.VendorId == AP.VendorId)
                {
                    throw new Exception("There is no Po With this Vendor");
                }
            }
            if (AP.MasterType == "WorkOrder")
            {
                var exist = await db.WorkOrder.FindAsync(AP.VendorId);
                if (exist != null)
                {
                    throw new Exception("API Invoice of this WorkOrder has been Already Created");
                }
            }

            if (AP.MasterType != "WorkOrder" && AP.MasterType != "PurchaseOrder")
            {
                throw new Exception("Invalid MasterType");
            }

            var exist3 = await db.APInvoice.FirstOrDefaultAsync(x => x.InvoiceNumber == AP.InvoiceNumber);

            if (exist3 != null)
            {
                throw new Exception("API Invoice already exist with this name");
            }







            var invoice = mapper.Map<APInvoice>(AP);
            invoice.CreatedBy = AP.CreatedBy;

            await db.APInvoice.AddAsync(invoice);
            await db.SaveChangesAsync();
        }



        //Update 
        public async Task UpdateAPInvoice(APInvoiceCUDTO AP, int id)
        {
            var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == id);
            if (invoice == null)
            {
                throw new Exception("invoice not found.");
            }

            var vendor = await db.Vendor.FirstOrDefaultAsync(x => x.VendorId == AP.VendorId);
            if (vendor == null)
            {
                throw new Exception("Vendor not found.");
            }

            if (AP.MasterType == "PurchaseOrder")
            {
                var purchaseOrder = await db.PurchaseOrder.FindAsync(AP.MasterId);
                if (purchaseOrder != null)
                {
                    throw new Exception("Purchase Order not found.");
                }
                if (purchaseOrder?.VendorId == AP.VendorId)
                {
                    throw new Exception("There is no Po With this Vendor");
                }
            }
            if (AP.MasterType == "WorkOrder")
            {
                var exist = await db.WorkOrder.FindAsync(AP.VendorId);
                if (exist != null)
                {
                    throw new Exception("API Invoice of this WorkOrder has been Already Created");
                }
            }

            if (AP.MasterType != "WorkOrder" && AP.MasterType != "PurchaseOrder")
            {
                throw new Exception("Invalid MasterType");
            }

            var exist3 = await db.APInvoice.FirstOrDefaultAsync(x => x.InvoiceNumber == AP.InvoiceNumber && x.APInvoiceId != AP.APInvoiceId);

            if (exist3 != null)
            {
                throw new Exception("API Invoice already exist with this name");
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



        //Delete API 
        public async Task<bool> DeleteInvoiceById(int id)
        {
            var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if (invoice != null)
            {
                db.APInvoice.Remove(invoice);
                await db.SaveChangesAsync();

                return true;

            }

            return false;
        }



        //Read All 
        public async Task<List<APInvoiceDTO>> GetAllAPInvoice()
        {
            //later on we will change this part 
            var userId = 1;

            var userdata = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == userId);
            List<APInvoice> res = new();
            //var userdata = await db.User.Include(x=> x.Role).FindAsync(userId);
            if (userdata == null)
            {
                throw new Exception("User data does not found");
            }

            if (userdata.Role.RoleName == "Vendor")
            {
                res = await db.APInvoice.Include(x => x.Vendor).Where(x => x.VendorId == userdata.MasterId).ToListAsync();
            }
            if (userdata.Role.RoleName == "CFO")
            {
                res = await db.APInvoice.Include(x => x.Vendor).ToListAsync();
            }
            if (userdata.Role.RoleName == "Manager" && userdata.MasterType == "Employee")
            {
                var dept = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == userdata.MasterId);
                if (dept == null)
                {
                    throw new Exception("No department for this manager");
                }

                res = await db.APInvoice.Include(x => x.Vendor).ToListAsync();

            }


            //var res = await db.APInvoice.Include(x => x.Vendor).ToListAsync();

            var data = mapper.Map<List<APInvoiceDTO>>(res);

            return data;
        }



        public async Task<APInvoiceDTO?> GetAPInvoiceById(int id)
        {
            var res = await db.APInvoice.Include(x => x.Vendor).FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if (res == null)
            {
                throw new Exception("Data not found ");
            }

            var data = mapper.Map<APInvoiceDTO>(res);

            return data;
        }






    }
}
