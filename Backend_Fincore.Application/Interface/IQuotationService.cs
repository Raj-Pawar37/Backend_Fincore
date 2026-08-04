using Backend_Fincore.Application.DTOs.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IQuotationService
    {
        Task AddQuotation(QuotationCDTO dto);

        Task UpdateQuotation(QuotationUDTO dto);

        Task DeleteQuotation(int quotationId);

        Task<List<QuotationDTO>> GetAllQuotation(QuotationPaginationDTO pagination);

        Task<int> GetQuotationCount(QuotationPaginationDTO pagination);

        Task<QuotationDTO> GetQuotationById(int quotationId);

        Task<List<QuotationDTO>> GetQuotationByRFQId(int rfqId);

        Task<QuotationComparisonDTO> getQuotationComparsion(int rfqId);

        Task SelectQuotation(QuotationSelectionDTO dto);
    }
}
