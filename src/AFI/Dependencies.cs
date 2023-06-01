using AFI.Config;
using AFI.Wrapper;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;

namespace AFI;

public static class Dependencies
{
    public static void Load(IServiceCollection services)
    {
        var env = EnvironmentVariables.Build();

        // Node.
        services.AddNodeJS();
        services.Configure<NodeJSProcessOptions>(options =>
            options.ProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "js"));
        services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
            options.TimeoutMS = 5000); // Fail faster.
        
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
        
        // Importer.
        services.AddSingleton<ImportProcessor>();
    }
}