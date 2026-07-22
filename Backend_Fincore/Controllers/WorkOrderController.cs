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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<WorkOrderReadDTO>>
            {
                Success = true,
                Message = "Work Orders fetched successfully.",
                Data = data,
                Error = null
            });
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
        public async Task<IActionResult> Add(WorkOrderWriteDTO dto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WorkOrderWriteDTO dto)
        {
            var data = await service.Update(id, dto);

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
                Message = "Work Order updated successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.Delete(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Work Order not found.",
                    Data = null,
                    Error = $"No Work Order found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Work Order deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}
