using Backend_Fincore.Application.DTOs.JournalEntry;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class JournalEntryService : IJournalEntryService
    {

        private readonly AppDbContext db;

        public JournalEntryService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task CreateEntry(JournalEntryCreateDTO dto)
        {
            //if (dto.Amount <= 0)
            //{
            //    throw new Exception("Journal amount must be greater than zero.");
            //}

            //if (dto.DebitAccountId == dto.CreditAccountId)
            //{
            //    throw new Exception("Debit and credit accounts cannot be the same.");
            //}

            //var companyExists = await db.Company.AnyAsync(x =>x.CompanyId == dto.CompanyId);

            //if (!companyExists)
            //{
            //    throw new Exception("Company not found.");
            //}

            //var accountIds = await db.AccountMaster
            //    .Where(x =>x.AccountMasterId == dto.DebitAccountId || x.AccountMasterId == dto.CreditAccountId)
            //    .Select(x => x.AccountMasterId)
            //    .ToListAsync();

            //if (!accountIds.Contains(dto.DebitAccountId))
            //{
            //    throw new Exception("Debit account not found.");
            //}

            //if (!accountIds.Contains(dto.CreditAccountId))
            //{
            //    throw new Exception("Credit account not found.");
            //}

            //var duplicateEntry = await db.JournalEntry.AnyAsync(x =>x.MasterId == dto.MasterId && x.MasterType == dto.MasterType);

            //if (duplicateEntry)
            //{
            //    throw new Exception($"Journal entry already exists for this {dto.MasterType}.");
            //}

            //var transactionGroupId = Guid.NewGuid();

            //var journalNumber = $"JE-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            //var debitEntry = new JournalEntry
            //{
            //    TransactionGroupId = transactionGroupId,
            //    JournalNumber = journalNumber,
            //    CompanyId = dto.CompanyId,
            //    MasterId = dto.MasterId,
            //    MasterType = dto.MasterType,
            //    AccountId = dto.DebitAccountId,
            //    Amount = dto.Amount,
            //    TransactionType = "Debit",
            //    Description = dto.Description,
            //    CreatedBy = dto.UserId,
            //    CreatedAt = DateTime.UtcNow,
            //    IsActive = 1
            //};

            //var creditEntry = new JournalEntry
            //{
            //    TransactionGroupId = transactionGroupId,
            //    JournalNumber = journalNumber,
            //    CompanyId = dto.CompanyId,
            //    MasterId = dto.MasterId,
            //    MasterType = dto.MasterType,
            //    AccountId = dto.CreditAccountId,
            //    Amount = dto.Amount,
            //    TransactionType = "Credit",
            //    Description = dto.Description,
            //    CreatedBy = dto.UserId,
            //    CreatedAt = DateTime.UtcNow,
            //    IsActive = 1
            //};

            //await db.JournalEntry.AddRangeAsync(debitEntry,creditEntry);
            //await db.SaveChangesAsync();
        
    }
    }
}
