using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace AIToolsAPI.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topicName;

        public KafkaProducerService(IConfiguration configuration)
        {
            var bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
            _topicName = configuration["KafkaConfig:TopicName"];

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendNotificationAsync(string messageValue)
        {
            try
            {
                var message = new Message<Null, string> { Value = messageValue };
                var result = await _producer.ProduceAsync(_topicName, message);
                Console.WriteLine($"[Kafka] Message sent to {result.Topic}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"[Kafka Error] {e.Error.Reason}");
            }
        }
    }
}