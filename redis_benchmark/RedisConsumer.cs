namespace redis_benchmark;

using StackExchange.Redis;
using System.Diagnostics;
using System.Threading.Tasks;

public class RedisConsumer : IDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisConsumer(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public void SetStringBenchmark(int operations, string keyPrefix)
    {
        string functionName = nameof(SetStringBenchmark);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < operations; i++)
        {
            _db.StringSet($"key:{keyPrefix}_{i}", $"value:{i}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations in {stopwatch.ElapsedMilliseconds} ms");
    }
    
    public void GetStringBenchmark(int operations, string keyPrefix)
    {
        string functionName = nameof(GetStringBenchmark);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < operations; i++)
        {
            _db.StringGet($"key:{keyPrefix}_{i}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations in {stopwatch.ElapsedMilliseconds} ms");
    }
    
    public async Task SetStringBenchmarkAsync(int operations, string keyPrefix, int networkLatencyInMs = 0)
    {
        string functionName = nameof(SetStringBenchmarkAsync);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var tasks = new Task[operations];
        for (int i = 0; i < operations; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                var result = await _db.StringSetAsync($"key:{keyPrefix}_{i}", $"value:{i}");
                if(networkLatencyInMs > 0)
                {
                    await SimulateNetworkLatencyAsync(networkLatencyInMs);
                }
                return result;
            });
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task GetStringBenchmarkAsync(int operations, string keyPrefix, int networkLatencyInMs = 0)
    {
        string functionName = nameof(GetStringBenchmarkAsync);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var tasks = new Task[operations];
        for (int i = 0; i < operations; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                var result = await _db.StringSetAsync($"key:{keyPrefix}_{i}", $"value:{i}");
                if(networkLatencyInMs > 0)
                {
                    await SimulateNetworkLatencyAsync(networkLatencyInMs);
                }
                return result;
            });
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task BatchSetStringBenchmarkAsync(int operations, string keyPrefix, int networkLatencyInMs = 0)
    {
        string functionName = nameof(BatchSetStringBenchmarkAsync);
        var batch = _db.CreateBatch();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var tasks = new Task<bool>[operations];

        for (int i = 0; i < operations; i++)
        {
            tasks[i] = batch.StringSetAsync($"key:{keyPrefix}_{i}", $"value:{i}");
        }

        if(networkLatencyInMs > 0)
        {
            await SimulateNetworkLatencyAsync(networkLatencyInMs);
        }

        batch.Execute();
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task BatchGetStringBenchmarkAsync(int operations, string keyPrefix, int networkLatencyInMs = 0)
    {
        string functionName = nameof(BatchGetStringBenchmarkAsync);
        var batch = _db.CreateBatch();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var tasks = new Task<RedisValue>[operations];
        
        for (int i = 0; i < operations; i++)
        {
            tasks[i] = batch.StringGetAsync($"key:{keyPrefix}_{i}");
        }

        if(networkLatencyInMs > 0)
        {
            await SimulateNetworkLatencyAsync(networkLatencyInMs);
        }
        batch.Execute();
        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    private async Task SimulateNetworkLatencyAsync(int latencyInMilliseconds)
    {
        // Simulate network latency
        await Task.Delay(latencyInMilliseconds);
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}
