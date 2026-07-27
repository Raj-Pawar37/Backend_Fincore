using AutoMapper;
using Backend_Fincore.Application.DTOs;
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
        private readonly ICurrentUserService currentUser;

        public DocumentService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<int> GetDocumentCount()
        {
            return await db.Document.Where(x => x.IsActive == 1).CountAsync();
        }
        public async Task<List<DocumentReadDTO>>GetAll(PaginationDTO pagination)
        {
            var search = db.Document.Where(x => x.IsActive == 1).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.FileName.Contains(pagination.Search) ||

                    x.MasterType.Contains(pagination.Search));  
            }
            var data = await search.Include(x => x.DocumentType)
                                        .Skip((pagination.PageNumber - 1)* pagination.PageSize)
                                        .Take(pagination.PageSize)
                                        .ToListAsync();
            return mapper.Map<List<DocumentReadDTO>>(data);
        }
        public async Task<DocumentReadDTO>GetById(int id)
        {
            var data = await db.Document.Include(x => x.DocumentType)
                .FirstOrDefaultAsync(x => x.DocumentId == id && x.IsActive == 1);
            if (data == null)
            {
                throw new Exception("Document not found.");
            }
            return mapper.Map<DocumentReadDTO>(data);
        }

        public async Task<DocumentReadDTO> AddDocument(DocumentWriteDTO dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new Exception("Please upload a file.");
            }
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
            bool documentTypeExists = await db.DocumentType.AnyAsync(x =>
                                x.DocumentTypeId == dto.DocumentTypeId &&x.IsActive == 1);

            if (!documentTypeExists)
            {
                throw new Exception("Invalid Document Type.");
            }

   
            // File Size Validation
            if (dto.File.Length > 5242880)
            {
                throw new Exception("Maximum file size is 5 MB.");
            }
           
            var relativeFolderPath = Path.Combine("Uploads", "Documents", currentUser.MasterType ?? "General", currentUser.MasterId.ToString());
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolderPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Unique File Name
            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            // Complete Path
            var fullPath = Path.Combine(folderPath, uniqueFileName);
            // Save File

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }
            // Mapping
            var data = mapper.Map<Document>(dto);

            data.FileName = dto.File.FileName;

            //data.FilePath =$"Uploads/Documents/{uniqueFileName}";
            data.FilePath = Path.Combine(relativeFolderPath, uniqueFileName).Replace("\\", "/");
            data.FileType = dto.File.ContentType;
            data.CreatedBy = currentUser.UserId;


            data.MasterId = currentUser.MasterId;
            data.MasterType = currentUser.MasterType;

            data.IsActive = 1;
            await db.Document.AddAsync(data);


            await db.SaveChangesAsync();

            data = await db.Document.Include(x => x.DocumentType).FirstOrDefaultAsync(x => x.DocumentId == data.DocumentId);
            return mapper.Map<DocumentReadDTO>(data);
        }
        public async Task UpdateDocument(int id, DocumentUpdateDTO dto)
        {
            // Find Existing Document

            var data = await db.Document.FirstOrDefaultAsync(x =>x.DocumentId == id && x.IsActive == 1);
            if (data == null)
            {
                throw new Exception("Document not found.");
            }
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new Exception("Please upload a file.");
            }
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
                throw new Exception(
                    "Only PDF, JPG and PNG files are allowed.");
            }
            // File Size Validation
            if (dto.File.Length > 5242880)
            {
                throw new Exception("Maximum file size is 5 MB.");
            }
   
            bool documentTypeExists = await db.DocumentType.AnyAsync(x =>
                                x.DocumentTypeId == dto.DocumentTypeId && x.IsActive == 1);

            if (!documentTypeExists)
            {
                throw new Exception("Invalid Document Type.");
            }

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), data.FilePath);


            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }


            // Upload Folder Path
            var masterType = currentUser.MasterType ?? data.MasterType ?? "General";
            var masterId = currentUser.MasterId != 0 ? currentUser.MasterId : data.MasterId;

            //var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Uploads","Documents");
            var relativeFolderPath = Path.Combine("Uploads", "Documents", masterType, masterId.ToString());
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolderPath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // Generate Unique File Name

            var uniqueFileName = Guid.NewGuid().ToString() + extension;


            // Complete File Path

            var fullPath = Path.Combine(folderPath, uniqueFileName);


            // Save New File

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }
            // Update Document Details

            data.DocumentTypeId = dto.DocumentTypeId;
            data.FileName = dto.File.FileName;
            data.Remarks = dto.Remarks;

            data.IsActive = dto.IsActive;

            //data.FilePath =$"Uploads/Documents/{uniqueFileName}";
            data.FilePath = Path.Combine(relativeFolderPath, uniqueFileName).Replace("\\", "/");

            data.FileType = dto.File.ContentType;

            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            // Save Changes
            await db.SaveChangesAsync();
        }
        public async Task DeleteDocument( int id)
        {
            var data = await db.Document.
                        FirstOrDefaultAsync(x => x.DocumentId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("Document not found.");
            }
            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }

    }
}
