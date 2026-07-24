using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService vendorService;

        public VendorController(IVendorService vendorService)
        {
            this.vendorService = vendorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDTO pagination)
        {
            var res = await vendorService.GetAll(pagination);
            var totalRecords = await vendorService.GetTotalVendorRecord();
            var totalPages = (int)Math.Ceiling(
                  totalRecords /
                  (double)pagination.PageSize);


            return Ok(new ApiResponse<List<VendorReadDTO>>
            {
                Success = true,
                Message = "Vendors fetch successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = res.Count
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await vendorService.GetById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Vendor not found.",
                    Data = null,
                    Error = $"No vendor found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<VendorReadDTO>
            {
                Success = true,
                Message = "Vendor retrieved successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddVendor(VendorWriteDTO vendor)
        {
            var data = await vendorService.AddVendor(vendor);

            return Ok(new ApiResponse<VendorReadDTO>
            {
                Success = true,
                Message = "Vendor added successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorWriteDTO vendor)
        {
            var result = await vendorService.UpdateVendor(id, vendor);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Vendor not found.",
                    Data = null,
                    Error = $"No vendor found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Vendor updated successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            try
            {
                var result = await vendorService.DeleteVendor(id);

                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Vendor not found.",
                        Data = null,
                        Error = $"No vendor found with Id = {id}"
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Vendor deleted successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unable to delete vendor.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
    }
}