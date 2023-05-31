using System.Globalization;
using AFI;
using AFI.Config;
using AFI.Wrapper;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var env = EnvironmentVariables.Build();

        // Node.
        services.AddNodeJS();
        services.Configure<NodeJSProcessOptions>(options =>
            options.ProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "js"));
        services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            options.TimeoutMS = 5000); // Fail quicker.
        
        // Wrapper.
        services.AddSingleton(new ConnectionInfo
        {
            ServerUrl = env.ServerUrl,
            ServerPassword = env.ServerPassword,
            BudgetSyncId = Guid.Parse(env.BudgetSyncId)
        });
        services.AddSingleton<Actual>();
        
        // Accounts.
        services.AddSingleton(new AccountsInfo(env.ImportBasePath));
    })
    .Build();

var accounts = host.Services.GetRequiredService<AccountsInfo>().Accounts;

var actual = host.Services.GetRequiredService<Actual>();

var caseInsensitive = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };
foreach (var kvp in accounts)
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

                await actual.AddTransactions(account.Account!.Value, transactions);
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
