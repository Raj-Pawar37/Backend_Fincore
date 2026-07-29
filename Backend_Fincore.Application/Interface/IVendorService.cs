using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Vendor;

namespace Backend_Fincore.Application.Interface
{
    public interface IVendorService
    {

        Task<List<VendorReadDTO>> GetAll(PaginationDTO pagination);

        Task<int> GetTotalVendorRecord(string? search);

        Task<VendorReadDTO> GetById(int id);

        Task<VendorReadDTO> AddVendor(VendorWriteDTO v);

        Task<bool> DeleteVendor(int id);

        Task<bool> UpdateVendor(int id, VendorWriteDTO v);

    }
}
