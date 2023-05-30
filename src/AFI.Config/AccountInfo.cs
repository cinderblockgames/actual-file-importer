namespace AFI.Config;

// account.json
public class AccountInfo
{
    // Required.
    public Guid? Account { get; set; }
    
    // Skip by default.
    public ushort? DateColumn { get; set; }
    public ushort? PayeeColumn { get; set; }
    public ushort? AmountColumn { get; set; }
    
    // Valued by default.
    public ushort? HeaderRows { get; set; }
    public string? DateFormat { get; set; }
    public string? Delimiter { get; set; }
}