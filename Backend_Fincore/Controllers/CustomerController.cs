using AutoMapper.Configuration.Annotations;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService service;

        public CustomerController(ICustomerService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalCustomerRecords();
            var totalPages = (int)Math.Ceiling(
                  totalRecords /
                  (double)pagination.PageSize);

            return Ok(new ApiResponse<List<CustomerReadDTO>>
            {
                Success = true,
                Message = "Customer details fetched successfully.",
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
        public async Task<IActionResult> GetByIdCustomer(int id)
        {
           var res= await  service.GetById(id);

            if (res == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null,
                    Error = $"No customer found with Id = {id}"
                });
            }
            return Ok(new ApiResponse<CustomerReadDTO>
            {
                Success = true,
                Message = "Customer Details fetched successfully",
                Data = res,
                Error = null
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerWriteDTO c)
        {
            var data= await service.AddCutomer(c);

            return Ok(new ApiResponse<CustomerReadDTO>
            { Success = true,
            Message=" Customer Added Successfully",
              Data= data,
              Error=null

            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var res = await service.DeleteCustomer(id);

            if (!res)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null,
                    Error = $"No customer found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer deleted successfully.",
                Data = null,
                Error = null
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerWriteDTO c)
        {
            var data = await service.UpdateCustomer(id, c);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null,
                    Error = $"No customer found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer updated successfully.",
                Data = null,
                Error = null
            });
        }
    }
}

