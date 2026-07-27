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

    public async Task<Guid?> GetLocalUserIdAsync(Guid identityId) //TODO do wywalenia
    {
        return await _dbContext.Users
            .Where(u => u.IdentityId == identityId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
        Guid identityId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize)
    {
        var userId = await GetLocalUserIdAsync(identityId);
        if (userId is null)
            return new CursorPaginatedList<MinimalConversationDto>([], false);

        var query =
        from c in _dbContext.Conversations
        where c.BuyerId == userId || c.SellerId == userId
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
                ListingTitle = x.Conversation.Listing.Title,   // translated to a SQL join, no Include needed
                x.LastMessage,
                OtherUser = x.Conversation.BuyerId == userId ? x.Conversation.Seller : x.Conversation.Buyer
            })
            .ToListAsync();

        var hasNextPage = page.Count > pageSize;
        var items = page.Take(pageSize).Select(x => new MinimalConversationDto(
            x.Id,
            x.ListingId,
            x.ListingTitle,
            new ConversationParticipantDto(x.OtherUser.Id, x.OtherUser.DisplayName, x.OtherUser.AvatarFileName),
            x.LastMessage == null ? null : new MessageDto(
                x.LastMessage.Id, x.LastMessage.ConversationId, x.LastMessage.SenderId,
                x.LastMessage.Body, x.LastMessage.MessageStatus, x.LastMessage.CreatedAt)
        )).ToList();

        return new CursorPaginatedList<MinimalConversationDto>(items, hasNextPage);
    }

    public async Task<ConversationDto?> GetConversationAsync(
        Guid identityId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize)
    {
        var userId = await GetLocalUserIdAsync(identityId);
        if (userId is null) return null;

        var conversation = await _dbContext.Conversations
            .Include(c => c.Buyer).Include(c => c.Seller).Include(c => c.Listing)
            .FirstOrDefaultAsync(c => c.Id == conversationId && (c.BuyerId == userId || c.SellerId == userId));

        if (conversation is null) return null;

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
            .Select(m => new MessageDto(m.Id, m.ConversationId, m.SenderId, m.Body, m.MessageStatus, m.CreatedAt))
            .ToList();

        return new ConversationDto(
            conversation.Id, conversation.ListingId, conversation.Listing.Title,
            new ConversationParticipantDto(conversation.Buyer.Id, conversation.Buyer.DisplayName, conversation.Buyer.AvatarFileName),
            new ConversationParticipantDto(conversation.Seller.Id, conversation.Seller.DisplayName, conversation.Seller.AvatarFileName),
            new CursorPaginatedList<MessageDto>(messages, hasNextPage));
    }

    public async Task<ConversationDto?> CreateConversationAsync(Guid identityId, ConversationRequest request)
    {
        var buyerId = await GetLocalUserIdAsync(identityId);
        if (buyerId is null) return null;

        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId);
        if (listing is null) return null;

        if (listing.UserId == buyerId) return null;

        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.ListingId == request.ListingId && c.BuyerId == buyerId && c.SellerId == listing.UserId);

        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                ListingId = request.ListingId,
                BuyerId = buyerId.Value,
                SellerId = listing.UserId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();
        }

        if (!string.IsNullOrWhiteSpace(request.FirstMessage) && await ValidateMessageAsync(request.FirstMessage))
        {
            await SaveMessageToDbAsync(conversation.Id, identityId, request.FirstMessage);
        }

        return await GetConversationAsync(identityId, conversation.Id, null, null, 10);
    }

    public async Task<bool> IsUserParticipantInConversationAsync(Guid identityId, Guid conversationId)
    {
        var userId = await GetLocalUserIdAsync(identityId);
        if (userId is null) return false;

        return await _dbContext.Conversations
            .AnyAsync(c => c.Id == conversationId && (c.BuyerId == userId || c.SellerId == userId));
    }

    public Task<bool> ValidateMessageAsync(string message)
    {
        var valid = !string.IsNullOrWhiteSpace(message) && message.Trim().Length <= MaxMessageLength;
        return Task.FromResult(valid);
    }

    public async Task<MessageDto?> SaveMessageToDbAsync(Guid conversationId, Guid identityId, string message)
    {
        var senderId = await GetLocalUserIdAsync(identityId);
        if (senderId is null) return null;

        var savedMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = senderId.Value,
            Body = message.Trim(),
            MessageStatus = MessageStatus.Unread,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Messages.Add(savedMessage);
        await _dbContext.SaveChangesAsync();

        return new MessageDto(savedMessage.Id, savedMessage.ConversationId, savedMessage.SenderId, savedMessage.Body, savedMessage.MessageStatus, savedMessage.CreatedAt);
    }
}