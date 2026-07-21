using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IAssetsService
    {

        Task<List<AssetsDTO>> GetAllAssetsList();

        Task<AssetsDTO> GetAssetById(int id);

        Task AddAssets(AssetsCUDTO assetDto);

        Task UpdateAssets(AssetsCUDTO assetDto, int id);

        Task<bool> DeleteAssetsByid(int id);
    }
}
