using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public class AssetsDTO
    {
        public int AssetId { get; set; }

        public int GRNId { get; set; }

        public string GRNNumber { get; set; } = null!;

        public int POItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public string AssetCode { get; set; } = null!;

        public string AssetName { get; set; } = null!;

        public decimal PurchaseCost { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string Status { get; set; } = null!;

        public int? AssignedTo { get; set; }

        public string? AssignedUserName { get; set; }
    }
}
