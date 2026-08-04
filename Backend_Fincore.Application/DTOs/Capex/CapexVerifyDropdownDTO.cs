public class CapexVerifyDropdownDTO
{
    public int CapexRequestId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string RequestedByName { get; set; } = string.Empty;
}