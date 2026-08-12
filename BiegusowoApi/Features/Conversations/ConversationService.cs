using Ardalis.Result;
using BiegusowoApi.Data;
using BiegusowoApi.Data.Models;
using BiegusowoApi.Data.Types;
using BiegusowoApi.Features.Conversations.Dtos;
using BiegusowoApi.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Features.Conversations;

public class ConversationService(ApplicationDbContext dbContext) : IConversationService
{
    private const int MaxMessageLength = 2000;

    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<CursorPaginatedList<MinimalConversationDto>> GetUserConversationsAsync(
        Guid userId, DateTimeOffset? beforeLastMessageAt, Guid? beforeConversationId, int pageSize)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
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
                ListingTitle = x.Conversation.Listing.Title,
                x.LastMessage,
                OtherUser = x.Conversation.BuyerId == userId ? x.Conversation.Seller : x.Conversation.Buyer,
                OtherUserIsSeller = x.Conversation.BuyerId == userId// if current user is buyer, the other side is the seller
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
        Guid userId, Guid conversationId, DateTimeOffset? beforeCreatedAt, Guid? beforeMessageId, int pageSize)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Result<ConversationDto>.Forbidden("Onboarding required");

        var conversation = await _dbContext.Conversations
            .Include(c => c.Buyer).Include(c => c.Seller).Include(c => c.Listing)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation is null)
            return Result<ConversationDto>.NotFound();

        if (conversation.BuyerId != user.Id && conversation.SellerId != user.Id)
            return Result<ConversationDto>.Forbidden();

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
            return Result<ConversationDto>.Conflict();
        }

        var listing = await _dbContext.Listings
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == request.ListingId);
        if (listing is null) return Result<ConversationDto>.NotFound();

        if (listing.UserId == buyerId) return Result<ConversationDto>.Invalid();

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
        
        ConversationParticipantDto recipient = new (
            listing.User.Id,
            true,
            listing.User.DisplayName,
            listing.User.AvatarFileName);

        MessageDto message = 
            await SaveMessageToDbAsync(conversation.Id, buyerId, request.FirstMessage);

        var messages = new List<MessageDto>() {message };
        var dto = new ConversationDto(
           conversation.Id, conversation.ListingId, conversation.Listing.Title,
           recipient,
           new CursorPaginatedList<MessageDto>([.. messages], false));
        return Result<ConversationDto>.Success(dto);
    }

    public async Task<bool> IsUserParticipantInConversationAsync(Guid userId, Guid conversationId)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return false;

        return await _dbContext.Conversations
            .AnyAsync(c => c.Id == conversationId && (c.BuyerId == userId || c.SellerId == userId));
    }

    public Task<bool> ValidateMessageAsync(string message)
    {
        var valid = !string.IsNullOrWhiteSpace(message) && message.Trim().Length <= MaxMessageLength;
        return Task.FromResult(valid);
    }

    public async Task<MessageDto> SaveMessageToDbAsync(Guid conversationId, Guid senderId, string message)
    {
        var savedMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = senderId,
            Body = message.Trim(),
            MessageStatus = MessageStatus.Unread,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Messages.Add(savedMessage);
        await _dbContext.SaveChangesAsync();

        return new MessageDto(savedMessage.Id, savedMessage.SenderId, savedMessage.Body, savedMessage.MessageStatus, savedMessage.CreatedAt);
    }
}