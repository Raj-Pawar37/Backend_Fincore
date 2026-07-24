using AutoMapper;
using Backend_Fincore.Application.DTOs.Quotation;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
{
    public class QuotationService : IQuotationService
    {

        private readonly AppDbContext db;
        private readonly IMapper mapper;



        public QuotationService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        public async Task AddQuotation(QuotationCUDTO dto)
        {
            int userId = 1;
            var rfqExists = await db.RFQ.AnyAsync(x => x.RFQId == dto.RFQId);
            if (!rfqExists)
            {
                throw new Exception("RFQ not found.");
            }

            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == dto.RFQVendorId && x.RFQId == dto.RFQId);
            if (rfqVendor == null)
            {
                throw new Exception("The selected vendor does not belong to this RFQ.");
            }

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x =>x.QuotationNumber == dto.QuotationNumber);
            if (duplicateQuotationNo)
            {
                throw new Exception("Quotation number already exists.");
            }

            var existingVendorQuotation = await db.Quotation.AnyAsync(x => x.RFQId == dto.RFQId && x.RFQVendorId == dto.RFQVendorId);
            if (existingVendorQuotation)
            {
                throw new Exception("This vendor has already submitted a quotation for this RFQ.");
            }

            var quotation = mapper.Map<Quotation>(dto);
            quotation.CreatedBy = userId;
            quotation.CreatedAt = DateTime.UtcNow;
            quotation.IsActive = 1;

            await db.Quotation.AddAsync(quotation);
            await db.SaveChangesAsync();

        }

        public async Task DeleteQuotation(int quotationId)
        {
            var quotation = await db.Quotation.FirstOrDefaultAsync(x => x.QuotationId == quotationId);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            if (quotation.Status == "Selected" || quotation.Status == "Approved")
            {
                throw new Exception($"A {quotation.Status} quotation cannot be deleted.");
            }

            db.Quotation.Remove(quotation);
            await db.SaveChangesAsync();

        }

        public async Task<List<QuotationDTO>> GetAllQuotation()
        {
            var data = await db.Quotation
                .AsNoTracking()
                .Select(x => new QuotationDTO
                {
                    QuotationId = x.QuotationId,
                    RFQId = x.RFQId,
                    RFQVendorId = x.RFQVendorId,
                    QuotationNumber = x.QuotationNumber,
                    Amount = x.Amount,
                    Status = x.Status,
                    Desc = x.Description,
                    // Change navigation properties as per your model.
                    VendorName = x.RFQVendor.Vendor.VendorName
                })
                .ToListAsync();

            return data;
        }

        public Task<QuotationDTO> GetQuotationById(int quotationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<QuotationDTO>> GetQuotationByRFQId(int rfqId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateQuotation(QuotationCUDTO dto)
        {
            int userId = 1;
            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId );
            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqExists = await db.RFQ.AnyAsync(x => x.RFQId == dto.RFQId);
            if (!rfqExists)
            {
                throw new Exception("RFQ not found.");
            }

            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x =>x.RFQVendorId == dto.RFQVendorId && x.RFQId == dto.RFQId);
            if (rfqVendor == null)
            {
                throw new Exception("The selected vendor does not belong to this RFQ.");
            }

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x => x.QuotationNumber == dto.QuotationNumber && x.QuotationId != dto.QuotationId);
            if (duplicateQuotationNo)
            {
                throw new Exception("Another quotation already exists with this quotation number.");
            }

            var vendorQuotationExists = await db.Quotation.AnyAsync(x =>x.RFQId == dto.RFQId && x.RFQVendorId == dto.RFQVendorId && x.QuotationId != dto.QuotationId);

            if (vendorQuotationExists)
            {
                throw new Exception("This vendor has already submitted another quotation for this RFQ.");
            }

            quotation.RFQId = dto.RFQId;
            quotation.RFQVendorId = dto.RFQVendorId;
            quotation.QuotationNumber = dto.QuotationNumber;
            quotation.Status = dto.Status;
            quotation.Description = dto.Desc;

            quotation.ModifiedBy = userId;
            quotation.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();


        }
    }
}
