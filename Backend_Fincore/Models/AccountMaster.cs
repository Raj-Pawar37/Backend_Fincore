using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class AccountMaster : BaseEntity
    {
        [Key]
        public int AccountMasterId { get; set; }

        public string AccountCode { get; set; } = null!;

        public string AccountName { get; set; } = null!;

        public string AccountType { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<JournalEntry> JournalEntries { get; set; }
    = new List<JournalEntry>();
    }
}