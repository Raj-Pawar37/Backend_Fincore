namespace Backend_Fincore.DTOs
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string RoleCode { get; set; } = null!;
        public string? RoleDescription { get; set; }
        public bool IsActive { get; set; }

    }

    public class DropdownDTO
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
