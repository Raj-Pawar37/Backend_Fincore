using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService service;

        public CompanyController(ICompanyService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await service.GetAll();

            return Ok(new ApiResponse<List<CompanyReadDTO>>
            {
                Success = true,
                Message = "Company details fetched successfully.",
                Data = res,
                Error = null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await service.GetById(id);

            if (res == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found.",
                    Data = null,
                    Error = $"No company found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<CompanyReadDTO>
            {
                Success = true,
                Message = "Company fetched successfully.",
                Data = res,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyWriteDTO dto)
        {
            var data = await service.AddCompany(dto);

            return Ok(new ApiResponse<CompanyReadDTO>
            {
                Success = true,
                Message = "Company created successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyWriteDTO dto)
        {
            var result = await service.UpdateCompany(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found.",
                    Data = null,
                    Error = $"No company found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Company updated successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var result = await service.DeleteCompany(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found.",
                    Data = null,
                    Error = $"No company found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Company deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}