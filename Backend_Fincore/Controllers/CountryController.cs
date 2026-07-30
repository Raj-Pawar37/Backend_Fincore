using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Country;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    [Route("api/v1/country")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService service;

        public CountryController(ICountryService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search)
        {
            var res = await service.GetAll(search);

            return Ok(new ApiResponse<List<CountryReadDTO>>
            {
                Success = true,
                Message = "Countries fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = res.Count
            });
        }
        [HttpGet("states")]
        public async Task<IActionResult>GetAllState(int countryId, string? search)
        {
            var res =
                await service.GetAllState(countryId, search);

            return Ok(new ApiResponse<List<StateReadDTO>>
            {
                Success = true,
                Message = "States fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = res.Count
            });
        }
        [HttpGet("cities")]
        public async Task<IActionResult>GetAllCity(int stateId, string? search)
        {
            var res =
                await service.GetAllCity(stateId, search);

            return Ok(new ApiResponse<List<CityReadDTO>>
            {
                Success = true,
                Message = "Cities fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = res.Count
            });
        }
    }
}
