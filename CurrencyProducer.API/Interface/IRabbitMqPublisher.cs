using Shared.Models;

namespace CurrencyProducer.API.Interface
{
    public interface IRabbitMqPublisher
    {
       
        Task PublishAsync(CurrencyRateMessage message);
       
    }
}
