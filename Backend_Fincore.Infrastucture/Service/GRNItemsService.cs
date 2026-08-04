    using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.GRNItems;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend_Fincore.Infrastucture.Service
{
    public class GRNItemsService : IGRNItemsService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        private readonly ICurrentUserService current;
        public GRNItemsService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        public async Task<int> GetAllGrnItemsCount()
        {
            return await db.GRNItem.CountAsync(x => x.IsActive == 1);
        }

        public async Task<List<GRNItemsDTO>> getAllGrnItems(PaginationDTO pagination)
        {
            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            IQueryable<GRNItem> query = db.GRNItem.Include(x => x.POItem).Include(x => x.GRN).ThenInclude(x => x.PurchaseOrder)
                                           .Where(x => x.IsActive == 1);


            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                   
                    break;

                case "Vendor":
                    query = query.Where(x => x.GRN.PurchaseOrder.VendorId == user.MasterId);
                    break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                                          x.POItem.ItemName.Contains(pagination.Search) ||
                                          x.GRN.GRNNumber.Contains(pagination.Search) ||
                                          x.GRN.Status.Contains(pagination.Search));
            }

            var result = await query.OrderByDescending(x => x.CreatedAt)
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();


            return mapper.Map<List<GRNItemsDTO>>(result);
        }

        public async Task<GRNItemsDTO> GetGRNItemById(int id)
        {
            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            var data = await db.GRNItem.Include(x => x.POItem).Include(x => x.GRN).ThenInclude(x => x.PurchaseOrder)
                                       .FirstOrDefaultAsync(x => x.GRNItemId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("GRN Item not found.");
            }

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                   
                    break;

                case "Vendor":
                                if (data.GRN.PurchaseOrder.VendorId != user.MasterId)
                                {
                                    throw new Exception("You are not authorized to view this GRN Item.");
                                }
                                break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            return mapper.Map<GRNItemsDTO>(data);
        }


        public async Task DeleteGRNItem(int id)
        {
            var grnItem = await db.GRNItem.Include(x => x.GRN).FirstOrDefaultAsync(x => x.GRNItemId == id && x.IsActive == 1);

            if (grnItem == null)
            {
                throw new Exception("GRN Item not found.");
            }

            if (grnItem.GRN == null || grnItem.GRN.IsActive != 1)
            {
                throw new Exception("GRN not found.");
            }

            if (grnItem.GRN.Status != "Draft")
            {
                throw new Exception("Only Draft GRN Items can be deleted.");
            }

            grnItem.IsActive = 0;
            grnItem.ModifiedBy = current.UserId;
            grnItem.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }


        public async Task AddGRNItem(GRNItemsCUDTO dto)
        {  
            var grn = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == dto.GRNId && x.IsActive == 1);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status != "Draft")
            {
                throw new Exception("GRN Items can only be added to a Draft GRN.");
            }
     
            var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == dto.POItemId && x.IsActive == 1);

            if (poItem == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

           
            bool exists = await db.GRNItem.AnyAsync(x => x.GRNId == dto.GRNId &&
                                                         x.POItemId == dto.POItemId &&
                                                         x.IsActive == 1);

            if (exists)
            {
                throw new Exception("This Purchase Order Item already exists in the selected GRN.");
            }

           
            decimal alreadyReceived = await db.GRNItem.Where(x =>
                                                                  x.POItemId == dto.POItemId &&
                                                                  x.IsActive == 1 &&
                                                                  x.GRN.IsActive == 1 &&
                                                                  x.GRN.Status == "Received")
                                                           .SumAsync(x => (decimal?)x.Qty) ?? 0;


            decimal remainingQty = poItem.Qty - alreadyReceived;

            if (remainingQty <= 0)
            {
                throw new Exception("This Purchase Order Item has already been fully received.");
            }

            if (dto.Qty <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }

            if (dto.Qty > remainingQty)
            {
                throw new Exception($"Maximum receivable quantity is {remainingQty}.");
            }

           
            var item = mapper.Map<GRNItem>(dto);

            item.CreatedBy = current.UserId;
            item.CreatedAt = DateTime.Now;

            await db.GRNItem.AddAsync(item);
            await db.SaveChangesAsync();
        }



    }
}
