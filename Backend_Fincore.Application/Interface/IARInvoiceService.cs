using Backend_Fincore.Application.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface IARInvoiceService
    {
        Task<List<ARInvoiceDto>> GetAllAsync();

        Task<ARInvoiceDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(ARInvoiceCreateDto dto);

        Task<bool> UpdateAsync(ARInvoiceUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}