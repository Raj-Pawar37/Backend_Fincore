using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Country;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService service;

        public CountryController(ICountryService service)
        {
            this.service = service;
        }

        //[HttpGet]
        //public async Task<IActionResult>GetAll([FromQuery] PaginationDTO pagination)
        //{
        //    var res = await service.GetAll(pagination);

        //    var totalRecords =await service.GetCountryCount(pagination);


        //    var totalPages =(int)Math.Ceiling(totalRecords / (double)pagination.PageSize);


        //    return Ok(
        //        new ApiResponse<List<CountryReadDTO>>
        //        {
        //            Success = true,

        //            Message = "Countries fetched successfully.",

        //            Data = res,

        //            Error = null,

        //            TotalNumberRecord = totalRecords,

        //            Metadata = new
        //            {
        //                pagination.PageNumber,
        //                pagination.PageSize,
        //                pagination.Search,
        //                TotalPages = totalPages,
        //                RecordsOnCurrentPage = res.Count
        //            }
        //        });

        //}
        //[HttpGet("States")]
        //public async Task<IActionResult> GetAllStates([FromQuery] StateFilterDTO pagination)
        //{
        //    var res = await service.GetAllState(pagination);

        //    var totalRecords =await service.GetStateCount(pagination);

        //    var totalPages =(int)Math.Ceiling(totalRecords /(double)pagination.PageSize);

        //    return Ok(new ApiResponse<List<StateReadDTO>>
        //    {
        //        Success = true,
        //        Message = "States fetched successfully.",
        //        Data = res,
        //        Error = null,
        //        TotalNumberRecord = totalRecords,
        //        Metadata = new
        //        {
        //            pagination.CountryId,
        //            pagination.PageNumber,
        //            pagination.PageSize,
        //            pagination.Search,
        //            TotalPages = totalPages,
        //            RecordsOnCurrentPage = res.Count
        //        }
        //    });
        //}



        //[HttpGet("Cities")]
        //public async Task<IActionResult> GetAllCities( [FromQuery] CityFilterDTO pagination)
        //{
        //    var res = await service.GetAllCity(pagination);

        //    var totalRecords =await service.GetCityCount(pagination);

        //    var totalPages = (int)Math.Ceiling( totalRecords /(double)pagination.PageSize);

        //    return Ok(new ApiResponse<List<CityReadDTO>>
        //    {
        //        Success = true,
        //        Message = "Cities fetched successfully.",
        //        Data = res,
        //        Error = null,
        //        TotalNumberRecord = totalRecords,

        //        Metadata = new
        //        {
        //            pagination.StateId,
        //            pagination.PageNumber,
        //            pagination.PageSize,
        //            pagination.Search,
        //            TotalPages = totalPages,
        //            RecordsOnCurrentPage = res.Count
        //        }
        //    });
        //}
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
        [HttpGet("States")]
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
        [HttpGet("Cities")]
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
