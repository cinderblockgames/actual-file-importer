using System.Collections.Immutable;
using System.Text.Json;

namespace AFI.Config;

public class AccountsInfo
{

    public IDictionary<string, AccountInfo> Accounts { get; }

    public AccountsInfo(string path)
    {
        Console.WriteLine($"Loading accounts from '{path}.'");
        var accounts = new Dictionary<string, AccountInfo>();
        
        var directories = Directory.EnumerateDirectories(path);
        foreach (var directory in directories)
        {
            var info = BuildAccountInfo(directory);
            if (info != null)
            {
                accounts[directory] = info;
            }
        }

        Accounts = accounts.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value);
    }

    private AccountInfo? BuildAccountInfo(string path)
    {
        var file = Path.Join(path, "account.json");
        if (File.Exists(file))
        {
            Console.WriteLine($"Loading account from '{path}.'");
            var definition = File.ReadAllText(file);
            var account = JsonSerializer.Deserialize<AccountInfo>(definition);
            ValidateAccountInfo(account!);
            return account;
        }

        Console.WriteLine($"Path '{path}' exists but does not contain an account.json definition.");
        return null;
    }

    private void ValidateAccountInfo(AccountInfo account)
    {
        if (!account.Account.HasValue || account.Account == Guid.Empty)
        {
            throw new Exception("Account Id missing.");
        }
        
        // Defaults.
        account.HeaderRows ??= 0;
        account.DateFormat ??= "yyyy-MM-dd";
        account.Delimiter ??= ",";
    }
    
}