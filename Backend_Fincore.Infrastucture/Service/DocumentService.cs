using AutoMapper;
using Backend_Fincore.Application.DTOs.Document;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
{
    public class DocumentService: IDocumentService
    {
        AppDbContext db;
        IMapper mapper;
        public DocumentService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }
        public async Task<List<DocumentReadDTO>>GetAll()
        {
            var data = await db.Document.Include(x => x.DocumentType).ToListAsync();
            return mapper.Map<List<DocumentReadDTO>>(data);
        }
        public async Task<DocumentReadDTO>GetById(int id)
        {
            var data = await db.Document.Include(x => x.DocumentType).FirstOrDefaultAsync(
                    x => x.DocumentId == id);
            if (data == null)
            {
                throw new Exception("Document not found.");
            }
            return mapper.Map<DocumentReadDTO>(data);
        }
    
        public async Task<DocumentReadDTO> AddDocument(DocumentWriteDTO dto)
        {
            // File Extension Validation

            string[] allowedExtensions =
            {
                 ".pdf",
                ".jpg",
                 ".jpeg",
                ".png"
            };


            var extension = Path.GetExtension(dto.File.FileName).ToLower();


            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Only PDF, JPG and PNG files are allowed.");
            }


            // File Size Validation
            if (dto.File.Length > 5242880)
            {
                throw new Exception("Maximum file size is 5 MB.");
            }
            // Master Type Validation

            string[] allowedMasterTypes ={"Company","Vendor","Customer","Employee" };


            if (!allowedMasterTypes.Contains(dto.MasterType))
            {
                throw new Exception(
                    "Invalid Master Type.");
            }


            // Upload Folder
            var folderPath =Path.Combine( Directory.GetCurrentDirectory(),"Uploads","Documents");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Unique File Name
            var uniqueFileName =Guid.NewGuid().ToString()+ extension;
            // Complete Path
            var fullPath = Path.Combine( folderPath, uniqueFileName);
            // Save File

            using (var stream = new FileStream( fullPath,FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }
            // Mapping
            var data = mapper.Map<Document>(dto);

            data.FileName = dto.File.FileName;

            data.FilePath =$"Uploads/Documents/{uniqueFileName}";
            data.FileType =dto.File.ContentType;
            data.CreatedBy = 1;
            data.IsActive = dto.IsActive ? (byte)1 : (byte)0;
            await db.Document.AddAsync(data);
            await db.SaveChangesAsync();

            data = await db.Document.Include(x => x.DocumentType).FirstOrDefaultAsync(x => x.DocumentId == data.DocumentId);
            return mapper.Map<DocumentReadDTO>(data);
        }
     
        public async Task UpdateDocument( int id,DocumentWriteDTO dto)
        {
            // Find Existing Document

            var data = await db.Document.FindAsync(id);

            if (data == null)
            {
                throw new Exception(
                    "Document not found.");
            }

            // File Extension Validation

            string[] allowedExtensions =
            {
                 ".pdf",
                ".jpg",
                ".jpeg",
                ".png"
            };

            var extension =Path.GetExtension(dto.File.FileName).ToLower();


            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception(
                    "Only PDF, JPG and PNG files are allowed.");
            }
            // File Size Validation
            if (dto.File.Length > 5242880)
            {
                throw new Exception("Maximum file size is 5 MB.");
            }
            // Master Type Validation
            string[] allowedMasterTypes =
            {
                "Company",
                "Vendor",
                "Customer",
                 "Employee"
             };

            if (!allowedMasterTypes.Contains(dto.MasterType))
            {
                throw new Exception("Invalid Master Type.");
            }


            // Delete Old Physical File

            var oldFilePath =Path.Combine(Directory.GetCurrentDirectory(),data.FilePath);


            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }


            // Upload Folder Path

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Uploads","Documents");


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // Generate Unique File Name

            var uniqueFileName = Guid.NewGuid().ToString() + extension;


            // Complete File Path

            var fullPath = Path.Combine( folderPath,uniqueFileName);


            // Save New File

            using (var stream = new FileStream( fullPath,FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }
            // Update Document Details

            data.DocumentTypeId =dto.DocumentTypeId;

            data.MasterId =dto.MasterId;

            data.MasterType =dto.MasterType;

            //data.FileName =dto.FileName;
            data.FileName = dto.File.FileName;
            data.Remarks =dto.Remarks;

            data.IsActive =dto.IsActive ? (byte)1 : (byte)0;

            data.FilePath =$"Uploads/Documents/{uniqueFileName}";

            data.FileType =dto.File.ContentType;

            data.ModifiedBy = 1;
            // Save Changes

            await db.SaveChangesAsync();
        }
        public async Task DeleteDocument( int id)
        {
            var data = await db.Document.FindAsync(id);

            if (data == null)
            {
                throw new Exception("Document not found.");
            }
            data.IsActive = 0;
            data.ModifiedBy = 1;
            await db.SaveChangesAsync();
        }

    }
}
