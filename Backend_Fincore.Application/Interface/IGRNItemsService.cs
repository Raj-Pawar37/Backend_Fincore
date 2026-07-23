using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs.GRN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IGRNItemsService
    {

        Task<List<GRNItemsDTO>> getAllGrnItems();

        Task<GRNItemsDTO> GetGRNItemById(int id);


        Task DeleteGRNItem(int id);
    }
}
