using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.JournalEntry
{
    public class JournalEntryCreateDTO
    {
        public int CompanyId { get; set; }

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public int DebitAccountMasterId { get; set; }

        public int CreditAccountMasterId { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
