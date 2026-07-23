using AutoMapper;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Backend_Fincore.Application.Services
{
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseRequisitionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<PurchaseRequisitionResponseDto>>> GetAllAsync()
        {
            var prs = await _context.PurchaseRequisition.ToListAsync();
            var prDtos = _mapper.Map<List<PurchaseRequisitionResponseDto>>(prs).ToList();

            return new ApiResponse<List<PurchaseRequisitionResponseDto>>
            {
                Success = true,
                Message = "Purchase Requisitions fetched successfully",
                Data = prDtos,
                TotalNumberRecord = prDtos.Count()
            };
        }

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> GetByIdAsync(int id)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);
            if (pr == null)
            {
                return new ApiResponse<PurchaseRequisitionResponseDto>
                {
                    Success = false,
                    Message = "Purchase Requisition not found",
                    Error = new { code = "NOT_FOUND" }
                };
            }

            var prDto = _mapper.Map<PurchaseRequisitionResponseDto>(pr);
            return new ApiResponse<PurchaseRequisitionResponseDto>
            {
                Success = true,
                Message = "Purchase Requisition fetched successfully",
                Data = prDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> CreateAsync(PurchaseRequisitionCreateDto dto)
        {
            var newPr = _mapper.Map<PurchaseRequisition>(dto);

            // Auto-generate PR Number (e.g., PR-2026-0001)
            int currentCount = await _context.PurchaseRequisition.CountAsync();
            newPr.PRNumber = $"PR-{DateTime.Now.Year}-{(currentCount + 1):D4}";
            newPr.Status = "Draft"; // Initial status before submission

            _context.PurchaseRequisition.Add(newPr);
            await _context.SaveChangesAsync();

            var prDto = _mapper.Map<PurchaseRequisitionResponseDto>(newPr);
            return new ApiResponse<PurchaseRequisitionResponseDto>
            {
                Success = true,
                Message = "Purchase requisition created successfully",
                Data = prDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);
            if (pr == null)
            {
                return new ApiResponse<PurchaseRequisitionResponseDto> { Success = false, Message = "Not found" };
            }

            // AutoMapper will now map Title, Description, AND Status 
            _mapper.Map(dto, pr);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);
            if (pr == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Purchase Requisition not found",
                    Data = false
                };
            }

            _context.PurchaseRequisition.Remove(pr);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Purchase Requisition deleted successfully",
                Data = true,
                TotalNumberRecord = 1
            };
        }
    }
}