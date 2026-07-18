using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class JournalEntry : BaseEntity
    {
        [Key]
        public int JournalEntryId { get; set; }

        public int CompanyId { get; set; }

        // APInvoiceId / ARInvoiceId / PaymentId
        public int MasterId { get; set; }

        // APInvoice / ARInvoice / Payment
        public string MasterType { get; set; } = null!;

        public int AccountId { get; set; }

        public decimal Amount { get; set; }

        // Credit / Debit
        public string TransactionType { get; set; } = null!;

        public string? Description { get; set; }


        // Navigation properties
        public Company Company { get; set; } = null!;

        public AccountMaster AccountMaster { get; set; } = null!;
    }
}