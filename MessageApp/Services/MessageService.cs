using MessageApp.DTOs;
using MessageApp.Interfaces;
using MessageApp.Models;
using MessageApp.Repositories;

namespace MessageApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<MessageDto?> CreateMessage(CreateMessageDto createMessageDto, int senderId)
        {
            var sender = await _userRepository.GetByIdAsync(senderId);
            if (sender == null) return null;

            if (createMessageDto.ReceiverId.HasValue)
            {
                var receiver = await _userRepository.GetByIdAsync(createMessageDto.ReceiverId.Value);
                if (receiver == null) return null;
            }

            if (createMessageDto.PreviousMessageId.HasValue)
            {
                var previousMessage = await _messageRepository.GetByIdAsync(createMessageDto.PreviousMessageId.Value);
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

            await _messageRepository.AddAsync(message);
            return await GetMessageDtoById(message.Id);
        }

        public async Task<IEnumerable<MessageDto>> GetPublicMessages()
        {
            var messages = await _messageRepository.GetPublicMessagesAsync();
            return messages.Select(m => new MessageDto
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
            });
        }

        public async Task<IEnumerable<MessageDto>> GetPrivateMessages(int userId)
        {
            var messages = await _messageRepository.GetPrivateMessagesAsync(userId);
            return messages.Select(m => new MessageDto
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
            });
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(int messageId)
        {
            var messages = await _messageRepository.GetMessageThreadAsync(messageId);
            return messages.Select(m => new MessageDto
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
            });
        }

        public async Task<MessageDto?> UpdateMessage(int messageId, UpdateMessageDto updateMessageDto, int userId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.SenderId != userId)
                return null;

            message.Title = updateMessageDto.Title;
            message.Content = updateMessageDto.Content;

            await _messageRepository.UpdateAsync(message);
            return await GetMessageDtoById(message.Id);
        }

        public async Task<bool> DeleteMessage(int messageId, int userId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.SenderId != userId)
                return false;

            await _messageRepository.DeleteAsync(messageId);
            return true;
        }

        public async Task<MessageDto?> GetMessageById(int messageId, int userId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null) return null;

            if (message.ReceiverId != null && message.SenderId != userId && message.ReceiverId != userId)
                return null;

            return await GetMessageDtoById(messageId);
        }

        private async Task<MessageDto?> GetMessageDtoById(int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null) return null;

            return new MessageDto
            {
                Id = message.Id,
                Title = message.Title,
                Content = message.Content,
                SentAt = message.SentAt,
                IsPublic = message.ReceiverId == null,
                Sender = new UserDto
                {
                    Id = message.Sender.Id,
                    Username = message.Sender.Username,
                    FirstName = message.Sender.FirstName,
                    LastName = message.Sender.LastName
                },
                Receiver = message.Receiver != null ? new UserDto
                {
                    Id = message.Receiver.Id,
                    Username = message.Receiver.Username,
                    FirstName = message.Receiver.FirstName,
                    LastName = message.Receiver.LastName
                } : null,
                PreviousMessageId = message.PreviousMessageId
            };
        }
    }
}