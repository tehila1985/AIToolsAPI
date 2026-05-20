using System;
using System.Threading;
using Confluent.Kafka;

namespace OrderConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string bootstrapServers = "localhost:9092";
         string topic = "order-notifications";
            string groupId = "final-test-group-xyz"; 
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true, 
                
                ApiVersionFallbackMs = 0,
                BrokerAddressFamily = BrokerAddressFamily.V4
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            using var cts = new CancellationTokenSource();
            
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

          
            consumer.Subscribe(topic);

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        
                        var result = consumer.Consume(cts.Token);
                        
                        if (result is null) continue;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n[New Message Received] Partition: {result.Partition}, Offset: {result.Offset}");
                        Console.ResetColor();
                        
                        Console.WriteLine($"Raw Order Data: {result.Message.Value}");

                        consumer.Commit(result);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"[Kafka Error] Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                consumer.Close();
                Console.WriteLine("[Consumer] Order Consumer stopped.");
            }
        }
    }
}