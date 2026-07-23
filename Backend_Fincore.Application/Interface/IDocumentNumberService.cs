using Backend_Fincore.Application.DTOs.DocumentNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IDocumentNumberService
    {
        Task<string> GenerateDocumentNumberAsync(string documentName);



        //CRUD 

        Task<DocumentNumberDTO> Create(DocumentNumberCreateTO doumentNumberCreateDTO);
        Task<DocumentNumberDTO> Update(DocumentNumberUpdateDTO documentNumberUpdateDTO);
        Task<List<DocumentNumberDTO>> ReadlAll();
        Task<DocumentNumberDTO> ReadById(int documentNumberId);
        Task<bool> Delete(int documentNumberId);

    }
}
