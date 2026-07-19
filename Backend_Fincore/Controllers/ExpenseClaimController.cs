//using Backend_Fincore.DTOs;
//using Backend_Fincore.Interface;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Backend_Fincore.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ExpenseClaimController : ControllerBase
//    {


//        IExpenseClaimService service;
//        public ExpenseClaimController(IExpenseClaimService service)
//        {
//            this.service = service;
//        }
//        [HttpGet]
//        public async Task<IActionResult> ExpenseClaim()
//        {
//            var data = await service.GetAllExpenseClaims();
//            return Ok(data);

//        }
//        [HttpPost]
//        public async Task<IActionResult> ExpenseClaim(ExpenseClaimDto dto)
//        {
//            await service.Create(dto);
//            return Ok("Added Successfully ");

//        }

//        [HttpGet("//{id}")]
//        public async Task<IActionResult> ExpenseClaim(int id)
//        {
//            var dt = await service.GetById(id);
//            if (id == null)
//            {
//                return NotFound();
//            }
//            return Ok(dt);

//        }

//        [HttpPut]
//        public async Task<IActionResult> ExpenseClaimUpdate(ExpenseClaimDto opd)
//        {
//            await service.Update(opd);
//            return Ok("Updated Successfully");

//        }

//        [HttpDelete("/{id}")]
//        public async Task<IActionResult> ExpenseClaimDelete(int id)
//        {
//            await service.Delete(id);
//            return Ok("Deleted Successfully");
//        }

//    }
//}
