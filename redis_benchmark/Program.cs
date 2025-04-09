namespace redis_benchmark;

using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using (var redisConsumer = new RedisConsumer("localhost:6379"))
        {
            await RunBenchmarks(redisConsumer);
        }
    }

    private static async Task RunBenchmarks(RedisConsumer redisConsumer)
    {
        int operations = 100; // Number of operations to perform
        int lowLatency = 10; // Low latency in milliseconds
        int highLatency = 100; // High latency in milliseconds

        // Perform synchronous benchmarks
        SetStringBenchmark(redisConsumer, operations, "sync");
        Console.WriteLine("=========================\r\n");
        GetStringBenchmark(redisConsumer, operations, "sync");
        Console.WriteLine("=========================\r\n");

        // Perform asynchronous benchmarks
        await SetStringBenchmarkAsync(redisConsumer, operations, "async");
        Console.WriteLine("=========================\r\n");
        await GetStringBenchmarkAsync(redisConsumer, operations, "async");
        Console.WriteLine("=========================\r\n");

        // Perform batch benchmarks
        await BatchSetStringBenchmarkAsync(redisConsumer, operations, "batch");
        Console.WriteLine("=========================\r\n");
        await BatchGetStringBenchmarkAsync(redisConsumer, operations, "batch");
        Console.WriteLine("=========================\r\n");

        // Perform asynchronous benchmarks with 10 ms network latency
        await SetStringBenchmarkAsync(redisConsumer, operations, "async_low_latency", lowLatency);
        Console.WriteLine("=========================\r\n");
        await GetStringBenchmarkAsync(redisConsumer, operations, "async_low_latency", lowLatency);
        Console.WriteLine("=========================\r\n");

        // Perform batch benchmarks with 10 ms network latency
        await BatchSetStringBenchmarkAsync(redisConsumer, operations, "batch_low_latency", lowLatency);
        Console.WriteLine("=========================\r\n");
        await BatchGetStringBenchmarkAsync(redisConsumer, operations, "batch_low_latency", lowLatency);
        Console.WriteLine("=========================\r\n");

        // Perform asynchronous benchmarks with 100 ms network latency
        await SetStringBenchmarkAsync(redisConsumer, operations, "async_high_latency", highLatency);
        Console.WriteLine("=========================\r\n");
        await GetStringBenchmarkAsync(redisConsumer, operations, "async_high_latency", highLatency);
        Console.WriteLine("=========================\r\n");

        // Perform batch benchmarks with 100 ms network latency
        await BatchSetStringBenchmarkAsync(redisConsumer, operations, "batch_high_latency", highLatency);
        Console.WriteLine("=========================\r\n");
        await BatchGetStringBenchmarkAsync(redisConsumer, operations, "batch_high_latency", highLatency);
        Console.WriteLine("=========================\r\n");
    }

    private static async Task BatchSetStringBenchmarkAsync(
        RedisConsumer redisConsumer, 
        int operations, 
        string keyPrefix,
        int networkLatencyInMs = 0)
    {
        // Implementation for async batch benchmark
        for (int i = 0; i < 5; i++)
        {
            await redisConsumer.BatchSetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}", networkLatencyInMs);
            operations *= 10; // Increase the number of operations for each iteration
        }
    }
    private static async Task BatchGetStringBenchmarkAsync(
        RedisConsumer redisConsumer, 
        int operations, 
        string keyPrefix, 
        int networkLatencyInMs = 0)
    {
        // Implementation for async batch benchmark
        for (int i = 0; i < 5; i++)
        {
            await redisConsumer.BatchGetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}", networkLatencyInMs);
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static async Task SetStringBenchmarkAsync(
        RedisConsumer redisConsumer, 
        int operations, 
        string keyPrefix, 
        int networkLatencyInMs = 0)
    {
        // Implementation for async benchmark
        for (int i = 0; i < 5; i++)
        {
            await redisConsumer.SetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}", networkLatencyInMs);
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static async Task GetStringBenchmarkAsync(
        RedisConsumer redisConsumer, 
        int operations, 
        string keyPrefix, 
        int networkLatencyInMs = 0)
    {
        // Implementation for async benchmark
        for (int i = 0; i < 5; i++)
        {
            await redisConsumer.GetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}", networkLatencyInMs);
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static void SetStringBenchmark(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        // Implementation for sync benchmark
        for (int i = 0; i < 4; i++)
        {
            redisConsumer.SetStringBenchmark(operations, $"{keyPrefix}_{i}");
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static void GetStringBenchmark(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        // Implementation for sync benchmark
        for (int i = 0; i < 4; i++)
        {
            redisConsumer.GetStringBenchmark(operations, $"{keyPrefix}_{i}");
            operations *= 10; // Increase the number of operations for each iteration
        }
    }
}
