using Backend_Fincore.Application.DTOs.QuotationItem;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Backend_Fincore.Infrastucture.Service
{
    public class QuotationItemService : IQuotationItemService
    {
        private readonly AppDbContext db;
        private readonly ICurrentUserService currentUser;
        private readonly IMapper mapper;

        public QuotationItemService(AppDbContext db, ICurrentUserService currentUser, IMapper mapper)
        {
            this.db = db;
            this.currentUser = currentUser;
            this.mapper = mapper;
        }

        public async Task AddQuotationItem(QuotationItemCDTO dto)
        {


            ValidateAmounts(dto.Quantity, dto.UnitPrice, dto.Tax, dto.Discount);

            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId && x.IsActive == 1);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x =>x.RFQItemId == dto.RFQItemId && x.IsActive == 1);

            if (rfqItem == null)
            {
                throw new Exception("RFQ item not found.");
            }

            if (rfqItem.RFQId != quotation.RFQId)
            {
                throw new Exception("The selected RFQ item does not belong to the quotation RFQ.");
            }

            var duplicateItem = await db.QuotationItem.AnyAsync(x =>x.QuotationId == dto.QuotationId &&x.RFQItemId == dto.RFQItemId && x.IsActive == 1);

            if (duplicateItem)
            {
                throw new Exception("This RFQ item has already been added to the quotation.");
            }



            var data = mapper.Map<QuotationItem>(dto);
            data.IsActive = 1;
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = currentUser.UserId;

            await db.QuotationItem.AddAsync(data);
            await db.SaveChangesAsync();

            await UpdateQuotationAmount(dto.QuotationId);
        }

        public async Task UpdateQuotationItem(QuotationItemUDTO dto)
        {


            ValidateAmounts(dto.Quantity, dto.UnitPrice, dto.Tax, dto.Discount);

            var quotationItem = await db.QuotationItem.FirstOrDefaultAsync(x =>x.QuotationItemId == dto.QuotationItemId && x.IsActive == 1);

            if (quotationItem == null)
            {
                throw new Exception("Quotation item not found.");
            }

            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId && x.IsActive == 1);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x =>x.RFQItemId == dto.RFQItemId && x.IsActive == 1);

            if (rfqItem == null)
            {
                throw new Exception("RFQ item not found.");
            }

            if (rfqItem.RFQId != quotation.RFQId)
            {
                throw new Exception("The selected RFQ item does not belong to the quotation RFQ.");
            }

            var duplicateItem = await db.QuotationItem.AnyAsync(x =>
                    x.QuotationId == dto.QuotationId &&
                    x.RFQItemId == dto.RFQItemId &&
                    x.QuotationItemId != dto.QuotationItemId &&
                    x.IsActive == 1);

            if (duplicateItem)
            {
                throw new Exception("This RFQ item already exists in the quotation.");
            }



            var data = mapper.Map<QuotationItem>(dto);
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            await UpdateQuotationAmount(dto.QuotationId);
        }

        public async Task DeleteQuotationItem(int quotationItemId)
        {
            var quotationItem = await db.QuotationItem.FirstOrDefaultAsync(x =>x.QuotationItemId == quotationItemId && x.IsActive == 1);

            if (quotationItem == null)
            {
                throw new Exception("Quotation item not found.");
            }

            if (quotationItem.Status == "Selected" ||
                quotationItem.Status == "Approved")
            {
                throw new Exception($"A {quotationItem.Status} quotation item cannot be deleted.");
            }

            int quotationId =quotationItem.QuotationId;

            quotationItem.IsActive = 0;
            quotationItem.ModifiedAt = DateTime.UtcNow;
            quotationItem.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();
            await UpdateQuotationAmount(quotationId);
        }

        public async Task<List<QuotationItemDTO>> GetAllQuotationItems()
        {
            var data = await db.QuotationItem.AsNoTracking()
                .Where(x=> x.IsActive == 1)
                .OrderByDescending(x =>x.QuotationItemId)
                .ToListAsync();

            return mapper.Map<List<QuotationItemDTO>>(data);
        }

        public async Task<QuotationItemDTO> GetQuotationItemById(int quotationItemId)
        {
            var data = await db.QuotationItem.AsNoTracking()
                .Where(x =>x.QuotationItemId == quotationItemId && x.IsActive == 1)
                .FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("Quotation item not found.");
            }

            return mapper.Map<QuotationItemDTO>(data);
        }

        public async Task<List<QuotationItemDTO>> GetQuotationItemsByQuotationId(int quotationId)
        {
            var quotationExists = await db.Quotation.AnyAsync(x => x.QuotationId == quotationId && x.IsActive == 1);
            if (!quotationExists)
            {
                throw new Exception("Quotation not found.");
            }

            var data = await db.QuotationItem.AsNoTracking()
                .Where(x =>x.QuotationId == quotationId && x.IsActive == 1)
                .OrderBy(x => x.QuotationItemId)
                .ToListAsync();


            return mapper.Map<List<QuotationItemDTO>>(data);
        }






        //Helper Functions 
        private static void ValidateAmounts(int quantity, decimal unitPrice, decimal tax, decimal discount)
        {
            if (quantity <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }

            if (unitPrice <= 0)
            {
                throw new Exception("Unit price must be greater than zero.");
            }

            if (tax < 0)
            {
                throw new Exception("Tax cannot be negative.");
            }

            if (discount < 0)
            {
                throw new Exception("Discount cannot be negative.");
            }

            decimal subTotal = quantity * unitPrice;

            if (discount > subTotal + tax)
            {
                throw new Exception("Discount cannot exceed the item amount.");
            }
        }

        private async Task UpdateQuotationAmount(int quotationId)
        {
            decimal totalAmount =await db.QuotationItem.Where(x =>x.QuotationId == quotationId && x.IsActive == 1).SumAsync(x =>(decimal?)
                        ((x.Quantity * x.UnitPrice) + x.Tax - x.Discount)) ?? 0;

            var quotation = await db.Quotation.FirstOrDefaultAsync(x => x.QuotationId == quotationId && x.IsActive == 1);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            quotation.Amount = totalAmount;
            quotation.ModifiedAt =  DateTime.UtcNow;
            quotation.ModifiedBy = currentUser.UserId;
            await db.SaveChangesAsync();
        }


    }
}
