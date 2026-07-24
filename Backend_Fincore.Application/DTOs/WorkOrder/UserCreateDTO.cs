public class UserCreateDTO
{
    public int RoleId { get; set; }
    public int MasterId { get; set; }
    public string MasterType { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }   
    public string Email { get; set; }
    public string? MobileNo { get; set; }
    public int FailedLoginAttempts { get; set; }
    public byte IsEmailVerified { get; set; }
}