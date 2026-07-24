using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Approval;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService service;

        public ApprovalController(IApprovalService service)
        {
            this.service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalApprovalRecord();
            var totalPages = (int)Math.Ceiling(
                 totalRecords /
                (double)pagination.PageSize);

            return Ok(new ApiResponse<List<ApprovalReadDTO>>
            {
                Success = true,
                Message = "Approval details fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = totalRecords,
                Metadata =new {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = res.Count
                }
            });
        }


        // GET : api/Approval/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var res = await service.GetById(id);

                return Ok(new ApiResponse<ApprovalReadDTO>
                {
                    Success = true,
                    Message = "Approval fetched successfully.",
                    Data = res,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Approval not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }


        // POST : api/Approval
        [HttpPost]
        public async Task<IActionResult> AddApproval(ApprovalWriteDTO dto)
        {
            try
            {
                var res = await service.AddApproval(dto);

                return Ok(new ApiResponse<ApprovalReadDTO>
                {
                    Success = true,
                    Message = "Approval created successfully.",
                    Data = res,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Approval creation failed.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }


      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApproval(int id, ApprovalWriteDTO dto)
        {
            try
            {
                await service.UpdateApproval(id, dto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Approval updated successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Approval update failed.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }


     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApproval(int id)
        {
            try
            {
                await service.DeleteApproval(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Approval deleted successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Approval not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
    }
}
