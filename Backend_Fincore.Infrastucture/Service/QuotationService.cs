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
        private readonly ICurrentUserService currentUser;


        public QuotationService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }


        public async Task AddQuotation(QuotationCDTO dto)
        {
            var rfqExists = await db.RFQ.AnyAsync(x => x.RFQId == dto.RFQId && x.IsActive == 1);
            if (!rfqExists)
            {
                throw new Exception("RFQ not found.");
            }

            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == dto.RFQVendorId && x.RFQId == dto.RFQId && x.IsActive == 1);
            if (rfqVendor == null)
            {
                throw new Exception("The selected vendor does not belong to this RFQ.");
            }

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x =>x.QuotationNumber == dto.QuotationNumber);
            if (duplicateQuotationNo)
            {
                throw new Exception("Quotation number already exists.");
            }

            var existingVendorQuotation = await db.Quotation.AnyAsync(x => x.RFQId == dto.RFQId && x.RFQVendorId == dto.RFQVendorId && x.IsActive == 1);
            if (existingVendorQuotation)
            {
                throw new Exception("This vendor has already submitted a quotation for this RFQ.");
            }

            var quotation = mapper.Map<Quotation>(dto);
            quotation.CreatedBy = currentUser.UserId;
            quotation.CreatedAt = DateTime.UtcNow;
            quotation.IsActive = 1;

            await db.Quotation.AddAsync(quotation);
            await db.SaveChangesAsync();

        }

        public async Task DeleteQuotation(int quotationId)
        {
            var quotation = await db.Quotation.FirstOrDefaultAsync(x => x.QuotationId == quotationId && x.IsActive == 1);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            if (quotation.Status == "Selected" || quotation.Status == "Approved")
            {
                throw new Exception($"A {quotation.Status} quotation cannot be deleted.");
            }

            quotation.IsActive = 0;
            quotation.ModifiedBy = currentUser.UserId;
            quotation.ModifiedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

        }

        public async Task<List<QuotationDTO>> GetAllQuotation()
        {
            var data = await db.Quotation
                .AsNoTracking()
                .Where(x=> x.IsActive == 1)
                .Select(x => new QuotationDTO
                {
                    QuotationId = x.QuotationId,
                    RFQId = x.RFQId,
                    RFQNumber = x.RFQ.RFQNumber,
                    RFQVendorId = x.RFQVendorId,
                    QuotationDate = x.CreatedAt,
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

        public async Task<QuotationDTO> GetQuotationById(int quotationId)
        {
            var data = await db.Quotation.AsNoTracking().Where(x => x.QuotationId == quotationId && x.IsActive == 1)
                .Select(x => new QuotationDTO
                {
                    QuotationId = x.QuotationId,
                    RFQId = x.RFQId,
                    RFQVendorId = x.RFQVendorId,
                    QuotationNumber = x.QuotationNumber,
                    Amount = x.Amount,
                    Status = x.Status,
                    Desc = x.Description,
                    VendorName = x.RFQVendor.Vendor.VendorName
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("Quotation not found.");
            }

            return data;

        }

        public async Task<List<QuotationDTO>> GetQuotationByRFQId(int rfqId)
        {
            var rfqExists = await db.RFQ.AnyAsync(x => x.RFQId == rfqId && x.IsActive == 1);
            if (!rfqExists)
            {
                throw new Exception("RFQ not found.");
            }

            var data = await db.Quotation.AsNoTracking().Where(x =>x.RFQId == rfqId && x.IsActive == 1)
                .OrderBy(x => x.Amount)
                .ToListAsync();

            return mapper.Map<List<QuotationDTO>>(data);
        }

        public async Task UpdateQuotation(QuotationUDTO dto)
        {
            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId && x.IsActive == 1);
            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqExists = await db.RFQ.AnyAsync(x => x.RFQId == dto.RFQId && x.IsActive == 1);
            if (!rfqExists)
            {
                throw new Exception("RFQ not found.");
            }

            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x =>x.RFQVendorId == dto.RFQVendorId && x.RFQId == dto.RFQId && x.IsActive == 1);
            if (rfqVendor == null)
            {
                throw new Exception("The selected vendor does not belong to this RFQ.");
            }

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x => x.QuotationNumber == dto.QuotationNumber && x.QuotationId != dto.QuotationId);
            if (duplicateQuotationNo)
            {
                throw new Exception("Another quotation already exists with this quotation number.");
            }

            var vendorQuotationExists = await db.Quotation.AnyAsync(x => x.RFQId == dto.RFQId &&
                x.RFQVendorId == dto.RFQVendorId &&
                x.QuotationId != dto.QuotationId &&
                x.IsActive == 1);

            if (vendorQuotationExists)
            {
                throw new Exception("This vendor has already submitted another quotation for this RFQ.");
            }

            quotation.RFQId = dto.RFQId;
            quotation.RFQVendorId = dto.RFQVendorId;
            quotation.QuotationNumber = dto.QuotationNumber;
            quotation.Status = dto.Status;
            quotation.Description = dto.Desc;

            quotation.ModifiedBy = currentUser.UserId;
            quotation.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();


        }
    }
}
