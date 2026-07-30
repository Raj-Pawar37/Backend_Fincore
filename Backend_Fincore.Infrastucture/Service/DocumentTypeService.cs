using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.DTOs.Department;
using Backend_Fincore.Application.DTOs.Document;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Models.Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class DocumentTypeService : IDocumentTypeService
    {
        AppDbContext db;
        IMapper mapper;
        private readonly ICurrentUserService currentUser;

        public DocumentTypeService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<DocumentTypeCUDTO> AddDocumentType(DocumentTypeWriteDTO dto)
        {
            bool documentTypeExists = await db.DocumentType.AnyAsync(x =>
                                    x.DocumentTypeName == dto.DocumentTypeName &&
                                    x.IsActive == 1);
            if (documentTypeExists)
            {
                throw new Exception("Document Type already exists.");
            }
            var data = mapper.Map<DocumentType>(dto);
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;

            data.CreatedAt = DateTime.Now;

            await db.DocumentType.AddAsync(data);

            await db.SaveChangesAsync();
            return mapper.Map<DocumentTypeCUDTO>(data);

        }

        public async Task DeleteDocumentType(int id)
        {
            var data = await db.DocumentType.FirstOrDefaultAsync(x =>
                            x.DocumentTypeId == id && x.IsActive == 1);
            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }

            //db.AccountMaster.Remove(data);
            data.IsActive = 0;//soft delete by vikas 
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();

        }

        public async Task<List<DocumentTypeCUDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.DocumentType.Where(x => x.IsActive == 1).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search)) {
                search = search.Where(x =>
                    x.DocumentTypeName.Contains(pagination.Search));
            }
            var data = await search
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();
            
            return mapper.Map<List<DocumentTypeCUDTO>>(data);
        }

        public async Task<DocumentTypeCUDTO> GetById(int id)
        {
            var data = await db.DocumentType.FirstOrDefaultAsync(x => x.DocumentTypeId == id&& x.IsActive==1);

            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }

            return mapper.Map<DocumentTypeCUDTO>(data);

        }

        public async Task<int> GetTotalRecordsDocType()
        {
            return await db.DocumentType.Where(x => x.IsActive == 1).CountAsync();
        }

        public async Task UpdateDocumentType(int id, DocumentTypeUpdateDTO dto)
        {
            bool documentTypeExists = await db.DocumentType.AnyAsync(x =>
                                     x.DocumentTypeName == dto.DocumentTypeName &&
                                     x.DocumentTypeId != id &&
                                     x.IsActive == 1);

            if (documentTypeExists)
            {
                throw new Exception("Document Type already exists.");
            }
            var data = await db.DocumentType.FirstOrDefaultAsync(x =>
                                     x.DocumentTypeId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            mapper.Map(dto, data);
            await db.SaveChangesAsync();
        }
        public async Task<List<DocumentTypeDropdownDTO>>GetDocumentTypeDropdown(string? searchText)
        {
            var search = db.DocumentType
                            .Where(x => x.IsActive == 1)
                            .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x =>
                            x.DocumentTypeName.Contains(searchText));
            }

            return await search
                        .OrderBy(x => x.DocumentTypeName)
                        .Take(20)
                        .Select(x => new DocumentTypeDropdownDTO
                        {
                            DocumentTypeId = x.DocumentTypeId,
                            DocumentTypeName = x.DocumentTypeName
                        })
                        .ToListAsync();
        }

    }
}
