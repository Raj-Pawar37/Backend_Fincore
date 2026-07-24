using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Domain.Models;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class GRNItemsService : IGRNItemsService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public GRNItemsService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<int> GetAllGrnItemsCount()
        {
            return await db.GRNItem.CountAsync();
        }

        public async Task<List<GRNItemsDTO>> getAllGrnItems(PaginationDTO pagination)
        {
            var search = db.GRN.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.GRNNumber.Contains(pagination.Search) ||

                    x.Status.Contains(pagination.Search)

                    );
            }

            var data1 = await search
                                   .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                   .Take(pagination.PageSize)
                                   .ToListAsync();

            mapper.Map<PurchaseOrderItemDTO>(data1);

            var data = await db.GRNItem.Include(x => x.POItem).ToListAsync();

            var res = mapper.Map<List<GRNItemsDTO>>(data);

            return res;
        }

        public async Task<GRNItemsDTO> GetGRNItemById(int id)
        {
            var data = await db.GRNItem.Include(x => x.GRN).Include(x => x.POItem)
                             .FirstOrDefaultAsync(x => x.GRNItemId == id);

            if (data == null)
            {
                throw new Exception("GRN Item not found.");
            }

            return mapper.Map<GRNItemsDTO>(data);
        }


        public async Task DeleteGRNItem(int id)
        {
           
            var grnItem = await db.GRNItem.Include(x => x.GRN).Include(x => x.POItem).FirstOrDefaultAsync(x => x.GRNItemId == id);


            if (grnItem == null)
            {
                throw new Exception("GRN Item not found.");
            }

            if (grnItem.GRN.Status == "Received")
            {
                throw new Exception("Received GRN Item cannot be deleted.");
            }

          
            // grnItem.POItem.Status = "Not Received";

            db.GRNItem.Remove(grnItem);

            await db.SaveChangesAsync();
        }


        public async Task AddGRNItem(GRNItemsCUDTO dto, int createdBy)
        {
           
            var grn = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == dto.GRNId);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status == "Received")
            {
                throw new Exception("Cannot add items. GRN is already Received.");
            }

            var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == dto.POItemId);

            if (poItem == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            if (poItem.Status == "Received")
            {
                throw new Exception("This Purchase Order Item is already Received.");
            }

         
            bool exists = await db.GRNItem.AnyAsync(x => x.POItemId == dto.POItemId);

            if (exists)
            {
                throw new Exception("This Purchase Order Item already exists in a GRN.");
            }

            var item = mapper.Map<GRNItem>(dto);

            item.CreatedBy = createdBy;
            item.CreatedAt = DateTime.Now;

            db.GRNItem.Add(item);

            // Update PO Item Status
            poItem.Status = "Received";      
            poItem.ModifiedBy = createdBy;
            poItem.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task<List<POItemsSearchDTO>> SearchPOItem(SearchPoiDTO dto)
        {
            var query = db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == dto.PurchaseOrderId).AsQueryable();


            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }

            
            if (!string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Where(x => x.ItemName.Contains(dto.SearchText));
            }

            
            var result = await query.OrderBy(x => x.ItemName).Take(20).ToListAsync();

            return mapper.Map<List<POItemsSearchDTO>>(result);
        }

    }
}
