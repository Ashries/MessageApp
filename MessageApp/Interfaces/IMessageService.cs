using MessageApp.DTOs;

namespace MessageApp.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto?> CreateMessage(CreateMessageDto createMessageDto, int senderId);
        Task<IEnumerable<MessageDto>> GetPublicMessages();
        Task<IEnumerable<MessageDto>> GetPrivateMessages(int userId);
        Task<IEnumerable<MessageDto>> GetMessageThread(int messageId);
        Task<MessageDto?> UpdateMessage(int messageId, UpdateMessageDto updateMessageDto, int userId);
        Task<bool> DeleteMessage(int messageId, int userId);
        Task<MessageDto?> GetMessageById(int messageId, int userId);
    }
}