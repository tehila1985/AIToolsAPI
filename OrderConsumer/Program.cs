using System;
using System.Threading;
using Confluent.Kafka;

namespace OrderConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "debug-group-" + Guid.NewGuid(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                ApiVersionFallbackMs = 0,
                BrokerAddressFamily = BrokerAddressFamily.V4,
                SessionTimeoutMs = 45000,
                HeartbeatIntervalMs = 3000,
            };

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, err) =>
                    Console.WriteLine($"[ERROR] {err.Code} | {err.Reason} | Fatal:{err.IsFatal}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                    Console.WriteLine($"[ASSIGNED] {string.Join(", ", partitions)}"))
                .SetPartitionsRevokedHandler((c, partitions) =>
                    Console.WriteLine($"[REVOKED] {string.Join(", ", partitions)}"))
                .Build();

            consumer.Subscribe("order-notifications");
            Console.WriteLine("[Consumer] Subscribed. Waiting for partition assignment...");

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(cts.Token);
                        if (result is null) continue;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[Message] Partition:{result.Partition} Offset:{result.Offset}");
                        Console.ResetColor();
                        Console.WriteLine($"Value: {result.Message.Value}");

                        consumer.Commit(result);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"[ConsumeException] {e.Error.Code} | {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                consumer.Close();
                Console.WriteLine("[Consumer] Stopped.");
            }
        }
    }
}
