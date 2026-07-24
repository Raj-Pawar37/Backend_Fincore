using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Document;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService service;

        public DocumentTypeController(IDocumentTypeService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalRecordsDocType();
            var totalPages = (int)Math.Ceiling(
                 totalRecords /
                (double)pagination.PageSize);

            return Ok(new ApiResponse<List<DocumentTypeCUDTO>>
            {
                Success = true,
                Message = "DocumentType Masters fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord=totalRecords,
                Metadata = new {
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
            try
            {
                var res = await service.GetById(id);

                return Ok(new ApiResponse<DocumentTypeCUDTO>
                {
                    Success = true,
                    Message = "DocumentType Master fetched successfully.",
                    Data = res,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "DocumentType Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAccountMaster(DocumentTypeCUDTO dto)
        {
            var res = await service.AddDocumentType(dto);

            return Ok(new ApiResponse<DocumentTypeCUDTO>
            {
                Success = true,
                Message = "DocumentType Master created successfully.",
                Data = res,
                Error = null
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountMaster(int id, DocumentTypeCUDTO dto)
        {
            try
            {
                await service.UpdateDocumentType(id, dto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "DocumentType Master updated successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "DocumentType Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountMaster(int id)
        {
            try
            {
                await service.DeleteDocumentType(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "DocumentType Master deleted successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "DocumentType Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
        [HttpGet("Dropdown")]
        public async Task<IActionResult>GetDocumentTypeDropdown([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetDocumentTypeDropdown(pagination);
            var totalRecords = await service.GetDocumentTypeDropdownCount(pagination);
            var totalPages =(int)Math.Ceiling(totalRecords /(double)pagination.PageSize);

            return Ok(
                new ApiResponse<
                    List<DocumentTypeDropdownDTO>>
                {
                    Success = true,
                    Message ="Document Types fetched successfully.",
                    Data = res,
                    Error = null,
                    TotalNumberRecord =totalRecords,
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
    }
}
