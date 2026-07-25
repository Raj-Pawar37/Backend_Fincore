using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ARInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IARInvoiceService
    {
        Task<List<ARInvoiceDto>> GetAllAsync(PaginationDTO pagination);

        Task<ARInvoiceDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(ARInvoiceCreateDto dto);

        Task<bool> UpdateAsync(ARInvoiceUpdateDto dto);

        Task<bool> DeleteAsync(int id);

        Task<ARInvoiceDto?> GenerateInvoiceAsync(GenerateInvoiceDto dto);

        Task<bool> GenerateInvoiceAsync(ARInvoiceGenerateDto dto);
    }
}
