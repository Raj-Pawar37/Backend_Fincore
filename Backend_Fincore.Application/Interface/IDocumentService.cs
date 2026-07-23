using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IDocumentService
    {
        Task<List<DocumentReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetDocumentCount();

        Task<DocumentReadDTO> GetById(int id);

        Task<DocumentReadDTO> AddDocument(DocumentWriteDTO dto);

        Task UpdateDocument(int id, DocumentWriteDTO dto);

        Task DeleteDocument(int id);
    }
}
