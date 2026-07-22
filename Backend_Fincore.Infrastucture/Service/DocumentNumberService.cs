using AutoMapper;
using Backend_Fincore.Application.DTOs.DocumentNumber;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Domain;
using Backend_Fincore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class DocumentNumberService : IDocumentNumberService
    {

        private readonly AppDbContext _db;
        IMapper mapper;

        public DocumentNumberService(AppDbContext db, IMapper mapper)
        {
            this.mapper = mapper;
            _db = db;
        }





        public async Task<string> GenerateDocumentNumberAsync(string documentName)
        {
            var data = await _db.Database.SqlQuery<string>($"EXEC SP_DocumentNumber_Generate @DocumentName = {documentName}").ToListAsync();

            return data.FirstOrDefault();
        }





        //Basic Crud
        public async Task<DocumentNumberDTO> Create(DocumentNumberCreateTO doumentNumberCreateDTO)
        {

            var exist = await _db.DocumentNumberMasters.FirstOrDefaultAsync(x => x.DocumentName == doumentNumberCreateDTO.DocumentName);
            if (exist != null)
            {
                throw new InvalidOperationException("Document name already exists.");
            }

            var data = mapper.Map<DocumentNumberMaster>(doumentNumberCreateDTO);
            await _db.DocumentNumberMasters.AddAsync(data);
            await _db.SaveChangesAsync();

            return mapper.Map<DocumentNumberDTO>(data);

        }



        public async Task<DocumentNumberDTO> Update(DocumentNumberUpdateDTO documentNumberUpdateDTO)
        {
            var exist = await _db.DocumentNumberMasters.FindAsync(documentNumberUpdateDTO.DocumentName);
            if (exist != null)
            {
                throw new KeyNotFoundException("Document not found");
            }


            var exist2 = await _db.DocumentNumberMasters.FirstOrDefaultAsync(x => x.DocumentName == documentNumberUpdateDTO.DocumentName);
            if (exist2 != null)
            {
                throw new InvalidOperationException("Document name already exists.");
            }


            var data = mapper.Map<DocumentNumberMaster>(documentNumberUpdateDTO);
            _db.DocumentNumberMasters.Update(data);
            await _db.SaveChangesAsync();
            return mapper.Map<DocumentNumberDTO>(data);
        }



        public async  Task<bool> Delete(int documentNumberId)
        {
            var data = await _db.DocumentNumberMasters.FindAsync(documentNumberId);
            if (data == null)
            {
                throw new KeyNotFoundException("Document number configuration not found.");
            }

            _db.DocumentNumberMasters.Remove(data);
            await _db.SaveChangesAsync();

            return true;

        }



        public async Task<DocumentNumberDTO> ReadById(int DocumentNumberId)
        {
            var exist = await _db.DocumentNumberMasters.FindAsync(DocumentNumberId);
            if (exist == null)
            {
                throw new KeyNotFoundException("Document number configuration not found.");
            }

            var data = await _db.DocumentNumberMasters.FindAsync(DocumentNumberId);
            return mapper.Map<DocumentNumberDTO>(data);
        }



        public async Task<List<DocumentNumberDTO>> ReadlAll()
        {
            var data = await _db.DocumentNumberMasters.ToListAsync();
            return mapper.Map<List<DocumentNumberDTO>>(data);
        }


    }
}
