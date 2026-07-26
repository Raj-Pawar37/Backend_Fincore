namespace Backend_Fincore.Application.Interface
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        int MasterId { get; }
        string? MasterType { get; }
        int RoleId { get; }
    }
}