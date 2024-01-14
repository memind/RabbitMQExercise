using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rabbit;

namespace RabbitMQ.API.Consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly IMessageConsumerService _service;

        public ConsumerController(IMessageConsumerService service) => _service = service;

        [HttpGet]
        public void Connected()
        {
            _service.PublishConnectedInfo(this.ControllerContext.GetType().Name);
            Task.Delay(2000);
        }
    }
}
