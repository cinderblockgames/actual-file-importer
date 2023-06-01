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
    
    private EnumerationOptions CaseInsensitive { get; } = new() { MatchCasing = MatchCasing.CaseInsensitive };

    protected override void Process()
    {
        foreach (var kvp in Accounts)
        {
            ProcessAccount(kvp.Key, kvp.Value);
        }
    }

    private void ProcessAccount(string directory, AccountInfo account)
    {
        try
        {
            Console.WriteLine($"Checking account '{account.Account}' for files to import.");
            foreach (var file in Directory.EnumerateFiles(directory, "*.csv", CaseInsensitive))
            {
                ProcessAccountFile(file, account);
            }

            Console.WriteLine($"Done with account '{account.Account}.'");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void ProcessAccountFile(string file, AccountInfo account)
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}