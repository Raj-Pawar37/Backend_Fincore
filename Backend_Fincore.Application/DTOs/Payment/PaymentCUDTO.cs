using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Payment
{
    public class PaymentCUDTO
    {
        public int PaymentId { get; set; }

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string TransactionType { get; set; } = null!;

        public string PaymentMode { get; set; } = null!;

        public string? Remarks { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
