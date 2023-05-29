using Jering.Javascript.NodeJS;

namespace AFI.Wrapper;

public class Actual
{
    
    #region " Constructor, Private Properties "
    
    private INodeJSService Node { get; }
    private ConnectionInfo ConnectionInfo { get; }

    public Actual(INodeJSService node, ConnectionInfo connectionInfo)
    {
        Node = node;
        ConnectionInfo = connectionInfo;
    }
    
    #endregion

    public async Task AddTransactions(Guid accountId, IEnumerable<Transaction> transactions)
    {
        await Invoke("addTransactions", new object[] { accountId, transactions });
    }

    #region " Invoke "
    
    private async Task Invoke(string method, object? arg = null)
    {
        await Invoke(method, new[] { arg });
    }
    
    private async Task Invoke(string method, object?[]? args)
    {
        await Node.InvokeFromFileAsync("api-wrapper.js", method, Args(args));
    }

    private async Task<T?> Invoke<T>(string method, object? arg = null)
    {
        return await Invoke<T>(method, new[] { arg });
    }

    private async Task<T?> Invoke<T>(string method, object?[]? args)
    {
        return await Node.InvokeFromFileAsync<T>("api-wrapper.js", method, Args(args));
    }

    private object?[] Args(object?[]? args)
    {
        var joined = new object?[] { ConnectionInfo };
        if (args != null)
        {
            joined = joined.Concat(args).ToArray();
        }

        return joined;
    }
    
    #endregion
    
}