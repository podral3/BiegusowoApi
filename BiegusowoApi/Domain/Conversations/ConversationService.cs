using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
using BiegusowoApi.Domain.Dtos.Conversation;
using BiegusowoApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Domain.Conversations;

public class ConversationService(ApplicationDbContext dbContext) : IConversationService
{
    private const int MaxMessageLength = 2000;

    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<User?> GetLocalUserIdAsync(Guid identityId) //TODO do wywalenia
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == identityId);
    }

    public async Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
        Guid identityId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize)
    {
        User? user = await GetLocalUserIdAsync(identityId);
        if (user is null)
            return new CursorPaginatedList<MinimalConversationDto>([], false);

        var query =
        from c in _dbContext.Conversations
        where c.BuyerId == user.Id || c.SellerId == user.Id
        let lastMessage = _dbContext.Messages
            .Where(m => m.ConversationId == c.Id)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault()
        select new { Conversation = c, LastMessage = lastMessage };

        if (beforeLastMessageAt is not null && beforeConversationId is not null)
        {
            query = query.Where(x =>
                (x.LastMessage != null ? x.LastMessage.CreatedAt : x.Conversation.CreatedAt) < beforeLastMessageAt ||
                ((x.LastMessage != null ? x.LastMessage.CreatedAt : x.Conversation.CreatedAt) == beforeLastMessageAt
                    && x.Conversation.Id < beforeConversationId));
        }

        var page = await query
            .OrderByDescending(x => x.LastMessage != null ? x.LastMessage.CreatedAt : x.Conversation.CreatedAt)
            .ThenByDescending(x => x.Conversation.Id)
            .Take(pageSize + 1)
            .Select(x => new
            {
                x.Conversation.Id,
                x.Conversation.ListingId,
                ListingTitle = x.Conversation.Listing.Title,
                x.LastMessage,
                OtherUser = x.Conversation.BuyerId == user.Id ? x.Conversation.Seller : x.Conversation.Buyer,
                OtherUserIsSeller = x.Conversation.BuyerId == user.Id// if current user is buyer, the other side is the seller
            })
            .ToListAsync();

        var hasNextPage = page.Count > pageSize;
        var items = page.Take(pageSize).Select(x => new MinimalConversationDto(
            x.Id,
            x.ListingId,
            x.ListingTitle,
            new ConversationParticipantDto(x.OtherUser.Id, x.OtherUserIsSeller, x.OtherUser.DisplayName, x.OtherUser.AvatarFileName),
            x.LastMessage == null ? null : new MessageDto(
                x.LastMessage.Id, x.LastMessage.SenderId,
                x.LastMessage.Body, x.LastMessage.MessageStatus, x.LastMessage.CreatedAt)
        )).ToList();

        return new CursorPaginatedList<MinimalConversationDto>(items, hasNextPage);
    }

    public async Task<Result<ConversationDto>> GetConversationAsync(
        Guid identityId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize)
    {
        User? user = await GetLocalUserIdAsync(identityId);
        if (user is null) return Result<ConversationDto>.Failure(ServiceError.Forbidden);

        var conversation = await _dbContext.Conversations
            .Include(c => c.Buyer).Include(c => c.Seller).Include(c => c.Listing)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation is null)
            return Result<ConversationDto>.Failure(ServiceError.NotFound);

        if (conversation.BuyerId != user.Id && conversation.SellerId != user.Id)
            return Result<ConversationDto>.Failure(ServiceError.Forbidden);

        var messageQuery = _dbContext.Messages.Where(m => m.ConversationId == conversationId);

        if (beforeCreatedAt is not null && beforeMessageId is not null)
        {
            messageQuery = messageQuery.Where(m =>
                m.CreatedAt < beforeCreatedAt ||
                (m.CreatedAt == beforeCreatedAt && m.Id < beforeMessageId));
        }

        var page = await messageQuery
            .OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id)
            .Take(pageSize + 1)
            .ToListAsync();

        var hasNextPage = page.Count > pageSize;
        var messages = page.Take(pageSize)
            .Select(m => new MessageDto(m.Id, m.SenderId, m.Body, m.MessageStatus, m.CreatedAt))
            .ToList();

        ConversationParticipantDto receipient;
        if (user.Id == conversation.BuyerId)
        {
            receipient = new ConversationParticipantDto(conversation.Seller.Id, true, conversation.Seller.DisplayName, conversation.Seller.AvatarFileName);
        }
        else
        {
            receipient = new ConversationParticipantDto(conversation.Buyer.Id, false, conversation.Buyer.DisplayName, conversation.Buyer.AvatarFileName);
        }

        var dto = new ConversationDto(
            conversation.Id, conversation.ListingId, conversation.Listing.Title,
            receipient,
            new CursorPaginatedList<MessageDto>(messages, hasNextPage));
        return Result<ConversationDto>.Success(dto);
    }

    public async Task<Result<ConversationDto>> CreateConversationAsync(Guid buyerId, ConversationRequest request)
    {
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.ListingId == request.ListingId && c.BuyerId == buyerId);
        if (conversation is not null)
        {
            return Result<ConversationDto>.Failure(ServiceError.Conflict);
        }

        var listing = await _dbContext.Listings
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == request.ListingId);
        if (listing is null) return Result<ConversationDto>.Failure(ServiceError.NotFound);

        if (listing.UserId == buyerId) return Result<ConversationDto>.Failure(ServiceError.Conflict);

        conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            BuyerId = buyerId,
            SellerId = listing.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync();
        
        ConversationParticipantDto receipient;
        if (buyerId == conversation.BuyerId)
        {
            receipient = new ConversationParticipantDto(conversation.Seller.Id, true, conversation.Seller.DisplayName, conversation.Seller.AvatarFileName);
        }
        else
        {
            receipient = new ConversationParticipantDto(conversation.Buyer.Id, false, conversation.Buyer.DisplayName, conversation.Buyer.AvatarFileName);
        }

        MessageDto? message = null;
        if (!string.IsNullOrWhiteSpace(request.FirstMessage) && await ValidateMessageAsync(request.FirstMessage))
        {
            message = await SaveMessageToDbAsync(conversation.Id, buyerId, request.FirstMessage);
        }

        if (message is null)
        {
            return Result<ConversationDto>.Failure(ServiceError.ValidationError);
        }

        var messages = new List<MessageDto>() {message };
        var dto = new ConversationDto(
           conversation.Id, conversation.ListingId, conversation.Listing.Title,
           receipient,
           new CursorPaginatedList<MessageDto>([.. messages], false));
        return Result<ConversationDto>.Success(dto);
    }

    public async Task<bool> IsUserParticipantInConversationAsync(Guid identityId, Guid conversationId)
    {
        User? user = await GetLocalUserIdAsync(identityId);
        if (user is null) return false;

        return await _dbContext.Conversations
            .AnyAsync(c => c.Id == conversationId && (c.BuyerId == user.Id || c.SellerId == user.Id));
    }

    public Task<bool> ValidateMessageAsync(string message)
    {
        var valid = !string.IsNullOrWhiteSpace(message) && message.Trim().Length <= MaxMessageLength;
        return Task.FromResult(valid);
    }

    public async Task<MessageDto?> SaveMessageToDbAsync(Guid conversationId, Guid identityId, string message)
    {
        User? sender= await GetLocalUserIdAsync(identityId);
        if (sender is null) return null;

        var savedMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = sender.Id,
            Body = message.Trim(),
            MessageStatus = MessageStatus.Unread,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Messages.Add(savedMessage);
        await _dbContext.SaveChangesAsync();

        return new MessageDto(savedMessage.Id, savedMessage.SenderId, savedMessage.Body, savedMessage.MessageStatus, savedMessage.CreatedAt);
    }
}