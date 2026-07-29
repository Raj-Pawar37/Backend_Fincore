using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/workOrder")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class WorkOrderController : ControllerBase
    {
        private readonly IWorkOrderService service;

        public WorkOrderController(IWorkOrderService service)
        {
            this.service = service;
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await service.GetById(id);

            if (data == null)
            {
                throw new Exception("Work Order Data Not Found ");
            }

            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message = "Work Order fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkOrderWriteDTO dto)
        {
            var data = await service.Create(dto);

            if (data == null)
            {
                throw new Exception("Work Order Data Not Found ");
            }
            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message = "Work Order created successfully.",
                Data = data,
                Error = null
            });
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            if (pagination.PageNumber <= 0)
                pagination.PageNumber = 1;

            if (pagination.PageSize <= 0)
                pagination.PageSize = 10;

            var data = await service.GetAll(pagination);

            var totalRecords = await service.GetWorkOrderCount(pagination);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<WorkOrderReadDTO>>
            {
                Success = true,
                Message = "Work Orders fetched successfully.",
                Data = data,
                Error = null,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = data.Count
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WorkOrderWriteDTO dto)
        {
            var data = await service.Update(id, dto);
            if (data != null)
            {
                throw new Exception("Work Order Data Not Found ");
            }

            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message = "Work Order updated successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
         var  dt =  await service.Delete(id);
            if(dt == null)
            {
                throw new Exception("Work Order Data Not Found ");

            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Work Order deleted successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> Verify(int id, int approvedBy, WorkOrderVerifyDTO dto)
        {
            var data = await service.Verify(id, approvedBy, dto);
            if (data == null)
            {
                throw new Exception("Work Order Data Not Found ");

            }
            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message = $"Work Order {dto.Status} successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var data = await service.GetDropdown();

            return Ok(new ApiResponse<List<WorkOrderDropdownDTO>>
            {
                Success = true,
                Message = "Work Order dropdown fetched successfully.",
                Data = data,
                Error = null
            });
        }
    }
}
