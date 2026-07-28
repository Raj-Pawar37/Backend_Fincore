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
            return await db.GRNItem.CountAsync();
        }

        public async Task<List<GRNItemsDTO>> getAllGrnItems(PaginationDTO pagination)
        {
            var user = await db.User.Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == current.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            IQueryable<GRNItem> query = db.GRNItem
                                          .Include(x => x.POItem)
                                          .Include(x => x.GRN)
                                          .ThenInclude(x => x.PurchaseOrder)
                                          .Where(x => x.IsActive == 1);


            if (user.Role.RoleName == "User" || user.Role.RoleName == "Employee")
            {
                throw new Exception("You are not authorized.");
            }

            // Manager / Senior Manager
            else if (user.Role.RoleName == "Manager" || user.Role.RoleName == "Senior Manager")

            {
                var employee = await db.Employee
                                      .FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId && x.IsActive == 1);

                if (employee == null)
                {
                    throw new Exception("Employee not found.");
                }

                var empIds = await db.Employee
                                     .Where(x => x.DepartmentId == employee.DepartmentId && x.IsActive == 1)
                                     .Select(x => x.EmployeeId)
                                     .ToListAsync();

                var userIds = await db.User.Where(x => x.MasterType == "Employee"
                                            && x.IsActive == 1
                                            && empIds.Contains(x.MasterId)
                                            && (x.Role.RoleName == "Manager"
                                             || x.Role.RoleName == "Senior Manager"))
                                             .Select(x => x.UserId)
                                             .ToListAsync();

                query = query.Where(x => userIds.Contains(x.GRN.CreatedBy));
            }


            else if (user.Role.RoleName == "Vendor")
            {
                query = query.Where(x => x.GRN.PurchaseOrder.VendorId == user.MasterId);
            }


            else if (user.Role.RoleName == "CFO")
            {

            }
            else
            {
                throw new Exception("Invalid role.");
            }


            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>x.POItem.ItemName.Contains(pagination.Search) ||
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
            var data = await db.GRNItem
                            .Include(x => x.GRN)
                            .Include(x => x.POItem)
                            .FirstOrDefaultAsync(x => x.GRNItemId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("GRN Item not found.");
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

            if (grnItem.GRN.Status == "Received")
            {
                throw new Exception("Received GRN Item cannot be deleted.");
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

            if (grn.Status == "Received")
            {
                throw new Exception("Cannot add items. GRN is already Received.");
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
                throw new Exception("Purchase Order Item already exists in this GRN.");
            }


            decimal alreadyReceived = await db.GRNItem.Where(x => x.POItemId == dto.POItemId && x.IsActive == 1 &&
                                                        x.GRN.Status == "Received")
                                                       .SumAsync(x => x.Qty);

            if (alreadyReceived + dto.Qty > poItem.Qty)
            {
                throw new Exception($"Maximum receivable quantity is {poItem.Qty - alreadyReceived}.");
            }

            var item = mapper.Map<GRNItem>(dto);

            item.CreatedAt = DateTime.Now;
            item.CreatedBy = current.UserId;

            await db.GRNItem.AddAsync(item);
            await db.SaveChangesAsync();
        }



    }
}
