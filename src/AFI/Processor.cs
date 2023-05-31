using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;

namespace AFI;

internal interface IProcessor
{
    void Start(TimeSpan interval);
    ManualResetEventSlim Stop();
}

internal abstract class Processor : IProcessor, IDisposable
{
    private readonly ManualResetEventSlim _complete = new();

    protected Processor()
    {
    }

    private Timer? _timer;

    public void Start(TimeSpan interval)
    {
        if (_timer == null)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += Tick;
            _timer.Start();
        }
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Elapsed -= Tick;
            using (_timer)
            {
                _timer.Stop();
            }
        }

        GC.SuppressFinalize(this);
    }

    public ManualResetEventSlim Stop()
    {
        if (_timer != null)
        {
            _timer.Stop();
        }

        if (!_running)
        {
            _complete.Set();
        }

        return _complete;
    }

    private bool _running;
    private readonly object _locker = new();

    private void Tick(object? p, ElapsedEventArgs e)
    {
        if (!_running)
        {
            bool run = false;
            lock (_locker)
            {
                if (!_running)
                {
                    _running = true;
                    run = true;
                }
            }

            if (run)
            {
                try
                {
                    Run();
                }
                finally
                {
                    lock (_locker)
                    {
                        _running = false;
                    }
                }
            }
        }
    }

    private void Run()
    {
        _complete.Reset();
        try
        {
            Process();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        _complete.Set();
    }

    protected abstract void Process();
}

internal static class ServiceProviderExtensions
{
    public static T BuildProcessor<T>(this IServiceProvider @this, TimeSpan interval)
        where T : IProcessor
    {
        var processor = @this.GetRequiredService<T>();
        processor.Start(interval);
        return processor;
    }
}