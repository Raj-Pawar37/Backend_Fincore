using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllPayment()
        {
            var payment = await paymentService.GetAllPayment();

            return Ok(new ApiResponse<List<PaymentDTO>>
            {
                Success = true,
                Message = "Payments fetched successfully.",
                Data = payment,
                Error = null
            });
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> getPaymentById(int id)
        {
            var data = await paymentService.GetPaymentById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Payment not found.",
                    Data = null,
                    Error = $"No Payment found with Id {id}."
                });
            }

            return Ok(new ApiResponse<PaymentDTO>
            {
                Success = true,
                Message = "Payment fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> addPayment(PaymentCUDTO payment)
        {
            await paymentService.AddPayment(payment);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Payment added successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updatePayment(int id, PaymentCUDTO dto)
        {
            await paymentService.UpdatePayment(dto, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Payment updated successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deletePaymentByid(int id)
        {
            var data = await paymentService.DeletePaymentById(id);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Payment not found.",
                    Data = null,
                    Error = $"No Payment found with Id {id}."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Payment deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}
