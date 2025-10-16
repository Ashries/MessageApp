using MessageApp.Models;

namespace MessageApp.Repositories
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(int id);
        Task<IEnumerable<Message>> GetPublicMessagesAsync();
        Task<IEnumerable<Message>> GetPrivateMessagesAsync(int userId);
        Task<IEnumerable<Message>> GetMessageThreadAsync(int messageId);
        Task AddAsync(Message message);
        Task UpdateAsync(Message message);
        Task DeleteAsync(int id);
    }
}