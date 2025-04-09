# Redis Benchmarking in C#

This repository provides a comprehensive benchmarking suite for Redis using **StackExchange.Redis** in C#. The project compares the performance of Redis operations (`SET` and `GET`) in various modes: **Synchronous**, **Asynchronous**, and **Batch Operations**. It is designed for developers looking to understand Redis's performance under different scenarios and workloads.

---

## Table of Contents
1. [Overview](#overview)
2. [Benchmarked Operations](#benchmarked-operations)
3. [Benchmark Results](#benchmark-results)
4. [How to Run the Benchmarks](#how-to-run-the-benchmarks)
5. [Code Explanation](#code-explanation)
6. [Insights and Takeaways](#insights-and-takeaways)

---

## Overview

Redis is a high-performance in-memory data store, widely used for caching, real-time analytics, and more. This project demonstrates:
- How Redis scales with increasing workloads.
- A comparison of **synchronous**, **asynchronous**, and **batch** operations.
- Benchmark results for up to **10 million operations**.

We leverage **StackExchange.Redis**, a popular C# client for Redis, to interact with a locally hosted Redis instance.

---

## Benchmarked Operations

The following Redis operations were benchmarked:
1. **Synchronous Operations**:
   - `SetStringBenchmark`: Stores key-value pairs.
   - `GetStringBenchmark`: Retrieves key-value pairs.
2. **Asynchronous Operations**:
   - `SetStringBenchmarkAsync`: Stores key-value pairs asynchronously.
   - `GetStringBenchmarkAsync`: Retrieves key-value pairs asynchronously.
3. **Batch Operations**:
   - `BatchSetStringBenchmarkAsync`: Groups and stores multiple key-value pairs using Redis batches.
   - `BatchGetStringBenchmarkAsync`: Groups and retrieves multiple key-value pairs using Redis batches.

---

## Benchmark Results

### Synchronous Operations
| Operations Count | `SetStringBenchmark` Time (ms) | `GetStringBenchmark` Time (ms) |
|------------------|--------------------------------|--------------------------------|
| 100              | 80                             | 18                             |
| 1,000            | 465                            | 144                            |
| 10,000           | 3,386                          | 1,449                          |
| 100,000          | 15,864                         | 14,429                         |

### Asynchronous Operations
| Operations Count | `SetStringBenchmarkAsync` Time (ms) | `GetStringBenchmarkAsync` Time (ms) |
|------------------|-------------------------------------|-------------------------------------|
| 100              | 1                                   | 0                                   |
| 1,000            | 3                                   | 0                                   |
| 10,000           | 24                                  | 8                                   |
| 100,000          | 264                                 | 125                                 |
| 1,000,000        | 1,220                               | 941                                 |
| 10,000,000       | 1,3755                              | 10,846                              |

### Batch Operations
| Operations Count | `BatchSetStringBenchmarkAsync` Time (ms) | `BatchGetStringBenchmarkAsync` Time (ms) |
|------------------|-----------------------------------------|-----------------------------------------|
| 100              | 2                                       | 11                                      |
| 1,000            | 2                                       | 19                                      |
| 10,000           | 28                                      | 86                                      |
| 100,000          | 160                                     | 429                                     |
| 1,000,000        | 2,260                                   | 2,395                                   |
| 10,000,000       | 29,484                                  | 1,8060                                  |

---

## How to Run the Benchmarks

### Prerequisites
- Install .Net 9
- Install **Redis** and run it locally (default port: 6379). You can install Redis using Docker:
  ```bash
  docker run -d --name redis -p 6379:6379 redis
  ```

## Running the benchmarks
1. Clone this repository:
  ```bash
  git clone https://github.com/vrn11/redis_benchmark.git
  ```
2. Navigate to the project directory:
  ```
  cd redis_benchmark
  ```
3. Run the benchmarks
  ``` 
  dotnet run
  ```
  The output will display the time taken for SET and GET operations in different modes and for increasing workloads.

## Code Explanation

### RedisConsumer Class
The `RedisConsumer` class provides methods to benchmark Redis operations in different modes:

- **SetStringBenchmark** and **GetStringBenchmark**: Synchronous benchmarks for basic `SET` and `GET`.
- **SetStringBenchmarkAsync** and **GetStringBenchmarkAsync**: Asynchronous benchmarks leveraging `Task.WhenAll()` for parallelism.
- **BatchSetStringBenchmarkAsync** and **BatchGetStringBenchmarkAsync**: Benchmarks using Redis batches.

### Program.cs
This file orchestrates the benchmarks, progressively increasing the number of operations (from 100 to 10 million) and invoking the methods in the `RedisConsumer` class.

## Insights and Takeaways

### 1. Asynchronous vs. Synchronous
Asynchronous operations significantly outperform synchronous ones, especially as workloads increase. For **100 Thousand operations**:

- Async `SET` took **264 ms**, while Sync `SET` took **15,864 ms**.
- Async `GET` took **125 ms**, while Sync `GET` took **14,429 ms**.

### 2. Asynchronous vs. Batch (Local Docker Environment)

**Batch Operations**:
Batch operations group multiple commands into a single network call using `batch.Execute()`, aiming to minimize network round-trip overhead.
In this local Docker environment with minimal network latency, batch operations show mixed results compared to asynchronous operations:

- For **smaller workloads (up to 10,000 operations)**, batch operations can sometimes be slightly faster or comparable to asynchronous operations, likely due to the reduced number of individual calls.
- However, for **larger workloads (100,000 operations and above)**, batch operations tend to become **slower** than individual asynchronous operations.

**Asynchronous Operations**:
Async operations leverage `Task.WhenAll()` to execute multiple commands concurrently, allowing the application to remain responsive while waiting for I/O.
In this local setup, asynchronous operations demonstrate better scalability for larger workloads, likely due to lower internal processing overhead compared to managing large batches.

**Batching Overhead in Low Latency**:
The overhead associated with creating and managing batches (queuing commands, serializing/deserializing batch requests/responses) appears to become more significant than the benefit of reduced network round trips when the network latency is very low.

**Key Takeaways (Local Docker Environment)**
- **Async is generally faster for larger workloads**: Parallel execution of individual asynchronous operations scales efficiently in this low-latency environment.
- **Batch can be faster for small, grouped operations**: For a few hundred to a few thousand related commands, batching might offer a slight advantage.
- **Batching overhead becomes significant at scale locally**: For millions of operations in a low-latency setup, the internal overhead of batching can outweigh its benefits.

**Important Note**: These results are specific to a local Docker environment with minimal network latency. In environments with higher network latency, the benefits of batching in reducing round-trip time are expected to be much more pronounced, potentially making batch operations faster than individual asynchronous calls for larger workloads. It is crucial to benchmark in your target deployment environment to get accurate performance characteristics.

### 3. Redis Scalability
Redis demonstrates good scalability, with asynchronous operations handling up to 10 million operations in around 10-11 seconds in this local test.

## Future Enhancements
- Add benchmarks for pipeline execution and compare with batch operations.
- Explore Redis Cluster Mode for distributed benchmarks and the impact of network latency.
- Include more advanced use cases like Lua scripting or custom serialization.
- Introduce simulated network latency to explicitly measure the benefits of batching under different network conditions.

## License
This project is licensed under the MIT License. Feel free to use and modify it for your purposes.

