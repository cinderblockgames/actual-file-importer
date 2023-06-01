using File = SLSAK.Docker.IO.File;

namespace AFI;
using Environment = SLSAK.Utilities.Environment;

public class EnvironmentVariables
{
    public string ServerUrl { get; }
    public string ServerPassword { get; }
    public string BudgetSyncId { get;  }
    public string ImportBasePath { get; }

    private EnvironmentVariables(string serverUrl, string serverPassword, string budgetSyncId, string importBasePath)
    {
        ServerUrl = serverUrl;
        ServerPassword = serverPassword;
        BudgetSyncId = budgetSyncId;
        ImportBasePath = importBasePath;
    }

    public static EnvironmentVariables Build()
    {
        var env = Environment.GetEnvironmentVariables(false);

        if (!env.TryGetValue("SERVER_URL", out string? serverUrl) || string.IsNullOrWhiteSpace(serverUrl))
        {
            throw new Exception("SERVER_URL must be valued.");
        }
        
        if (!env.TryGetValue("SERVER_PASSWORD", out string? serverPassword) || string.IsNullOrWhiteSpace(serverPassword))
        {
            if (!env.TryGetValue("SERVER_PASSWORD_FILE", out string? serverPasswordFile) ||
                string.IsNullOrWhiteSpace(serverPasswordFile) ||
                !File.Exists(serverPasswordFile))
            {
                throw new Exception("SERVER_PASSWORD or SERVER_PASSWORD_FILE (and related file) must be valued.");
            }

            serverPassword = File.ReadAllText(serverPasswordFile);
            if (string.IsNullOrWhiteSpace(serverPassword))
            {
                throw new Exception("SERVER_PASSWORD or SERVER_PASSWORD_FILE (and related file) must be valued.");
            }
        }
        
        if (!env.TryGetValue("BUDGET_SYNC_ID", out string? budgetSyncId) || string.IsNullOrWhiteSpace(budgetSyncId))
        {
            throw new Exception("BUDGET_SYNC_ID must be valued.");
        }

        if (!env.TryGetValue("IMPORT_BASE_PATH", out string? importBasePath) || string.IsNullOrWhiteSpace(importBasePath))
        {
            Console.WriteLine("IMPORT_BASE_PATH not provided; defaulting to '/import.'");
            importBasePath = "/import";
        }

        return new EnvironmentVariables(serverUrl, serverPassword, budgetSyncId, importBasePath);
    }
}