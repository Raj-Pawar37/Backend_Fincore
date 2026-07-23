using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Payment
{
    public class PaymentMasterSearchDTO
    {
        public int MasterId { get; set; }

        public string MasterType { get; set; } = string.Empty;

        public string DocumentNumber { get; set; } = string.Empty;

        public string PartyName { get; set; } = string.Empty;

        public DateTime DocumentDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public string PaymentDirection { get; set; } = string.Empty;
    }
}
