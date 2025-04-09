# Redis Benchmarking in C\#

This repository provides a comprehensive benchmarking suite for Redis using **StackExchange.Redis** in C\#. The project compares the performance of Redis operations (`SET` and `GET`) in various modes: **Synchronous**, **Asynchronous**, and **Batch Operations**. It is designed for developers looking to understand Redis's performance under different scenarios and workloads.

---

## Table of Contents

1.  [Overview](#overview)
2.  [Benchmarked Operations](#benchmarked-operations)
3.  [Benchmark Results](#benchmark-results)
4.  [How to Run the Benchmarks](#how-to-run-the-benchmarks)
5.  [Code Explanation](#code-explanation)
6.  [Insights and Takeaways](#insights-and-takeaways)

---

## Overview

Redis is a high-performance in-memory data store, widely used for caching, real-time analytics, and more. This project demonstrates:

  - How Redis scales with increasing workloads.
  - A comparison of **synchronous**, **asynchronous**, and **batch** operations.
  - Benchmark results for up to **1 million operations**.

We leverage **StackExchange.Redis**, a popular C\# client for Redis, to interact with a locally hosted Redis instance.

---

## Benchmarked Operations

The following Redis operations were benchmarked:

1.  **Synchronous Operations**:
      - `SetStringBenchmark`: Stores key-value pairs.
      - `GetStringBenchmark`: Retrieves key-value pairs.
2.  **Asynchronous Operations**:
      - `SetStringBenchmarkAsync`: Stores key-value pairs asynchronously.
      - `GetStringBenchmarkAsync`: Retrieves key-value pairs asynchronously.
3.  **Batch Operations**:
      - `BatchSetStringBenchmarkAsync`: Groups and stores multiple key-value pairs using Redis batches.
      - `BatchGetStringBenchmarkAsync`: Groups and retrieves multiple key-value pairs using Redis batches.

---

## Benchmark Results

### Synchronous Operations

| Operations Count | `SetStringBenchmark` Time (ms) | `GetStringBenchmark` Time (ms) |
| :--------------- | :------------------------------ | :------------------------------ |
| 100              | 19                              | 16                              |
| 1,000            | 171                             | 144                             |
| 10,000           | 1,618                           | 1,475                           |
| 100,000          | 14,190                          | 14,395                          |

### Asynchronous Operations (0 ms Latency)

| Operations Count | `SetStringBenchmarkAsync` Time (ms) | `GetStringBenchmarkAsync` Time (ms) |
| :--------------- | :---------------------------------- | :---------------------------------- |
| 100              | 2                                   | 0                                   |
| 1,000            | 2                                   | 1                                   |
| 10,000           | 19                                  | 18                                  |
| 100,000          | 231                                 | 171                                 |
| 1,000,000        | 1,975                               | 1,984                               |

### Batch Operations (0 ms Latency)

| Operations Count | `BatchSetStringBenchmarkAsync` Time (ms) | `BatchGetStringBenchmarkAsync` Time (ms) |
| :--------------- | :-------------------------------------- | :-------------------------------------- |
| 100              | 1                                       | 1                                       |
| 1,000            | 4                                       | 1                                       |
| 10,000           | 28                                      | 20                                      |
| 100,000          | 156                                     | 97                                      |
| 1,000,000        | 1,675                                   | 1,581                                   |

### Asynchronous Operations (10 ms Latency)

| Operations Count | `SetStringBenchmarkAsync` Time (ms) | `GetStringBenchmarkAsync` Time (ms) |
| :--------------- | :---------------------------------- | :---------------------------------- |
| 100              | 12                                  | 14                                  |
| 1,000            | 77                                  | 13                                  |
| 10,000           | 42                                  | 25                                  |
| 100,000          | 215                                 | 243                                 |
| 1,000,000        | 2,416                               | 2,433                               |

### Batch Operations (10 ms Latency)

| Operations Count | `BatchSetStringBenchmarkAsync` Time (ms) | `BatchGetStringBenchmarkAsync` Time (ms) |
| :--------------- | :-------------------------------------- | :-------------------------------------- |
| 100              | 12                                      | 11                                      |
| 1,000            | 13                                      | 12                                      |
| 10,000           | 31                                      | 30                                      |
| 100,000          | 158                                     | 130                                     |
| 1,000,000        | 1,995                                   | 1,342                                   |

### Asynchronous Operations (100 ms Latency)

| Operations Count | `SetStringBenchmarkAsync` Time (ms) | `GetStringBenchmarkAsync` Time (ms) |
| :--------------- | :---------------------------------- | :---------------------------------- |
| 100              | 101                                 | 101                                 |
| 1,000            | 102                                 | 102                                 |
| 10,000           | 130                                 | 117                                 |
| 100,000          | 392                                 | 315                                 |
| 1,000,000        | 3,025                               | 2,803                               |

### Batch Operations (100 ms Latency)

| Operations Count | `BatchSetStringBenchmarkAsync` Time (ms) | `BatchGetStringBenchmarkAsync` Time (ms) |
| :--------------- | :-------------------------------------- | :-------------------------------------- |
| 100              | 102                                     | 101                                     |
| 1,000            | 102                                     | 103                                     |
| 10,000           | 132                                     | 123                                     |
| 100,000          | 211                                     | 203                                     |
| 1,000,000        | 2,196                                   | 1,476                                   |

-----

## How to Run the Benchmarks

### Prerequisites

  - Install .Net 9

  - Install **Redis** and run it locally (default port: 6379). You can install Redis using Docker:

    ```bash
    docker run -d --name redis -p 6379:6379 redis
    ```

## Running the benchmarks

1.  Clone this repository:

    ```bash
    git clone [https://github.com/vrn11/redis_benchmark.git](https://github.com/vrn11/redis_benchmark.git)
    ```

2.  Navigate to the project directory:

    ```bash
    cd redis_benchmark
    ```

3.  Run the benchmarks:

    ```bash
    dotnet run
    ```

    The output will display the time taken for SET and GET operations in different modes and for increasing workloads under various simulated network latencies.

## Code Explanation

### RedisConsumer Class

The `RedisConsumer` class provides methods to benchmark Redis operations in different modes:

  - **SetStringBenchmark** and **GetStringBenchmark**: Synchronous benchmarks for basic `SET` and `GET`.
  - **SetStringBenchmarkAsync** and **GetStringBenchmarkAsync**: Asynchronous benchmarks leveraging `Task.WhenAll()` for parallelism, with simulated network latency.
  - **BatchSetStringBenchmarkAsync** and **BatchGetStringBenchmarkAsync**: Benchmarks using Redis batches, with simulated network latency applied to the batch execution.

### Program.cs

This file orchestrates the benchmarks, progressively increasing the number of operations (from 100 to 1 million) and invoking the methods in the `RedisConsumer` class with different simulated network latency values (0 ms, 10 ms, and 100 ms).

## Insights and Takeaways

### 1\. Asynchronous vs. Synchronous

Asynchronous operations consistently and significantly outperform synchronous ones across all workload sizes. The synchronous operations become prohibitively slow as the number of operations increases due to blocking the calling thread for each Redis command.

### 2\. Impact of Network Latency on Asynchronous Operations

The introduction of simulated network latency directly impacts the performance of asynchronous operations. As the latency increases:

  - The time taken for asynchronous operations also increases, as each individual `SET` or `GET` operation incurs the simulated delay.
  - For **100,000 operations** with **100 ms latency**:
    - Async `SET` took **392 ms** (compared to **231 ms** with 0 ms latency).
    - Async `GET` took **315 ms** (compared to **171 ms** with 0 ms latency).

### 3\. Impact of Network Latency on Batch Operations

Batch operations demonstrate their advantage more clearly with increasing network latency:

  - With **0 ms latency**, batch operations are generally comparable to or slightly faster than asynchronous operations for smaller to medium workloads. However, at very high operation counts (1,000,000), asynchronous operations can sometimes become slightly faster for `GET`.
  - With **10 ms latency**:
    - Batch `SET` shows comparable performance to async `SET`.
    - Batch `GET` often performs better than async `GET` at higher operation counts.
  - With **100 ms latency**:
    - Batch operations show a significant performance advantage over individual asynchronous operations, especially as the number of operations increases. The single network round trip for the batch becomes much more efficient than the individual round trips for each asynchronous call. For **1,000,000 operations**:
      - Batch `SET` took **2,196 ms** (compared to async `SET` at **3,025 ms**).
      - Batch `GET` took **1,476 ms** (compared to async `GET` at **2,803 ms**).

### Key Takeaways

  - **For low-latency environments (like local setups):** Asynchronous operations provide excellent performance and scalability. Batch operations offer marginal gains for smaller grouped operations but can introduce overhead at very high scales.
  - **For high-latency environments (simulated):** Batch operations become significantly more beneficial as they reduce the number of network round trips. The higher the latency and the more operations batched, the greater the performance improvement.
  - **Synchronous operations are highly inefficient for any significant workload and should be avoided in performance-critical applications.**
  - The optimal choice between asynchronous and batch operations depends heavily on the network latency and the nature of your workload (number of operations, grouping of operations).

## Future Enhancements

  - Add benchmarks for pipeline execution and compare with batch operations under different latencies.
  - Explore Redis Cluster Mode for distributed benchmarks and the impact of network latency in a more complex topology.
  - Include more advanced use cases like Lua scripting or custom serialization.
  - Investigate optimal batch sizes for different latency scenarios and workloads.

## License

This project is licensed under the MIT License. Feel free to use and modify it for your purposes.