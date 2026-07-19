namespace Backend_Fincore.DTOs
{
    public class PermissionDTO
    {
        public int? PermissionId { get; set; }

        public string PermissionName { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
