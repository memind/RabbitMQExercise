using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rabbit;

namespace RabbitMQ.API.Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IMessagePublisherService _service;

        public PublisherController(IMessagePublisherService service) => _service = service;

        [HttpGet("/Backup")]
        public void Backup()
        {
            _service.PublishBackUpInfo();
            Task.Delay(2000);
        }


        [HttpGet("/start")]
        public void Start()
        {
            _service.PublishStartTest();
            Task.Delay(2000);
        }
        
    }
}
