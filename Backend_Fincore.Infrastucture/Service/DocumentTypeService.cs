using AutoMapper;
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
        public DocumentTypeService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }
        public async Task<DocumentTypeCUDTO> AddDocumentType(DocumentTypeCUDTO dto)
        {
            var data = mapper.Map<DocumentType>(dto);

            data.CreatedBy = 1;

            //data.CreatedAt = DateTime.Now;

            await db.DocumentType.AddAsync(data);

            await db.SaveChangesAsync();
            return mapper.Map<DocumentTypeCUDTO>(data);

        }

        public async Task DeleteDocumentType(int id)
        {
            var data = await db.DocumentType.FindAsync(id);
            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }

            //db.AccountMaster.Remove(data);
            data.IsActive = 0;//soft delete by vikas 
            await db.SaveChangesAsync();

        }

        public async Task<List<DocumentTypeCUDTO>> GetAll()
        {
            var data = await db.DocumentType.ToListAsync();
            
            return mapper.Map<List<DocumentTypeCUDTO>>(data);
        }

        public async Task<DocumentTypeCUDTO> GetById(int id)
        {
            var data = await db.DocumentType.FirstOrDefaultAsync(x => x.DocumentTypeId == id);

            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }

            return mapper.Map<DocumentTypeCUDTO>(data);

        }

        public async Task UpdateDocumentType(int id, DocumentTypeCUDTO dto)
        {
            var data = await db.DocumentType.FindAsync(id);

            if (data == null)
            {
                throw new Exception("DocumentType Master not found.");
            }
            data.ModifiedBy = 1;//further i need to add jwt userid here
            mapper.Map(dto, data);
            await db.SaveChangesAsync();
        }
    }
}
