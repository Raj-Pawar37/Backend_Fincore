using AutoMapper;
using Backend_Fincore.Application.DTOs.Quotation;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Domain.Models;
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


        public async Task<int> GetQuotationCount(QuotationPaginationDTO pagination)
        {
            IQueryable<Quotation> query = GetQuotationQuery(pagination);

            return await query.CountAsync();
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

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x =>x.QuotationNumber == dto.QuotationNumber && x.IsActive == 1);
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

        public async Task<List<QuotationDTO>> GetAllQuotation(QuotationPaginationDTO pagination)
        {
            if (pagination.PageNumber <= 0)
            {
                pagination.PageNumber = 1;
            }

            if (pagination.PageSize <= 0)
            {
                pagination.PageSize = 10;
            }


            IQueryable<Quotation> query = GetQuotationQuery(pagination);

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new QuotationDTO
                {
                    QuotationId = x.QuotationId,

                    RFQId = x.RFQId,
                    RFQNumber = x.RFQ.RFQNumber,

                    RFQVendorId = x.RFQVendorId,
                    VendorId = x.RFQVendor.VendorId,
                    VendorName = x.RFQVendor.Vendor.VendorName,

                    QuotationDate = x.CreatedAt,
                    QuotationNumber = x.QuotationNumber,
                    Amount = x.Amount,
                    Status = x.Status,
                    Desc = x.Description
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

            var duplicateQuotationNo = await db.Quotation.AnyAsync(x => x.QuotationNumber == dto.QuotationNumber && x.QuotationId != dto.QuotationId && x.IsActive == 1);
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







        //Helper Functions 


        private IQueryable<Quotation> GetQuotationQuery(QuotationPaginationDTO pagination)
        {
            IQueryable<Quotation> query = db.Quotation.AsNoTracking().Where(x => x.IsActive == 1);

            // Vendor filter
            if (pagination.VendorId.HasValue && pagination.VendorId.Value > 0)
            {
                int vendorId = pagination.VendorId.Value;
                query = query.Where(x =>x.RFQVendor.VendorId == vendorId);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(pagination.Status))
            {
                string status = pagination.Status.Trim();
                query = query.Where(x => x.Status == status);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string search = pagination.Search.Trim();

                query = query.Where(x =>
                    x.QuotationNumber.Contains(search) ||
                    x.RFQ.RFQNumber.Contains(search) ||
                    x.RFQ.Title.Contains(search) ||
                    x.RFQVendor.Vendor.VendorName.Contains(search) ||
                    x.Status.Contains(search));
            }

            return query;
        }

        public async Task<QuotationComparisonDTO> getQuotationComparsion(int rfqId)
        {
            var rfq = await db.RFQ.AsNoTracking().FirstOrDefaultAsync(x => x.RFQId == rfqId && x.IsActive == 1);

            if (rfq == null) throw new Exception("RFQ not found.");

            var quotationItems = await db.QuotationItem.AsNoTracking()
                .Include(x => x.RFQItem)
                .Include(x => x.Quotation)
                .ThenInclude(x => x.RFQVendor)
                .ThenInclude(x => x.Vendor)
                .Where(x => x.Quotation.RFQId == rfqId && x.IsActive == 1 && x.Quotation.IsActive == 1)
                .ToListAsync();

            var response = mapper.Map<QuotationComparisonDTO>(rfq);
            response.Items = mapper.Map<List<QuotationComparisonItemDTO>>(quotationItems);
            return response;
        }



        public async Task SelectQuotation(QuotationSelectionDTO dto)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                var rfq = await db.RFQ.FirstOrDefaultAsync(x=> x.RFQId == dto.RFQId && x.Status != "Closed" && x.IsActive == 1);
                if (rfq == null) throw new Exception("RFQ Not Found");

                var quotations = await db.Quotation.Where(x => x.RFQId == rfq.RFQId && x.IsActive == 1).ToListAsync();
                if (quotations.Count == 0) throw new Exception("No quotation Found for this RFQ");

                var quotationIds = quotations.Select(x => x.QuotationId).ToList();
                var quotationItems = await db.QuotationItem.Where(x => quotationIds.Contains(x.QuotationItemId) && x.IsActive == 1).ToListAsync();
                if (quotationItems.Count != dto.SelectedQuotationItemIds.Distinct().Count()) throw new Exception("Quoatation Selected Item May be not present in this ");

                var selectedQuotationItems = quotationItems.Where(x => dto.SelectedQuotationItemIds.Contains(x.QuotationItemId)).ToList();
                if (selectedQuotationItems.Count != dto.SelectedQuotationItemIds.Distinct().Count()) throw new Exception("Invalid quotation item selected.");
                
                var duplicateRFQ = selectedQuotationItems.GroupBy(x => x.RFQItemId).Any(x => x.Count() > 1);
                if (duplicateRFQ) throw new Exception("Multiple RFQItem Ids Has been Selected");

                //Main Logic Starts here 
                foreach (var item in quotationItems)
                {
                    item.Status = "Rejected";
                    item.ModifiedBy = currentUser.UserId;
                    item.ModifiedAt = DateTime.Now;
                }

                foreach (var item in selectedQuotationItems)
                {
                    item.Status = "Selected";
                    item.ModifiedBy = currentUser.UserId;
                    item.ModifiedAt = DateTime.Now;
                }


                var selectedQuotationIds = selectedQuotationItems.Select(x => x.QuotationId).Distinct().ToList();
                foreach (var quotation in quotations)
                {
                    quotation.Status = selectedQuotationIds.Contains(quotation.QuotationId) ? "Approved" : "Rejected";
                    quotation.ModifiedBy = currentUser.UserId;
                    quotation.ModifiedAt = DateTime.Now;
                }

                rfq.Status = "Closed";
                rfq.ModifiedBy = currentUser.UserId;
                rfq.ModifiedAt = DateTime.Now;


                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            
        }
    }
}
    