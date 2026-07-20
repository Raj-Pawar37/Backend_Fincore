using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public class AssetsCUDTO
    {
        public int AssetId { get; set; }

        public int GRNId { get; set; }

        public int POItemId { get; set; }

      

        public string AssetName { get; set; } = null!;

        public decimal PurchaseCost { get; set; }

        public DateTime PurchaseDate { get; set; }

        // Available / Assigned / Retired
        public string Status { get; set; } = null!;

        public int? AssignedTo { get; set; }

    }
}
