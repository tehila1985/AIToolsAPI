using System.Threading.Tasks;

namespace AIToolsAPI.Services
{
    public interface IKafkaProducerService
    {
        Task SendNotificationAsync(string messageValue);
    }
}