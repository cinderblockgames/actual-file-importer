using System.Text.Json.Serialization;

// https://actualbudget.github.io/docs/developers/API/#transaction

namespace AFI.Wrapper;

public class Transaction
{
    
    #region " Required "
    
    // Jed Fox:
    // You don’t need to pass the `account`, the add transaction logic will automatically set it to the `accountId` you pass in
    // [JsonPropertyName("account")] // id, required
    // public Guid Account { get; set; }
    
    [JsonPropertyName("date"), JsonDateOnly] // date, required
    public DateTime? Date { get; set; }
    
    #endregion
    
    #region " Optional "
    
    [JsonPropertyName("id")] // id
    public Guid? Id { get; set; }
    
    [JsonPropertyName("amount")] // amount
    public int? AmountInCents { get; set; }
    
    [JsonPropertyName("payee")] // id, overrides payee_name.
    public Guid? Payee { get; set; }
    
    [JsonPropertyName("payee_name")] // string, matched payee will be used or new payee will be created
    public string? PayeeName { get; set; }
    
    [JsonPropertyName("imported_payee")] // string, can be anything
    public string? ImportedPayee { get; set; }
    
    [JsonPropertyName("category")] // id
    public Guid? Category { get; set; }
    
    [JsonPropertyName("notes")] // string
    public string? Notes { get; set; }
    
    [JsonPropertyName("imported_id")] // string, usually given by the bank, used to avoid duplicate transactions
    public string? ImportedId { get; set; }
    
    [JsonPropertyName("transfer_id")] // string, the `id` of the transaction in the other account for the transfer
    public string? TransferId { get; set; }
    
    [JsonPropertyName("cleared")] // boolean, a flag indicating if the transaction has cleared or not
    public bool? Cleared { get; set; }
    
    [JsonPropertyName("subtransactions")] // Transaction[], array of subtransactions for a split transaction
    public IEnumerable<Transaction[]>? SubTransactions { get; set; }
    
    #endregion
    
}