using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class RFQService : IRFQService

        
    {
        private readonly AppDbContext _context;
        IMapper _mapper;

        public RFQService(AppDbContext context,IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<ApiResponse<RFQResponseDto>> Create(RFQCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Map and save the main RFQ
                var newRfq = _mapper.Map<RFQ>(dto);

                int currentCount = await _context.RFQ.CountAsync();
                newRfq.RFQNumber = $"RFQ-{DateTime.Now.Year}-{(currentCount + 1):D4}";
                newRfq.Status = "Draft";

                _context.RFQ.Add(newRfq);
                await _context.SaveChangesAsync(); // Saves to generate the new RFQId

                // 2. Map and save RFQ Items
                if (dto.RFQItems.Any())
                {
                    var rfqItems = _mapper.Map<List<RFQItem>>(dto.RFQItems);
                    foreach (var item in rfqItems)
                    {
                        item.RFQId = newRfq.RFQId;
                    }
                    _context.RFQItem.AddRange(rfqItems);
                }

                // 3. Create and save Vendor Invitations
                if (dto.VendorIds.Any())
                {
                    var vendorInvitations = dto.VendorIds.Select(vendorId => new RFQVendor
                    {
                        RFQId = newRfq.RFQId,
                        VendorId = vendorId,
                        SentDate = DateTime.UtcNow,
                        ResponseStatus = "Invited"
                    }).ToList();

                    _context.RFQVendor.AddRange(vendorInvitations);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var rfqDto = _mapper.Map<RFQResponseDto>(newRfq);
                return new ApiResponse<RFQResponseDto>
                {
                    Success = true,
                    Message = "RFQ created successfully with items and vendors",
                    Data = rfqDto,
                    TotalNumberRecord = 1
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<RFQResponseDto>
                {
                    Success = false,
                    Message = "Failed to create RFQ",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<RFQResponseDto>>> GetAll()
        {
            var rfqs = await _context.RFQ.ToListAsync();
            var dtos = _mapper.Map<List<RFQResponseDto>>(rfqs).ToList();

            return new ApiResponse<List<RFQResponseDto>>
            {
                Success = true,
                Message = "RFQs fetched successfully",
                Data = dtos,
                TotalNumberRecord = dtos.Count()
            };


        }

        public async Task<ApiResponse<RFQResponseDto>> GetAllById(int id)
        {
            var rfq = await _context.RFQ.FindAsync(id);
            if (rfq == null)
            {
                return new ApiResponse<RFQResponseDto>
                {
                    Success = false,
                    Message = "RFQ not found",
                    Error = new { code = "not found" }
                };
            }

            var dto = _mapper.Map<RFQResponseDto>(rfq);
            return new ApiResponse<RFQResponseDto>
            {
                Success = true,
                Message = "RFQ found by Id success",
                Data = dto,
                TotalNumberRecord = 1
            };


        }

       
    }
    }

