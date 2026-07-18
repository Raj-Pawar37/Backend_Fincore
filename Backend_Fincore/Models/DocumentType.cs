using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Backend_Fincore.Models
{
    public class DocumentType : BaseEntity
    {
        [Key]
        public int DocumentTypeId { get; set; }

        public string DocumentTypeName { get; set; } = null!;

        // Navigation property
        public ICollection<Document> Documents { get; set; }
            = new List<Document>();
    }
}