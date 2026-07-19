public class CompanyReadDTO
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string CompanyCode { get; set; } = null!;

    public string? GSTNo { get; set; }

    public string PANNo { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CityName { get; set; } = null!;
}