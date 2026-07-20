using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class AssetsService : IAssetsService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        public AssetsService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddAssets(AssetsCUDTO assetDto)
        {
            var grn = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == assetDto.GRNId);

            if(grn == null)
            {
                throw new Exception("GRN not found");
            }

            var purchaseItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == assetDto.POItemId);

            if(purchaseItem == null)
            {
                throw new Exception("Purchase Order Item not found");

            }

            if(assetDto.AssignedTo != null)
            {
                var user = await db.User.FirstOrDefaultAsync(x => x.UserId == assetDto.AssignedTo);

                if (user == null)
                {
                    throw new Exception("Assigned User not found.");
                }
            }

            var res = mapper.Map<Asset>(assetDto);

            var currentYear = DateTime.Now.Year;

            var lastAsset = await db.Asset.Where(x => x.CreatedAt.Year == currentYear)
                                  .OrderByDescending(x => x.AssetId)
                                  .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastAsset != null)
            {
                var parts = lastAsset.AssetCode.Split('-');
                nextNumber = int.Parse(parts[2]) + 1;   
            }

            res.AssetCode = $"AST-{currentYear}-{nextNumber:D4}";

            //res.CreatedBy=userid
            res.CreatedBy = 1;

            await db.Asset.AddAsync(res);
            await db.SaveChangesAsync();


        }

        public async Task<bool> DeleteAssetsByid(int id)
        {
            var asset = await db.Asset.FirstOrDefaultAsync(x => x.AssetId == id);

            if(asset != null)
            {
                db.Asset.Remove(asset);
                await db.SaveChangesAsync();

                return true;
            }

            return false;

                    
        }

        public async Task<List<AssetsDTO>> GetAllAssetsList()
        {
            var res = await db.Asset.Include(x => x.GRN).Include(x => x.PurchaseOrderItem)
                            .Include(x => x.AssignedToUser).ToListAsync();

            var data = mapper.Map<List<AssetsDTO>>(res);

            return data;
            
        }

        public async Task<AssetsDTO> GetAssetById(int id)
        {
            var res = await db.Asset.Include(x => x.GRN).Include(x => x.PurchaseOrderItem)
                           .Include(x => x.AssignedToUser).FirstOrDefaultAsync(x => x.AssetId == id);

            var data = mapper.Map<AssetsDTO>(res);

            return data;
        }

        public async Task UpdateAssets(AssetsCUDTO assetDto, int id)
        {
            var assets = await db.Asset.FirstOrDefaultAsync(x => x.AssetId == id);

            if (assets == null)
            {
                throw new Exception("Asset not found.");
            }

            
            var grn = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == assetDto.GRNId);


            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            
            var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == assetDto.POItemId);


            if (poItem == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            
            if (assetDto.AssignedTo.HasValue)
            {
                var user = await db.User.FirstOrDefaultAsync(x => x.UserId == assetDto.AssignedTo.Value);


                if (user == null)
                {
                    throw new Exception("Assigned User not found.");
                }
            }

            assets.GRNId = assetDto.GRNId;
            assets.POItemId = assetDto.POItemId;
            assets.AssetName = assetDto.AssetName;
            assets.PurchaseCost = assetDto.PurchaseCost;
            assets.PurchaseDate = assetDto.PurchaseDate;
            assets.Status = assetDto.Status;
            assets.AssignedTo = assetDto.AssignedTo;

           
            //assets.ModifiedBy = userid
            assets.ModifiedBy = 1;
            assets.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();


        }
    }
}
