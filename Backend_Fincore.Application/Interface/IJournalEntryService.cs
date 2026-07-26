using Backend_Fincore.Application.DTOs.JournalEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IJournalEntryService
    {
        Task CreateEntry(JournalEntryCreateDTO dto);
    }
}
