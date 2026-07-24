using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface ICountryService
    {
        //    Task<List<CountryReadDTO>> GetAll(PaginationDTO pagination);

        //    Task<int> GetCountryCount(PaginationDTO pagination);

        //    Task<List<StateReadDTO>> GetAllState(StateFilterDTO pagination);

        //    Task<int> GetStateCount(StateFilterDTO pagination);

        //    Task<List<CityReadDTO>> GetAllCity(CityFilterDTO pagination);

        //Task<int> GetCityCount(CityFilterDTO pagination);
        Task<List<CountryReadDTO>> GetAll(string? search);

        Task<List<StateReadDTO>> GetAllState(int CountryId, string? search);

        Task<List<CityReadDTO>> GetAllCity(int StateId, string? search);

    }

}
