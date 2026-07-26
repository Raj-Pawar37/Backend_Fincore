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

namespace Backend_Fincore.Infrastucture.Service
{
    public class QuotationItemService : IQuotationItemService
    {
        private readonly AppDbContext db;

        public QuotationItemService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task AddQuotationItem(QuotationItemCUDTO dto)
        {
            int userId = 1;

            ValidateAmounts(dto);

            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x =>x.RFQItemId == dto.RFQItemId);

            if (rfqItem == null)
            {
                throw new Exception("RFQ item not found.");
            }

            if (rfqItem.RFQId != quotation.RFQId)
            {
                throw new Exception("The selected RFQ item does not belong to the quotation RFQ.");
            }

            var duplicateItem = await db.QuotationItem.AnyAsync(x =>x.QuotationId == dto.QuotationId &&x.RFQItemId == dto.RFQItemId);

            if (duplicateItem)
            {
                throw new Exception("This RFQ item has already been added to the quotation.");
            }

            var quotationItem = new QuotationItem
            {
                QuotationId = dto.QuotationId,
                RFQItemId = dto.RFQItemId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Tax = dto.Tax,
                Discount = dto.Discount,
                Status = dto.Status,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1
            };

            await db.QuotationItem.AddAsync(quotationItem);
            await db.SaveChangesAsync();

            await UpdateQuotationAmount(dto.QuotationId);
        }

        public async Task UpdateQuotationItem(QuotationItemCUDTO dto)
        {
            int userId = 1;

            ValidateAmounts(dto);

            var quotationItem = await db.QuotationItem.FirstOrDefaultAsync(x =>x.QuotationItemId == dto.QuotationItemId);

            if (quotationItem == null)
            {
                throw new Exception("Quotation item not found.");
            }

            var quotation = await db.Quotation.FirstOrDefaultAsync(x =>x.QuotationId == dto.QuotationId);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x =>x.RFQItemId == dto.RFQItemId);

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
                    x.QuotationItemId != dto.QuotationItemId);

            if (duplicateItem)
            {
                throw new Exception("This RFQ item already exists in the quotation.");
            }

            int oldQuotationId =quotationItem.QuotationId;

            quotationItem.QuotationId = dto.QuotationId;
            quotationItem.RFQItemId = dto.RFQItemId;
            quotationItem.Quantity = dto.Quantity;
            quotationItem.UnitPrice = dto.UnitPrice;
            quotationItem.Tax = dto.Tax;
            quotationItem.Discount = dto.Discount;
            quotationItem.Status = dto.Status;
            quotationItem.ModifiedBy = userId;
            quotationItem.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            await UpdateQuotationAmount(dto.QuotationId);

            if (oldQuotationId != dto.QuotationId)
            {
                await UpdateQuotationAmount(oldQuotationId);
            }
        }

        public async Task DeleteQuotationItem(int quotationItemId)
        {
            var quotationItem = await db.QuotationItem.FirstOrDefaultAsync(x =>x.QuotationItemId == quotationItemId);

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

            db.QuotationItem.Remove(quotationItem);
            await db.SaveChangesAsync();
            await UpdateQuotationAmount(quotationId);
        }

        public async Task<List<QuotationItemDTO>> GetAllQuotationItems()
        {
            var data = await db.QuotationItem.AsNoTracking().OrderByDescending(x =>x.QuotationItemId)
                .Select(x => new QuotationItemDTO
                {
                    QuotationItemId =x.QuotationItemId,
                    QuotationId =x.QuotationId,
                    RFQItemId =x.RFQItemId,
                    ItemName =x.RFQItem.Name,
                    Quantity = x.Quantity,
                    UnitPrice =x.UnitPrice,
                    Tax = x.Tax,
                    Discount = x.Discount,
                    SubTotal = x.Quantity * x.UnitPrice,
                    TotalAmount =(x.Quantity * x.UnitPrice) + x.Tax - x.Discount,
                    Status = x.Status
                }).ToListAsync();
            return data;
        }

        public async Task<QuotationItemDTO> GetQuotationItemById(int quotationItemId)
        {
            var data = await db.QuotationItem.AsNoTracking().Where(x =>x.QuotationItemId == quotationItemId)
               .Select(x => new QuotationItemDTO
               {
                   QuotationItemId = x.QuotationItemId,
                   QuotationId = x.QuotationId,
                   RFQItemId = x.RFQItemId,
                   ItemName = x.RFQItem.Name,
                   Quantity = x.Quantity,
                   UnitPrice = x.UnitPrice,
                   Tax = x.Tax,
                   Discount = x.Discount,
                   SubTotal = x.Quantity * x.UnitPrice,
                   TotalAmount =(x.Quantity * x.UnitPrice) + x.Tax - x.Discount,
                   Status = x.Status
               }).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("Quotation item not found.");
            }

            return data;
        }

        public async Task<List<QuotationItemDTO>> GetQuotationItemsByQuotationId(int quotationId)
        {
            var quotationExists = await db.Quotation.AnyAsync(x => x.QuotationId == quotationId);
            if (!quotationExists)
            {
                throw new Exception("Quotation not found.");
            }

            var data = await db.QuotationItem.AsNoTracking()
                .Where(x =>x.QuotationId == quotationId)
                .OrderBy(x => x.QuotationItemId)
                .Select(x => new QuotationItemDTO
                {
                    QuotationItemId = x.QuotationItemId,
                    QuotationId = x.QuotationId,
                    RFQItemId = x.RFQItemId,
                    ItemName =x.RFQItem.Name,
                    Quantity = x.Quantity,
                    UnitPrice =x.UnitPrice,
                    Tax =x.Tax,
                    Discount = x.Discount,
                    SubTotal = x.Quantity * x.UnitPrice,
                    TotalAmount = (x.Quantity * x.UnitPrice) + x.Tax - x.Discount,
                    Status =x.Status
                }).ToListAsync();
            return data;
        }






        //Helper Functions 
        private static void ValidateAmounts(QuotationItemCUDTO dto)
        {
            if (dto.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }

            if (dto.UnitPrice <= 0)
            {
                throw new Exception("Unit price must be greater than zero.");
            }

            if (dto.Tax < 0)
            {
                throw new Exception("Tax cannot be negative.");
            }

            if (dto.Discount < 0)
            {
                throw new Exception("Discount cannot be negative.");
            }

            decimal subTotal = dto.Quantity * dto.UnitPrice;

            if (dto.Discount > subTotal + dto.Tax)
            {
                throw new Exception("Discount cannot exceed the item amount.");
            }
        }

        private async Task UpdateQuotationAmount(int quotationId)
        {
            decimal totalAmount =await db.QuotationItem.Where(x =>x.QuotationId == quotationId).SumAsync(x =>(decimal?)
                        ((x.Quantity * x.UnitPrice) + x.Tax - x.Discount)) ?? 0;

            var quotation = await db.Quotation.FirstOrDefaultAsync(x => x.QuotationId == quotationId);

            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

            quotation.Amount = totalAmount;
            quotation.ModifiedAt =DateTime.UtcNow;
            await db.SaveChangesAsync();
        }


    }
}
