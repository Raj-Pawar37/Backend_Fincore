using Backend_Fincore.DTOs.APInvoice;

namespace Backend_Fincore.Interface
{
    public interface IAPInvoiceService
    {

        Task<List<APInvoiceDTO>> GetAllAPInvoice();

        Task<APInvoiceDTO?> GetAPInvoiceById(int id);

        Task AddAPInvoice(APInvoiceCUDTO dto);

        Task UpdateAPInvoice(APInvoiceCUDTO dto, int id);

        Task<bool> DeleteInvoiceById(int id);
    }
}
