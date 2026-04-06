using CurrencyProducer.API.Interface;
using CurrencyProducer.API.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Models;

namespace CurrencyProducer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class CurrencyController : ControllerBase
    {
        private readonly IRabbitMqPublisher _publisher;
        private readonly CurrencyService _service = new CurrencyService(new HttpClient());
         
        public CurrencyController(IRabbitMqPublisher publisher, CurrencyService    currencyService)
        {
            _publisher = publisher;
            _service = currencyService;
        }

        [HttpPost("publish")]
        public IActionResult Publish([FromBody] CurrencyRateMessage message)
        {
            _publisher.PublishAsync(message);
            return Ok("Message published");
        }

        [HttpPost("publishrates")]
        public async Task<ActionResult<CurrencyRatesResponseDto>> GetRates()
        {
            await _service.PublishRatesAsync();
            return Ok("All currency rates published to RabbitMQ");
        }
    }
}