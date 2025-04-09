namespace redis_benchmark;

using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var redisConsumer = new RedisConsumer("localhost:6379");
        int operations = 100; // Number of operations to perform
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
    }


    private static async Task BatchSetStringBenchmarkAsync(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        // Implementation for async batch benchmark
        for (int i = 0; i < 6; i++)
        {
            await redisConsumer.BatchSetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}");
            operations *= 10; // Increase the number of operations for each iteration
        }
    }
    private static async Task BatchGetStringBenchmarkAsync(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        // Implementation for async batch benchmark
        for (int i = 0; i < 6; i++)
        {
            await redisConsumer.BatchGetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}");
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static async Task SetStringBenchmarkAsync(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        // Implementation for async benchmark
        for (int i = 0; i < 6; i++)
        {
            await redisConsumer.SetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}");
            operations *= 10; // Increase the number of operations for each iteration
        }
    }

    private static async Task GetStringBenchmarkAsync(RedisConsumer redisConsumer, int operations, string keyPrefix)
    {
        for (int i = 0; i < 6; i++)
        {
            await redisConsumer.GetStringBenchmarkAsync(operations, $"{keyPrefix}_{i}");
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
