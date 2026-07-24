using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IGRNItemsService
    {

        Task<List<GRNItemsDTO>> getAllGrnItems(PaginationDTO pagination);

        Task<int> GetAllGrnItemsCount();
        Task<GRNItemsDTO> GetGRNItemById(int id);


        Task DeleteGRNItem(int id);


        Task AddGRNItem(GRNItemsCUDTO dto, int createdBy);

        Task<List<POItemsSearchDTO>> SearchPOItem(SearchPoiDTO dto);
    }
}
