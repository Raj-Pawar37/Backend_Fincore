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
        Task AddQuotation(QuotationCUDTO dto);

        Task UpdateQuotation(QuotationCUDTO dto);

        Task DeleteQuotation(int quotationId);

        Task<List<QuotationDTO>> GetAllQuotation();

        Task<QuotationDTO> GetQuotationById(int quotationId);

        Task<List<QuotationDTO>> GetQuotationByRFQId(int rfqId);
    }
}
