using System.Globalization;
using AFI.Config;
using AFI.Wrapper;

namespace AFI;

internal class ImportProcessor : Processor
{
    private IDictionary<string, AccountInfo> Accounts { get; }
    private Actual Actual { get; }
    
    public ImportProcessor(AccountsInfo accounts, Actual actual)
    {
        Accounts = accounts.Accounts;
        Actual = actual;
    }

    protected override void Process()
    {
        var caseInsensitive = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
        foreach (var kvp in Accounts)
        {
            var path = kvp.Key;
            var account = kvp.Value;
            try
            {
                Console.WriteLine($"Checking account '{account.Account}' for files to import.");
                foreach (var file in Directory.EnumerateFiles(path, "*.csv", caseInsensitive))
                {
                    try
                    {
                        Console.WriteLine($"Processing file '{file}.'");
                        var rows = File.ReadAllLines(file).Skip(account.HeaderRows!.Value);
                        var transactions = new List<Transaction>();
                        foreach (var row in rows)
                        {
                            var fields = row.Split(account.Delimiter);
                            var transaction = new Transaction();

                            if (account.DateColumn.HasValue)
                            {
                                transaction.Date = DateTime.ParseExact(
                                    fields[account.DateColumn!.Value],
                                    account.DateFormat!,
                                    CultureInfo.InvariantCulture
                                );
                            }

                            if (account.PayeeColumn.HasValue)
                            {
                                transaction.PayeeName = fields[account.PayeeColumn!.Value];
                            }

                            if (account.AmountColumn.HasValue)
                            {
                                transaction.AmountInCents = Convert.ToInt32(
                                    decimal.Parse(fields[account.AmountColumn!.Value]) * 100
                                );
                            }

                            transactions.Add(transaction);
                        }

                        Actual.AddTransactions(account.Account!.Value, transactions).GetAwaiter().GetResult();
                        File.Delete(file); // Done with this file.
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2);
                    }
                }

                Console.WriteLine($"Done with account '{account.Account}.'");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}