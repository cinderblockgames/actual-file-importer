using AFI;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Starting up; please wait.");

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(Dependencies.Load)
    .Build();

// Check for files every five minutes.
var importer = host.Services.BuildProcessor<ImportProcessor>(
#if DEBUG
    TimeSpan.FromMinutes(1)
#else
    TimeSpan.FromMinutes(5)
#endif
);

var stop = new ManualResetEventSlim();
AppDomain.CurrentDomain.ProcessExit += (_, _) => stop.Set();
Console.WriteLine("Running.  Awaiting SIGTERM.");
stop.Wait();

Console.WriteLine("Shutting down; please wait.");
importer.Stop().Wait();
Console.WriteLine("Shutdown complete.");

