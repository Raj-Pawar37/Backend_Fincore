using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Country;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class CountryService: ICountryService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        public CountryService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;

        }
        //public async Task<List<CountryReadDTO>>GetAll(PaginationDTO pagination)
        //{
        //    var search = db.Country.Where(x => x.IsActive == 1).AsQueryable();
        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>
        //            x.CountryName.Contains(pagination.Search)
        //            ||
        //            x.CountryCode.Contains(pagination.Search));
        //    }
        //    var data = await search
        //        .OrderBy(x => x.CountryName)
        //        .Skip((pagination.PageNumber - 1)* pagination.PageSize)
        //        .Take(pagination.PageSize)
        //        .ToListAsync();
        //    return mapper.Map<List<CountryReadDTO>>(data);
        //}
        //public async Task<int>GetCountryCount(PaginationDTO pagination)
        //{
        //    var search = db.Country.Where(x => x.IsActive == 1).AsQueryable();

        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>
        //            x.CountryName.Contains(pagination.Search)
        //            ||
        //            x.CountryCode.Contains(pagination.Search));
        //    }
        //    return await search.CountAsync();
        //}

        //public async Task<List<StateReadDTO>>GetAllState(StateFilterDTO pagination)
        //{
        //    var search = db.State
        //        .Where(x => x.IsActive == 1)

        //        .Where(x => x.CountryId == pagination.CountryId)

        //        .AsQueryable();


        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>

        //            x.StateName.Contains(pagination.Search)

        //            ||

        //            x.StateCode.Contains(pagination.Search));
        //    }


        //    var data = await search
        //        .OrderBy(x => x.StateName)
        //        .Skip((pagination.PageNumber - 1) * pagination.PageSize)
        //        .Take(pagination.PageSize)
        //        .ToListAsync();


        //    return mapper.Map<List<StateReadDTO>>(data);
        //}
        //public async Task<int>GetStateCount(StateFilterDTO pagination)
        //{
        //    var search = db.State
        //        .Where(x => x.IsActive == 1)
        //        .Where(x => x.CountryId == pagination.CountryId)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>
        //            x.StateName.Contains(pagination.Search)
        //            ||
        //            x.StateCode.Contains(pagination.Search));
        //    }
        //    return await search.CountAsync();
        //}
        //public async Task<List<CityReadDTO>>GetAllCity(CityFilterDTO pagination)
        //{
        //    var search = db.City
        //        .Where(x => x.IsActive == 1)
        //        .Where(x => x.StateId == pagination.StateId)
        //        .AsQueryable();


        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>x.CityName.Contains(pagination.Search));
        //    }
        //    var data = await search
        //        .OrderBy(x => x.CityName)
        //        .Skip((pagination.PageNumber - 1)* pagination.PageSize)
        //        .Take(pagination.PageSize)
        //        .ToListAsync();
        //    return mapper.Map<List<CityReadDTO>>(data);
        //}
        //public async Task<int>GetCityCount(CityFilterDTO pagination)
        //{
        //    var search = db.City
        //        .Where(x => x.IsActive == 1)
        //        .Where(x => x.StateId == pagination.StateId)
        //        .AsQueryable();
        //    if (!string.IsNullOrEmpty(pagination.Search))
        //    {
        //        search = search.Where(x =>
        //            x.CityName.Contains(pagination.Search));
        //    }
        //    return await search.CountAsync();
        //}
        public async Task<List<CountryReadDTO>> GetAll(string? searchText)
        {
            var search = db.Country
                            .Where(x => x.IsActive == 1)
                            .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x =>
                            x.CountryName.Contains(searchText)
                            ||
                            x.CountryCode.Contains(searchText));
            }

            var data = await search
                            .OrderBy(x => x.CountryName)
                            .Take(20)
                            .ToListAsync();

            return mapper.Map<List<CountryReadDTO>>(data);
        }
        public async Task<List<StateReadDTO>>GetAllState(int countryId, string? searchText)
        {
            var search = db.State
                            .Where(x => x.IsActive == 1)
                            .Where(x => x.CountryId == countryId)
                            .AsQueryable();


            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x =>
                            x.StateName.Contains(searchText)
                            ||
                            x.StateCode.Contains(searchText));
            }


            var data = await search
                            .OrderBy(x => x.StateName)
                            .Take(20)
                            .ToListAsync();


            return mapper.Map<List<StateReadDTO>>(data);
        }
        public async Task<List<CityReadDTO>>GetAllCity(int stateId, string? searchText)
        {
            var search = db.City
                            .Where(x => x.IsActive == 1)
                            .Where(x => x.StateId == stateId)
                            .AsQueryable();


            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x =>
                            x.CityName.Contains(searchText));
            }


            var data = await search
                            .OrderBy(x => x.CityName)
                            .Take(20)
                            .ToListAsync();


            return mapper.Map<List<CityReadDTO>>(data);
        }
    }
}
