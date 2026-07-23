using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.GRN;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class GRNItemsService : IGRNItemsService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public GRNItemsService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<GRNItemsDTO>> getAllGrnItems()
        {
            var data = await db.GRNItem.Include(x => x.POItem).ToListAsync();

            var res = mapper.Map<List<GRNItemsDTO>>(data);

            return res;
        }

        public async Task<GRNItemsDTO> GetGRNItemById(int id)
        {
            var data = await db.GRNItem.Include(x => x.POItem).FirstOrDefaultAsync(x => x.GRNItemId == id);

            if (data == null)
            {
                throw new Exception("GRN Item not found.");
            }

            return mapper.Map<GRNItemsDTO>(data);
        }


        public async Task DeleteGRNItem(int id)
        {
            var data = await db.GRNItem.FirstOrDefaultAsync(x => x.GRNItemId == id);


            if (data == null)
            {
                throw new Exception("GRN Item not found.");
            }

            db.GRNItem.Remove(data);

            await db.SaveChangesAsync();
        }

    }
}
