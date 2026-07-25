using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.ARInvoice
{
    public class ARInvoiceDto
    {
        public int ARInvoiceId { get; set; }

        public int CustomerId { get; set; }

        public int RevenueEntryId { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string? Status { get; set; }

        public string? PONumber { get; set; }
    }
}
