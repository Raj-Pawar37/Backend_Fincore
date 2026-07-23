using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Work Order not found.",
                    Data = null,
                    Error = $"No Work Order found with Id = {id}"
                });
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
        public async Task<IActionResult> Create(
         WorkOrderWriteDTO dto)
        {
            var data = await service.Create(dto);

            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message = "Work Order created successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            var data = await service.GetAll(userId);

            return Ok(
                new ApiResponse<List<WorkOrderReadDTO>>
                {
                    Success = true,
                    Message =
                        "Work Orders fetched successfully.",
                    Data = data,
                    Error = null
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            WorkOrderWriteDTO dto)
        {
            var data = await service.Update(id, dto);

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
            await service.Delete(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Work Order deleted successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> Verify(
            int id,
            int approvedBy,
            WorkOrderVerifyDTO dto)
        {
            var data = await service.Verify(
                id,
                approvedBy,
                dto);

            return Ok(new ApiResponse<WorkOrderReadDTO>
            {
                Success = true,
                Message =
                    $"Work Order {dto.Status} successfully.",
                Data = data,
                Error = null
            });
        }
    }
}
