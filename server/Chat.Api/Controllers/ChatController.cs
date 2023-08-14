using Chat.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/message")]
public class MessageController : Controller
{
    private readonly IChatService _chat;
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService, IChatService chat)
    {
        _messageService = messageService;
        _chat = chat;
    }

    [HttpGet("chats/history")]
    public async Task<IActionResult> GetHistory(Guid chatId)
    {
        try
        {
            var messages = await _messageService.GetFromChat(chatId);
            if (!messages.Any()) return NotFound("No messages found for the given chat ID.");
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("messages/all")]
    public async Task<IActionResult> GetAllMessages()
    {
        try
        {
            var messages = await _messageService.GetAll();
            if (!messages.Any()) return NotFound("No messages found.");
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("chats/free")]
    public async Task<IActionResult> GetFreeChats()
    {
        try
        {
            var chats = await _chat.GetFree();
            if (!chats.Any()) return NotFound("No free chats found.");
            return Ok(chats);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetChats()
    {
        try
        {
            var chats = await _chat.GetAll();
            if (!chats.Any()) return NotFound("No chats found.");
            return Ok(chats);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    private IActionResult HandleError(Exception ex)
    {
        // Log the exception here if needed
        return Problem(
            ex.Message,
            statusCode: 500,
            title: "An error occurred"
        );
    }
}