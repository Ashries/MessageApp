using MessageApp.Data;
using MessageApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageApp.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageContext _context;

        public MessageRepository(MessageContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetPublicMessagesAsync()
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == null)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetPrivateMessagesAsync(int userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .Where(m => m.ReceiverId != null)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessageThreadAsync(int messageId)
        {
            var currentMessage = await _context.Messages
                .Include(m => m.PreviousMessage)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (currentMessage == null) return Enumerable.Empty<Message>();

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
                .ToListAsync();

            return threadMessages;
        }

        public async Task AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}