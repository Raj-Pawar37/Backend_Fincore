using Backend_Fincore.Application.DTOs.QuotationItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IQuotationItemService
    {
        Task AddQuotationItem(QuotationItemCDTO dto);

        Task UpdateQuotationItem(QuotationItemUDTO dto);

        Task DeleteQuotationItem(int quotationItemId);

        Task<List<QuotationItemDTO>> GetAllQuotationItems();

        Task<QuotationItemDTO> GetQuotationItemById(int quotationItemId);

        Task<List<QuotationItemDTO>> GetQuotationItemsByQuotationId(int quotationId);
    }
}
