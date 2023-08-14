using System.Text.Json;
using Chat.Api.Producer;
using Chat.Domain.Dto;
using Chat.Domain.Messages;
using Chat.Domain.Metadata;
using Chat.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Chat.Api.Controllers;
[ApiController]
[Route("api/metadata")]
public class MetaDataController : Controller
{
    private readonly ICacheService _cacheService;
    private readonly IMongoDbContext _mongoDbContext;
    private readonly IRabbitMqProducer _producer;
    
    public MetaDataController(ICacheService cacheService, IMongoDbContext mongoDbContext, IRabbitMqProducer producer)
    {
        _cacheService = cacheService;
        _cacheService.ChangeDatabase(Database.Meta);
        _mongoDbContext = mongoDbContext;
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> Set([FromForm] Guid requestId, [FromForm] MongoFile meta)
    {
        var metaJson = JsonSerializer.Serialize(meta);
        Console.WriteLine($"Meta received: {metaJson}");
        _cacheService.SetData(requestId.ToString(), metaJson);
        _producer.SendMessage(new MetaUploadMessage() { RequestId = requestId }, "ChatApp.Meta");

        return Ok("Metadata successfully uploaded");
    }
}