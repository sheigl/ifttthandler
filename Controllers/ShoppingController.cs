using System;
using ifttthandler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace ifttthandler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ILogger<ShoppingController> _logger;
        private readonly IConfiguration config;
        private readonly IModel _channel;

        public ShoppingController(
            ILogger<ShoppingController> logger,
            IConfiguration config,
            IModel channel
            )
        {
            _logger = logger;
            this.config = config;
            _channel = channel;

            _channel.QueueDeclare(
                queue: "shoppinglist",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        [HttpPost]
        public IActionResult Post([FromBody] ListItem item) 
        {
            if (!CheckApiKey(item)) return Unauthorized();

            item.Action = Models.Action.Create;
            _channel.BasicPublish(
                exchange: "",
                routingKey: "shoppinglist",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item))
            );

            _logger.LogInformation(item.ToString());
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] ListItem item) 
        {
            if (!CheckApiKey(item)) return Unauthorized();

            item.Action = Models.Action.Update;
            _channel.BasicPublish(
                exchange: "",
                routingKey: "shoppinglist",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item))
            );

            _logger.LogInformation(item.ToString());
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] ListItem item) 
        {
            if (!CheckApiKey(item)) return Unauthorized();
            
            item.Action = Models.Action.Delete;
            _channel.BasicPublish(
                exchange: "",
                routingKey: "shoppinglist",
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item))
            );

            _logger.LogInformation(item.ToString());
            return Ok();
        }

        private bool CheckApiKey(ListItem item)
        {            
            var appSettings = config;

            var apiKey = appSettings["ApiKey"] ?? throw new InvalidOperationException("ApiKey was not found in configuration");
            if (item.ApiKey != apiKey)
            {
                return false;
            }

            item.ApiKey = null;

            return true;
        }
    }
}
