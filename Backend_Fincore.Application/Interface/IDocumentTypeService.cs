using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Department;
using Backend_Fincore.Application.DTOs.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IDocumentTypeService
    {
        Task<List<DocumentTypeCUDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalRecordsDocType();
        Task<DocumentTypeCUDTO> GetById(int id);

        Task<DocumentTypeCUDTO> AddDocumentType(DocumentTypeCUDTO dto);

        Task UpdateDocumentType(int id, DocumentTypeCUDTO dto);

        Task DeleteDocumentType(int id);

        Task<List<DocumentTypeDropdownDTO>>GetDocumentTypeDropdown(PaginationDTO pagination);
        Task<int>GetDocumentTypeDropdownCount(PaginationDTO pagination);
    }
}
