using Microsoft.AspNetCore.Mvc;
using MessageApp.DTOs;
using MessageApp.Interfaces;

namespace MessageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto createMessageDto)
        {
            var userId = GetCurrentUserId();

            var message = await _messageService.CreateMessage(createMessageDto, userId);
            if (message == null)
                return BadRequest("Invalid sender, receiver, or previous message");

            return CreatedAtAction(nameof(GetMessageById), new { id = message.Id }, message);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicMessages()
        {
            var messages = await _messageService.GetPublicMessages();
            return Ok(messages);
        }

        [HttpGet("private")]
        public async Task<IActionResult> GetPrivateMessages()
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetPrivateMessages(userId);
            return Ok(messages);
        }

        [HttpGet("thread/{messageId}")]
        public async Task<IActionResult> GetMessageThread(int messageId)
        {
            var messages = await _messageService.GetMessageThread(messageId);
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.GetMessageById(id, userId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] UpdateMessageDto updateMessageDto)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.UpdateMessage(id, updateMessageDto, userId);
            if (message == null)
                return NotFound("Message not found or you don't have permission to edit it");

            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _messageService.DeleteMessage(id, userId);
            if (!result)
                return NotFound("Message not found or you don't have permission to delete it");

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            if (Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) &&
                int.TryParse(userIdHeader, out var userId))
            {
                return userId;
            }
            return 1; // Default user for testing
        }
    }
}