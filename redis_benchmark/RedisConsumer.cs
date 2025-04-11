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

    public async Task LuaSetStringBenchmarkBatchingAsync(int operations, string keyPrefix, int batchSize, int networkLatencyInMs = 0)
    {
        string functionName = nameof(LuaSetStringBenchmarkBatchingAsync);

        string luaScript = @"
            for i=1,tonumber(ARGV[1]) do
                redis.call('SET', KEYS[i], ARGV[i+1])
            end
        ";
        
        // Max number of parameters for Lua script: 1048576 (1024*1024)
        // https://github.com/StackExchange/StackExchange.Redis/blob/main/src/StackExchange.Redis/PhysicalConnection.cs#L856

        int totalBatches = (int)Math.Ceiling((double)operations / batchSize);
        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            int currentBatchSize = Math.Min(batchSize, operations - (batchIndex * batchSize));
            RedisKey[] keys = new RedisKey[currentBatchSize];
            RedisValue[] values = new RedisValue[currentBatchSize + 1];
            values[0] = currentBatchSize.ToString();

            for (int i = 0; i < currentBatchSize; i++)
            {
                int globalIndex = batchIndex * batchSize + i;
                keys[i] = $"{keyPrefix}_{globalIndex}";
                values[i + 1] = $"value:{globalIndex}";
            }

            if (networkLatencyInMs > 0)
            {
                await SimulateNetworkLatencyAsync(networkLatencyInMs);
            }

            await _db.ScriptEvaluateAsync(
                luaScript,
                keys: keys,
                values: values
            );
        }

        stopwatch.Stop();

        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task LuaGetStringBenchmarkBatchingAsync(int operations, string keyPrefix, int batchSize, int networkLatencyInMs = 0)
    {
        string functionName = nameof(LuaGetStringBenchmarkBatchingAsync);

        // Lua script for batch GET operations
        string luaScript = @"
            local result = {}
            for i=1,tonumber(ARGV[1]) do
                table.insert(result, redis.call('GET', KEYS[i]))
            end
            return result
        ";

        Stopwatch stopwatch = Stopwatch.StartNew();

        int totalBatches = (int)Math.Ceiling((double)operations / batchSize);

        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            // Determine the size of the current batch
            int currentBatchSize = Math.Min(batchSize, operations - (batchIndex * batchSize));

            // Prepare keys and values for the current batch
            RedisKey[] keys = new RedisKey[currentBatchSize];
            RedisValue[] values = new RedisValue[currentBatchSize + 1];
            values[0] = currentBatchSize.ToString(); // Pass the batch size as the first argument

            for (int i = 0; i < currentBatchSize; i++)
            {
                int globalIndex = batchIndex * batchSize + i;
                keys[i] = $"{keyPrefix}_{globalIndex}";
                values[i + 1] = string.Empty; // Placeholder for the value
            }

            if (networkLatencyInMs > 0)
            {
                await SimulateNetworkLatencyAsync(networkLatencyInMs);
            }

            // Execute Lua script for the current batch
            var results = await _db.ScriptEvaluateAsync(
                luaScript,
                keys: keys,
                values: values
            );
        }

        stopwatch.Stop();

        Console.WriteLine($"Performed {operations} {functionName} operations with {networkLatencyInMs} ms latency in {stopwatch.ElapsedMilliseconds} ms");
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }

    private async Task SimulateNetworkLatencyAsync(int latencyInMilliseconds)
    {
        // Simulate network latency
        await Task.Delay(latencyInMilliseconds);
    }
}
