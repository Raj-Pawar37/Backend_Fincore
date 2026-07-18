using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Payment : BaseEntity
    {
        [Key]
        public int PaymentId { get; set; }

        // APInvoiceId / ARInvoiceId / ExpenseClaimId
        public int MasterId { get; set; }

        // APInvoice / ARInvoice / ExpenseClaim
        public string MasterType { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        // Debit / Credit
        public string TransactionType { get; set; } = null!;

        // Cash / BankTransfer / Cheque / UPI
        public string PaymentMode { get; set; } = null!;

        public string? Remarks { get; set; }
    }
}