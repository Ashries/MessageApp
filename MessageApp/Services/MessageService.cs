using MessageApp.Data;
using MessageApp.Models;
using MessageApp.DTOs;
using MessageApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessageContext _context;

        public MessageService(MessageContext context)
        {
            _context = context;
        }

        public async Task<MessageDto?> CreateMessage(CreateMessageDto createMessageDto, int senderId)
        {
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null) return null;

            if (createMessageDto.ReceiverId.HasValue)
            {
                var receiver = await _context.Users.FindAsync(createMessageDto.ReceiverId.Value);
                if (receiver == null) return null;
            }

            if (createMessageDto.PreviousMessageId.HasValue)
            {
                var previousMessage = await _context.Messages.FindAsync(createMessageDto.PreviousMessageId.Value);
                if (previousMessage == null) return null;
            }

            var message = new Message
            {
                Title = createMessageDto.Title,
                Content = createMessageDto.Content,
                SenderId = senderId,
                ReceiverId = createMessageDto.ReceiverId,
                PreviousMessageId = createMessageDto.PreviousMessageId,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return await GetMessageDtoById(message.Id);
        }

        public async Task<IEnumerable<MessageDto>> GetPublicMessages()
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == null)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsPublic = true,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        FirstName = m.Sender.FirstName,
                        LastName = m.Sender.LastName
                    },
                    PreviousMessageId = m.PreviousMessageId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageDto>> GetPrivateMessages(int userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .Where(m => m.ReceiverId != null)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsPublic = false,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        FirstName = m.Sender.FirstName,
                        LastName = m.Sender.LastName
                    },
                    Receiver = m.Receiver != null ? new UserDto
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        FirstName = m.Receiver.FirstName,
                        LastName = m.Receiver.LastName
                    } : null,
                    PreviousMessageId = m.PreviousMessageId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(int messageId)
        {
            var currentMessage = await _context.Messages
                .Include(m => m.PreviousMessage)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (currentMessage == null) return Enumerable.Empty<MessageDto>();

            while (currentMessage.PreviousMessageId.HasValue)
            {
                currentMessage = await _context.Messages
                    .Include(m => m.PreviousMessage)
                    .FirstOrDefaultAsync(m => m.Id == currentMessage.PreviousMessageId.Value);
                if (currentMessage == null) break;
            }

            var threadMessages = await _context.Messages
                .Where(m => m.Id == currentMessage.Id || m.PreviousMessageId == currentMessage.Id)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsPublic = m.ReceiverId == null,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        FirstName = m.Sender.FirstName,
                        LastName = m.Sender.LastName
                    },
                    Receiver = m.Receiver != null ? new UserDto
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        FirstName = m.Receiver.FirstName,
                        LastName = m.Receiver.LastName
                    } : null,
                    PreviousMessageId = m.PreviousMessageId
                })
                .ToListAsync();

            return threadMessages;
        }

        public async Task<MessageDto?> UpdateMessage(int messageId, UpdateMessageDto updateMessageDto, int userId)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null || message.SenderId != userId)
                return null;

            message.Title = updateMessageDto.Title;
            message.Content = updateMessageDto.Content;

            await _context.SaveChangesAsync();

            return await GetMessageDtoById(message.Id);
        }

        public async Task<bool> DeleteMessage(int messageId, int userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || message.SenderId != userId)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MessageDto?> GetMessageById(int messageId, int userId)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null) return null;

            if (message.ReceiverId != null && message.SenderId != userId && message.ReceiverId != userId)
                return null;

            return await GetMessageDtoById(messageId);
        }

        private async Task<MessageDto?> GetMessageDtoById(int messageId)
        {
            return await _context.Messages
                .Where(m => m.Id == messageId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsPublic = m.ReceiverId == null,
                    Sender = new UserDto
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        FirstName = m.Sender.FirstName,
                        LastName = m.Sender.LastName
                    },
                    Receiver = m.Receiver != null ? new UserDto
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        FirstName = m.Receiver.FirstName,
                        LastName = m.Receiver.LastName
                    } : null,
                    PreviousMessageId = m.PreviousMessageId
                })
                .FirstOrDefaultAsync();
        }
    }
}